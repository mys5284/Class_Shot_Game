using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//해당 어틑리뷰트를 통해서 필수컴포넌트 지정
[RequireComponent(typeof(MoveAgent))]
public class MoveAgent : MonoBehaviour
{
    //리스트는 가변 배열 길이로서
    //데이터가 추가/삭제 됨에 따라서 길이 및 인덱스가 바뀜
    public List<Transform> wayPoints;
    public int nextIdx; //다음 순찰지점의 인덱스

    NavMeshAgent agent;
    Transform enemyTr; //*

    //프로퍼티 관련
    readonly float patrolSpeed = 1.5f;
    readonly float traceSpeed = 4f;
    float damping = 1f; //회전 속도 조절 계수 *

    bool _patrolling; //실제 값이 저장되는 변수
    public bool patrolling //대외적으로 표출되는 프로퍼티
    {
        get
        {
            return _patrolling;
        }
        set
        {
            //value는 외부에서 전달되는 값
            _patrolling = value;
            if (_patrolling)
            {
                agent.speed = patrolSpeed;
                damping = 1f; //*
                MoveWayPoint();
            }
        }
    }

    Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return _traceTarget;}
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            damping = 7f; //*
            //대상 추적 메소드 호출
            TraceTarget(_traceTarget);
        }
    }

    public float speed
    {
        get{ return agent.velocity.magnitude; }
    }

    void Start()
    {
        enemyTr = GetComponent<Transform>(); //*
        agent = GetComponent<NavMeshAgent>();
        //목적지에 가까워질수록 속도 줄이는 옵션 비활성화
        agent.autoBraking = false;
        agent.updateRotation = false; //*


        agent.speed = patrolSpeed;

        //하이어라키뷰에서 WayPointGroup 이름의 오브젝트 검색
        var group = GameObject.Find("WayPointGroup");
        if (group != null)
        {
            //WayPointGroup 하위에 있는 모든 Transform 컴포넌트 추출
            //추출된 컴포넌트를 List wayPoints에 자동 추가
            //다만 이때에 주의할 점은 ~~~s InChildren 메소드는
            //부모 오브젝트가 0번째에 들어가고 이후 자식이 들어감
            group.GetComponentsInChildren<Transform>(wayPoints);
            //따라서, 0번째 index 요소를 지움으로서 부모 오브젝트 제외
            wayPoints.RemoveAt(0);

            //첫번째 순찰 위치를 랜덤하게 추출
            nextIdx = Random.Range(0, wayPoints.Count); //*
        }

        //웨이포인트로 움직이는 메소드 호출
       // MoveWayPoint();
       this.patrolling = true; //*
    }

    void MoveWayPoint()
    {  
        //에이전트가 최단경로 계산중이라면 이동 안함
        if(agent.isPathStale)
        {
            return;
        }
        //경로 계산 끝났으면
        //다음 목적지를 리스트에서 추출한 위치로 설정
        agent.destination = wayPoints[nextIdx].position;
        //에이전트 활성화
        agent.isStopped = false;
    }

    void TraceTarget(Vector3 pos) //추적
    {
        if (agent.isPathStale)
            return;

        agent.destination = pos;
        agent.isStopped = false;
    }
   
    public void Stop() //멈출때
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }

    void Update()
    {
        //이동 중이라면 *
        if(!agent.isStopped)
        {
            //NavMeshAgent가 가야할 방향을 쿼터니언 각도로 변환 *
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            //위에서 계산한 방향 각도를 토대로 Enemy의 rotation 값 변경 *
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }

        //순찰 상태 아닐경우(추적상태) 에는 순찰지점 변경 로직 수행X
        if(!_patrolling)    
            return ;
        
        //움직이는 중이나 목적지에 거의 도착한 상태
        //다음 목적지를 결정하기 위한 단계
        if (agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <= 0.5f)
        {
            //nextIdx++;
            //nextIdx = nextIdx % wayPoints.Count;
            nextIdx = Random.Range(0, wayPoints.Count); //*
            MoveWayPoint();
        }

    }
}
