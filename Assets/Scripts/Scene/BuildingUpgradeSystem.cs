using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 建筑升级像素变化系统 - Building Upgrade Pixel Change System
/// 管理建筑物在不同等级下的像素外观变化
/// Manages pixel appearance changes of buildings at different levels
/// </summary>
public class BuildingUpgradeSystem : MonoBehaviour
{
    [System.Serializable]
    public class BuildingLevel
    {
        public int level;
        public Sprite buildingSprite;          // 建筑像素图 / Building pixel sprite
        public string levelName;               // 等级名称 / Level name
        public Color tintColor = Color.white;  // 着色 / Tint color
        public ParticleSystem upgradeEffect;   // 升级特效 / Upgrade effect
        public AudioClip upgradeSound;         // 升级音效 / Upgrade sound
    }

    [System.Serializable]
    public class Building
    {
        public string buildingName;
        public GameObject buildingObject;
        public SpriteRenderer spriteRenderer;
        public int currentLevel = 0;
        public List<BuildingLevel> levels = new List<BuildingLevel>();
        public Transform effectSpawnPoint;
    }

    [Header("建筑配置 / Building Configuration")]
    [SerializeField] private List<Building> buildings = new List<Building>();
    
    [Header("升级动画设置 / Upgrade Animation Settings")]
    [SerializeField] private float upgradeDuration = 1f;
    [SerializeField] private AnimationCurve upgradeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] private AudioSource audioSource;
    
    [Header("像素效果 / Pixel Effects")]
    [SerializeField] private ParticleSystem constructionParticles;
    [SerializeField] private Color constructionColor = new Color(1f, 0.8f, 0f);

    /// <summary>
    /// 升级建筑 / Upgrade building
    /// </summary>
    public bool UpgradeBuilding(string buildingName)
    {
        Building building = GetBuilding(buildingName);
        if (building == null)
        {
            Debug.LogWarning($"建筑不存在: {buildingName} / Building not found: {buildingName}");
            return false;
        }

        if (building.currentLevel >= building.levels.Count - 1)
        {
            Debug.LogWarning($"建筑已达到最高等级 / Building already at max level: {buildingName}");
            return false;
        }

        StartCoroutine(UpgradeBuildingAnimation(building));
        return true;
    }

    /// <summary>
    /// 升级建筑动画协程 / Upgrade building animation coroutine
    /// </summary>
    private System.Collections.IEnumerator UpgradeBuildingAnimation(Building building)
    {
        int newLevel = building.currentLevel + 1;
        BuildingLevel levelData = building.levels[newLevel];

        // 播放升级音效 / Play upgrade sound
        if (audioSource != null && levelData.upgradeSound != null)
        {
            audioSource.PlayOneShot(levelData.upgradeSound);
        }

        // 播放升级粒子效果 / Play upgrade particle effect
        if (levelData.upgradeEffect != null && building.effectSpawnPoint != null)
        {
            var effect = Instantiate(levelData.upgradeEffect, building.effectSpawnPoint.position, Quaternion.identity);
            effect.Play();
            Destroy(effect.gameObject, 3f);
        }
        else if (constructionParticles != null && building.effectSpawnPoint != null)
        {
            var effect = Instantiate(constructionParticles, building.effectSpawnPoint.position, Quaternion.identity);
            var main = effect.main;
            main.startColor = constructionColor;
            effect.Play();
            Destroy(effect.gameObject, 3f);
        }

        // 渐变到新像素图 / Fade to new pixel sprite
        SpriteRenderer renderer = building.spriteRenderer;
        if (renderer != null)
        {
            Sprite originalSprite = renderer.sprite;
            Color originalColor = renderer.color;
            
            float elapsed = 0f;
            while (elapsed < upgradeDuration)
            {
                elapsed += Time.deltaTime;
                float t = upgradeCurve.Evaluate(elapsed / upgradeDuration);
                
                // 闪烁效果 / Flash effect
                renderer.color = Color.Lerp(originalColor, Color.white, Mathf.PingPong(elapsed * 10f, 1f) * 0.3f);
                
                // 缩放脉冲 / Scale pulse
                building.buildingObject.transform.localScale = Vector3.one * (1f + Mathf.Sin(elapsed * 10f) * 0.1f);
                
                yield return null;
            }

            // 应用新外观 / Apply new appearance
            renderer.sprite = levelData.buildingSprite;
            renderer.color = levelData.tintColor;
            building.buildingObject.transform.localScale = Vector3.one;
        }

        // 更新等级 / Update level
        building.currentLevel = newLevel;
        
        Debug.Log($"建筑升级完成: {building.buildingName} -> 等级 {newLevel} / Building upgraded: {building.buildingName} -> Level {newLevel}");
    }

    /// <summary>
    /// 设置建筑等级 / Set building level
    /// </summary>
    public void SetBuildingLevel(string buildingName, int level)
    {
        Building building = GetBuilding(buildingName);
        if (building == null) return;

        if (level < 0 || level >= building.levels.Count)
        {
            Debug.LogWarning($"无效的建筑等级: {level} / Invalid building level: {level}");
            return;
        }

        building.currentLevel = level;
        BuildingLevel levelData = building.levels[level];

        // 立即应用外观 / Apply appearance immediately
        if (building.spriteRenderer != null)
        {
            building.spriteRenderer.sprite = levelData.buildingSprite;
            building.spriteRenderer.color = levelData.tintColor;
        }
    }

    /// <summary>
    /// 获取建筑 / Get building
    /// </summary>
    private Building GetBuilding(string buildingName)
    {
        foreach (var building in buildings)
        {
            if (building.buildingName == buildingName)
                return building;
        }
        return null;
    }

    /// <summary>
    /// 获取建筑当前等级 / Get building current level
    /// </summary>
    public int GetBuildingLevel(string buildingName)
    {
        Building building = GetBuilding(buildingName);
        return building != null ? building.currentLevel : -1;
    }

    /// <summary>
    /// 添加建筑 / Add building
    /// </summary>
    public void AddBuilding(Building building)
    {
        buildings.Add(building);
        if (building.currentLevel >= 0 && building.currentLevel < building.levels.Count)
        {
            SetBuildingLevel(building.buildingName, building.currentLevel);
        }
    }
}
