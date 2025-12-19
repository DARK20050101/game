using UnityEngine;
using Photon.Pun;
using System;

namespace PixelCoopRPG.Network
{
    /// <summary>
    /// Synchronizes player position, rotation, and state across network.
    /// Optimized for low-latency 2-player gameplay.
    /// </summary>
    [RequireComponent(typeof(PhotonView))]
    public class PlayerNetworkSync : MonoBehaviourPun, IPunObservable
    {
        [Header("Sync Settings")]
        [SerializeField] private bool syncPosition = true;
        [SerializeField] private bool syncRotation = true;
        [SerializeField] private bool syncAnimation = true;
        
        [Header("Interpolation")]
        [SerializeField] private float positionLerpSpeed = 10f;
        [SerializeField] private float rotationLerpSpeed = 10f;
        
        [Header("Network Optimization")]
        [SerializeField] private float sendRate = 20f; // Updates per second
        [SerializeField] private float positionThreshold = 0.01f; // Min distance to sync
        
        // Sync data
        private Vector3 networkPosition;
        private Quaternion networkRotation;
        private Vector3 networkVelocity;
        private string currentAnimation;
        
        // Components
        private Transform cachedTransform;
        private Rigidbody2D rb2d;
        
        // Performance tracking
        private float lastSendTime;
        private Vector3 lastSentPosition;
        
        private void Awake()
        {
            cachedTransform = transform;
            rb2d = GetComponent<Rigidbody2D>();
            
            networkPosition = cachedTransform.position;
            networkRotation = cachedTransform.rotation;
        }
        
        private void Update()
        {
            if (!photonView.IsMine)
            {
                // Interpolate to network position/rotation for smooth movement
                if (syncPosition)
                {
                    cachedTransform.position = Vector3.Lerp(
                        cachedTransform.position, 
                        networkPosition, 
                        Time.deltaTime * positionLerpSpeed
                    );
                }
                
                if (syncRotation)
                {
                    cachedTransform.rotation = Quaternion.Lerp(
                        cachedTransform.rotation, 
                        networkRotation, 
                        Time.deltaTime * rotationLerpSpeed
                    );
                }
            }
            else
            {
                // Track performance for local player
                TrackSyncPerformance();
            }
        }
        
        /// <summary>
        /// Photon serialization callback for custom data sync
        /// </summary>
        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if (stream.IsWriting)
            {
                // Send data to network
                if (syncPosition)
                {
                    stream.SendNext(cachedTransform.position);
                    if (rb2d != null)
                    {
                        stream.SendNext(rb2d.velocity);
                    }
                }
                
                if (syncRotation)
                {
                    stream.SendNext(cachedTransform.rotation);
                }
                
                if (syncAnimation)
                {
                    stream.SendNext(currentAnimation);
                }
                
                lastSendTime = Time.time;
                lastSentPosition = cachedTransform.position;
            }
            else
            {
                // Receive data from network
                if (syncPosition)
                {
                    networkPosition = (Vector3)stream.ReceiveNext();
                    if (rb2d != null)
                    {
                        networkVelocity = (Vector3)stream.ReceiveNext();
                    }
                }
                
                if (syncRotation)
                {
                    networkRotation = (Quaternion)stream.ReceiveNext();
                }
                
                if (syncAnimation)
                {
                    currentAnimation = (string)stream.ReceiveNext();
                }
                
                // Track lag for performance monitoring
                float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime));
                NetworkPerformanceTracker.Instance?.RecordLatency(lag);
            }
        }
        
        /// <summary>
        /// Set animation state for network sync
        /// </summary>
        public void SetAnimationState(string animationName)
        {
            if (photonView.IsMine)
            {
                currentAnimation = animationName;
            }
        }
        
        /// <summary>
        /// Track synchronization performance
        /// </summary>
        private void TrackSyncPerformance()
        {
            if (NetworkPerformanceTracker.Instance != null)
            {
                float distanceMoved = Vector3.Distance(cachedTransform.position, lastSentPosition);
                NetworkPerformanceTracker.Instance.RecordSyncData(
                    distanceMoved, 
                    Time.time - lastSendTime
                );
            }
        }
        
        /// <summary>
        /// RPC for synchronizing important events
        /// </summary>
        [PunRPC]
        public void RPC_SyncEvent(string eventName, string eventData)
        {
            Debug.Log($"[PlayerNetworkSync] Event received: {eventName} - {eventData}");
            // Handle synchronized events (attacks, abilities, etc.)
        }
        
        /// <summary>
        /// Send event to all players
        /// </summary>
        public void SendSyncEvent(string eventName, string eventData)
        {
            if (photonView.IsMine)
            {
                photonView.RPC(nameof(RPC_SyncEvent), RpcTarget.All, eventName, eventData);
            }
        }
    }
}
