using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : AiEvent
{

     Vector3 previousPosition; // ��¼��һ֡��λ��
     Vector3 movementDirection; // ��ǰ�˶�����

    bool can = true;
    new void Start()
    {
        //base.enemyLaser = GetComponent<EnemyLaser>();
        base.Start(); // ���ø���� Start ����
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

            // ���㵱ǰ�˶�����
            movementDirection = (transform.position - previousPosition).normalized;
            previousPosition = transform.position; // ������һ֡��λ��
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
