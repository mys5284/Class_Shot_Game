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
            //스파크 이펙트 생성하는 메소드 호출
            ShowEffect(collision);
           // Destroy(collision.gameObject);
            collision.gameObject.SetActive(false); 
        }
    }

    void ShowEffect(Collision collision)
    {
        //충돌 지점의 정보를 추출
        ContactPoint contactPoint = collision.contacts[0];

        //법선 벡터가 이루는 각도를 추출(계산)
        //FromToRotation(A, B) = A 의 방향을 B 방향으로 돌린다.
        //즉, 면의 수직이 되는 노말벡터의 방향으로 돌려준다는 의미
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, contactPoint.normal);

        //충돌 지점에서 법선벡터의 방향으로 스파크 이펙트(Z축 방향이 법선벡터쪽)를 동적생성
        GameObject effect = Instantiate(sparkEffect, contactPoint.point, rot);
        //동적생성된 탄흔 효과의 부모를 설정해주어 같이 움직이도록함
        effect.transform.SetParent(this.transform);
    }
}
