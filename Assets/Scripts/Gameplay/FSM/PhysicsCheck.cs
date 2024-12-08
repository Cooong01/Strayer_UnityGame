using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

// ��Ҫ����������ײ���
public class PhysicsCheck : MonoBehaviour
{
    [Header("������")]
    public Transform groundCheckPoint;
    public float groundCheckRadius = 0;
    public LayerMask groundLayer;
    public bool isTouchGround;

    [Header("ǽ�ڼ��")]
    public Transform wallCheckPoint;
    public float wallCheckLength = 0;
    public LayerMask wallLayer;
    public bool isTouchWall;

    [Header("���ӷ���λ�ü��")]
    public Transform mirrorPlaceCheckPoint;
    public float mirrorPlaceCheckRadius = 0;
    public LayerMask mirrorPlaceLayer;
    public bool isTouchMirrorPlace = false;
    public GameObject touchedMirrorPlaceObj; //�������浱ǰ��⵽�ľ��ӷ�������

    [Header("�������Ӽ��")]
    public Transform mirrorClosePoint;
    public float mirrorCloseCheckRadius = 0;
    public LayerMask mirrorLayer;
    public bool isTouchMirror = false;
    public int mirrorNumber; //��ǰ�����ľ��ӵ����
    public GameObject touchedMirrorObj; //��ǰ�����ľ��Ӷ���

    [Header("�뿪���Ӽ��")]
    //todo:��������֮��Ҫ�뿪����һ�ξ��룬�˼��Ϊ�գ����ɴ��͡�������
    public bool isAwayFromMirror; //�����ж��Ƿ����뿪�˾��ӡ��������ڣ�һ��ĳ�δ���֮���һ���뿪���ӣ���boolֵһֱΪtrue��ֱ����һ�δ���֮����Ϊfalse��Ȼ���⵽�뿪��ʱ��������Ϊtrue��
    public Transform mirrorAwayPoint;
    public float mirrorAwayCheckRadius = 0;
    public int mirrorAwayNumber; //��ǰ�����ľ��ӵ����
    public GameObject touchedAwayMirrorObj; //��ǰ�����ľ��Ӷ���
    public bool shouldDetectAway = false; //��ǰ�Ƿ�Ҫ��⡣��ֵ������������player״̬������
    public GameObject shouldDetectionMirrorObj; //��ǰ��Ҫ���ľ��Ӷ���

    [Header("����֮�������")]
    public SpriteShapeController spriteShapeController; //���ƿɴ��͵ľ���֮�������

    [Header("�����ؿ���ɵ���")]
    public Transform endPoint;
    public float endCheckRadius = 0;
    public LayerMask endLayer;
    public bool isTouchEnd;
    public bool isTouchingEnd =false; //����ת���ؿ������Ʋ�������⡣
    public bool enemySatisfy = false; //��ǰ�Ƿ���ɽ������Ҫ�� 

    [Header("�����ؿ���ʼ����")]
    public Transform startPoint;
    public float startCheckRadius = 0;
    public LayerMask startLayer;
    public bool isTouchStart = false;
    public bool isTouchingStart = false; //����ת���ؿ������Ʋ�������⡣

    [Header("�������")]
    public Transform BGPoint;
    public float BGCheckRadius = 0;
    public LayerMask BGLayer;
    public bool isTouchBG;
    public GameObject BGObj;

    [Header("�Ի�����")]
    public Transform talkCheckPoint;
    public float talkCheckRadius = 0;
    public LayerMask talkLayer;
    public bool isTouchTalk;
    public Transform TalkName;
    public bool LeftTalk = true; //�Ƿ��Ѿ������뿪�¼�


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
        //����Ƿ�Ӵ�����
        isTouchGround = Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, groundLayer) || Physics2D.OverlapCircle(groundCheckPoint.position, groundCheckRadius, wallLayer);
        //����Ƿ�Ӵ�ǽ��(����Ӵ���
        isTouchWall = Physics2D.Raycast(wallCheckPoint.position, transform.right * transform.localScale.x, wallCheckLength, wallLayer);
        //����Ƿ������µı�����
        isTouchBG = Physics2D.OverlapCircle(BGPoint.position, BGCheckRadius, BGLayer);
        //����Ƿ��ڶԻ��㸽��
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

        //�����Ŀ�귶Χ���ж��������ʩ���󣬷���������Ǹ�
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

        //�����Ŀ�귶Χ���ж�����ӣ�����������Ǹ�
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


        //����Ƿ��뿪���ӷ�Χ
        if (shouldDetectAway)
        {
            //�����Ŀ�귶Χ���ж�����ӣ�����������Ǹ�
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
            else //�뿪�˾��ӷ�Χ
            {
                isAwayFromMirror = true;
                touchedAwayMirrorObj = null;
                shouldDetectAway = false;
            }
        }


        isTouchEnd = Physics2D.OverlapCircle(endPoint.position, endCheckRadius, endLayer);
        //todo�����������߼��ĵ���������
        if (isTouchEnd && !isTouchingEnd && enemySatisfy)
        {
            print("�ȵ��ؿ���β��");
            EventCenter.Instance.EventTrigger(E_EventType.E_EnterLevel);
            enemySatisfy = false;
            isTouchingEnd = true;
            isTouchingStart = false;
            Level.Instance.EnterNextLevel();
        }

        isTouchStart = Physics2D.OverlapCircle(startPoint.position, startCheckRadius, startLayer);
        if (isTouchStart && !isTouchingStart)
        {
            print("�ȵ��ؿ���ͷ��");
            isTouchingEnd = false;
            isTouchingStart = true;
            Level.Instance.EnteredNextLevel();
            this.GetComponent<PlayerController>().parameter.isEnteringNextLevel = false;
        }

    }

    //�����������п�������
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // �� Scene ��ͼ�л��Ƽ�ⷶΧ
        Gizmos.DrawWireSphere((Vector2)groundCheckPoint.position, groundCheckRadius);
        Gizmos.DrawLine(wallCheckPoint.position, wallCheckPoint.position + Vector3.right * transform.localScale.x * wallCheckLength);
        Gizmos.DrawWireSphere((Vector2)mirrorPlaceCheckPoint.position, mirrorPlaceCheckRadius);
        Gizmos.DrawWireSphere((Vector2)endPoint.position, endCheckRadius);
        Gizmos.DrawWireSphere((Vector2)startPoint.position, startCheckRadius);
        Gizmos.DrawWireSphere((Vector2)mirrorAwayPoint.position, mirrorAwayCheckRadius);

        Gizmos.DrawWireSphere((Vector2)talkCheckPoint.position, talkCheckRadius);
    }
}

