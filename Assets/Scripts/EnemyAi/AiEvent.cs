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

    //[Header("检测范围")]
    public bool DrawLine;
    public int angle; // 扇形的角度
    public float Deteradius; // 扇形的半径
    //public int rayCount; // 射线数量
    public LayerMask targetLayer; // 目标检测层
    public float CrossRadius; // 徘徊半径
    public float WaitTime;



    //[Header("追击")]
    public float FollowDis;
    public float MoveSpeed;

    //[Header("重力与跳跃")]
    public LayerMask obstacleLayer; // 障碍物层
    public LayerMask GroundedLayer; // 地面层
    public float jumpHeight = 2f; // 跳跃高度

    LineRenderer lineRenderer;
    SpriteRenderer SpriteRenderer;

    [HideInInspector]
    public LineRenderer RainBow;
    Transform PlayerValue;
    [HideInInspector]
    public EnemyWander WanderState;
    [HideInInspector]
    public EnemyState JumpState; // 初始状态

    Vector3 initialPosition; // AI初始位置
     bool movingRight = true; // 是否向右移动
    bool CanMove = true;
     Vector3 JumpY; // 当前速度

    //[HideInInspector]
    //public EnemyLaser enemyLaser;

    //bool isFiring = false;
    public Animator animator;


    public void Start()
    {
        SpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
        initialPosition = transform.position; // 设置初始位置
        JumpState = EnemyState.Grounded;
        WanderState = EnemyWander.StartLose; // 初始状态为开始丢失玩家
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
            // 返回初始位置
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
                WanderState = EnemyWander.LosePlayer; // 到达初始位置后切换到丢失玩家状态
            }
        }

        //if (WanderState == EnemyWander.LosePlayer && CanMove)
        //{

        //    SpriteRenderer.flipX = movingRight;

        //    float direction = movingRight ? 1 : -1;
        //    transform.Translate(Vector3.right * direction * MoveSpeed * Time.deltaTime);

        //    // 限制在CrossRadius范围内
        //    Vector3 h = transform.position- initialPosition;
        //    h.y = 0;
        //    if (h.magnitude > CrossRadius + 0.8)
        //    {
        //        movingRight = !movingRight; // 切换方向

        //        StartCoroutine(WaitSameTime(WaitTime));
        //    }
        //}
    }


    //IEnumerator WaitSameTime(float time) {
    //    CanMove = false; // 停止移动
    //    yield return new WaitForSeconds(time);
    //    CanMove = true; // 恢复移动
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

            // 判断是否能跳的过去
            if (hit.collider == lastHitCollider && Time.time - lastCheckTime < ( 1 + 0.5))
            {
                WanderState = EnemyWander.StartLose; // 切换到开始丢失玩家状态
                movingRight = !movingRight;


            }

            // 更新最后检测时间和检测到的物体
            lastCheckTime = Time.time;
            lastHitCollider = hit.collider;

            // 获取射线起点到碰撞点之间的距离
            float distanceToGround = hit.distance;
            if (distanceToGround < 0.3f)
            {
                float offset = (0.3f - distanceToGround) * forward.normalized.x;
                transform.position += new Vector3(-offset, 0, 0);
            }

            // 遇到障碍物时跳跃
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
        // 检测是否在地面上
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector3.down, 0.01f + (float)(transform.localScale.y * 0.5), GroundedLayer);
        if (groundHit.collider != null)
        {
            JumpY.y = 0; // 重置垂直速度
            JumpState = EnemyState.Grounded;

            //// 获取射线起点到碰撞点之间的距离
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

        // 应用重力
        if (JumpState != EnemyState.Grounded)
        {
            JumpY.y += Physics2D.gravity.y * Time.deltaTime;
        }

        // 移动敌人
        transform.Translate(JumpY * Time.deltaTime);

    }

    // 绘制扇形的可视化边缘
    void DrawSector(float radius, int rayCount)
    {
        // 设置LineRenderer的顶点数量（射线数量 + 1，因为要包含起点和终点）
        int positionCount = rayCount + 1;
        lineRenderer.positionCount = positionCount;

        Vector3[] vector3s = GetPoins(radius, rayCount);

        // 在LineRenderer中绘制扇形的边缘
        lineRenderer.SetPositions(vector3s);
    }


    // 扇形区域发射射线，使用 LineRenderer 的顶点
    void FireSectorRays(float radius,int rayCount)
    {
        bool hasHit = false; // 标记是否有任何射线击中目标

        Vector3[] poins = GetPoins(radius , rayCount);

        // 遍历 LineRenderer 的每个顶点，从这些点发射射线
        for (int i = 0; i < poins.Length; i++)
        {
            Vector2 rayStart = transform.position; // 射线从物体的中心发出
            Vector2 rayEnd = poins[i]; // 获取LineRenderer的顶点位置
            Vector2 rayDirection = (rayEnd - rayStart).normalized; // 计算射线的方向

            // 发射射线
            RaycastHit2D hit = Physics2D.Raycast(rayStart, rayDirection, radius, targetLayer);

            if (DrawLine)
            {
                // 绘制射线（用于调试）
                Debug.DrawRay(rayStart, rayDirection * radius, Color.red);
            }

            // 如果射线碰到了物体
            if (hit.collider != null)
            {
                if (!hasHit) // 如果还没有设置过PlayerValue，设置它
                {
                    PlayerValue = hit.collider.transform; // 设置 PlayerValue
                    hasHit = true; // 标记已击中目标
                    WanderState = EnemyWander.FindPlayer; // 切换到找到玩家状态
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
                    WanderState = EnemyWander.StartLose; // 切换到开始丢失玩家状态

                }
               
            }
        }

    }

    IEnumerator WaitChange() {
        yield return new WaitForSeconds(WaitTime);
        PlayerValue = null;
        WanderState = EnemyWander.StartLose; // 切换到开始丢失玩家状态
    }

    Vector3[] GetPoins(float radius ,int rayCount)
    {
        List<Vector3> poins = new List<Vector3>();

        Vector2 direction = RotateVector(Vector2.right, transform.rotation.eulerAngles.z); // 基准方向为物体右侧

        // 计算每条射线的角度间隔
        float angleStep = (float)angle / rayCount;

        // 绘制扇形的边缘
        for (int i = 0; i <= rayCount; i++)
        {
            // 当前射线的角度
            float currentAngle = -angle / 2 + angleStep * i;

            // 计算射线的方向
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

    // 旋转向量
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
