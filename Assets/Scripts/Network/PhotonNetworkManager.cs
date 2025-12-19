using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

namespace PixelCoopRPG.Network
{
    /// <summary>
    /// Core network manager for Photon PUN 2 integration.
    /// Handles connection, room creation, and player synchronization.
    /// </summary>
    public class PhotonNetworkManager : MonoBehaviourPunCallbacks
    {
        [Header("Photon Settings")]
        [SerializeField] private string gameVersion = "1.0";
        [SerializeField] private byte maxPlayersPerRoom = 2;
        
        [Header("Connection Status")]
        [SerializeField] private bool autoConnect = true;
        
        private static PhotonNetworkManager instance;
        public static PhotonNetworkManager Instance => instance;
        
        public bool IsConnected => PhotonNetwork.IsConnected;
        public bool IsInRoom => PhotonNetwork.InRoom;
        public int PlayerCount => PhotonNetwork.CurrentRoom?.PlayerCount ?? 0;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Configure Photon settings
            PhotonNetwork.AutomaticallySyncScene = true;
        }
        
        private void Start()
        {
            if (autoConnect && !PhotonNetwork.IsConnected)
            {
                ConnectToPhoton();
            }
        }
        
        /// <summary>
        /// Connect to Photon Cloud
        /// </summary>
        public void ConnectToPhoton()
        {
            if (!PhotonNetwork.IsConnected)
            {
                PhotonNetwork.GameVersion = gameVersion;
                PhotonNetwork.ConnectUsingSettings();
                Debug.Log($"[PhotonNetworkManager] Connecting to Photon Cloud...");
            }
        }
        
        /// <summary>
        /// Create or join a room for 2-player gameplay
        /// </summary>
        public void JoinOrCreateRoom(string roomName = null)
        {
            if (!PhotonNetwork.IsConnected)
            {
                Debug.LogWarning("[PhotonNetworkManager] Not connected to Photon. Connect first.");
                return;
            }
            
            RoomOptions roomOptions = new RoomOptions
            {
                MaxPlayers = maxPlayersPerRoom,
                IsVisible = true,
                IsOpen = true
            };
            
            if (string.IsNullOrEmpty(roomName))
            {
                // Join random room or create new one
                PhotonNetwork.JoinRandomOrCreateRoom(roomOptions: roomOptions);
            }
            else
            {
                PhotonNetwork.JoinOrCreateRoom(roomName, roomOptions, TypedLobby.Default);
            }
        }
        
        /// <summary>
        /// Leave current room
        /// </summary>
        public void LeaveRoom()
        {
            if (PhotonNetwork.InRoom)
            {
                PhotonNetwork.LeaveRoom();
                Debug.Log("[PhotonNetworkManager] Leaving room...");
            }
        }
        
        /// <summary>
        /// Disconnect from Photon
        /// </summary>
        public void Disconnect()
        {
            if (PhotonNetwork.IsConnected)
            {
                PhotonNetwork.Disconnect();
                Debug.Log("[PhotonNetworkManager] Disconnecting from Photon...");
            }
        }
        
        #region Photon Callbacks
        
        public override void OnConnectedToMaster()
        {
            Debug.Log($"[PhotonNetworkManager] Connected to Master Server. Region: {PhotonNetwork.CloudRegion}");
            NetworkPerformanceTracker.Instance?.OnConnected();
        }
        
        public override void OnDisconnected(DisconnectCause cause)
        {
            Debug.LogWarning($"[PhotonNetworkManager] Disconnected: {cause}");
            NetworkPerformanceTracker.Instance?.OnDisconnected(cause);
        }
        
        public override void OnJoinedRoom()
        {
            Debug.Log($"[PhotonNetworkManager] Joined room: {PhotonNetwork.CurrentRoom.Name} ({PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers})");
            NetworkPerformanceTracker.Instance?.OnJoinedRoom();
        }
        
        public override void OnLeftRoom()
        {
            Debug.Log("[PhotonNetworkManager] Left room");
        }
        
        public override void OnPlayerEnteredRoom(Player newPlayer)
        {
            Debug.Log($"[PhotonNetworkManager] Player entered: {newPlayer.NickName} (Total: {PhotonNetwork.CurrentRoom.PlayerCount})");
        }
        
        public override void OnPlayerLeftRoom(Player otherPlayer)
        {
            Debug.Log($"[PhotonNetworkManager] Player left: {otherPlayer.NickName} (Remaining: {PhotonNetwork.CurrentRoom.PlayerCount})");
        }
        
        public override void OnJoinRandomFailed(short returnCode, string message)
        {
            Debug.Log($"[PhotonNetworkManager] Join random failed. Creating new room...");
            JoinOrCreateRoom();
        }
        
        public override void OnCreateRoomFailed(short returnCode, string message)
        {
            Debug.LogError($"[PhotonNetworkManager] Create room failed: {message}");
        }
        
        #endregion
    }
}
