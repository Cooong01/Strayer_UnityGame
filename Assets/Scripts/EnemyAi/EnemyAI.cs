using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : AiEvent
{

     Vector3 previousPosition; // 记录上一帧的位置
     Vector3 movementDirection; // 当前运动方向

    bool can = true;
    new void Start()
    {
        //base.enemyLaser = GetComponent<EnemyLaser>();
        base.Start(); // 调用父类的 Start 方法
        StartCoroutine(MakeRay());
    }

    void Update()
    {
        if (can) {

            if (WanderState == EnemyWander.FindPlayer)
            {
                base.FindPlayer();
            }
            else
            {
                base.LosePlayer();
            }

            // 计算当前运动方向
            movementDirection = (transform.position - previousPosition).normalized;
            previousPosition = transform.position; // 更新上一帧的位置
        }



    }
    void FixedUpdate()
    {
        base.CheckGrounded();
    }

    IEnumerator MakeRay()
    {
        while (true)
        {
            base.FireRay();
            base.CheckObstacle(GetMovementDirection());
            yield return null;
        }
    }

    Vector3 GetMovementDirection()
    {
        return movementDirection;
    }

    public void StopMove() {
        can = false;
        animator.Play("Dead");
    }
}
