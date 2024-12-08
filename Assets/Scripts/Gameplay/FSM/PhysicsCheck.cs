using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

// 主要用来进行碰撞检测
public class PhysicsCheck : MonoBehaviour
{
    [Header("地面检测")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0;
    public LayerMask groundLayer;
    public bool isTouchGround;

    [Header("墙壁检测")]
    public Transform wallCheckPoint;
    public float wallCheckLength = 0;
    public LayerMask wallLayer;
    public bool isTouchWall;

    [Header("镜子放置位置检测")]
    public Transform mirrorPlaceCheckPoint;
    public float mirrorPlaceCheckRadius = 0;
    public LayerMask mirrorPlaceLayer;
    public bool isTouchMirrorPlace = false;
    public GameObject touchedMirrorPlaceObj; //用来保存当前检测到的镜子放置物体

    [Header("靠近镜子检测")]
    public Transform mirrorClosePoint;
    public float mirrorCloseCheckRadius = 0;
    public LayerMask mirrorLayer;
    public bool isTouchMirror = false;
    public int mirrorNumber; //当前碰到的镜子的序号
    public GameObject touchedMirrorObj; //当前碰到的镜子对象

    [Header("离开镜子检测")]
    //todo:当传送完之后，要离开镜子一段距离，此检测为空，“可传送”才允许。
    public bool isAwayFromMirror; //用来判断是否是离开了镜子。生命周期：一旦某次传送之后第一次离开镜子，此bool值一直为true，直到下一次传送之后设为false，然后检测到离开的时候重新设为true。
    public Transform mirrorAwayPoint;
    public float mirrorAwayCheckRadius = 0;
    public int mirrorAwayNumber; //当前碰到的镜子的序号
    public GameObject touchedAwayMirrorObj; //当前碰到的镜子对象
    public bool shouldDetectAway = false; //当前是否要检测。该值的生命周期由player状态来管理。
    public GameObject shouldDetectionMirrorObj; //当前需要检测的镜子对象

    [Header("镜子之间的流光")]
    public SpriteShapeController spriteShapeController; //控制可传送的镜子之间的流光

    [Header("靠近关卡完成点检测")]
    public Transform endPoint;
    public float endCheckRadius = 0;
    public LayerMask endLayer;
    public bool isTouchEnd;
    public bool isTouchingEnd =false; //正在转换关卡，控制不反复检测。
    public bool enemySatisfy = false; //当前是否完成解救数量要求 

    [Header("靠近关卡起始点检测")]
    public Transform startPoint;
    public float startCheckRadius = 0;
    public LayerMask startLayer;
    public bool isTouchStart = false;
    public bool isTouchingStart = false; //正在转换关卡，控制不反复检测。

    [Header("背景检测")]
    public Transform BGPoint;
    public float BGCheckRadius = 0;
    public LayerMask BGLayer;
    public bool isTouchBG;
    public GameObject BGObj;

    [Header("对话点检测")]
    public Transform talkCheckPoint;
    public float talkCheckRadius = 0;
    public LayerMask talkLayer;
    public bool isTouchTalk;
    public Transform TalkName;
    public bool LeftTalk = true; //是否已经发过离开事件


    private void Start()
    {
        EventCenter.Instance.AddEventListener(E_EventType.E_canSuccess, CanSuccess);
    }

    private void CanSuccess()
    {
        enemySatisfy = true;
    }

    private void Update()
    {
        Check();
    }

    public void Check()
    {
        //检测是否接触地面
        isTouchGround = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer) || Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, wallLayer);
        //检测是否接触墙壁(正面接触）
        isTouchWall = Physics2D.Raycast(wallCheckPoint.position, transform.right * transform.localScale.x, wallCheckLength, wallLayer);
        //检测是否正在新的背景里
        isTouchBG = Physics2D.OverlapCircle(BGPoint.position, BGCheckRadius, BGLayer);
        //检测是否在对话点附近
        isTouchTalk = Physics2D.OverlapCircle(talkCheckPoint.position, talkCheckRadius, talkLayer);

        if (isTouchTalk)
        {
            LeftTalk = false;
            Collider2D[] talkItems = Physics2D.OverlapCircleAll(talkCheckPoint.position, talkCheckRadius, talkLayer);
            float min = float.MaxValue;
            foreach(var talk in talkItems)
            {
                float distance = Vector3.Distance(talk.transform.position, this.transform.position);
                if (distance < min)
                {
                    min = distance;
                    TalkName = talk.transform;
                }
                
            }
            EventCenter.Instance.EventTrigger(E_EventType.E_CloseTalk);
        }
        else if(!LeftTalk)
        {
            LeftTalk = true;
            EventCenter.Instance.EventTrigger(E_EventType.E_LeaveTalk);
        }

