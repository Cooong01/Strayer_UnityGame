using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态机相关数据类的基类
/// </summary>
[Serializable]
public class Parameter : MonoBehaviour
{
    [Header("基本属性")]
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public PhysicsCheck check;
}
