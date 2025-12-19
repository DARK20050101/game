using System;
using System.IO;
using UnityEngine;
using Newtonsoft.Json;

namespace PixelCoopRPG.Data
{
    /// <summary>
    /// Interface for local data storage operations.
    /// Handles player progress, character data, and base layouts.
    /// </summary>
    public interface ILocalStorage
    {
        void SavePlayerData(PlayerData data);
        PlayerData LoadPlayerData();
        void SaveGameProgress(GameProgressData data);
        GameProgressData LoadGameProgress();
        void SaveBaseLayout(BaseLayoutData data);
        BaseLayoutData LoadBaseLayout();
        void ClearAllData();
        bool DataExists(string dataType);
    }
    
    /// <summary>
    /// Local storage implementation using JSON files.
    /// Data stored in Application.persistentDataPath for cross-platform support.
    /// </summary>
    public class LocalStorageManager : MonoBehaviour, ILocalStorage
    {
        private static LocalStorageManager instance;
        public static LocalStorageManager Instance => instance;
        
        [Header("Storage Settings")]
        [SerializeField] private bool enableEncryption = false; // Future enhancement
        [SerializeField] private bool autoSave = true;
        [SerializeField] private float autoSaveInterval = 60f; // Auto-save every 60 seconds
        
        private string saveDirectory;
        private float lastSaveTime;
        
        // File names
        private const string PLAYER_DATA_FILE = "player_data.json";
        private const string PROGRESS_DATA_FILE = "progress_data.json";
        private const string BASE_LAYOUT_FILE = "base_layout.json";
        
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            
            instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initialize save directory
            saveDirectory = Application.persistentDataPath;
            Debug.Log($"[LocalStorageManager] Save directory: {saveDirectory}");
            
            // Ensure directory exists
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }
        }
        
        private void Update()
        {
            // Auto-save functionality
            if (autoSave && Time.time - lastSaveTime >= autoSaveInterval)
            {
                AutoSave();
                lastSaveTime = Time.time;
            }
        }
        
        /// <summary>
        /// Save player data to local storage
        /// </summary>
        public void SavePlayerData(PlayerData data)
        {
            try
            {
                data.lastSaveTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                string filePath = Path.Combine(saveDirectory, PLAYER_DATA_FILE);
                File.WriteAllText(filePath, json);
                Debug.Log($"[LocalStorageManager] Player data saved: {filePath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"[LocalStorageManager] Failed to save player data: {e.Message}");
            }
        }
        
        /// <summary>
        /// Load player data from local storage
        /// </summary>
        public PlayerData LoadPlayerData()
        {
            try
            {
                string filePath = Path.Combine(saveDirectory, PLAYER_DATA_FILE);
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    PlayerData data = JsonConvert.DeserializeObject<PlayerData>(json);
                    Debug.Log($"[LocalStorageManager] Player data loaded: {data.playerName}, Level {data.level}");
                    return data;
                }
                else
                {
                    Debug.Log("[LocalStorageManager] No player data found. Creating new.");
                    return new PlayerData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[LocalStorageManager] Failed to load player data: {e.Message}");
                return new PlayerData();
            }
        }
        
        /// <summary>
        /// Save game progress data
        /// </summary>
        public void SaveGameProgress(GameProgressData data)
        {
            try
            {
                data.lastPlayTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                string filePath = Path.Combine(saveDirectory, PROGRESS_DATA_FILE);
                File.WriteAllText(filePath, json);
                Debug.Log($"[LocalStorageManager] Progress data saved");
            }
            catch (Exception e)
            {
                Debug.LogError($"[LocalStorageManager] Failed to save progress data: {e.Message}");
            }
        }
        
        /// <summary>
        /// Load game progress data
        /// </summary>
        public GameProgressData LoadGameProgress()
        {
            try
            {
                string filePath = Path.Combine(saveDirectory, PROGRESS_DATA_FILE);
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    GameProgressData data = JsonConvert.DeserializeObject<GameProgressData>(json);
                    Debug.Log($"[LocalStorageManager] Progress data loaded");
                    return data;
                }
                else
                {
                    Debug.Log("[LocalStorageManager] No progress data found. Creating new.");
                    return new GameProgressData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[LocalStorageManager] Failed to load progress data: {e.Message}");
                return new GameProgressData();
            }
        }
        
        /// <summary>
        /// Save base layout data
        /// </summary>
        public void SaveBaseLayout(BaseLayoutData data)
        {
            try
            {
                data.lastModified = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                string json = JsonConvert.SerializeObject(data, Formatting.Indented);
                string filePath = Path.Combine(saveDirectory, BASE_LAYOUT_FILE);
                File.WriteAllText(filePath, json);
                Debug.Log($"[LocalStorageManager] Base layout saved: {data.buildings.Length} buildings");
            }
            catch (Exception e)
            {
                Debug.LogError($"[LocalStorageManager] Failed to save base layout: {e.Message}");
            }
        }
        
        /// <summary>
        /// Load base layout data
        /// </summary>
        public BaseLayoutData LoadBaseLayout()
        {
            try
            {
                string filePath = Path.Combine(saveDirectory, BASE_LAYOUT_FILE);
                if (File.Exists(filePath))
                {
                    string json = File.ReadAllText(filePath);
                    BaseLayoutData data = JsonConvert.DeserializeObject<BaseLayoutData>(json);
                    Debug.Log($"[LocalStorageManager] Base layout loaded: {data.buildings.Length} buildings");
                    return data;
                }
                else
                {
                    Debug.Log("[LocalStorageManager] No base layout found. Creating new.");
                    return new BaseLayoutData();
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"[LocalStorageManager] Failed to load base layout: {e.Message}");
                return new BaseLayoutData();
            }
        }
        
        /// <summary>
        /// Clear all local data
        /// </summary>
        public void ClearAllData()
        {
            try
            {
                string[] files = { PLAYER_DATA_FILE, PROGRESS_DATA_FILE, BASE_LAYOUT_FILE };
                foreach (string file in files)
                {
                    string filePath = Path.Combine(saveDirectory, file);
                    if (File.Exists(filePath))
                    {
                        File.Delete(filePath);
                    }
                }
                Debug.Log("[LocalStorageManager] All local data cleared");
            }
            catch (Exception e)
            {
                Debug.LogError($"[LocalStorageManager] Failed to clear data: {e.Message}");
            }
        }
        
        /// <summary>
        /// Check if data file exists
        /// </summary>
        public bool DataExists(string dataType)
        {
            string fileName = dataType switch
            {
                "player" => PLAYER_DATA_FILE,
                "progress" => PROGRESS_DATA_FILE,
                "base" => BASE_LAYOUT_FILE,
                _ => null
            };
            
            if (fileName == null) return false;
            
            string filePath = Path.Combine(saveDirectory, fileName);
            return File.Exists(filePath);
        }
        
        /// <summary>
        /// Auto-save current game state
        /// </summary>
        private void AutoSave()
        {
            Debug.Log("[LocalStorageManager] Auto-save triggered");
            // Auto-save logic can be implemented here
            // For now, this is a placeholder for future enhancement
        }
    }
}
