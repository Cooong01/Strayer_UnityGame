using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 切换镜子模式的中间状态。
/// 没有制作对应的角色动画，因此废弃。
/// </summary>
public class PlayerChangePlayerModeState : PlayerState
{
    public PlayerChangePlayerModeState(PlayerFSM manager, string animationName) : base(manager, animationName){}


}
