using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Damage : MonoBehaviour
{
    const string bulletTag = "BULLET";
    const string enemyTag = "ENEMY";

    float initHp = 100f;
    public float currHp;

    //��������Ʈ �� �̺�Ʈ ����
    public delegate void PlayerDieHandler();
    //��������Ʈ���� �Ļ��� �̺�Ʈ
    public static event PlayerDieHandler PlayerDieEvent;

    public Image bloodScreen;

    public Image hpBar;
    readonly Color initColor = new Vector4(0, 1f, 0f, 1f);
    Color currColor;

    void Start()
    {
        currHp = initHp;

        //ü�¹� �ʱ� ���� ����
        hpBar.color = initColor;
        currColor = initColor;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(bulletTag))
        {
            Destroy(other.gameObject);

            //���� ȭ�� ȿ�� �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(ShowBloodScreen());

            currHp -= 5;
            DisplayHpBar();
           // print(currHp);

            if(currHp <= 0f )
            {
                //�÷��̾� ���� �޼ҵ� ȣ��
                PlayerDie();
            }
        }
    }

    IEnumerator ShowBloodScreen()
    {
        //�������� ������ 2 ~ 30 �� ������ �����ϰ� ����
        bloodScreen.color = new Color(1, 0, 0, Random.Range(0.2f, 0.3f));
        yield return new WaitForSeconds(0.1f);
        //�ٽ� ���� 0, ���� 0
        bloodScreen.color = Color.clear;
    }

    void PlayerDie()
    {
        //print("�÷��̾� ���");
        //GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);

        //foreach(GameObject enemy in enemies)
        //{
        //    //SendMessage("ȣ���� �޼����̸�", ���� ���)
        //    //SendMessage �޼ҵ�� Ư�� ��ũ��Ʈ�� �����ϴ� ���� �ƴ϶�
        //    //������Ʈ�� ���Ե� ��� ��ũ��Ʈ�� �ϳ��� �˻��ؼ�
        //    //�ش� �ϴ� �̸��� �����ϸ� �ش� �޼ҵ带 ȣ�� �ϴ� ���
        //    enemy.SendMessage("E_PlayerDie", SendMessageOptions.DontRequireReceiver);
        //}

        //�̺�Ʈ ȣ��
        PlayerDieEvent();
        //�̱��� ������ �̿��Ͽ� �ս��� ����
        GameManager.instance.isGameOver = true;
    }
    
    void DisplayHpBar()
    {
        //ü�º���
        float currHpRate = currHp / initHp;

        if(currHpRate > 0.5f) //ü���� 50% ���� ũ�ٸ�
        {
            //�������� ���� = ��� + ���� => ���
            currColor.r = (1 - currHpRate) * 2f;
        }
        else //ü���� 50% ������ ��
        {
            //����� ���� = ��� - ��� => ����
            currColor.g = currHpRate * 2f;
        }

        hpBar.color = currColor; //ü�¹� ���� ����
        hpBar.fillAmount = currHpRate; //ü�¹� ũ�� ����
    }
}
