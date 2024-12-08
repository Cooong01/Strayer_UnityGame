using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyWander
{
    FindPlayer,
    StartLose,
    LosePlayer,
}

public enum EnemyState
{
    Grounded,
    Jumping,
    Falling
}


public class AiEvent : MonoBehaviour
{

    public float Blood;

    //[Header("��ⷶΧ")]
    public bool DrawLine;
    public int angle; // ���εĽǶ�
    public float Deteradius; // ���εİ뾶
    //public int rayCount; // ��������
    public LayerMask targetLayer; // Ŀ�����
    public float CrossRadius; // �ǻ��뾶
    public float WaitTime;



    //[Header("׷��")]
    public float FollowDis;
    public float MoveSpeed;

    //[Header("��������Ծ")]
    public LayerMask obstacleLayer; // �ϰ����
    public LayerMask GroundedLayer; // �����
    public float jumpHeight = 2f; // ��Ծ�߶�

    LineRenderer lineRenderer;
    SpriteRenderer SpriteRenderer;

    [HideInInspector]
    public LineRenderer RainBow;
    Transform PlayerValue;
    [HideInInspector]
    public EnemyWander WanderState;
    [HideInInspector]
    public EnemyState JumpState; // ��ʼ״̬

    Vector3 initialPosition; // AI��ʼλ��
     bool movingRight = true; // �Ƿ������ƶ�
    bool CanMove = true;
     Vector3 JumpY; // ��ǰ�ٶ�

    //[HideInInspector]
    //public EnemyLaser enemyLaser;

    //bool isFiring = false;
    public Animator animator;


    public void Start()
    {
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
        initialPosition = transform.position; // ���ó�ʼλ��
        JumpState = EnemyState.Grounded;
        WanderState = EnemyWander.StartLose; // ��ʼ״̬Ϊ��ʼ��ʧ���
    }

