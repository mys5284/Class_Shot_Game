using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


//�ش� ��z����Ʈ�� ���ؼ� �ʼ�������Ʈ ����
[RequireComponent(typeof(MoveAgent))]
public class MoveAgent : MonoBehaviour
{
    //����Ʈ�� ���� �迭 ���̷μ�
    //�����Ͱ� �߰�/���� �ʿ� ���� ���� �� �ε����� �ٲ�
    public List<Transform> wayPoints;
    public int nextIdx; //���� ���������� �ε���

    NavMeshAgent agent;
    Transform enemyTr; //*

    //������Ƽ ����
    readonly float patrolSpeed = 1.5f;
    readonly float traceSpeed = 4f;
    float damping = 1f; //ȸ�� �ӵ� ���� ��� *

    bool _patrolling; //���� ���� ����Ǵ� ����
    public bool patrolling //��������� ǥ��Ǵ� ������Ƽ
    {
        get
        {
            return _patrolling;
        }
        set
        {
            //value�� �ܺο��� ���޵Ǵ� ��
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
            //��� ���� �޼ҵ� ȣ��
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
        //�������� ����������� �ӵ� ���̴� �ɼ� ��Ȱ��ȭ
        agent.autoBraking = false;
        agent.updateRotation = false; //*


        agent.speed = patrolSpeed;

        //���̾��Ű�信�� WayPointGroup �̸��� ������Ʈ �˻�
        var group = GameObject.Find("WayPointGroup");
        if (group != null)
        {
            //WayPointGroup ������ �ִ� ��� Transform ������Ʈ ����
            //����� ������Ʈ�� List wayPoints�� �ڵ� �߰�
            //�ٸ� �̶��� ������ ���� ~~~s InChildren �޼ҵ��
            //�θ� ������Ʈ�� 0��°�� ���� ���� �ڽ��� ��
            group.GetComponentsInChildren<Transform>(wayPoints);
            //����, 0��° index ��Ҹ� �������μ� �θ� ������Ʈ ����
            wayPoints.RemoveAt(0);

            //ù��° ���� ��ġ�� �����ϰ� ����
            nextIdx = Random.Range(0, wayPoints.Count); //*
        }

        //��������Ʈ�� �����̴� �޼ҵ� ȣ��
       // MoveWayPoint();
       this.patrolling = true; //*
    }

    void MoveWayPoint()
    {  
        //������Ʈ�� �ִܰ�� ������̶�� �̵� ����
        if(agent.isPathStale)
        {
            return;
        }
        //��� ��� ��������
        //���� �������� ����Ʈ���� ������ ��ġ�� ����
        agent.destination = wayPoints[nextIdx].position;
        //������Ʈ Ȱ��ȭ
        agent.isStopped = false;
    }

    void TraceTarget(Vector3 pos) //����
    {
        if (agent.isPathStale)
            return;

        agent.destination = pos;
        agent.isStopped = false;
    }
   
    public void Stop() //���⶧
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }

    void Update()
    {
        //�̵� ���̶�� *
        if(!agent.isStopped)
        {
            //NavMeshAgent�� ������ ������ ���ʹϾ� ������ ��ȯ *
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            //������ ����� ���� ������ ���� Enemy�� rotation �� ���� *
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }

        //���� ���� �ƴҰ��(��������) ���� �������� ���� ���� ����X
        if(!_patrolling)    
            return ;
        
        //�����̴� ���̳� �������� ���� ������ ����
        //���� �������� �����ϱ� ���� �ܰ�
        if (agent.velocity.sqrMagnitude >= 0.2f * 0.2f && agent.remainingDistance <= 0.5f)
        {
            //nextIdx++;
            //nextIdx = nextIdx % wayPoints.Count;
            nextIdx = Random.Range(0, wayPoints.Count); //*
            MoveWayPoint();
        }

    }
}
