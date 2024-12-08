using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    void OnEnter();// ����״̬ʱ�ķ���
    void OnUpdate();// ���·���
    void OnFixedUpdate();// �̶����·���
    void OnExit();// �˳�״̬ʱ�ķ���
}

