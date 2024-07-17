using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyAI : MonoBehaviour
{
    public enum State
    {
        PATROL,
        TRACE,
        ATTACK,
        DIE
    }

    public State state = State.PATROL;

    Transform playerTr; //플레이어 위치 저장 변수
    Transform enemyTr; //적의 위치 저장 변수
    Animator animator;


    public float attackDist = 5f; //공격 사거리
    public float traceDist = 10f; //추적 사거리
    public bool isDie = false; //사망 여부 판단 변수

    //코루틴에서 사용할 시간 지연 변수
    WaitForSeconds ws;

    //Enemy의 움직임을 제어하는 MoveAgent 스크립트 가져오기
    MoveAgent moveAgent;

    EnemyFire enemyFire;

    //애니메이션 제어 파라미터
    readonly int hashMove = Animator.StringToHash("IsMove");
    readonly int hashSpeed = Animator.StringToHash("Speed");
    readonly int hashDie = Animator.StringToHash("Die"); 
    readonly int hashDieIdx = Animator.StringToHash("DieIdx"); 
    readonly int hashOffset = Animator.StringToHash("Offset");
    readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed"); 
    readonly int hashPlayerDie = Animator.StringToHash("PlayerDie");

    EnemyFOV enemyFOV;

    void Awake()
    {
        
        var player = GameObject.FindGameObjectWithTag("PLAYER");
        if (player != null)
        {
            playerTr = player.GetComponent<Transform>();
        }
        enemyTr = GetComponent<Transform>();
        moveAgent = GetComponent<MoveAgent>();
        enemyFire = GetComponent<EnemyFire>();  
        animator = GetComponent<Animator>();
        enemyFOV = GetComponent<EnemyFOV>();

        ws = new WaitForSeconds(0.3f);

        animator.SetFloat(hashOffset, Random.Range(0f, 1f));
        animator.SetFloat(hashWalkSpeed, Random.Range(1f, 1.2f));
    }

    private void OnEnable()
    {
        //코루틴 함수는 반드시 StartCoroutine을 통해서 실행
        StartCoroutine(CheckState());
        StartCoroutine(Action());

        //Damage 스크립트에 있는 정적 이벤트에
        //EnemyAI 스크립트의 E_PlayerDie 메소드를 연결시켜줌
        //이벤트 연결 시에는 += 뺄때는 -= 을 씀
        Damage.PlayerDieEvent += E_PlayerDie;
    }

    private void OnDisable()
    {
        Damage.PlayerDieEvent -= E_PlayerDie;
    }

    IEnumerator CheckState()
    {
        //오브젝트 풀 등 여러 스크립트들이 준비가 끝날때 까지 잠시 대기
        yield return new WaitForSeconds(1f);

        //생존해 있는동안 무한 루프
        while (!isDie)
        {
            //상태가 사망이면 코루틴 함수 종료
            if (state == State.DIE)
            {
                yield break;
            }

            float dist = Vector3.Distance(playerTr.position, enemyTr.position);

            if (dist <= attackDist)
            {
                //플레이어가 내 시야에 보일때(장애물 없이)

                if (enemyFOV.isViewPlayer())
                    state = State.ATTACK;
                else
                    state = State.TRACE;
            }
            else if (enemyFOV.isTracePlayer())
            {
                state = State.TRACE;
            }
            else
            {
                state = State.PATROL;
            }
            yield return ws;
        }
    }

    //변화된 State에 따라서 실제 행동을 처리하는 코루틴 함수
    IEnumerator Action()
    {
        while (!isDie)
        {
            yield return ws;

            switch(state)
            {
                case State.PATROL:
                    enemyFire.isFire = false;
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.TRACE:
                    enemyFire.isFire = false;
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    if(!enemyFire.isFire)
                    {
                        enemyFire.isFire = true;
                    }
                    break;
                case State.DIE:
                    this.gameObject.tag = "Untagged";

                    isDie = true; 
                    enemyFire.isFire = false; 
                    moveAgent.Stop();
                    animator.SetInteger(hashDieIdx, Random.Range(0, 3)); 
                    animator.SetTrigger(hashDie); 

                    //사망이후 콜라이더 비활성화 해줘야 총알 안맞음
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    private void Update()
    {
        //MoveAgent가 가지고 있는 speed 프로퍼티 값을
        //애니메이터의 speed 파라미터에 전달하여 속도변경
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    public void E_PlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        //동작중인 코루틴 모두 종료
        StopAllCoroutines();
        animator.SetTrigger(hashPlayerDie);
    }
}
