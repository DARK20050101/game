using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

/// <summary>
/// NetworkManager handles all Photon PUN 2 connection and room management.
/// This is the central hub for multiplayer functionality.
/// </summary>
public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static NetworkManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Settings
    [Header("Connection Settings")]
    [Tooltip("Photon App Version - change to force client updates")]
    public string gameVersion = "1.0";
    
    [Tooltip("Maximum players per room")]
    public byte maxPlayersPerRoom = 2;

    [Header("Matchmaking Settings")]
    [Tooltip("Level difference tolerance for matchmaking")]
    public int levelDifferenceTolerance = 3;

    [Header("Performance Settings")]
    [Tooltip("Target send rate (updates per second)")]
    public int sendRate = 20;
    
    [Tooltip("Target serialization rate (state updates per second)")]
    public int serializationRate = 10;
    #endregion

    #region State
    private bool isConnecting = false;
    private string playerName = "Player";
    private int playerLevel = 1;
    
    // Room join callback
    public System.Action OnRoomJoinedCallback;
    public System.Action OnRoomJoinFailedCallback;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        // Configure Photon settings
        PhotonNetwork.SendRate = sendRate;
        PhotonNetwork.SerializationRate = serializationRate;
        
        // Load player data
        LoadPlayerData();
    }
    #endregion

    #region Connection Methods
    /// <summary>
    /// Connect to Photon Master Server
    /// </summary>
    public void Connect()
    {
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("Already connected to Photon");
            return;
        }

        isConnecting = true;
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.NickName = playerName;
        
        Debug.Log($"Connecting to Photon as {playerName} (Level {playerLevel})...");
        PhotonNetwork.ConnectUsingSettings();
    }

    /// <summary>
    /// Disconnect from Photon
    /// </summary>
    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.Disconnect();
        }
    }
    #endregion

    #region Room Management
    /// <summary>
    /// Create a new room with optional room name
    /// </summary>
    public void CreateRoom(string roomName = null)
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("Not connected to Photon. Connecting first...");
            Connect();
            return;
        }

        if (string.IsNullOrEmpty(roomName))
        {
            roomName = $"Room_{Random.Range(1000, 9999)}";
        }

        RoomOptions roomOptions = new RoomOptions
        {
            MaxPlayers = maxPlayersPerRoom,
            IsVisible = true,
            IsOpen = true,
            // Store player level for matchmaking
            CustomRoomProperties = new ExitGames.Client.Photon.Hashtable
            {
                { "minLevel", playerLevel - levelDifferenceTolerance },
                { "maxLevel", playerLevel + levelDifferenceTolerance }
            },
            CustomRoomPropertiesForLobby = new string[] { "minLevel", "maxLevel" }
        };

        Debug.Log($"Creating room: {roomName}");
        PhotonNetwork.CreateRoom(roomName, roomOptions);
    }

    /// <summary>
    /// Join a random room with level-appropriate matchmaking
    /// </summary>
    public void JoinRandomRoom()
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("Not connected to Photon. Connecting first...");
            Connect();
            return;
        }

        // Filter rooms by player level
        ExitGames.Client.Photon.Hashtable expectedProperties = new ExitGames.Client.Photon.Hashtable
        {
            // Find rooms where player level is within tolerance
        };

        Debug.Log($"Searching for room (Player Level: {playerLevel} ±{levelDifferenceTolerance})...");
        PhotonNetwork.JoinRandomRoom(expectedProperties, maxPlayersPerRoom);
    }

    /// <summary>
    /// Join a specific room by name
    /// </summary>
    public void JoinRoom(string roomName)
    {
        if (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.LogWarning("Not connected to Photon. Connecting first...");
            Connect();
            return;
        }

        Debug.Log($"Joining room: {roomName}");
        PhotonNetwork.JoinRoom(roomName);
    }

    /// <summary>
    /// Leave current room
    /// </summary>
    public void LeaveRoom()
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("Leaving room...");
            PhotonNetwork.LeaveRoom();
        }
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Photon Master Server");
        isConnecting = false;
        
        // Automatically join lobby to see available rooms
        PhotonNetwork.JoinLobby();
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning($"Disconnected from Photon: {cause}");
        isConnecting = false;
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Joined Photon Lobby");
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name} ({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})");
        OnRoomJoinedCallback?.Invoke();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to join room: {message}");
        OnRoomJoinFailedCallback?.Invoke();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log($"No random room available: {message}. Creating new room...");
        CreateRoom();
    }

    public override void OnCreatedRoom()
    {
        Debug.Log($"Room created successfully: {PhotonNetwork.CurrentRoom.Name}");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        Debug.LogError($"Failed to create room: {message}");
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"Player joined: {newPlayer.NickName}");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"Player left: {otherPlayer.NickName}");
    }

    public override void OnLeftRoom()
    {
        Debug.Log("Left room");
    }
    #endregion

    #region Player Data
    private void LoadPlayerData()
    {
        // Load from DataManager (will be implemented)
        playerName = PlayerPrefs.GetString("PlayerName", "Player");
        playerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
    }

    public void SetPlayerData(string name, int level)
    {
        playerName = name;
        playerLevel = level;
        PhotonNetwork.NickName = playerName;
    }
    #endregion

    #region Utility
    /// <summary>
    /// Get current connection status info
    /// </summary>
    public string GetConnectionStatus()
    {
        if (PhotonNetwork.IsConnected)
        {
            if (PhotonNetwork.InRoom)
            {
                return $"In Room: {PhotonNetwork.CurrentRoom.Name} ({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})";
            }
            else if (PhotonNetwork.InLobby)
            {
                return "In Lobby";
            }
            else
            {
                return "Connected";
            }
        }
        else if (isConnecting)
        {
            return "Connecting...";
        }
        else
        {
            return "Disconnected";
        }
    }

    /// <summary>
    /// Check if we can start gameplay (in room with at least 2 players)
    /// </summary>
    public bool CanStartGame()
    {
        return PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount >= 2;
    }
    #endregion
}
