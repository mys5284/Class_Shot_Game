using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour
{
    public Transform target;
    public float moveDamping = 15f;  //이동 속도 계수
    public float rotateDamping = 10f;  //회전속도 계수
    public float distance = 5f;  //추적 대상과의 거리
    public float height = 4f;  //추적 대상과의 높이
    public float targetOffset = 2f;  //추적 좌표의 오프셋

    Transform tr;

    [Header("장애물 감지 관련")]
    public float heightAboveWall = 7f;
    public float colliderRadius = 1f;
    public float overDamping = 5f;
    float originHeight;

    [Header("장애물 감지 관련")]
    public float heightAboveObstacle = 12f;
    //플레이어한테 뿌릴 레이캐스트 높이 오프셋
    public float castOffset = 1f;

    void Start()
    {
        tr = GetComponent<Transform>();
        originHeight = height;
    }
   
    private void Update() //*
    {
        #region 벽 충돌
        //CheckSphere(생성위치, 반경)
        //충돌 유무 체크해서 충돌할 경우 높이를 부드럽게 상승
        if(Physics.CheckSphere(tr.position, colliderRadius))
        {
            height = Mathf.Lerp(height, heightAboveWall, Time.deltaTime * overDamping);
        }
        else //뭔가 충돌을 안하면 원래 높이로 부드럽게 조정
        {
            height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
        }
        #endregion

        #region 장애물 충돌
        //레이캐스트가 겨냥함 지점을 설정
        //플레이어의 발바닥 위치에서 조금 윗쪽
        Vector3 castTarget = target.position + (target.up * castOffset);
        //방향 벡터 계산
        Vector3 castDir = (castTarget - tr.position).normalized;

        RaycastHit hit;
        if(Physics.Raycast(tr.position, castDir, out hit, Mathf.Infinity))
        {
            //플레이어가 레이캐스트에 충돌하지 않았다 = 장애물이 있다
            if(!hit.collider.CompareTag("PLAYER"))
            {
                height = Mathf.Lerp(height, heightAboveObstacle, Time.deltaTime * overDamping);
            }
            else
            {
                height = Mathf.Lerp(height, originHeight, Time.deltaTime * overDamping);
            }
        }

        #endregion

    }

    void LateUpdate()
    {
        var camPos = target.position - (target.forward * distance) + (target.up * height);
        // var 변수는 가변 자료형이라고 볼 수 있지만, 처음 값이 설정될 때 지정된 타입으로 굳혀진다.
        // 이후 타입의 변경은 불가하다
        // var는 Vector3 값이지만 가변자료형을 사용하는 이유는 유연성을 위해서 사용한다.
        //object와 var의 차이 
        //var은 스택에 싸이는 값형식 object는 힙에 메모리 참조형으로 사용됨.

        //이동할 때 속도 계수 적용
        tr.position = Vector3.Lerp(tr.position, camPos, Time.deltaTime * moveDamping);

        //회전할 때 속도 계수 적용
        tr.rotation = Quaternion.Slerp(tr.rotation, target.rotation, Time.deltaTime * rotateDamping);

        //카메라가 위치 및 회전 이동 후에 타겟을 바라보도록
        tr.LookAt(target.position + (target.up * targetOffset));
        
               
    }

    //기즈모라는 씬뷰에서 보이는 가상의 라인들을 확인하기 위한 메소드
    private void OnDrawGizmos()
    {
        //기즈모의 색상 지정
        Gizmos.color = Color.green;
        //DrawWireSphere(위치, 반경)
        //기즈모에는 몇가지 지정된 모양이 있음
        //DrawWireSphere는 선으로 이루어진 구형 모양
        Gizmos.DrawWireSphere(target.position + (target.up * targetOffset), 0.1f);
        //메인카메라와 대상간의 선을 표시
        //DrawLine(A위치, B위치) = A와 B 사이 선 긋기
        Gizmos.DrawLine(target.position + (target.up * targetOffset), transform.position);

        //카메라의 충돌체 표현
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, colliderRadius);

        //플레이어 캐릭터가 장애물에 가려졌는지 판단할 레이
        Gizmos.color = Color.red;
        Gizmos.DrawLine(target.position + (target.up * castOffset), transform.position);
    }

  
}
