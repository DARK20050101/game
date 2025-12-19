using UnityEngine;
using Photon.Pun;
using System.Collections.Generic;

/// <summary>
/// Manages world state synchronization for multiplayer.
/// Master Client has authority over world state.
/// </summary>
public class WorldNetworkSync : MonoBehaviourPun
{
    #region Singleton
    public static WorldNetworkSync Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    #endregion

    #region Settings
    [Header("World Sync Settings")]
    [Tooltip("Map seed for deterministic generation")]
    public int mapSeed = 0;
    
    [Tooltip("How often to validate world state (seconds)")]
    public float stateValidationInterval = 5f;
    #endregion

    #region State
    private Dictionary<int, Vector3> monsterPositions = new Dictionary<int, Vector3>();
    private Dictionary<int, int> monsterHealths = new Dictionary<int, int>();
    private Dictionary<int, bool> resourceNodeStates = new Dictionary<int, bool>();
    
    private float lastValidationTime = 0f;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Generate and distribute map seed
            mapSeed = Random.Range(1, 999999);
            photonView.RPC("RPC_SyncMapSeed", RpcTarget.AllBuffered, mapSeed);
            
            Debug.Log($"Master Client: Generated map seed {mapSeed}");
        }
    }

    private void Update()
    {
        // Periodic state validation
        if (Time.time - lastValidationTime >= stateValidationInterval)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                BroadcastWorldState();
            }
            lastValidationTime = Time.time;
        }
    }
    #endregion

    #region Map Synchronization
    [PunRPC]
    private void RPC_SyncMapSeed(int seed)
    {
        mapSeed = seed;
        Debug.Log($"Received map seed: {seed}");
        
        // TODO: Integrate with MapGenerator when implemented
        // MapGenerator.Instance?.GenerateMap(seed);
    }

    /// <summary>
    /// Get the synchronized map seed for deterministic generation
    /// </summary>
    public int GetMapSeed()
    {
        return mapSeed;
    }
    #endregion

    #region Monster Synchronization
    /// <summary>
    /// Register a monster spawn (Master Client only)
    /// </summary>
    public void RegisterMonsterSpawn(int monsterId, Vector3 position, int health)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        monsterPositions[monsterId] = position;
        monsterHealths[monsterId] = health;

        // Broadcast to other clients
        photonView.RPC("RPC_SyncMonsterSpawn", RpcTarget.OthersBuffered, monsterId, position, health);
    }

    [PunRPC]
    private void RPC_SyncMonsterSpawn(int monsterId, Vector3 position, int health)
    {
        monsterPositions[monsterId] = position;
        monsterHealths[monsterId] = health;
        
        Debug.Log($"Monster {monsterId} spawned at {position}");
        // Instantiate monster on client
    }

    /// <summary>
    /// Update monster state (Master Client only)
    /// </summary>
    public void UpdateMonsterState(int monsterId, Vector3 position, int health)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        if (monsterPositions.ContainsKey(monsterId))
        {
            monsterPositions[monsterId] = position;
            monsterHealths[monsterId] = health;
        }
    }

    /// <summary>
    /// Monster defeated (Master Client only)
    /// </summary>
    public void MonsterDefeated(int monsterId, Vector3 lootPosition)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        monsterPositions.Remove(monsterId);
        monsterHealths.Remove(monsterId);

        // Broadcast to other clients
        photonView.RPC("RPC_MonsterDefeated", RpcTarget.AllBuffered, monsterId, lootPosition);
    }

    [PunRPC]
    private void RPC_MonsterDefeated(int monsterId, Vector3 lootPosition)
    {
        monsterPositions.Remove(monsterId);
        monsterHealths.Remove(monsterId);
        
        Debug.Log($"Monster {monsterId} defeated at {lootPosition}");
        // Spawn loot, remove monster
    }
    #endregion

    #region Resource Node Synchronization
    /// <summary>
    /// Register resource node state
    /// </summary>
    public void SetResourceNodeState(int nodeId, bool isActive)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        resourceNodeStates[nodeId] = isActive;
        photonView.RPC("RPC_SyncResourceNode", RpcTarget.OthersBuffered, nodeId, isActive);
    }

    [PunRPC]
    private void RPC_SyncResourceNode(int nodeId, bool isActive)
    {
        resourceNodeStates[nodeId] = isActive;
        Debug.Log($"Resource node {nodeId} is now {(isActive ? "active" : "depleted")}");
    }
    #endregion

    #region State Validation
    /// <summary>
    /// Broadcast complete world state (Master Client only)
    /// </summary>
    private void BroadcastWorldState()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        // Create state snapshot
        int[] monsterIds = new int[monsterPositions.Count];
        Vector3[] positions = new Vector3[monsterPositions.Count];
        int[] healths = new int[monsterHealths.Count];
        
        int index = 0;
        foreach (var kvp in monsterPositions)
        {
            monsterIds[index] = kvp.Key;
            positions[index] = kvp.Value;
            healths[index] = monsterHealths[kvp.Key];
            index++;
        }

        // Broadcast state
        photonView.RPC("RPC_ValidateWorldState", RpcTarget.Others, monsterIds, positions, healths);
    }

    [PunRPC]
    private void RPC_ValidateWorldState(int[] monsterIds, Vector3[] positions, int[] healths)
    {
        // Validate and correct local state
        for (int i = 0; i < monsterIds.Length; i++)
        {
            int id = monsterIds[i];
            Vector3 pos = positions[i];
            int health = healths[i];

            if (monsterPositions.ContainsKey(id))
            {
                // Check for desync
                float distance = Vector3.Distance(monsterPositions[id], pos);
                if (distance > 1f) // Tolerance: 1 unit
                {
                    Debug.LogWarning($"Monster {id} desync detected: {distance} units");
                    monsterPositions[id] = pos;
                    monsterHealths[id] = health;
                }
            }
            else
            {
                // Missing monster, add it
                Debug.LogWarning($"Missing monster {id}, syncing from master");
                monsterPositions[id] = pos;
                monsterHealths[id] = health;
            }
        }
    }
    #endregion

    #region Loot Distribution
    /// <summary>
    /// Coordinate loot drop between players
    /// </summary>
    [PunRPC]
    public void RPC_DistributeLoot(int itemId, int playerId)
    {
        Debug.Log($"Item {itemId} awarded to player {playerId}");
        // Handle loot distribution
    }
    #endregion
}
