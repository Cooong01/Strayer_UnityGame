using UnityEngine;
public class EnemyAiJump : MonoBehaviour
{
    public Transform player; // ���λ��
    public float moveSpeed = 2f; // �ƶ��ٶ�
    public float jumpHeight = 2f; // ��Ծ�߶�
    public LayerMask obstacleLayer; // �ϰ����
    public float detectionDistance = 1f; // �ϰ��������
    public float groundCheckDistance = 0.1f; // ������ľ���
    public LayerMask groundLayer; // �����

    private Vector3 velocity; // ��ǰ�ٶ�
    EnemyState state = EnemyState.Grounded; // ��ʼ״̬

    void Update()
    {
        // ���ǰ���ϰ���
        CheckObstacle();


        // Ӧ���ٶ�
        ApplyMovement();

        // ����Ƿ��ڵ�����
        CheckGrounded();

    }


    void CheckObstacle()
    {
        Vector3 forward = velocity;
        forward.y = 0;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, forward, detectionDistance, obstacleLayer);

        if (hit.collider != null && state == EnemyState.Grounded)
        {
            // �����ϰ���ʱ��Ծ
            Jump();
        }
        else
        {
            // ׷�����
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
        // ����Ƿ��ڵ�����
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        if (groundHit.collider != null)
        {
            state = EnemyState.Grounded;
            velocity.y = 0; // ���ô�ֱ�ٶ�

        }
        //�ڵذ�Ϊ��⣬��������jump�������Ķ���Ϊfall
        else if (state != EnemyState.Jumping)
        {
            state = EnemyState.Falling;
        }
    }

    void ApplyMovement()
    {
        // Ӧ������
        if (state != EnemyState.Grounded)
        {
            velocity.y += Physics2D.gravity.y * Time.deltaTime;
        }

        // �ƶ�����
        transform.Translate(velocity * Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        // �ڱ༭���л��Ƽ������
        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.right * detectionDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundCheckDistance);
    }
}
