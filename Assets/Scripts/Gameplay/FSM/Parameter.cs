using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ״̬�����������Ļ���
/// </summary>
[Serializable]
public class Parameter : MonoBehaviour
{
    [Header("��������")]
    [HideInInspector] public Animator animator;
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public PhysicsCheck check;
}
