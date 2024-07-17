using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    Camera uiCamera; //ĵ���� ��� �ִ� ī�޶�
    Canvas canvas;

    RectTransform rectParent;
    RectTransform rectHp;

    [HideInInspector]
    public Vector3 offset = Vector3.zero; //HpBar �̹����� ��ġ ������ ������
    [HideInInspector]
    public Transform targetTr; //���� ��� Transform ������Ʈ

    void Start()
    {
        //�������� �� �� �θ��� ĵ������ �������� ���Ͽ�
        //InParent �� �����
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = this.gameObject.GetComponent<RectTransform>();
    }


    void LateUpdate()
    {
        //���� ��ǥ�� > ��ũ�� ��ǥ�� ��ȯ
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position +  offset);

        //ī�޶� �������� �� �� ��ǥ�� ����
        if(screenPos.z < 0f)
        {
            screenPos *= -1f;
        }
        //RectTransform ��ǥ���� ���޹��� ����
        var localPos = Vector2.zero;
        //��ũ�� ��ǥ > ��ƮƮ������ ��ǥ�� ��ȯ
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

        //���������� ��ȯ�� RectTransform ��ǥ�� rectHp �� ����
        rectHp.localPosition = localPos;
    }
}
