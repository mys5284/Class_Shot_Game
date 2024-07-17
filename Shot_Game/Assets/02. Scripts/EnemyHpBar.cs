using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHpBar : MonoBehaviour
{
    Camera uiCamera; //캔버스 찍고 있는 카메라
    Canvas canvas;

    RectTransform rectParent;
    RectTransform rectHp;

    [HideInInspector]
    public Vector3 offset = Vector3.zero; //HpBar 이미지의 위치 조절용 오프셋
    [HideInInspector]
    public Transform targetTr; //추적 대상 Transform 컴포넌트

    void Start()
    {
        //동적생성 될 때 부모인 캔버스를 가져오기 위하여
        //InParent 를 사용함
        canvas = GetComponentInParent<Canvas>();
        uiCamera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = this.gameObject.GetComponent<RectTransform>();
    }


    void LateUpdate()
    {
        //월드 좌표를 > 스크린 좌표로 변환
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position +  offset);

        //카메라가 뒷쪽으로 갈 때 좌표값 보정
        if(screenPos.z < 0f)
        {
            screenPos *= -1f;
        }
        //RectTransform 좌표값을 전달받을 변수
        var localPos = Vector2.zero;
        //스크린 좌표 > 렉트트랜스폼 좌표로 변환
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rectParent, screenPos, uiCamera, out localPos);

        //최종적으로 변환된 RectTransform 좌표를 rectHp 에 전달
        rectHp.localPosition = localPos;
    }
}
