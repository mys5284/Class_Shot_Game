using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    public GameObject sparkEffect;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("BULLET"))
        {
            //����ũ ����Ʈ �����ϴ� �޼ҵ� ȣ��
            ShowEffect(collision);
           // Destroy(collision.gameObject);
            collision.gameObject.SetActive(false); 
        }
    }

    void ShowEffect(Collision collision)
    {
        //�浹 ������ ������ ����
        ContactPoint contactPoint = collision.contacts[0];

        //���� ���Ͱ� �̷�� ������ ����(���)
        //FromToRotation(A, B) = A �� ������ B �������� ������.
        //��, ���� ������ �Ǵ� �븻������ �������� �����شٴ� �ǹ�
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contactPoint.normal);

        //�浹 �������� ���������� �������� ����ũ ����Ʈ(Z�� ������ ����������)�� ��������
        GameObject effect = Instantiate(sparkEffect, contactPoint.point, rot);
        //���������� ź�� ȿ���� �θ� �������־� ���� �����̵�����
        effect.transform.SetParent(this.transform);
    }
}
