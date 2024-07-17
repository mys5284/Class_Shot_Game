using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// EnemyFOV 라는 스크립트에 사용될 커스텀 에디터라고 명시
[CustomEditor(typeof(EnemyFOV))] 
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        //EnemyFOV 클래스
        EnemyFOV fov = (EnemyFOV)target;

        //원주 위의 시작점의 좌표를 계산(뷰앵글(120도의 1/2))
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);

        Handles.color = Color.white;
        //DrawWireDisc(원점 좌표, 노멀 벡트, 원의 반지름)
        Handles.DrawWireDisc(fov.transform.position, Vector3.up, fov.viewRange);

        Handles.color = new Color(1, 1, 1, 0.2f);
        //DrawSolidArc(원점 좌표, 노멀 베터, 부채꼴 시작위치, 부채꼴 각도, 부채꼴 범위)
        Handles.DrawSolidArc(fov.transform.position, Vector3.up, fromAnglePos, fov.viewAngle, fov.viewRange);
        //텍스트 출력
        Handles.Label(fov.transform.position + (fov.transform.forward * 2f), fov.viewAngle.ToString());
    }
}
