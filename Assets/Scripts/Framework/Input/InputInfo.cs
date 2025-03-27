using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������Ϣ
/// </summary>
public class InputInfo
{
    public enum E_KeyOrMouse
    {
        // ��������
        Key,
        // �������
        Mouse,
    }

    public enum E_InputType
    {
        // ����
        Down,
        // ̧��
        Up,
        // ����
        Always,
    }

    //�����豸�����͡������̻������
    public E_KeyOrMouse keyOrMouse;
    //���붯�������͡���̧�𡢰��¡�����
    public E_InputType inputType;
    //KeyCode
    public KeyCode key;
    //mouseID
    public int mouseID;

    /// <summary>
    /// ���������ʼ��
    /// </summary>
    public InputInfo(E_InputType inputType, KeyCode key)
    {
        this.keyOrMouse = E_KeyOrMouse.Key;
        this.inputType = inputType;
        this.key = key;
    }

    /// <summary>
    /// ��������ʼ��
    /// </summary>
    public InputInfo(E_InputType inputType, int mouseID)
    {
        this.keyOrMouse = E_KeyOrMouse.Mouse;
        this.inputType = inputType;
        this.mouseID = mouseID;
    }
}
