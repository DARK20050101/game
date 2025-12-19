using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 像素对讲机语音按钮 - Pixel Walkie-Talkie Voice Button
/// 包含复古音效的按压式语音通话按钮
/// Press-to-talk voice button with retro sound effects
/// </summary>
public class PixelWalkieTalkieButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("UI组件 / UI Components")]
    [SerializeField] private Image buttonImage;
    [SerializeField] private Sprite buttonNormalSprite;
    [SerializeField] private Sprite buttonPressedSprite;
    [SerializeField] private Text statusText;
    
    [Header("音效配置 / Audio Configuration")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pressSound;      // 按下音效 / Press sound
    [SerializeField] private AudioClip releaseSound;    // 松开音效 / Release sound
    [SerializeField] private AudioClip staticNoiseLoop; // 静态噪音循环 / Static noise loop
    
    [Header("视觉效果 / Visual Effects")]
    [SerializeField] private Image signalIndicator;     // 信号指示器 / Signal indicator
    [SerializeField] private Color talkingColor = Color.green;
    [SerializeField] private Color idleColor = Color.gray;
    [SerializeField] private ParticleSystem signalParticles; // 信号粒子效果 / Signal particles
    
    [Header("动画设置 / Animation Settings")]
    [SerializeField] private float buttonPressScale = 0.9f;
    [SerializeField] private float animationSpeed = 10f;

    private bool isTalking = false;
    private Vector3 originalScale;
    private bool isPressed = false;

    private void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
        
        originalScale = transform.localScale;
        
        if (signalIndicator != null)
            signalIndicator.color = idleColor;
    }

    /// <summary>
    /// 按下按钮 / Button pressed
    /// </summary>
    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        StartTalking();
    }

    /// <summary>
    /// 松开按钮 / Button released
    /// </summary>
    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        StopTalking();
    }

    /// <summary>
    /// 开始语音通话 / Start voice communication
    /// </summary>
    private void StartTalking()
    {
        isTalking = true;
        
        // 更新按钮外观 / Update button appearance
        if (buttonImage != null && buttonPressedSprite != null)
            buttonImage.sprite = buttonPressedSprite;
        
        // 缩放动画 / Scale animation
        transform.localScale = originalScale * buttonPressScale;
        
        // 播放按下音效 / Play press sound
        if (pressSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f); // 轻微音调变化
            audioSource.PlayOneShot(pressSound);
        }
        
        // 开始静态噪音 / Start static noise
        if (staticNoiseLoop != null)
        {
            audioSource.clip = staticNoiseLoop;
            audioSource.loop = true;
            audioSource.volume = 0.3f;
            audioSource.Play();
        }
        
        // 更新状态指示器 / Update status indicator
        if (signalIndicator != null)
            signalIndicator.color = talkingColor;
        
        // 播放信号粒子 / Play signal particles
        if (signalParticles != null)
            signalParticles.Play();
        
        // 更新状态文本 / Update status text
        if (statusText != null)
            statusText.text = "通话中... / TALKING...";
        
        // 触发网络语音传输 / Trigger network voice transmission
        StartVoiceTransmission();
    }

    /// <summary>
    /// 停止语音通话 / Stop voice communication
    /// </summary>
    private void StopTalking()
    {
        isTalking = false;
        
        // 恢复按钮外观 / Restore button appearance
        if (buttonImage != null && buttonNormalSprite != null)
            buttonImage.sprite = buttonNormalSprite;
        
        // 恢复缩放 / Restore scale
        transform.localScale = originalScale;
        
        // 播放松开音效 / Play release sound
        if (releaseSound != null)
        {
            audioSource.Stop();
            audioSource.loop = false;
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(releaseSound);
        }
        
        // 更新状态指示器 / Update status indicator
        if (signalIndicator != null)
            signalIndicator.color = idleColor;
        
        // 停止信号粒子 / Stop signal particles
        if (signalParticles != null)
            signalParticles.Stop();
        
        // 更新状态文本 / Update status text
        if (statusText != null)
            statusText.text = "按住说话 / HOLD TO TALK";
        
        // 停止网络语音传输 / Stop network voice transmission
        StopVoiceTransmission();
    }

    /// <summary>
    /// 开始语音传输 / Start voice transmission
    /// </summary>
    private void StartVoiceTransmission()
    {
        // 这里集成实际的语音传输逻辑（如 Photon Voice, Unity Voice Chat等）
        // Integrate actual voice transmission logic here (e.g., Photon Voice, Unity Voice Chat)
        Debug.Log("开始语音传输 / Voice transmission started");
    }

    /// <summary>
    /// 停止语音传输 / Stop voice transmission
    /// </summary>
    private void StopVoiceTransmission()
    {
        // 停止实际的语音传输
        // Stop actual voice transmission
        Debug.Log("停止语音传输 / Voice transmission stopped");
    }

    /// <summary>
    /// 获取当前通话状态 / Get current talking state
    /// </summary>
    public bool IsTalking()
    {
        return isTalking;
    }

    private void Update()
    {
        // 信号指示器闪烁效果 / Signal indicator blinking effect
        if (isTalking && signalIndicator != null)
        {
            float alpha = Mathf.PingPong(Time.time * 2f, 1f);
            Color color = signalIndicator.color;
            color.a = 0.5f + alpha * 0.5f;
            signalIndicator.color = color;
        }
    }
}
