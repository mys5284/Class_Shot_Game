using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    public float viewRange = 15f; //추적 사정거리 범위
    [Range(0, 360)]
    public float viewAngle = 120f; //시야각 범위

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

    //원주위의 점 좌표를 반환하는 메소드
    public Vector3 CirclePoint(float angle)
    {
        //적의 조컬 좌표계를 기준으로 계산을 해야함
        //따라서 y축 회전값을 더함 
        angle += transform.rotation.y;
        //기본적인 삼각함수는 라디안값을 기준으로함
        //따라서 우리가 사용하는 디그리(~~~도) 값을 라디안으로 변환해주기 위하여
        //Mathf.Deg2Rad 을 곱해줌
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0, Mathf.Cos(angle * Mathf.Deg2Rad));
    }

    public bool isTracePlayer()
    {
        bool isTrace = false;
        //추적 범위 내에서 플레이어 검출
        Collider[] colls = Physics.OverlapSphere(enemyTr.position, viewRange, 1 << playerLayer);

        //범위내에 플레이어가 존재한다면
        if (colls.Length == 1)
        {
            //Enemy와 플레이어의 사이의 방향 벡터 계산
            Vector3 dir = (playerTr.position - enemyTr.position).normalized;

            //시야각 범위에 존재하는지 확인
            //Angle(A, B) = A에서 B 까지 사잇각을 계산
            //따라서 위에서 계산된 플레이어 사이의 각도가 +- 60도 이내인지 확인
            if (Vector3.Angle(enemyTr.forward, dir) < viewAngle * 0.5f)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }

    //Enemy의 시야에 플레이어가 보이는지 확인하는 메소드
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
