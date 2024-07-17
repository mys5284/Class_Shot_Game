using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffect; //폭팔 효과 프리팹

       
    int hitCount = 0; //총알 맞은 횟수
    Rigidbody rb;


    //찌그러진 드럼통의 메쉬를 저장할 배열
    public Mesh[] meshes;
    MeshFilter meshFilter;

    public Texture[] textures;
    MeshRenderer _renderer;

    //폭발반경
    public float expRadius = 10f;

    Shake shake; 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshFilter = GetComponent<MeshFilter>();

        _renderer = GetComponent<MeshRenderer>();
        _renderer.material.mainTexture = textures[Random.Range(0, textures.Length)];

        shake = GameObject.Find("CameraRig").GetComponent<Shake>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //충돌한 오브젝트가 BULLET 태그를 지녔다면
        if (collision.collider.CompareTag("BULLET"))
        {
            hitCount++; //맞은 횟수 증가
            if(hitCount == 3)
            {
                //폭발 메소드 호출
                ExpBarrel();
            }
        }


    }

    void ExpBarrel()
    {
        //폭발 이펙트를 드럼통의위치(transform.position)
        //자체 회전값을 가지고 동적 생성
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2f);
        //드럼통이 잘 날라가도록 하기 위해서 질량값을 1로
        //rb.mass = 1f;
        //rb.AddForce(Vector3.up * 400f);

        IndirectDamage(transform.position);

        //난수(랜덤값 발생)
        int idx = Random.Range(0, meshes.Length);
        //위에서 추출한 랜덤 값을 통해서 메쉬 배열에 있는 메쉬 랜덤하게 골라오기
        meshFilter.sharedMesh = meshes[idx];

        //드럼통 폭발의 경우 흔들리는 정도가 크므로
        //매개변수를 큰 값을 지정
        StartCoroutine(shake.ShakeCamera(0.1f, 0.2f, 0.5f)); 
        
    }

    void IndirectDamage(Vector3 pos)
    {
        //OverlapSphere(시작위치, 반경, 검출 레이어)
        //위치로부터 반경 사이의 검출레이어에 해단되는 오브젝트의 충돌체 정보를 모두 가져옴
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 8); //1<<8 대신 2의 8승인 256을 넣어도 된다. 1<<8 || 1<<0 은 2의 8승: 256과 2의 0승 :1의 더한 값인 257을 넣어도 같다.

        foreach(var coll in colls)
        {
            //검출된 놈들로부터 리지드 바디 하나씩 뽑아오기
            var _rb = coll.GetComponent<Rigidbody>();
            _rb.mass = 1f;

            //AddExplosionForce(횡폭발력, 시작위치, 반경, 종 폭발력)
            //횡 = 가로, 종 = 세로
            _rb.AddExplosionForce(600f, pos, expRadius, 500f);
        }
    }
}
