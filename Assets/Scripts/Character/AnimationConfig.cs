using UnityEngine;

/// <summary>
/// 角色动画配置 - 每职业8动作帧配置
/// Character Animation Config - 8 action frames per class configuration
/// </summary>
[CreateAssetMenu(fileName = "AnimationConfig", menuName = "Game/Animation Config")]
public class AnimationConfig : ScriptableObject
{
    [System.Serializable]
    public class AnimationFrameSet
    {
        public string stateName;
        public Sprite[] frames; // 像素帧序列 / Pixel frame sequence
        public int frameRate = 12; // 帧率 / Frame rate
        public bool loop = true; // 是否循环 / Whether to loop
    }

    [System.Serializable]
    public class ClassAnimationSet
    {
        public string className; // "Warrior", "Mage", "Archer", etc.
        
        [Header("8个基础动作 / 8 Basic Actions")]
        public AnimationFrameSet idle;
        public AnimationFrameSet walk;
        public AnimationFrameSet run;
        public AnimationFrameSet attack;
        public AnimationFrameSet skill;
        public AnimationFrameSet hit;
        public AnimationFrameSet death;
        public AnimationFrameSet interact;

        [Header("视觉效果配置 / Visual Effects Config")]
        public ParticleSystem skillParticlePrefab;
        public Color characterHighlightColor = Color.white;
    }

    [Header("职业动画配置 / Class Animation Configurations")]
    public ClassAnimationSet[] classAnimations;

    /// <summary>
    /// 根据职业名获取动画配置 / Get animation config by class name
    /// </summary>
    public ClassAnimationSet GetClassAnimation(string className)
    {
        foreach (var classAnim in classAnimations)
        {
            if (classAnim.className == className)
                return classAnim;
        }
        return null;
    }
}
