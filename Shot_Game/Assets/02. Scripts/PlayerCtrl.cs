using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//클래스의 경우 Serializable(직렬화) 라는 속성을 반드시 명시해줘야함
//그래야만 인스펙터에 노출됨

[Serializable]
public class PlayerAnim
{
    public AnimationClip idle;
    public AnimationClip runB;
    public AnimationClip runF;
    public AnimationClip runL;
    public AnimationClip runR;

}

public class PlayerCtrl : MonoBehaviour
{
    float h = 0f;
    float v = 0f;
    float r = 0f; //회전값 저장할 변수
    



    Transform tr;
    public float moveSpeed = 10f; //이동 속도 계수
    public float rotSpeed = 100f; //회전 속도 계수

    //애니메이션 관련 변수들
    public PlayerAnim playerAnim;
    Animation anim;


    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        //현재 재생해야될 애니메이션 클립을 idle로 설정
        anim.clip = playerAnim.idle;
        anim.Play();
    }

    // Update is called once per frame
    void Update()
    {
        h = Input.GetAxis("Horizontal");
        v = Input.GetAxis("Vertical");
        r = Input.GetAxis("Mouse X");



        Vector3 moveDir = (Vector3.forward * v) + (Vector3.right * h);
        //벡터 정규화
        moveDir = moveDir.normalized;

        //Translate(방향 * 속도, 기준좌표)
        //기준좌표 = Local(Self) / Global
        //플레이어 입장에서는 Local 좌표계를 기준으로
        //앞뒤좌우로 움직이도록
        tr.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);

        //Rotate(축 방향 * 속도)
        //ex) 양꼬치 or 회오리감자
        tr.Rotate(Vector3.up * rotSpeed * r * Time.deltaTime);

        //애니메이션 전환
        if (v >= 0.1f)//앞
        {
            //V값이 양수 일 때 (전면 이동)
            //CrossFade(전환될 애니메이션의 이름, 전환 시간)
            anim.CrossFade(playerAnim.runF.name, 0.3f);
        }
        else if (v <= -0.1f) //뒤
        {
            anim.CrossFade(playerAnim.runB.name, 0.3f);
        }
        else if (h >= 0.1f) //오
        {
            anim.CrossFade(playerAnim.runR.name, 0.3f);
        }
        else if (h <= -0.1f) //왼
        {
            anim.CrossFade(playerAnim.runL.name, 0.3f);
        }
        else //아이들
        {
            anim.CrossFade(playerAnim.idle.name, 0.3f);
        }
    }
}
