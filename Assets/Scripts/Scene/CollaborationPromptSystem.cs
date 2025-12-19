using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 协作提示系统 - 用于稀有点的协作互动
/// Collaboration Prompt System - For rare point cooperation interactions
/// </summary>
public class CollaborationPromptSystem : MonoBehaviour
{
    [System.Serializable]
    public class CollaborationPoint
    {
        public string pointName;
        public Transform pointTransform;
        public int requiredPlayers = 2;
        public float activationRadius = 5f;
        public Sprite promptIcon;
        public string promptText;
        public bool isActive = false;
    }

    [Header("协作点配置 / Collaboration Points Config")]
    [SerializeField] private List<CollaborationPoint> collaborationPoints = new List<CollaborationPoint>();
    
    [Header("UI组件 / UI Components")]
    [SerializeField] private GameObject promptPrefab;
    [SerializeField] private Canvas worldCanvas;
    
    [Header("视觉效果 / Visual Effects")]
    [SerializeField] private Color highlightColor = new Color(1f, 0.8f, 0f, 1f); // 金色高亮
    [SerializeField] private ParticleSystem collaborationParticles;
    [SerializeField] private float pulseSpeed = 2f;

    private Dictionary<CollaborationPoint, GameObject> activePrompts = new Dictionary<CollaborationPoint, GameObject>();
    private List<Transform> nearbyPlayers = new List<Transform>();

    private void Update()
    {
        CheckCollaborationPoints();
    }

    /// <summary>
    /// 检查协作点状态 / Check collaboration points status
    /// </summary>
    private void CheckCollaborationPoints()
    {
        foreach (var point in collaborationPoints)
        {
            if (point.pointTransform == null) continue;

            // 检测范围内的玩家 / Detect players in range
            Collider[] colliders = Physics.OverlapSphere(point.pointTransform.position, point.activationRadius);
            nearbyPlayers.Clear();
            
            foreach (var collider in colliders)
            {
                if (collider.CompareTag("Player"))
                {
                    nearbyPlayers.Add(collider.transform);
                }
            }

            // 更新协作点状态 / Update collaboration point status
            bool shouldBeActive = nearbyPlayers.Count >= point.requiredPlayers;
            
            if (shouldBeActive && !point.isActive)
            {
                ActivateCollaborationPoint(point);
            }
            else if (!shouldBeActive && point.isActive)
            {
                DeactivateCollaborationPoint(point);
            }

            // 更新提示UI / Update prompt UI
            UpdatePromptUI(point, nearbyPlayers.Count);
        }
    }

    /// <summary>
    /// 激活协作点 / Activate collaboration point
    /// </summary>
    private void ActivateCollaborationPoint(CollaborationPoint point)
    {
        point.isActive = true;
        
        // 显示协作提示 / Show collaboration prompt
        if (promptPrefab != null && !activePrompts.ContainsKey(point))
        {
            GameObject prompt = Instantiate(promptPrefab, worldCanvas.transform);
            prompt.transform.position = point.pointTransform.position + Vector3.up * 2f;
            
            // 配置提示内容 / Configure prompt content
            var promptUI = prompt.GetComponent<CollaborationPromptUI>();
            if (promptUI != null)
            {
                promptUI.SetPromptData(point.promptIcon, point.promptText, highlightColor);
            }
            
            activePrompts[point] = prompt;
        }

        // 播放激活粒子效果 / Play activation particle effect
        if (collaborationParticles != null)
        {
            var particles = Instantiate(collaborationParticles, point.pointTransform.position, Quaternion.identity);
            particles.Play();
            Destroy(particles.gameObject, 3f);
        }

        Debug.Log($"协作点激活: {point.pointName} / Collaboration point activated: {point.pointName}");
    }

    /// <summary>
    /// 停用协作点 / Deactivate collaboration point
    /// </summary>
    private void DeactivateCollaborationPoint(CollaborationPoint point)
    {
        point.isActive = false;
        
        // 移除提示UI / Remove prompt UI
        if (activePrompts.ContainsKey(point))
        {
            Destroy(activePrompts[point]);
            activePrompts.Remove(point);
        }

        Debug.Log($"协作点停用: {point.pointName} / Collaboration point deactivated: {point.pointName}");
    }

    /// <summary>
    /// 更新提示UI / Update prompt UI
    /// </summary>
    private void UpdatePromptUI(CollaborationPoint point, int currentPlayers)
    {
        if (!activePrompts.ContainsKey(point)) return;

        var promptUI = activePrompts[point].GetComponent<CollaborationPromptUI>();
        if (promptUI != null)
        {
            promptUI.UpdatePlayerCount(currentPlayers, point.requiredPlayers);
            
            // 脉冲高亮效果 / Pulsing highlight effect
            if (point.isActive)
            {
                float pulse = Mathf.PingPong(Time.time * pulseSpeed, 1f);
                promptUI.SetHighlightIntensity(0.5f + pulse * 0.5f);
            }
        }
    }

    /// <summary>
    /// 添加协作点 / Add collaboration point
    /// </summary>
    public void AddCollaborationPoint(CollaborationPoint point)
    {
        collaborationPoints.Add(point);
    }

    /// <summary>
    /// 移除协作点 / Remove collaboration point
    /// </summary>
    public void RemoveCollaborationPoint(CollaborationPoint point)
    {
        if (point.isActive)
        {
            DeactivateCollaborationPoint(point);
        }
        collaborationPoints.Remove(point);
    }

    private void OnDrawGizmosSelected()
    {
        // 在编辑器中可视化协作范围 / Visualize collaboration range in editor
        Gizmos.color = Color.yellow;
        foreach (var point in collaborationPoints)
        {
            if (point.pointTransform != null)
            {
                Gizmos.DrawWireSphere(point.pointTransform.position, point.activationRadius);
            }
        }
    }
}

/// <summary>
/// 协作提示UI组件 / Collaboration Prompt UI Component
/// </summary>
public class CollaborationPromptUI : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private Text promptText;
    [SerializeField] private Text playerCountText;
    [SerializeField] private Image highlightImage;

    public void SetPromptData(Sprite icon, string text, Color highlightColor)
    {
        if (iconImage != null && icon != null)
            iconImage.sprite = icon;
        
        if (promptText != null)
            promptText.text = text;
        
        if (highlightImage != null)
            highlightImage.color = highlightColor;
    }

    public void UpdatePlayerCount(int current, int required)
    {
        if (playerCountText != null)
            playerCountText.text = $"{current}/{required} 玩家";
    }

    public void SetHighlightIntensity(float intensity)
    {
        if (highlightImage != null)
        {
            Color color = highlightImage.color;
            color.a = intensity;
            highlightImage.color = color;
        }
    }
}
