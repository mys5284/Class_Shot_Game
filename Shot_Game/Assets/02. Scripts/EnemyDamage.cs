using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    const string bulletTag = "BULLET";

    float iniHp = 100f; //초기 피통
    float hp = 100f;
    GameObject bloodEffect;

    public GameObject hpBarPrefab; 
    public Vector3 hpBaroffset = new Vector3(0f, 2.2f, 0f); 

    Canvas uiCanvas; 
    Image hpBarImage; 

    void Start()
    {
        //Resources.Load<GameObject>("파일경로")
        bloodEffect = Resources.Load<GameObject>("Blood");
        SetHpBar();
    }

    void SetHpBar() 
    {
        uiCanvas = GameObject.Find("UI_Canvas").GetComponent<Canvas>(); 
        //hpBar 를 동적 생성하면서 캔버스의 자식으로 넣어줌
        GameObject hpBar = Instantiate(hpBarPrefab, uiCanvas.transform);
        //hpBarImage = 빨간색
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];

        //체력바가 따라가야할 대상과 오프셋 설정
        var enemyHpBar = hpBar.GetComponent<EnemyHpBar>();
        enemyHpBar.targetTr = this.gameObject.transform;
        enemyHpBar.offset = hpBaroffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag(bulletTag))
        {
            //혈흔효과 생성 함수 호출
            ShowBloodEffect(collision);

           // Destroy(collision.gameObject);
           collision.gameObject.SetActive(false);


            //BulletCtrl에 작성한 damage 변수의 값을 가져와서
            //체력에서 - 해줌
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
            //체력바의 빨간색 게이지 줄이기
            hpBarImage.fillAmount = hp / iniHp; 

            if(hp <= 0f) 
            {
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;

                //게임 매니저의 킬 카운트 증가 함수 호출 //*
                GameManager.instance.InKillCount();
                //사망 후 콜라이더 비활성화 //*
                GetComponent<CapsuleCollider>().enabled = false;
            }
        }
    }

    void ShowBloodEffect(Collision coll)
    {
        //충돌지점 위치 가져오기
        Vector3 pos = coll.contacts[0].point;

        //충돌 지점의 법선벡터 구하기
        Vector3 _nomal = coll.contacts[0].normal;

        //총알의 탄두와 마주보는 방향값 구하기
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _nomal);

        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(blood, 1f);

    }

    
}
