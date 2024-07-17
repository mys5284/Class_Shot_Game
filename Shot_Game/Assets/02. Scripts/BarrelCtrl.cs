using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCtrl : MonoBehaviour
{
    public GameObject expEffect; //���� ȿ�� ������

       
    int hitCount = 0; //�Ѿ� ���� Ƚ��
    Rigidbody rb;


    //��׷��� �巳���� �޽��� ������ �迭
    public Mesh[] meshes;
    MeshFilter meshFilter;

    public Texture[] textures;
    MeshRenderer _renderer;

    //���߹ݰ�
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
        //�浹�� ������Ʈ�� BULLET �±׸� ����ٸ�
        if (collision.collider.CompareTag("BULLET"))
        {
            hitCount++; //���� Ƚ�� ����
            if(hitCount == 3)
            {
                //���� �޼ҵ� ȣ��
                ExpBarrel();
            }
        }


    }

    void ExpBarrel()
    {
        //���� ����Ʈ�� �巳������ġ(transform.position)
        //��ü ȸ������ ������ ���� ����
        GameObject effect = Instantiate(expEffect, transform.position, Quaternion.identity);
        Destroy(effect, 2f);
        //�巳���� �� ���󰡵��� �ϱ� ���ؼ� �������� 1��
        //rb.mass = 1f;
        //rb.AddForce(Vector3.up * 400f);

        IndirectDamage(transform.position);

        //����(������ �߻�)
        int idx = Random.Range(0, meshes.Length);
        //������ ������ ���� ���� ���ؼ� �޽� �迭�� �ִ� �޽� �����ϰ� ������
        meshFilter.sharedMesh = meshes[idx];

        //�巳�� ������ ��� ��鸮�� ������ ũ�Ƿ�
        //�Ű������� ū ���� ����
        StartCoroutine(shake.ShakeCamera(0.1f, 0.2f, 0.5f)); 
        
    }

    void IndirectDamage(Vector3 pos)
    {
        //OverlapSphere(������ġ, �ݰ�, ���� ���̾�)
        //��ġ�κ��� �ݰ� ������ ���ⷹ�̾ �شܵǴ� ������Ʈ�� �浹ü ������ ��� ������
        Collider[] colls = Physics.OverlapSphere(pos, expRadius, 1 << 8); //1<<8 ��� 2�� 8���� 256�� �־ �ȴ�. 1<<8 || 1<<0 �� 2�� 8��: 256�� 2�� 0�� :1�� ���� ���� 257�� �־ ����.

        foreach(var coll in colls)
        {
            //����� ���κ��� ������ �ٵ� �ϳ��� �̾ƿ���
            var _rb = coll.GetComponent<Rigidbody>();
            _rb.mass = 1f;

            //AddExplosionForce(Ⱦ���߷�, ������ġ, �ݰ�, �� ���߷�)
            //Ⱦ = ����, �� = ����
            _rb.AddExplosionForce(600f, pos, expRadius, 500f);
        }
    }
}
