using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

/// <summary>
/// Monitors network performance metrics for debugging and QA
/// </summary>
public class NetworkPerformanceMonitor : MonoBehaviourPunCallbacks
{
    #region Singleton
    public static NetworkPerformanceMonitor Instance { get; private set; }

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
    [Header("Monitoring Settings")]
    [Tooltip("Enable detailed logging")]
    public bool enableLogging = true;
    
    [Tooltip("Display performance UI")]
    public bool showPerformanceUI = true;

    [Header("Thresholds")]
    [Tooltip("Ping threshold for warnings (ms)")]
    public int pingWarningThreshold = 150;
    
    [Tooltip("Ping threshold for errors (ms)")]
    public int pingErrorThreshold = 200;
    
    [Tooltip("Packet loss warning threshold (%)")]
    public float packetLossWarningThreshold = 5f;
    #endregion

    #region Performance Metrics
    // Network metrics
    private int currentPing = 0;
    private Queue<int> pingHistory = new Queue<int>(60); // Last 60 samples
    private float averagePing = 0f;
    
    // Packet loss tracking
    private int packetsSent = 0;
    private int packetsReceived = 0;
    private float packetLossRate = 0f;
    
    // Desync tracking
    private int desyncEventsCount = 0;
    private float lastDesyncTime = 0f;
    
    // Connection quality
    private ConnectionQuality connectionQuality = ConnectionQuality.Excellent;
    #endregion

    #region Unity Lifecycle
    private void Update()
    {
        if (PhotonNetwork.IsConnected)
        {
            UpdateNetworkMetrics();
        }
    }

    private void OnGUI()
    {
        if (showPerformanceUI && PhotonNetwork.IsConnected)
        {
            DrawPerformanceUI();
        }
    }
    #endregion

    #region Metrics Update
    private void UpdateNetworkMetrics()
    {
        // Update ping
        currentPing = PhotonNetwork.GetPing();
        pingHistory.Enqueue(currentPing);
        
        if (pingHistory.Count > 60)
        {
            pingHistory.Dequeue();
        }

        // Calculate average ping
        int sum = 0;
        foreach (int ping in pingHistory)
        {
            sum += ping;
        }
        averagePing = pingHistory.Count > 0 ? (float)sum / pingHistory.Count : 0f;

        // Update connection quality
        UpdateConnectionQuality();

        // Log warnings
        if (enableLogging)
        {
            if (currentPing > pingErrorThreshold)
            {
                Debug.LogWarning($"High ping detected: {currentPing}ms");
            }
        }
    }

    private void UpdateConnectionQuality()
    {
        if (averagePing < 100)
        {
            connectionQuality = ConnectionQuality.Excellent;
        }
        else if (averagePing < 150)
        {
            connectionQuality = ConnectionQuality.Good;
        }
        else if (averagePing < 200)
        {
            connectionQuality = ConnectionQuality.Fair;
        }
        else
        {
            connectionQuality = ConnectionQuality.Poor;
        }
    }
    #endregion

    #region Desync Detection
    /// <summary>
    /// Report a desync event
    /// </summary>
    public void ReportDesync(string description, float deviation)
    {
        desyncEventsCount++;
        lastDesyncTime = Time.time;

        if (enableLogging)
        {
            Debug.LogWarning($"Desync detected: {description} (deviation: {deviation:F2})");
        }
    }

    /// <summary>
    /// Validate position sync between clients
    /// </summary>
    public bool ValidatePositionSync(Vector3 localPos, Vector3 networkPos, float tolerance = 0.5f)
    {
        float distance = Vector3.Distance(localPos, networkPos);
        if (distance > tolerance)
        {
            ReportDesync($"Position mismatch", distance);
            return false;
        }
        return true;
    }
    #endregion

    #region Performance UI
    private void DrawPerformanceUI()
    {
        int x = 10;
        int y = 10;
        int lineHeight = 20;
        int width = 300;
        int height = 180;

        // Background
        GUI.Box(new Rect(x, y, width, height), "Network Performance");

        y += 25;

        // Connection status
        GUI.Label(new Rect(x + 10, y, width - 20, lineHeight), 
            $"Status: {PhotonNetwork.NetworkClientState}");
        y += lineHeight;

        // Ping
        Color pingColor = GetPingColor(currentPing);
        GUI.contentColor = pingColor;
        GUI.Label(new Rect(x + 10, y, width - 20, lineHeight), 
            $"Ping: {currentPing}ms (Avg: {averagePing:F0}ms)");
        GUI.contentColor = Color.white;
        y += lineHeight;

        // Connection quality
        GUI.Label(new Rect(x + 10, y, width - 20, lineHeight), 
            $"Quality: {connectionQuality}");
        y += lineHeight;

        // Players in room
        if (PhotonNetwork.InRoom)
        {
            GUI.Label(new Rect(x + 10, y, width - 20, lineHeight), 
                $"Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{PhotonNetwork.CurrentRoom.MaxPlayers}");
            y += lineHeight;
        }

        // Packet loss
        if (packetLossRate > 0)
        {
            Color lossColor = packetLossRate > packetLossWarningThreshold ? Color.red : Color.yellow;
            GUI.contentColor = lossColor;
            GUI.Label(new Rect(x + 10, y, width - 20, lineHeight), 
                $"Packet Loss: {packetLossRate:F1}%");
            GUI.contentColor = Color.white;
            y += lineHeight;
        }

        // Desync events
        if (desyncEventsCount > 0)
        {
            GUI.contentColor = Color.red;
            GUI.Label(new Rect(x + 10, y, width - 20, lineHeight), 
                $"Desync Events: {desyncEventsCount}");
            GUI.contentColor = Color.white;
            y += lineHeight;
        }

        // Traffic stats
        GUI.Label(new Rect(x + 10, y, width - 20, lineHeight), 
            $"Traffic: In {PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesIn / 1024}KB / Out {PhotonNetwork.NetworkingClient.LoadBalancingPeer.BytesOut / 1024}KB");
    }

    private Color GetPingColor(int ping)
    {
        if (ping < 100)
            return Color.green;
        else if (ping < 150)
            return Color.yellow;
        else if (ping < 200)
            return new Color(1f, 0.5f, 0f); // Orange
        else
            return Color.red;
    }
    #endregion

    #region Public API
    /// <summary>
    /// Get current ping
    /// </summary>
    public int GetCurrentPing()
    {
        return currentPing;
    }

    /// <summary>
    /// Get average ping
    /// </summary>
    public float GetAveragePing()
    {
        return averagePing;
    }

    /// <summary>
    /// Get connection quality
    /// </summary>
    public ConnectionQuality GetConnectionQuality()
    {
        return connectionQuality;
    }

    /// <summary>
    /// Get desync count
    /// </summary>
    public int GetDesyncCount()
    {
        return desyncEventsCount;
    }

    /// <summary>
    /// Reset metrics
    /// </summary>
    public void ResetMetrics()
    {
        pingHistory.Clear();
        desyncEventsCount = 0;
        packetsSent = 0;
        packetsReceived = 0;
        packetLossRate = 0f;
    }
    #endregion

    #region Photon Callbacks
    public override void OnConnectedToMaster()
    {
        if (enableLogging)
            Debug.Log("Performance Monitor: Connected to Master");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        if (enableLogging)
            Debug.Log($"Performance Monitor: Disconnected - {cause}");
        ResetMetrics();
    }
    #endregion

    #region Enums
    public enum ConnectionQuality
    {
        Excellent,  // < 100ms
        Good,       // 100-150ms
        Fair,       // 150-200ms
        Poor        // > 200ms
    }
    #endregion
}
