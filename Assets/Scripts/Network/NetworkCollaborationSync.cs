using UnityEngine;
using Photon.Pun;

/// <summary>
/// 网络协作系统 - Network Collaboration System
/// 在多人游戏中同步协作点状态和玩家协作进度
/// Synchronizes collaboration point states and player cooperation progress in multiplayer
/// </summary>
[RequireComponent(typeof(PhotonView))]
public class NetworkCollaborationSync : MonoBehaviourPun
{
    private CollaborationPromptSystem collaborationSystem;

    private void Awake()
    {
        collaborationSystem = GetComponent<CollaborationPromptSystem>();
    }

    /// <summary>
    /// 触发协作点激活（仅Master Client）
    /// Trigger collaboration point activation (Master Client only)
    /// </summary>
    public void ActivateCollaborationPoint(string pointName)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("只有Master Client可以激活协作点 / Only Master Client can activate collaboration points");
            return;
        }

        // 通过RPC通知所有客户端 / Notify all clients via RPC
        photonView.RPC("RPC_ActivatePoint", RpcTarget.All, pointName);
    }

    [PunRPC]
    private void RPC_ActivatePoint(string pointName)
    {
        // 在所有客户端激活协作点效果
        // Activate collaboration point effects on all clients
        Debug.Log($"协作点激活: {pointName} / Collaboration point activated: {pointName}");
    }

    /// <summary>
    /// 玩家进入协作范围 / Player enters collaboration range
    /// </summary>
    public void PlayerEnterCollaborationRange(int playerActorNumber, string pointName)
    {
        photonView.RPC("RPC_PlayerEnterRange", RpcTarget.All, playerActorNumber, pointName);
    }

    [PunRPC]
    private void RPC_PlayerEnterRange(int playerActorNumber, string pointName)
    {
        Debug.Log($"玩家 {playerActorNumber} 进入协作点: {pointName}");
    }

    /// <summary>
    /// 玩家离开协作范围 / Player leaves collaboration range
    /// </summary>
    public void PlayerLeaveCollaborationRange(int playerActorNumber, string pointName)
    {
        photonView.RPC("RPC_PlayerLeaveRange", RpcTarget.All, playerActorNumber, pointName);
    }

    [PunRPC]
    private void RPC_PlayerLeaveRange(int playerActorNumber, string pointName)
    {
        Debug.Log($"玩家 {playerActorNumber} 离开协作点: {pointName}");
    }
}