        if (isTouchBG)
        {
            BGObj = Physics2D.OverlapCircle(BGPoint.position, BGCheckRadius, BGLayer).gameObject;

        }

        //如果在目标范围内有多个镜子设施对象，返回最近的那个
        Collider2D[] mirrorPlaces = Physics2D.OverlapCircleAll(mirrorPlaceCheckPoint.position, mirrorPlaceCheckRadius, mirrorPlaceLayer);
        if (mirrorPlaces.Length > 0)
        {
            isTouchMirrorPlace = true;
            float distanceMin = float.MaxValue;
            foreach(var place in mirrorPlaces)
            {
                if (distanceMin > Vector3.Distance(place.gameObject.transform.position, this.transform.position))
                {
                    touchedMirrorPlaceObj = place.gameObject;
                    distanceMin = Vector3.Distance(place.gameObject.transform.position, this.transform.position);
                }
            }
        }
        else
        {
            touchedMirrorPlaceObj = null;
            isTouchMirrorPlace = false;
        }

        //如果在目标范围内有多个镜子，返回最近的那个
        Collider2D[] mirrors = Physics2D.OverlapCircleAll(mirrorClosePoint.position, mirrorCloseCheckRadius, mirrorLayer);
        if (mirrors.Length > 0)
        {
            isTouchMirror = true;
            float distanceMin = float.MaxValue;
            foreach (var mirror in mirrors)
            {
                if (distanceMin > Vector3.Distance(mirror.gameObject.transform.position, this.transform.position))
                {
                    touchedMirrorObj = mirror.gameObject;
                    distanceMin = Vector3.Distance(mirror.gameObject.transform.position, this.transform.position);
                }
            }
        }
        else
        {
            isTouchMirror = false;
        }


        //检测是否离开镜子范围
        if (shouldDetectAway)
        {
            //如果在目标范围内有多个镜子，返回最近的那个
            Collider2D[] mirrorsAway = Physics2D.OverlapCircleAll(mirrorAwayPoint.position, mirrorAwayCheckRadius, mirrorLayer);
            if (mirrorsAway.Length > 0)
            {
                isAwayFromMirror = false;
                float distanceMin = float.MaxValue;
                foreach (var mirror in mirrorsAway)
                {
                    if (distanceMin > Vector3.Distance(mirror.gameObject.transform.position, this.transform.position))
                    {
                        touchedAwayMirrorObj = mirror.gameObject;
                        distanceMin = Vector3.Distance(mirror.gameObject.transform.position, this.transform.position);
                    }
                }
            }
            else //离开了镜子范围
            {
                isAwayFromMirror = true;
                touchedAwayMirrorObj = null;
                shouldDetectAway = false;
            }
        }


        isTouchEnd = Physics2D.OverlapCircle(endPoint.position, endCheckRadius, endLayer);
        //todo：参数控制逻辑改到控制器里
        if (isTouchEnd && !isTouchingEnd && enemySatisfy)
        {
            print("踩到关卡结尾了");
            EventCenter.Instance.EventTrigger(E_EventType.E_EnterLevel);
            enemySatisfy = false;
            isTouchingEnd = true;
            isTouchingStart = false;
            Level.Instance.EnterNextLevel();
        }

        isTouchStart = Physics2D.OverlapCircle(startPoint.position, startCheckRadius, startLayer);
        if (isTouchStart && !isTouchingStart)
        {
            print("踩到关卡开头了");
            isTouchingEnd = false;
            isTouchingStart = true;
            Level.Instance.EnteredNextLevel();
            this.GetComponent<PlayerController>().parameter.isEnteringNextLevel = false;
        }

    }

    //单纯用来进行开发测试
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // 在 Scene 视图中绘制检测范围
        Gizmos.DrawWireSphere((Vector2)groundCheckPoint.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + Vector3.right * transform.localScale.x * wallCheckLength);
        Gizmos.DrawWireSphere((Vector2)mirrorPlaceCheckPoint.position, mirrorPlaceCheckRadius);
        Gizmos.DrawWireSphere((Vector2)endPoint.position, endCheckRadius);
        Gizmos.DrawWireSphere((Vector2)startPoint.position, startCheckRadius);
        Gizmos.DrawWireSphere((Vector2)mirrorAwayPoint.position, mirrorAwayCheckRadius);

        Gizmos.DrawWireSphere((Vector2)talkCheckPoint.position, talkCheckRadius);
    }
}

