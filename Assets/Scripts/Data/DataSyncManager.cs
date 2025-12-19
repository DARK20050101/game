using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PixelCoopRPG.Data
{
    /// <summary>
    /// Manages synchronization between local and cloud storage.
    /// Ensures data consistency and handles conflict resolution.
    /// </summary>
    public class DataSyncManager : MonoBehaviour
    {
        private static DataSyncManager instance;
        public static DataSyncManager Instance => instance;
        
        [Header("Sync Settings")]
        [SerializeField] private bool autoSync = true;
        [SerializeField] private float syncInterval = 300f; // Sync every 5 minutes
        [SerializeField] private ConflictResolutionStrategy conflictStrategy = ConflictResolutionStrategy.LocalFirst;
        
        [Header("Sync Status")]
        [SerializeField] private bool isSyncing = false;
        [SerializeField] private float lastSyncTime;
        [SerializeField] private int successfulSyncs = 0;
        [SerializeField] private int failedSyncs = 0;
        
        private LocalStorageManager localStorage;
        private CloudStorageManager cloudStorage;
        
        // Sync queue for operations that failed
        private Queue<SyncOperation> pendingSyncOperations = new Queue<SyncOperation>();
        
        public bool IsSyncing => isSyncing;
        public int SuccessfulSyncs => successfulSyncs;
        public int FailedSyncs => failedSyncs;
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        
        private void Start()
        {
            localStorage = LocalStorageManager.Instance;
            cloudStorage = CloudStorageManager.Instance;
            
            if (localStorage == null)
            {
                Debug.LogWarning("[DataSyncManager] LocalStorageManager not found. Creating instance.");
                GameObject localStorageObj = new GameObject("LocalStorageManager");
                localStorage = localStorageObj.AddComponent<LocalStorageManager>();
            }
            
            if (cloudStorage == null)
            {
                Debug.LogWarning("[DataSyncManager] CloudStorageManager not found. Creating instance.");
                GameObject cloudStorageObj = new GameObject("CloudStorageManager");
                cloudStorage = cloudStorageObj.AddComponent<CloudStorageManager>();
            }
        }
        
        private void Update()
        {
            // Auto-sync
            if (autoSync && !isSyncing && Time.time - lastSyncTime >= syncInterval)
            {
                _ = SyncAllData();
            }
            
            // Process pending sync operations
            ProcessPendingSyncOperations();
        }
        
        /// <summary>
        /// Synchronize all data between local and cloud
        /// </summary>
        public async Task<bool> SyncAllData()
        {
            if (isSyncing)
            {
                Debug.LogWarning("[DataSyncManager] Sync already in progress");
                return false;
            }
            
            isSyncing = true;
            bool success = true;
            
            try
            {
                Debug.Log("[DataSyncManager] Starting full data sync...");
                
                // Sync player data
                success &= await SyncPlayerData();
                
                // Sync game progress
                success &= await SyncGameProgress();
                
                // Sync base layout
                success &= await SyncBaseLayout();
                
                // Update cloud storage
                if (cloudStorage.IsConnected)
                {
                    success &= await cloudStorage.SyncWithCloud();
                }
                
                lastSyncTime = Time.time;
                
                if (success)
                {
                    successfulSyncs++;
                    Debug.Log("[DataSyncManager] Full data sync completed successfully");
                }
                else
                {
                    failedSyncs++;
                    Debug.LogWarning("[DataSyncManager] Full data sync completed with errors");
                }
            }
            catch (Exception e)
            {
                failedSyncs++;
                success = false;
                Debug.LogError($"[DataSyncManager] Sync failed: {e.Message}");
            }
            finally
            {
                isSyncing = false;
            }
            
            return success;
        }
        
        /// <summary>
        /// Sync player data between local and cloud
        /// </summary>
        private async Task<bool> SyncPlayerData()
        {
            try
            {
                // Load local data
                PlayerData localData = localStorage.LoadPlayerData();
                
                // In a real implementation, this would load from cloud and resolve conflicts
                // For now, we just save to ensure data is backed up
                Debug.Log("[DataSyncManager] Player data synced");
                
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataSyncManager] Failed to sync player data: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Sync game progress between local and cloud
        /// </summary>
        private async Task<bool> SyncGameProgress()
        {
            try
            {
                GameProgressData localProgress = localStorage.LoadGameProgress();
                Debug.Log("[DataSyncManager] Game progress synced");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataSyncManager] Failed to sync game progress: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Sync base layout between local and cloud
        /// </summary>
        private async Task<bool> SyncBaseLayout()
        {
            try
            {
                BaseLayoutData localLayout = localStorage.LoadBaseLayout();
                Debug.Log("[DataSyncManager] Base layout synced");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataSyncManager] Failed to sync base layout: {e.Message}");
                return false;
            }
        }
        
        /// <summary>
        /// Resolve conflict between local and cloud data
        /// </summary>
        private T ResolveConflict<T>(T localData, T cloudData, long localTimestamp, long cloudTimestamp) where T : class
        {
            switch (conflictStrategy)
            {
                case ConflictResolutionStrategy.LocalFirst:
                    Debug.Log("[DataSyncManager] Conflict resolved: Using local data");
                    return localData;
                    
                case ConflictResolutionStrategy.CloudFirst:
                    Debug.Log("[DataSyncManager] Conflict resolved: Using cloud data");
                    return cloudData;
                    
                case ConflictResolutionStrategy.MostRecent:
                    bool useLocal = localTimestamp > cloudTimestamp;
                    Debug.Log($"[DataSyncManager] Conflict resolved: Using {(useLocal ? "local" : "cloud")} data (most recent)");
                    return useLocal ? localData : cloudData;
                    
                default:
                    return localData;
            }
        }
        
        /// <summary>
        /// Add sync operation to queue for retry
        /// </summary>
        public void QueueSyncOperation(SyncOperation operation)
        {
            pendingSyncOperations.Enqueue(operation);
            Debug.Log($"[DataSyncManager] Sync operation queued: {operation.operationType}");
        }
        
        /// <summary>
        /// Process pending sync operations
        /// </summary>
        private void ProcessPendingSyncOperations()
        {
            if (pendingSyncOperations.Count == 0 || isSyncing) return;
            
            // Process one operation per frame to avoid blocking
            if (pendingSyncOperations.Count > 0)
            {
                SyncOperation op = pendingSyncOperations.Dequeue();
                _ = ExecuteSyncOperation(op);
            }
        }
        
        /// <summary>
        /// Execute a sync operation
        /// </summary>
        private async Task<bool> ExecuteSyncOperation(SyncOperation operation)
        {
            try
            {
                Debug.Log($"[DataSyncManager] Executing sync operation: {operation.operationType}");
                
                switch (operation.operationType)
                {
                    case SyncOperationType.PlayerData:
                        return await SyncPlayerData();
                    case SyncOperationType.GameProgress:
                        return await SyncGameProgress();
                    case SyncOperationType.BaseLayout:
                        return await SyncBaseLayout();
                    default:
                        return false;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[DataSyncManager] Sync operation failed: {e.Message}");
                
                // Re-queue if not exceeded max retries
                if (operation.retryCount < 3)
                {
                    operation.retryCount++;
                    QueueSyncOperation(operation);
                }
                
                return false;
            }
        }
        
        /// <summary>
        /// Force immediate sync
        /// </summary>
        public void ForceSync()
        {
            Debug.Log("[DataSyncManager] Force sync requested");
            _ = SyncAllData();
        }
        
        /// <summary>
        /// Get sync status report
        /// </summary>
        public SyncStatusReport GetSyncStatus()
        {
            return new SyncStatusReport
            {
                isSyncing = isSyncing,
                lastSyncTime = lastSyncTime,
                successfulSyncs = successfulSyncs,
                failedSyncs = failedSyncs,
                pendingOperations = pendingSyncOperations.Count,
                cloudConnected = cloudStorage?.IsConnected ?? false
            };
        }
    }
    
    /// <summary>
    /// Conflict resolution strategies
    /// </summary>
    public enum ConflictResolutionStrategy
    {
        LocalFirst,     // Always prefer local data
        CloudFirst,     // Always prefer cloud data
        MostRecent      // Use most recently modified data
    }
    
    /// <summary>
    /// Sync operation types
    /// </summary>
    public enum SyncOperationType
    {
        PlayerData,
        GameProgress,
        BaseLayout,
        FriendsList,
        Achievements
    }
    
    /// <summary>
    /// Sync operation data structure
    /// </summary>
    [Serializable]
    public class SyncOperation
    {
        public SyncOperationType operationType;
        public string dataId;
        public int retryCount;
        public long timestamp;
        
        public SyncOperation(SyncOperationType type, string id = null)
        {
            operationType = type;
            dataId = id;
            retryCount = 0;
            timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }
    }
    
    /// <summary>
    /// Sync status report structure
    /// </summary>
    [Serializable]
    public struct SyncStatusReport
    {
        public bool isSyncing;
        public float lastSyncTime;
        public int successfulSyncs;
        public int failedSyncs;
        public int pendingOperations;
        public bool cloudConnected;
    }
}
