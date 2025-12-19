using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections.Generic;

namespace PixelCoopRPG.Network
{
    /// <summary>
    /// Tracks network performance metrics including latency, frame drops, and sync accuracy.
    /// Key performance indicators: ≤200ms latency target, sync accuracy monitoring.
    /// </summary>
    public class NetworkPerformanceTracker : MonoBehaviour
    {
        private static NetworkPerformanceTracker instance;
        public static NetworkPerformanceTracker Instance => instance;
        
        [Header("Performance Thresholds")]
        [SerializeField] private float targetLatency = 0.2f; // 200ms target
        [SerializeField] private float maxAcceptableLatency = 0.5f; // 500ms max
        [SerializeField] private int targetFPS = 60;
        
        [Header("Monitoring Settings")]
        [SerializeField] private bool enableLogging = true;
        [SerializeField] private float reportInterval = 5f; // Report every 5 seconds
        
        // Performance metrics
        private List<float> latencySamples = new List<float>();
        private List<float> fpsSamples = new List<float>();
        private int frameDropCount = 0;
        private float lastReportTime;
        
        // Connection metrics
        private float connectionStartTime;
        private int totalDisconnects = 0;
        private float totalConnectedTime = 0f;
        
        // Sync metrics
        private float totalSyncDistance = 0f;
        private int syncSampleCount = 0;
        private float avgSyncAccuracy = 0f;
        
        // Current stats
        public float CurrentPing => PhotonNetwork.GetPing();
        public float AverageLatency => CalculateAverage(latencySamples);
        public float AverageFPS => CalculateAverage(fpsSamples);
        public float ConnectionUptime => totalConnectedTime;
        public int DisconnectCount => totalDisconnects;
        public float SyncAccuracy => avgSyncAccuracy;
        
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
        
        private void Update()
        {
            if (PhotonNetwork.IsConnected)
            {
                // Track FPS
                float currentFPS = 1f / Time.unscaledDeltaTime;
                fpsSamples.Add(currentFPS);
                
                // Detect frame drops
                if (currentFPS < targetFPS * 0.8f) // 80% of target
                {
                    frameDropCount++;
                }
                
                // Track connection time
                totalConnectedTime += Time.deltaTime;
                
                // Periodic reporting
                if (Time.time - lastReportTime >= reportInterval)
                {
                    ReportPerformanceMetrics();
                    lastReportTime = Time.time;
                }
                
                // Limit sample size
                if (latencySamples.Count > 100)
                {
                    latencySamples.RemoveRange(0, 50);
                }
                if (fpsSamples.Count > 100)
                {
                    fpsSamples.RemoveRange(0, 50);
                }
            }
        }
        
        /// <summary>
        /// Record latency measurement
        /// </summary>
        public void RecordLatency(float latencySeconds)
        {
            latencySamples.Add(latencySeconds);
            
            // Check if latency exceeds threshold
            if (latencySeconds > maxAcceptableLatency)
            {
                Debug.LogWarning($"[NetworkPerformanceTracker] High latency detected: {latencySeconds * 1000:F0}ms");
            }
        }
        
        /// <summary>
        /// Record synchronization data for accuracy tracking
        /// </summary>
        public void RecordSyncData(float distanceMoved, float timeSinceLastSync)
        {
            totalSyncDistance += distanceMoved;
            syncSampleCount++;
            
            // Calculate sync accuracy (lower is better)
            avgSyncAccuracy = totalSyncDistance / Mathf.Max(syncSampleCount, 1);
        }
        
        /// <summary>
        /// Called when connected to Photon
        /// </summary>
        public void OnConnected()
        {
            connectionStartTime = Time.time;
            Debug.Log("[NetworkPerformanceTracker] Connection established. Monitoring started.");
        }
        
        /// <summary>
        /// Called when disconnected from Photon
        /// </summary>
        public void OnDisconnected(DisconnectCause cause)
        {
            totalDisconnects++;
            Debug.Log($"[NetworkPerformanceTracker] Disconnect recorded. Cause: {cause}. Total disconnects: {totalDisconnects}");
        }
        
        /// <summary>
        /// Called when joined a room
        /// </summary>
        public void OnJoinedRoom()
        {
            // Reset room-specific metrics
            frameDropCount = 0;
            Debug.Log("[NetworkPerformanceTracker] Joined room. Resetting room metrics.");
        }
        
        /// <summary>
        /// Generate and log performance report
        /// </summary>
        private void ReportPerformanceMetrics()
        {
            if (!enableLogging) return;
            
            PerformanceReport report = GenerateReport();
            
            Debug.Log($"[NetworkPerformanceTracker] Performance Report:\n" +
                      $"  Ping: {report.currentPing}ms\n" +
                      $"  Avg Latency: {report.averageLatency:F1}ms (Target: {targetLatency * 1000}ms)\n" +
                      $"  Avg FPS: {report.averageFPS:F1} (Target: {targetFPS})\n" +
                      $"  Frame Drops: {report.frameDrops}\n" +
                      $"  Sync Accuracy: {report.syncAccuracy:F4}\n" +
                      $"  Uptime: {report.connectionUptime:F1}s\n" +
                      $"  Disconnects: {report.totalDisconnects}\n" +
                      $"  Status: {report.performanceStatus}");
        }
        
        /// <summary>
        /// Generate comprehensive performance report
        /// </summary>
        public PerformanceReport GenerateReport()
        {
            return new PerformanceReport
            {
                currentPing = CurrentPing,
                averageLatency = AverageLatency * 1000, // Convert to ms
                averageFPS = AverageFPS,
                frameDrops = frameDropCount,
                syncAccuracy = SyncAccuracy,
                connectionUptime = ConnectionUptime,
                totalDisconnects = DisconnectCount,
                performanceStatus = EvaluatePerformanceStatus()
            };
        }
        
        /// <summary>
        /// Evaluate overall performance status
        /// </summary>
        private string EvaluatePerformanceStatus()
        {
            float avgLatency = AverageLatency;
            float avgFPS = AverageFPS;
            
            if (avgLatency <= targetLatency && avgFPS >= targetFPS * 0.9f)
            {
                return "Excellent";
            }
            else if (avgLatency <= maxAcceptableLatency && avgFPS >= targetFPS * 0.7f)
            {
                return "Good";
            }
            else if (avgLatency <= maxAcceptableLatency * 1.5f)
            {
                return "Fair";
            }
            else
            {
                return "Poor";
            }
        }
        
        /// <summary>
        /// Calculate average from sample list
        /// </summary>
        private float CalculateAverage(List<float> samples)
        {
            if (samples.Count == 0) return 0f;
            
            float sum = 0f;
            foreach (float sample in samples)
            {
                sum += sample;
            }
            return sum / samples.Count;
        }
        
        /// <summary>
        /// Performance report data structure
        /// </summary>
        [Serializable]
        public struct PerformanceReport
        {
            public int currentPing;
            public float averageLatency;
            public float averageFPS;
            public int frameDrops;
            public float syncAccuracy;
            public float connectionUptime;
            public int totalDisconnects;
            public string performanceStatus;
        }
    }
}
