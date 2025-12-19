using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Manages all game data persistence (local storage)
/// Handles saving/loading player data, progress, and settings
/// </summary>
public class DataManager : MonoBehaviour
{
    #region Singleton
    public static DataManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        InitializeDataManager();
    }
    #endregion

    #region Settings
    [Header("Save Settings")]
    [Tooltip("Auto-save interval in seconds")]
    public float autoSaveInterval = 60f;
    
    [Tooltip("Number of backup saves to keep")]
    public int maxBackups = 3;
    
    [Tooltip("Enable encryption for save files")]
    public bool enableEncryption = false;
    #endregion

    #region Paths
    private string saveDirectory;
    private string playerDataPath;
    private string baseLayoutPath;
    private string settingsPath;
    #endregion

    #region State
    private PlayerData currentPlayerData;
    private BaseLayoutData currentBaseLayout;
    private GameSettings currentSettings;
    
    private float lastSaveTime = 0f;
    private bool isDirty = false; // Flag for unsaved changes
    #endregion

    #region Initialization
    private void InitializeDataManager()
    {
        // Set up save paths
        saveDirectory = Path.Combine(Application.persistentDataPath, "SaveData");
        playerDataPath = Path.Combine(saveDirectory, "player.json");
        baseLayoutPath = Path.Combine(saveDirectory, "base_layout.json");
        settingsPath = Path.Combine(saveDirectory, "settings.json");

        // Create save directory if it doesn't exist
        if (!Directory.Exists(saveDirectory))
        {
            Directory.CreateDirectory(saveDirectory);
            Debug.Log($"Created save directory: {saveDirectory}");
        }

        // Load existing data or create new
        LoadAllData();
    }
    #endregion

    #region Unity Lifecycle
    private void Update()
    {
        // Auto-save
        if (isDirty && Time.time - lastSaveTime >= autoSaveInterval)
        {
            SaveAllData();
        }
    }

    private void OnApplicationQuit()
    {
        // Save on exit
        if (isDirty)
        {
            SaveAllData();
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        // Save when app is paused (mobile)
        if (pauseStatus && isDirty)
        {
            SaveAllData();
        }
    }
    #endregion

    #region Player Data Management
    /// <summary>
    /// Get current player data
    /// </summary>
    public PlayerData GetPlayerData()
    {
        return currentPlayerData;
    }

    /// <summary>
    /// Update player data and mark as dirty
    /// </summary>
    public void UpdatePlayerData(PlayerData data)
    {
        currentPlayerData = data;
        MarkDirty();
    }

    /// <summary>
    /// Save player data to file
    /// </summary>
    public void SavePlayerData()
    {
        try
        {
            string json = JsonUtility.ToJson(currentPlayerData, true);
            
            // Create backup before saving
            CreateBackup(playerDataPath);
            
            File.WriteAllText(playerDataPath, json);
            Debug.Log($"Player data saved to {playerDataPath}");
            
            lastSaveTime = Time.time;
            isDirty = false;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save player data: {e.Message}");
        }
    }

    /// <summary>
    /// Load player data from file
    /// </summary>
    public void LoadPlayerData()
    {
        if (File.Exists(playerDataPath))
        {
            try
            {
                string json = File.ReadAllText(playerDataPath);
                currentPlayerData = JsonUtility.FromJson<PlayerData>(json);
                Debug.Log($"Player data loaded from {playerDataPath}");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load player data: {e.Message}");
                // Try to load from backup
                if (!LoadFromBackup(playerDataPath, out currentPlayerData))
                {
                    // Create new player data
                    currentPlayerData = new PlayerData();
                }
            }
        }
        else
        {
            Debug.Log("No saved player data found, creating new");
            currentPlayerData = new PlayerData();
        }
    }
    #endregion

    #region Base Layout Management
    /// <summary>
    /// Get current base layout
    /// </summary>
    public BaseLayoutData GetBaseLayout()
    {
        return currentBaseLayout;
    }

    /// <summary>
    /// Update base layout
    /// </summary>
    public void UpdateBaseLayout(BaseLayoutData data)
    {
        currentBaseLayout = data;
        MarkDirty();
    }

    /// <summary>
    /// Save base layout to file
    /// </summary>
    public void SaveBaseLayout()
    {
        try
        {
            string json = JsonUtility.ToJson(currentBaseLayout, true);
            CreateBackup(baseLayoutPath);
            File.WriteAllText(baseLayoutPath, json);
            Debug.Log("Base layout saved");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save base layout: {e.Message}");
        }
    }

    /// <summary>
    /// Load base layout from file
    /// </summary>
    public void LoadBaseLayout()
    {
        if (File.Exists(baseLayoutPath))
        {
            try
            {
                string json = File.ReadAllText(baseLayoutPath);
                currentBaseLayout = JsonUtility.FromJson<BaseLayoutData>(json);
                Debug.Log("Base layout loaded");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load base layout: {e.Message}");
                if (!LoadFromBackup(baseLayoutPath, out currentBaseLayout))
                {
                    currentBaseLayout = new BaseLayoutData();
                }
            }
        }
        else
        {
            currentBaseLayout = new BaseLayoutData();
        }
    }
    #endregion

    #region Settings Management
    /// <summary>
    /// Get current settings
    /// </summary>
    public GameSettings GetSettings()
    {
        return currentSettings;
    }

    /// <summary>
    /// Update settings
    /// </summary>
    public void UpdateSettings(GameSettings settings)
    {
        currentSettings = settings;
        ApplySettings();
        SaveSettings();
    }

    /// <summary>
    /// Save settings to file
    /// </summary>
    public void SaveSettings()
    {
        try
        {
            string json = JsonUtility.ToJson(currentSettings, true);
            File.WriteAllText(settingsPath, json);
            Debug.Log("Settings saved");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save settings: {e.Message}");
        }
    }

    /// <summary>
    /// Load settings from file
    /// </summary>
    public void LoadSettings()
    {
        if (File.Exists(settingsPath))
        {
            try
            {
                string json = File.ReadAllText(settingsPath);
                currentSettings = JsonUtility.FromJson<GameSettings>(json);
                ApplySettings();
                Debug.Log("Settings loaded");
            }
            catch (Exception e)
            {
                Debug.LogError($"Failed to load settings: {e.Message}");
                currentSettings = new GameSettings();
            }
        }
        else
        {
            currentSettings = new GameSettings();
        }
    }

    private void ApplySettings()
    {
        // Apply audio settings
        AudioListener.volume = currentSettings.masterVolume;
        
        // Apply graphics settings with validation
        int width = Mathf.Clamp(currentSettings.resolutionWidth, 640, 7680);
        int height = Mathf.Clamp(currentSettings.resolutionHeight, 480, 4320);
        
        // Validate aspect ratio is reasonable (between 4:3 and 21:9)
        float aspectRatio = (float)width / height;
        if (aspectRatio < 1.0f || aspectRatio > 3.5f)
        {
            Debug.LogWarning($"Invalid aspect ratio {aspectRatio:F2}, falling back to 16:9");
            width = 1920;
            height = 1080;
        }
        
        try
        {
            Screen.SetResolution(width, height, currentSettings.fullscreen);
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to set resolution: {e.Message}. Using current resolution.");
        }
        
        Application.targetFrameRate = Mathf.Clamp(currentSettings.targetFrameRate, 30, 240);
    }
    #endregion

    #region Save/Load All
    /// <summary>
    /// Save all data
    /// </summary>
    public void SaveAllData()
    {
        SavePlayerData();
        SaveBaseLayout();
        SaveSettings();
        Debug.Log("All data saved");
    }

    /// <summary>
    /// Load all data
    /// </summary>
    public void LoadAllData()
    {
        LoadPlayerData();
        LoadBaseLayout();
        LoadSettings();
        Debug.Log("All data loaded");
    }
    #endregion

    #region Backup System
    /// <summary>
    /// Create backup of a save file
    /// </summary>
    private void CreateBackup(string filePath)
    {
        if (!File.Exists(filePath)) return;

        try
        {
            // Shift existing backups
            for (int i = maxBackups - 1; i > 0; i--)
            {
                string oldBackup = $"{filePath}.backup{i}";
                string newBackup = $"{filePath}.backup{i + 1}";
                
                if (File.Exists(oldBackup))
                {
                    if (i == maxBackups - 1)
                    {
                        File.Delete(oldBackup); // Delete oldest backup
                    }
                    else
                    {
                        File.Move(oldBackup, newBackup);
                    }
                }
            }

            // Create new backup
            string backupPath = $"{filePath}.backup1";
            File.Copy(filePath, backupPath, true);
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to create backup: {e.Message}");
        }
    }

    /// <summary>
    /// Load data from most recent backup
    /// </summary>
    private bool LoadFromBackup<T>(string filePath, out T data) where T : new()
    {
        data = default(T);

        for (int i = 1; i <= maxBackups; i++)
        {
            string backupPath = $"{filePath}.backup{i}";
            if (File.Exists(backupPath))
            {
                try
                {
                    string json = File.ReadAllText(backupPath);
                    data = JsonUtility.FromJson<T>(json);
                    Debug.Log($"Loaded from backup: {backupPath}");
                    return true;
                }
                catch (Exception e)
                {
                    Debug.LogWarning($"Failed to load backup {i}: {e.Message}");
                }
            }
        }

        Debug.LogError("All backups failed, creating new data");
        data = new T();
        return false;
    }
    #endregion

    #region Utility
    /// <summary>
    /// Mark data as dirty (needs saving)
    /// </summary>
    public void MarkDirty()
    {
        isDirty = true;
    }

    /// <summary>
    /// Force immediate save
    /// </summary>
    public void ForceSave()
    {
        SaveAllData();
    }

    /// <summary>
    /// Delete all save data (for testing)
    /// </summary>
    public void DeleteAllSaveData()
    {
        if (Directory.Exists(saveDirectory))
        {
            Directory.Delete(saveDirectory, true);
            Debug.Log("All save data deleted");
        }
        InitializeDataManager();
    }
    #endregion
}