    public void FindPlayer()
    {
        Vector3 direction = (PlayerValue.position - transform.position).normalized;
        float dotProduct = Vector3.Dot(transform.right, direction);

        if (dotProduct > 0.5)
        {
            SpriteRenderer.flipX = true;
        }
        else
        {
            SpriteRenderer.flipX = false;
        }

        direction.y = 0;

        Vector3 WithPlayer = transform.position - PlayerValue.position;

        WithPlayer.y = 0;

        transform.Translate(direction * MoveSpeed * Time.deltaTime);

        if (!IsAnimationPlaying("Walk") && !IsAnimationPlaying("Jump")) {

            animator.Play("Walk");
        }

        

        //if (WithPlayer.magnitude <= FollowDis)
        //{
            
        //}
        //else
        //{
        //    Debug.Log("kkk");
        //    //if (!isFiring)
        //    //    StartCoroutine(Fire());
        //}

    }
    private bool IsAnimation(string animationName)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName);
    }



    public bool IsAnimationPlaying(string name)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(name) && stateInfo.normalizedTime < 1.0f;
    }


    public void LosePlayer()
    {
        if (WanderState == EnemyWander.StartLose)
        {
            // ���س�ʼλ��
            Vector3 direction = (initialPosition - transform.position).normalized;
            float dotProduct = Vector3.Dot(transform.right, direction);

            if (dotProduct > 0.5)
            {
                SpriteRenderer.flipX = true;
            }
            else
            {
                SpriteRenderer.flipX = false;
            }

            direction.y = 0;

            transform.Translate(direction * MoveSpeed * Time.deltaTime);

            if ( direction.magnitude < 0.1f)
            {
                animator.Play("Idle");
                WanderState = EnemyWander.LosePlayer; // �����ʼλ�ú��л�����ʧ���״̬
            }
        }

        //if (WanderState == EnemyWander.LosePlayer && CanMove)
        //{

        //    SpriteRenderer.flipX = movingRight;

        //    float direction = movingRight ? 1 : -1;
        //    transform.Translate(Vector3.right * direction * MoveSpeed * Time.deltaTime);

        //    // ������CrossRadius��Χ��
        //    Vector3 h = transform.position- initialPosition;
        //    h.y = 0;
        //    if (h.magnitude > CrossRadius + 0.8)
        //    {
        //        movingRight = !movingRight; // �л�����

        //        StartCoroutine(WaitSameTime(WaitTime));
        //    }
        //}
    }


    //IEnumerator WaitSameTime(float time) {
    //    CanMove = false; // ֹͣ�ƶ�
    //    yield return new WaitForSeconds(time);
    //    CanMove = true; // �ָ��ƶ�
    //}


    public void FireRay()
    {
        if (DrawLine)
        {
            DrawSector((WanderState != EnemyWander.FindPlayer)? Deteradius : FollowDis,
                (int)(((WanderState != EnemyWander.FindPlayer) ? Deteradius : FollowDis) / 0.3));
        }
        else
        {

            lineRenderer.positionCount = 0;
        }

        if (WanderState != EnemyWander.FindPlayer)
        {
            FireSectorRays(Deteradius,(int)(Deteradius / 0.3));
        }
        else {
            FireSectorRays(FollowDis,(int)(FollowDis / 0.3));
        }

       
    }



     float lastCheckTime = 0f;
     Collider2D lastHitCollider = null;

    public void CheckObstacle(Vector3 velocity)
    {

        Vector3 forward = velocity;
        forward.y = 0;

        Vector3 h = transform.position;
        h.x += forward.normalized.x * (float)(transform.localScale.x * 0.5);

        RaycastHit2D hit = Physics2D.Raycast(h, forward.normalized, 0.3f, obstacleLayer);

        if (hit.collider != null && JumpState == EnemyState.Grounded)
        {

            // �ж��Ƿ������Ĺ�ȥ
            if (hit.collider == lastHitCollider && Time.time - lastCheckTime < ( 1 + 0.5))
            {
                WanderState = EnemyWander.StartLose; // �л�����ʼ��ʧ���״̬
                movingRight = !movingRight;


            }

            // ���������ʱ��ͼ�⵽������
            lastCheckTime = Time.time;
            lastHitCollider = hit.collider;

            // ��ȡ������㵽��ײ��֮��ľ���
            float distanceToGround = hit.distance;
            if (distanceToGround < 0.3f)
            {
                float offset = (0.3f - distanceToGround) * forward.normalized.x;
                transform.position += new Vector3(-offset, 0, 0);
            }

            // �����ϰ���ʱ��Ծ
            JumpState = EnemyState.Jumping;
            JumpY.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
            transform.Translate(JumpY * Time.deltaTime);

            if (!IsAnimationPlaying("Jump")) {
                animator.Play("Jump");
            
            }
        }

        if (DrawLine)
        {
            Debug.DrawRay(h, forward * 0.3f, Color.blue);
        }
    }

    public void CheckGrounded()
    {
        // ����Ƿ��ڵ�����
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector3.down, 0.01f + (float)(transform.localScale.y * 0.5), GroundedLayer);
        if (groundHit.collider != null)
        {
            JumpY.y = 0; // ���ô�ֱ�ٶ�
            JumpState = EnemyState.Grounded;

            //// ��ȡ������㵽��ײ��֮��ľ���
            //float distanceToGround = groundHit.distance;

            //if (distanceToGround < transform.localScale.y * 0.5)
            //{
            //    float offset = (transform.localScale.y * 0.5f) - distanceToGround;
            //    transform.position += new Vector3(0, offset, 0);
            //}

        }
        else if (JumpState != EnemyState.Jumping)
        {
            JumpState = EnemyState.Falling;
        }

        if (DrawLine)
        {
            Debug.DrawRay(transform.position, Vector3.down *(0.01f + (float)(transform.localScale.y * 0.5)), Color.blue);
        }

        // Ӧ������
        if (JumpState != EnemyState.Grounded)
        {
            JumpY.y += Physics2D.gravity.y * Time.deltaTime;
        }

        // �ƶ�����
        transform.Translate(JumpY * Time.deltaTime);

    }

    // �������εĿ��ӻ���Ե
    void DrawSector(float radius, int rayCount)
    {
        // ����LineRenderer�Ķ����������������� + 1����ΪҪ���������յ㣩
        int positionCount = rayCount + 1;
        lineRenderer.positionCount = positionCount;

        Vector3[] vector3s = GetPoins(radius, rayCount);

        // ��LineRenderer�л������εı�Ե
        lineRenderer.SetPositions(vector3s);
    }


    // �������������ߣ�ʹ�� LineRenderer �Ķ���
    void FireSectorRays(float radius,int rayCount)
    {
        bool hasHit = false; // ����Ƿ����κ����߻���Ŀ��

        Vector3[] poins = GetPoins(radius , rayCount);

        // ���� LineRenderer ��ÿ�����㣬����Щ�㷢������
        for (int i = 0; i < poins.Length; i++)
        {
            Vector2 rayStart = transform.position; // ���ߴ���������ķ���
            Vector2 rayEnd = poins[i]; // ��ȡLineRenderer�Ķ���λ��
            Vector2 rayDirection = (rayEnd - rayStart).normalized; // �������ߵķ���

            // ��������
            RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, radius, targetLayer);

            if (DrawLine)
            {
                // �������ߣ����ڵ��ԣ�
                Debug.DrawRay(rayStart, rayDirection * radius, Color.red);
            }

            // �����������������
            if (hit.collider != null)
            {
                if (!hasHit) // �����û�����ù�PlayerValue��������
                {
                    PlayerValue = hit.collider.transform; // ���� PlayerValue
                    hasHit = true; // ����ѻ���Ŀ��
                    WanderState = EnemyWander.FindPlayer; // �л����ҵ����״̬
                }
            }
        }
        if (!hasHit)
        {
            if (WanderState == EnemyWander.FindPlayer)
            {
                if (radius == FollowDis)
                {
                    StartCoroutine(WaitChange());

                }
                else {
                    PlayerValue = null;
                    WanderState = EnemyWander.StartLose; // �л�����ʼ��ʧ���״̬

                }
               
            }
        }

    }

    IEnumerator WaitChange() {
        yield return new WaitForSeconds(WaitTime);
        PlayerValue = null;
        WanderState = EnemyWander.StartLose; // �л�����ʼ��ʧ���״̬
    }

    Vector3[] GetPoins(float radius ,int rayCount)
    {
        List<Vector3> poins = new List<Vector3>();

        Vector2 direction = RotateVector(Vector2.right, transform.rotation.eulerAngles.z); // ��׼����Ϊ�����Ҳ�

        // ����ÿ�����ߵĽǶȼ��
        float angleStep = (float)angle / rayCount;

        // �������εı�Ե
        for (int i = 0; i <= rayCount; i++)
        {
            // ��ǰ���ߵĽǶ�
            float currentAngle = -angle / 2 + angleStep * i;

            // �������ߵķ���
            Vector2 rayDirection = Quaternion.Euler(0, 0, currentAngle) * direction;
            Vector3 endPoint = transform.position + (Vector3)(rayDirection * radius);

            poins.Add(endPoint);
        }
        return poins.ToArray();
    }

    //IEnumerator Fire()
    //{
    //    isFiring = true;

    //    while (true)
    //    {
    //        if (WanderState == EnemyWander.FindPlayer)
    //        {
    //            yield return new WaitForSeconds(0.5f);

    //            enemyLaser.ChangeToRainBow(transform.position, PlayerValue.position - transform.position);

    //            yield return new WaitForSeconds(0.5f);

    //            RainBow.gameObject.SetActive(false);

    //            yield return new WaitForSeconds(3f);
    //        }
    //        else
    //        {
    //            break;
    //        }
    //    }

    //    RainBow.gameObject.SetActive(false);
    //    isFiring = false;
    //    yield break;
    //}

    // ��ת����
    private Vector2 RotateVector(Vector2 originalDirection, float angleInDegrees)
    {
        float angleInRadians = angleInDegrees * Mathf.Deg2Rad;
        float cos = Mathf.Cos(angleInRadians);
        float sin = Mathf.Sin(angleInRadians);

        float newX = originalDirection.x * cos - originalDirection.y * sin;
        float newY = originalDirection.x * sin + originalDirection.y * cos;

        return new Vector2(newX, newY).normalized;
    }

    public void Attacked() {
        Blood--;
        if (Blood <= 0) {

            Destroy(gameObject);
        }
    
    }
}
