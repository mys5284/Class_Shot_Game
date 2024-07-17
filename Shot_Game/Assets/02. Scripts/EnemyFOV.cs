using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    public float viewRange = 15f; //���� �����Ÿ� ����
    [Range(0, 360)]
    public float viewAngle = 120f; //�þ߰� ����

    Transform enemyTr;
    Transform playerTr;
    int playerLayer;
    int obstacleLayer;
    int layerMask;

    void Start()
    {
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").transform;

        playerLayer = LayerMask.NameToLayer("PLAYER");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE");
        layerMask = 1 << playerLayer | 1 << obstacleLayer;
    }

    //�������� �� ��ǥ�� ��ȯ�ϴ� �޼ҵ�
    public Vector3 CirclePoint(float angle)
    {
        //���� ���� ��ǥ�踦 �������� ����� �ؾ���
        //���� y�� ȸ������ ���� 
        angle += transform.rotation.y;
        //�⺻���� �ﰢ�Լ��� ���Ȱ��� ����������
        //���� �츮�� ����ϴ� ��׸�(~~~��) ���� �������� ��ȯ���ֱ� ���Ͽ�
        //Mathf.Deg2Rad �� ������
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public bool isTracePlayer()
    {
        bool isTrace = false;
        //���� ���� ������ �÷��̾� ����
        Collider[] colls = Physics.OverlapSphere(enemyTr.position, viewRange, 1 << playerLayer);

        //�������� �÷��̾ �����Ѵٸ�
        if (colls.Length == 1)
        {
            //Enemy�� �÷��̾��� ������ ���� ���� ���
            Vector3 dir = (playerTr.position - enemyTr.position).normalized;

            //�þ߰� ������ �����ϴ��� Ȯ��
            //Angle(A, B) = A���� B ���� ���հ��� ���
            //���� ������ ���� �÷��̾� ������ ������ +- 60�� �̳����� Ȯ��
            if (Vector3.Angle(enemyTr.forward, dir) < viewAngle * 0.5f)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }

    //Enemy�� �þ߿� �÷��̾ ���̴��� Ȯ���ϴ� �޼ҵ�
    public bool isViewPlayer()
    {
        bool isView = false;
        RaycastHit hit;
        Vector3 dir = (playerTr.position - enemyTr.position).normalized;

        if(Physics.Raycast(enemyTr.position, dir, out hit, viewRange, layerMask))
        {
            isView = (hit.collider.CompareTag("PLAYER"));
        }    
        return isView;
    }
}
