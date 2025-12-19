using UnityEngine;
using Photon.Pun;

/// <summary>
/// Handles synchronization of player state across the network.
/// Attach this to the player prefab.
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class PlayerNetworkSync : MonoBehaviourPun, IPunObservable
{
    #region Serialized Fields
    [Header("Synchronization Settings")]
    [Tooltip("Enable position synchronization")]
    public bool syncPosition = true;
    
    [Tooltip("Enable rotation synchronization")]
    public bool syncRotation = true;
    
    [Tooltip("Enable animation synchronization")]
    public bool syncAnimation = true;

    [Header("Interpolation Settings")]
    [Tooltip("Position interpolation speed")]
    public float positionLerpSpeed = 10f;
    
    [Tooltip("Rotation interpolation speed")]
    public float rotationLerpSpeed = 10f;
    #endregion

    #region Private Fields
    // Network state
    private Vector3 networkPosition;
    private Quaternion networkRotation;
    private Vector3 networkVelocity;
    
    // Animation state
    private string currentAnimationState;
    private Animator animator;
    
    // Lag compensation
    private float lag = 0f;
    #endregion

    #region Unity Lifecycle
    private void Awake()
    {
        animator = GetComponent<Animator>();
        
        // Initialize network state to current state
        networkPosition = transform.position;
        networkRotation = transform.rotation;
    }

    private void Update()
    {
        if (!photonView.IsMine)
        {
            // Interpolate to network state for remote players
            if (syncPosition)
            {
                transform.position = Vector3.Lerp(
                    transform.position,
                    networkPosition + networkVelocity * lag,
                    Time.deltaTime * positionLerpSpeed
                );
            }

            if (syncRotation)
            {
                transform.rotation = Quaternion.Lerp(
                    transform.rotation,
                    networkRotation,
                    Time.deltaTime * rotationLerpSpeed
                );
            }
        }
    }
    #endregion

    #region IPunObservable Implementation
    /// <summary>
    /// Called by PUN to serialize/deserialize player state
    /// </summary>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // We own this player: send data to others
            if (syncPosition)
            {
                stream.SendNext(transform.position);
                stream.SendNext(GetComponent<Rigidbody2D>()?.velocity ?? Vector2.zero);
            }

            if (syncRotation)
            {
                stream.SendNext(transform.rotation);
            }

            if (syncAnimation && animator != null)
            {
                stream.SendNext(currentAnimationState);
            }
        }
        else
        {
            // Network player: receive data from owner
            if (syncPosition)
            {
                networkPosition = (Vector3)stream.ReceiveNext();
                networkVelocity = (Vector3)stream.ReceiveNext();
            }

            if (syncRotation)
            {
                networkRotation = (Quaternion)stream.ReceiveNext();
            }

            if (syncAnimation && animator != null)
            {
                string animState = (string)stream.ReceiveNext();
                if (animState != currentAnimationState)
                {
                    PlayAnimation(animState);
                }
            }

            // Calculate lag for prediction
            lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
        }
    }
    #endregion

    #region Animation Sync
    /// <summary>
    /// Trigger animation sync across network
    /// </summary>
    public void SyncAnimation(string stateName)
    {
        currentAnimationState = stateName;
        if (animator != null)
        {
            PlayAnimation(stateName);
        }
    }

    private void PlayAnimation(string stateName)
    {
        currentAnimationState = stateName;
        if (animator != null)
        {
            animator.Play(stateName);
        }
    }
    #endregion

    #region RPC Methods
    /// <summary>
    /// Sync player action (attack, skill use, etc.) via RPC
    /// </summary>
    [PunRPC]
    public void RPC_PlayerAction(string actionType, Vector3 position, Vector3 direction)
    {
        Debug.Log($"Player {photonView.Owner.NickName} performed action: {actionType} at {position}");
        
        // Trigger appropriate action
        switch (actionType)
        {
            case "Attack":
                // Trigger attack animation/effect
                SyncAnimation("Attack");
                break;
            case "Skill":
                // Trigger skill animation/effect
                SyncAnimation("Skill");
                break;
            case "Interact":
                // Trigger interaction
                SyncAnimation("Interact");
                break;
        }
    }

    /// <summary>
    /// Trigger action on all clients
    /// </summary>
    public void TriggerAction(string actionType, Vector3 position, Vector3 direction)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_PlayerAction", RpcTarget.All, actionType, position, direction);
        }
    }
    #endregion
}
