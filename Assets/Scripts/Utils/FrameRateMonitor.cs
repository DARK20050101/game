using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Monitors client-side performance including frame rate and frame drops
/// </summary>
public class FrameRateMonitor : MonoBehaviour
{
    #region Singleton
    public static FrameRateMonitor Instance { get; private set; }

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
    [Header("Monitor Settings")]
    [Tooltip("Enable performance monitoring")]
    public bool enableMonitoring = true;
    
    [Tooltip("Display FPS counter")]
    public bool showFPSCounter = true;
    
    [Tooltip("Log frame drops")]
    public bool logFrameDrops = true;

    [Header("Thresholds")]
    [Tooltip("Target FPS")]
    public int targetFPS = 60;
    
    [Tooltip("Frame drop threshold (FPS)")]
    public int frameDropThreshold = 45;
    
    [Tooltip("Consecutive frame drops to trigger alert")]
    public int consecutiveDropsAlert = 5;
    #endregion

    #region Performance Data
    private Queue<float> frameTimeHistory = new Queue<float>(60);
    private float currentFPS = 0f;
    private float averageFPS = 0f;
    private float minFPS = 999f;
    private float maxFPS = 0f;
    
    private int frameDropCount = 0;
    private int consecutiveDrops = 0;
    private float lastDropTime = 0f;
    
    private float deltaTime = 0f;
    #endregion

    #region Unity Lifecycle
    private void Update()
    {
        if (!enableMonitoring) return;

        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        
        UpdateFPSMetrics();
        DetectFrameDrops();
    }

    private void OnGUI()
    {
        if (!showFPSCounter) return;

        DrawFPSCounter();
    }
    #endregion

    #region Metrics Update
    private void UpdateFPSMetrics()
    {
        // Calculate current FPS
        currentFPS = 1.0f / deltaTime;
        
        // Track frame times
        frameTimeHistory.Enqueue(deltaTime);
        if (frameTimeHistory.Count > 60)
        {
            frameTimeHistory.Dequeue();
        }

        // Calculate average FPS
        float sum = 0f;
        foreach (float time in frameTimeHistory)
        {
            sum += time;
        }
        float avgDelta = sum / frameTimeHistory.Count;
        averageFPS = 1.0f / avgDelta;

        // Track min/max
        if (currentFPS < minFPS)
            minFPS = currentFPS;
        if (currentFPS > maxFPS)
            maxFPS = currentFPS;
    }

    private void DetectFrameDrops()
    {
        if (currentFPS < frameDropThreshold)
        {
            frameDropCount++;
            consecutiveDrops++;
            lastDropTime = Time.time;

            if (consecutiveDrops >= consecutiveDropsAlert && logFrameDrops)
            {
                Debug.LogWarning($"Sustained frame drops detected: {currentFPS:F0} FPS (consecutive: {consecutiveDrops})");
            }
        }
        else
        {
            consecutiveDrops = 0;
        }
    }
    #endregion

    #region UI Display
    private void DrawFPSCounter()
    {
        int x = Screen.width - 210;
        int y = 10;
        int lineHeight = 20;
        int width = 200;
        int height = 120;

        // Background
        GUI.Box(new Rect(x, y, width, height), "Performance");

        y += 25;

        // Current FPS
        Color fpsColor = GetFPSColor(currentFPS);
        GUI.contentColor = fpsColor;
        GUI.Label(new Rect(x + 10, y, width - 20, lineHeight), 
            $"FPS: {currentFPS:F0}");
        GUI.contentColor = Color.white;
        y += lineHeight;

        // Average FPS
        GUI.Label(new Rect(x + 10, y, width - 20, lineHeight), 
            $"Avg: {averageFPS:F0}");
        y += lineHeight;

        // Min/Max FPS
        GUI.Label(new Rect(x + 10, y, width - 20, lineHeight), 
            $"Min: {minFPS:F0} | Max: {maxFPS:F0}");
        y += lineHeight;

        // Frame drops
        if (frameDropCount > 0)
        {
            GUI.contentColor = Color.yellow;
            GUI.Label(new Rect(x + 10, y, width - 20, lineHeight), 
                $"Drops: {frameDropCount}");
            GUI.contentColor = Color.white;
        }
    }

    private Color GetFPSColor(float fps)
    {
        if (fps >= targetFPS)
            return Color.green;
        else if (fps >= frameDropThreshold)
            return Color.yellow;
        else
            return Color.red;
    }
    #endregion

    #region Public API
    /// <summary>
    /// Get current FPS
    /// </summary>
    public float GetCurrentFPS()
    {
        return currentFPS;
    }

    /// <summary>
    /// Get average FPS
    /// </summary>
    public float GetAverageFPS()
    {
        return averageFPS;
    }

    /// <summary>
    /// Get frame drop count
    /// </summary>
    public int GetFrameDropCount()
    {
        return frameDropCount;
    }

    /// <summary>
    /// Reset metrics
    /// </summary>
    public void ResetMetrics()
    {
        frameTimeHistory.Clear();
        currentFPS = 0f;
        averageFPS = 0f;
        minFPS = 999f;
        maxFPS = 0f;
        frameDropCount = 0;
        consecutiveDrops = 0;
    }

    /// <summary>
    /// Get performance summary
    /// </summary>
    public string GetPerformanceSummary()
    {
        return $"FPS: Current={currentFPS:F1}, Avg={averageFPS:F1}, Min={minFPS:F1}, Max={maxFPS:F1}, Drops={frameDropCount}";
    }
    #endregion
}
