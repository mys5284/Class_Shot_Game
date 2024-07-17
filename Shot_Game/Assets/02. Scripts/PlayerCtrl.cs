using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ŭ������ ��� Serializable(����ȭ) ��� �Ӽ��� �ݵ�� ����������
//�׷��߸� �ν����Ϳ� �����

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
    float r = 0f; //ȸ���� ������ ����
    



    Transform tr;
    public float moveSpeed = 10f; //�̵� �ӵ� ���
    public float rotSpeed = 100f; //ȸ�� �ӵ� ���

    //�ִϸ��̼� ���� ������
    public PlayerAnim playerAnim;
    Animation anim;


    // Start is called before the first frame update
    void Start()
    {
        tr = GetComponent<Transform>();
        anim = GetComponent<Animation>();
        //���� ����ؾߵ� �ִϸ��̼� Ŭ���� idle�� ����
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
        //���� ����ȭ
        moveDir = moveDir.normalized;

        //Translate(���� * �ӵ�, ������ǥ)
        //������ǥ = Local(Self) / Global
        //�÷��̾� ���忡���� Local ��ǥ�踦 ��������
        //�յ��¿�� �����̵���
        tr.Translate(moveDir * moveSpeed * Time.deltaTime, Space.Self);

        //Rotate(�� ���� * �ӵ�)
        //ex) �粿ġ or ȸ��������
        tr.Rotate(Vector3.up * rotSpeed * r * Time.deltaTime);

        //�ִϸ��̼� ��ȯ
        if (v >= 0.1f)//��
        {
            //V���� ��� �� �� (���� �̵�)
            //CrossFade(��ȯ�� �ִϸ��̼��� �̸�, ��ȯ �ð�)
            anim.CrossFade(playerAnim.runF.name, 0.3f);
        }
        else if (v <= -0.1f) //��
        {
            anim.CrossFade(playerAnim.runB.name, 0.3f);
        }
        else if (h >= 0.1f) //��
        {
            anim.CrossFade(playerAnim.runR.name, 0.3f);
        }
        else if (h <= -0.1f) //��
        {
            anim.CrossFade(playerAnim.runL.name, 0.3f);
        }
        else //���̵�
        {
            anim.CrossFade(playerAnim.idle.name, 0.3f);
        }
    }
}
