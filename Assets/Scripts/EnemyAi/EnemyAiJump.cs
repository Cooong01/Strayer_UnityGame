using UnityEngine;
public class EnemyAiJump : MonoBehaviour
{
    public Transform player; // 玩家位置
    public float moveSpeed = 2f; // 移动速度
    public float jumpHeight = 2f; // 跳跃高度
    public LayerMask obstacleLayer; // 障碍物层
    public float detectionDistance = 1f; // 障碍物检测距离
    public float groundCheckDistance = 0.1f; // 检测地面的距离
    public LayerMask groundLayer; // 地面层

    private Vector3 velocity; // 当前速度
    EnemyState state = EnemyState.Grounded; // 初始状态

    void Update()
    {
        // 检测前方障碍物
        CheckObstacle();


        // 应用速度
        ApplyMovement();

        // 检测是否在地面上
        CheckGrounded();

    }


    void CheckObstacle()
    {
        Vector3 forward = velocity;
        forward.y = 0;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, forward, detectionDistance, obstacleLayer);

        if (hit.collider != null && state == EnemyState.Grounded)
        {
            // 遇到障碍物时跳跃
            Jump();
        }
        else
        {
            // 追击玩家
            ChasePlayer();
        }
    }


    void Jump()
    {
        velocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics2D.gravity.y));
        state = EnemyState.Jumping;
    }

    void ChasePlayer()
    {
        if (player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            velocity.x = direction.x * moveSpeed;
        }
    }

    void CheckGrounded()
    {
        // 检测是否在地面上
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        if (groundHit.collider != null)
        {
            state = EnemyState.Grounded;
            velocity.y = 0; // 重置垂直速度

        }
        //在地板为检测，除非特殊jump，其他的都改为fall
        else if (state != EnemyState.Jumping)
        {
            state = EnemyState.Falling;
        }
    }

    void ApplyMovement()
    {
        // 应用重力
        if (state != EnemyState.Grounded)
        {
            velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }

        // 移动敌人
        transform.Translate(velocity * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        // 在编辑器中绘制检测射线
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * detectionDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
