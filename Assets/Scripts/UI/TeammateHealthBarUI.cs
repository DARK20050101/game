using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 队友像素心形血条系统 - Teammate Pixel Hearts Health Bar System
/// </summary>
public class TeammateHealthBarUI : MonoBehaviour
{
    [System.Serializable]
    public class PixelHeart
    {
        public Image heartImage;
        public Sprite fullHeart;
        public Sprite halfHeart;
        public Sprite emptyHeart;
        public bool isFilled = true;
    }

    [Header("血条配置 / Health Bar Configuration")]
    [SerializeField] private List<PixelHeart> hearts = new List<PixelHeart>();
    [SerializeField] private int maxHearts = 10;
    
    [Header("队友信息 / Teammate Info")]
    [SerializeField] private Text teammateNameText;
    [SerializeField] private Image teammateAvatar;
    
    [Header("像素风格设置 / Pixel Style Settings")]
    [SerializeField] private Color healthyColor = new Color(1f, 0.3f, 0.3f); // 红色心形
    [SerializeField] private Color lowHealthColor = new Color(0.5f, 0.5f, 0.5f); // 灰色
    [SerializeField] private float heartSpacing = 8f; // 像素间距

    private int currentHealth = 100;
    private int maxHealth = 100;

    private void Start()
    {
        InitializeHearts();
    }

    /// <summary>
    /// 初始化心形血条 / Initialize heart health bar
    /// </summary>
    private void InitializeHearts()
    {
        // 根据最大心形数量创建UI元素
        // Create UI elements based on max hearts count
        for (int i = hearts.Count; i < maxHearts; i++)
        {
            // 此处应该从预制件实例化心形图标
            // Heart icons should be instantiated from prefabs here
        }
        
        UpdateHealthDisplay();
    }

    /// <summary>
    /// 更新血量显示 / Update health display
    /// </summary>
    public void UpdateHealth(int health, int maxHealth)
    {
        this.currentHealth = Mathf.Clamp(health, 0, maxHealth);
        this.maxHealth = maxHealth;
        UpdateHealthDisplay();
    }

    /// <summary>
    /// 更新心形显示 / Update heart display
    /// </summary>
    private void UpdateHealthDisplay()
    {
        // 计算需要显示的心形数量 / Calculate hearts to display
        int heartsToShow = Mathf.CeilToInt(maxHealth / 10f);
        float healthPerHeart = maxHealth / (float)heartsToShow;
        
        for (int i = 0; i < hearts.Count && i < heartsToShow; i++)
        {
            float heartMinHealth = i * healthPerHeart;
            float heartMaxHealth = (i + 1) * healthPerHeart;
            
            if (currentHealth >= heartMaxHealth)
            {
                // 满心 / Full heart
                hearts[i].heartImage.sprite = hearts[i].fullHeart;
                hearts[i].heartImage.color = healthyColor;
                hearts[i].isFilled = true;
            }
            else if (currentHealth > heartMinHealth)
            {
                // 半心 / Half heart
                hearts[i].heartImage.sprite = hearts[i].halfHeart;
                hearts[i].heartImage.color = healthyColor;
                hearts[i].isFilled = false;
            }
            else
            {
                // 空心 / Empty heart
                hearts[i].heartImage.sprite = hearts[i].emptyHeart;
                hearts[i].heartImage.color = lowHealthColor;
                hearts[i].isFilled = false;
            }
        }
    }

    /// <summary>
    /// 设置队友信息 / Set teammate information
    /// </summary>
    public void SetTeammateInfo(string name, Sprite avatar)
    {
        if (teammateNameText != null)
            teammateNameText.text = name;
        
        if (teammateAvatar != null && avatar != null)
            teammateAvatar.sprite = avatar;
    }

    /// <summary>
    /// 触发受伤动画 / Trigger damage animation
    /// </summary>
    public void PlayDamageAnimation()
    {
        // 心形闪烁效果 / Heart flash effect
        StartCoroutine(FlashHearts());
    }

    private System.Collections.IEnumerator FlashHearts()
    {
        float flashDuration = 0.2f;
        float elapsed = 0f;
        
        while (elapsed < flashDuration)
        {
            float alpha = Mathf.PingPong(elapsed * 10f, 1f);
            foreach (var heart in hearts)
            {
                Color color = heart.heartImage.color;
                color.a = alpha;
                heart.heartImage.color = color;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        
        // 恢复正常透明度 / Restore normal alpha
        foreach (var heart in hearts)
        {
            Color color = heart.heartImage.color;
            color.a = 1f;
            heart.heartImage.color = color;
        }
    }
}
