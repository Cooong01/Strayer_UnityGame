using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 回收镜子
/// 由于没有制作相关角色动画，本状态没有实际入口。
/// </summary>
public class PlayerRecycleMirrorState : PlayerState
{
    public PlayerRecycleMirrorState(PlayerFSM manager, string animationName) : base(manager, animationName){}


}
