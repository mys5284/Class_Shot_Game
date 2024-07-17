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

    Transform playerTr; //�÷��̾� ��ġ ���� ����
    Transform enemyTr; //���� ��ġ ���� ����
    Animator animator;


    public float attackDist = 5f; //���� ��Ÿ�
    public float traceDist = 10f; //���� ��Ÿ�
    public bool isDie = false; //��� ���� �Ǵ� ����

    //�ڷ�ƾ���� ����� �ð� ���� ����
    WaitForSeconds ws;

    //Enemy�� �������� �����ϴ� MoveAgent ��ũ��Ʈ ��������
    MoveAgent moveAgent;

    EnemyFire enemyFire;

    //�ִϸ��̼� ���� �Ķ����
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
        //�ڷ�ƾ �Լ��� �ݵ�� StartCoroutine�� ���ؼ� ����
        StartCoroutine(CheckState());
        StartCoroutine(Action());

        //Damage ��ũ��Ʈ�� �ִ� ���� �̺�Ʈ��
        //EnemyAI ��ũ��Ʈ�� E_PlayerDie �޼ҵ带 ���������
        //�̺�Ʈ ���� �ÿ��� += ������ -= �� ��
        Damage.PlayerDieEvent += E_PlayerDie;
    }

    private void OnDisable()
    {
        Damage.PlayerDieEvent -= E_PlayerDie;
    }

    IEnumerator CheckState()
    {
        //������Ʈ Ǯ �� ���� ��ũ��Ʈ���� �غ� ������ ���� ��� ���
        yield return new WaitForSeconds(1f);

        //������ �ִµ��� ���� ����
        while (!isDie)
        {
            //���°� ����̸� �ڷ�ƾ �Լ� ����
            if (state == State.DIE)
            {
                yield break;
            }

            float dist = Vector3.Distance(playerTr.position, enemyTr.position);

            if (dist <= attackDist)
            {
                //�÷��̾ �� �þ߿� ���϶�(��ֹ� ����)

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

    //��ȭ�� State�� ���� ���� �ൿ�� ó���ϴ� �ڷ�ƾ �Լ�
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

                    //������� �ݶ��̴� ��Ȱ��ȭ ����� �Ѿ� �ȸ���
                    GetComponent<CapsuleCollider>().enabled = false;
                    break;
            }
        }
    }

    private void Update()
    {
        //MoveAgent�� ������ �ִ� speed ������Ƽ ����
        //�ִϸ������� speed �Ķ���Ϳ� �����Ͽ� �ӵ�����
        animator.SetFloat(hashSpeed, moveAgent.speed);
    }

    public void E_PlayerDie()
    {
        moveAgent.Stop();
        enemyFire.isFire = false;
        //�������� �ڷ�ƾ ��� ����
        StopAllCoroutines();
        animator.SetTrigger(hashPlayerDie);
    }
}
