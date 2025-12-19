using UnityEngine;
using Photon.Pun;

/// <summary>
/// 网络动画同步器 - Network Animation Synchronizer
/// 用于Photon PUN 2多人游戏中同步角色动画状态
/// For synchronizing character animation states in Photon PUN 2 multiplayer
/// </summary>
[RequireComponent(typeof(CharacterAnimationController))]
[RequireComponent(typeof(PhotonView))]
public class NetworkAnimationSync : MonoBehaviourPun, IPunObservable
{
    private CharacterAnimationController animController;
    private CharacterAnimationController.AnimationState lastSyncedState;

    private void Awake()
    {
        animController = GetComponent<CharacterAnimationController>();
    }

    /// <summary>
    /// 播放动画并同步到网络 / Play animation and sync to network
    /// </summary>
    public void PlayAnimationNetworked(CharacterAnimationController.AnimationState state)
    {
        // 本地立即播放 / Play locally immediately
        animController.PlayAnimation(state);

        // 如果是本地玩家，通过RPC同步到其他客户端
        // If local player, sync to other clients via RPC
        if (photonView.IsMine)
        {
            photonView.RPC("RPC_PlayAnimation", RpcTarget.Others, (int)state);
        }
    }

    /// <summary>
    /// RPC：播放动画 / RPC: Play animation
    /// </summary>
    [PunRPC]
    private void RPC_PlayAnimation(int stateIndex)
    {
        CharacterAnimationController.AnimationState state = 
            (CharacterAnimationController.AnimationState)stateIndex;
        animController.PlayAnimation(state);
    }

    /// <summary>
    /// Photon序列化回调 / Photon serialization callback
    /// </summary>
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 发送当前动画状态 / Send current animation state
            stream.SendNext((int)animController.GetCurrentState());
        }
        else
        {
            // 接收动画状态 / Receive animation state
            int stateIndex = (int)stream.ReceiveNext();
            CharacterAnimationController.AnimationState state = 
                (CharacterAnimationController.AnimationState)stateIndex;
            
            // 只在状态变化时更新 / Only update if state changed
            if (state != lastSyncedState)
            {
                animController.PlayAnimation(state);
                lastSyncedState = state;
            }
        }
    }
}
