using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

// EnemyFOV ��� ��ũ��Ʈ�� ���� Ŀ���� �����Ͷ�� ���
[CustomEditor(typeof(EnemyFOV))] 
public class FOVEditor : Editor
{
    private void OnSceneGUI()
    {
        //EnemyFOV Ŭ����
        EnemyFOV fov = (EnemyFOV)target;

        //���� ���� �������� ��ǥ�� ���(��ޱ�(120���� 1/2))
        Vector3 fromAnglePos = fov.CirclePoint(-fov.viewAngle * 0.5f);

        Handles.color = Color.white;
        //DrawWireDisc(���� ��ǥ, ��� ��Ʈ, ���� ������)
        Handles.DrawWireDisc(fov.transform.position, Vector3.up, fov.viewRange);

        Handles.color = new Color(1, 1, 1, 0.2f);
        //DrawSolidArc(���� ��ǥ, ��� ����, ��ä�� ������ġ, ��ä�� ����, ��ä�� ����)
        Handles.DrawSolidArc(fov.transform.position, Vector3.up, fromAnglePos, fov.viewAngle, fov.viewRange);
        //�ؽ�Ʈ ���
        Handles.Label(fov.transform.position + (fov.transform.forward * 2f), fov.viewAngle.ToString());
    }
}
