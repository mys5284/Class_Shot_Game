using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    const string bulletTag = "BULLET";

    float iniHp = 100f; //�ʱ� ����
    float hp = 100f;
    GameObject bloodEffect;

    public GameObject hpBarPrefab; 
    public Vector3 hpBaroffset = new Vector3(0f, 2.2f, 0f); 

    Canvas uiCanvas; 
    Image hpBarImage; 

    void Start()
    {
        //Resources.Load<GameObject>("���ϰ��")
        bloodEffect = Resources.Load<GameObject>("Blood");
        SetHpBar();
    }

    void SetHpBar() 
    {
        uiCanvas = GameObject.Find("UI_Canvas").GetComponent<Canvas>(); 
        //hpBar �� ���� �����ϸ鼭 ĵ������ �ڽ����� �־���
        GameObject hpBar = Instantiate(hpBarPrefab, uiCanvas.transform);
        //hpBarImage = ������
        hpBarImage = hpBar.GetComponentsInChildren<Image>()[1];

        //ü�¹ٰ� ���󰡾��� ���� ������ ����
        var enemyHpBar = hpBar.GetComponent<EnemyHpBar>();
        enemyHpBar.targetTr = this.gameObject.transform;
        enemyHpBar.offset = hpBaroffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag(bulletTag))
        {
            //����ȿ�� ���� �Լ� ȣ��
            ShowBloodEffect(collision);

           // Destroy(collision.gameObject);
           collision.gameObject.SetActive(false);


            //BulletCtrl�� �ۼ��� damage ������ ���� �����ͼ�
            //ü�¿��� - ����
            hp -= collision.gameObject.GetComponent<BulletCtrl>().damage;
            //ü�¹��� ������ ������ ���̱�
            hpBarImage.fillAmount = hp / iniHp; 

            if(hp <= 0f) 
            {
                GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
                hpBarImage.GetComponentsInParent<Image>()[1].color = Color.clear;

                //���� �Ŵ����� ų ī��Ʈ ���� �Լ� ȣ�� //*
                GameManager.instance.InKillCount();
                //��� �� �ݶ��̴� ��Ȱ��ȭ //*
                GetComponent<CapsuleCollider>().enabled = false;
            }
        }
    }

    void ShowBloodEffect(Collision coll)
    {
        //�浹���� ��ġ ��������
        Vector3 pos = coll.contacts[0].point;

        //�浹 ������ �������� ���ϱ�
        Vector3 _nomal = coll.contacts[0].normal;

        //�Ѿ��� ź�ο� ���ֺ��� ���Ⱚ ���ϱ�
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _nomal);

        GameObject blood = Instantiate<GameObject>(bloodEffect, pos, rot);
        Destroy(blood, 1f);

    }

    
}
