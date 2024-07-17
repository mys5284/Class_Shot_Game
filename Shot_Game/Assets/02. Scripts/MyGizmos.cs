using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public enum Type { PATROLPOINT, SPAWNPOINT};
    const string spawnPointImg = "Enemy";
    public Type type = Type.PATROLPOINT;

    public Color _color = Color.yellow;
    public float _radius = 0.1f;

    private void OnDrawGizmos()
    {
        //기즈모의 타입이 경계라면
        if (type == Type.PATROLPOINT)
        {
            //기즈모 색상
            Gizmos.color = _color;
            //기즈모 모양(위치, 크기)
            Gizmos.DrawSphere(transform.position, _radius);
        }
        else //리스폰 포인트면
        {
            Gizmos.color= _color;

            //DrawIcon(위치, 이미지 파일명, 스케일 적용 여부)
            //스케일 적용 여부의 경우는 씬뷰의 카메라 줌 인/아웃에
            //따라서 아이콘 크기가 커지고 작아지는 효과
            Gizmos.DrawIcon(transform.position + Vector3.up, spawnPointImg, true);
            Gizmos.DrawSphere(transform.position, _radius);
        }
    }
}
