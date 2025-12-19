using UnityEngine;

/// <summary>
/// 角色动画控制器 - 管理8个动作状态帧
/// Character Animation Controller - Manages 8 action state frames
/// </summary>
public class CharacterAnimationController : MonoBehaviour
{
    // 动画状态枚举 / Animation State Enum
    public enum AnimationState
    {
        Idle,       // 待机
        Walk,       // 走
        Run,        // 跑
        Attack,     // 攻击
        Skill,      // 技能
        Hit,        // 受击
        Death,      // 死亡
        Interact    // 互动
    }

    [Header("动画组件 / Animation Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("粒子效果 / Particle Effects")]
    [SerializeField] private ParticleSystem skillParticleEffect;
    [SerializeField] private ParticleSystem mageSpellEffect;

    private AnimationState currentState;
    private string currentClass; // "Warrior", "Mage", "Archer", etc.

    private void Awake()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 设置角色职业 / Set character class
    /// </summary>
    public void SetCharacterClass(string className)
    {
        currentClass = className;
    }

    /// <summary>
    /// 播放动画状态 / Play animation state
    /// </summary>
    public void PlayAnimation(AnimationState state)
    {
        if (currentState == state) return;

        currentState = state;
        
        // 更新动画器参数 / Update animator parameters
        animator.SetInteger("AnimationState", (int)state);
        
        // 特殊效果处理 / Handle special effects
        HandleSpecialEffects(state);
    }

    /// <summary>
    /// 处理特殊视觉效果 / Handle special visual effects
    /// </summary>
    private void HandleSpecialEffects(AnimationState state)
    {
        // 技能施放粒子效果 / Skill casting particle effects
        if (state == AnimationState.Skill)
        {
            if (skillParticleEffect != null)
                skillParticleEffect.Play();

            // 法师专属施法效果 / Mage-specific spell effect
            if (currentClass == "Mage" && mageSpellEffect != null)
            {
                mageSpellEffect.Play();
            }
        }
    }

    /// <summary>
    /// 获取当前动画状态 / Get current animation state
    /// </summary>
    public AnimationState GetCurrentState()
    {
        return currentState;
    }

    /// <summary>
    /// 检查是否正在播放动作 / Check if action is playing
    /// </summary>
    public bool IsPlayingAction()
    {
        return currentState == AnimationState.Attack || 
               currentState == AnimationState.Skill || 
               currentState == AnimationState.Hit ||
               currentState == AnimationState.Interact;
    }
}
