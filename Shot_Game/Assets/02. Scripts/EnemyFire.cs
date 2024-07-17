using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyFire : MonoBehaviour
{
    AudioSource audio;
    Animator animator;
    Transform playerTr;
    Transform enemyTr;

    readonly int hashFire = Animator.StringToHash("Fire");
    readonly int hashReload = Animator.StringToHash("Reload");

    //�ڵ����� ���� ����
    [Header("�ڵ�����")]
    float nextFire = 0f;
    readonly float fireRate = 0.1f; //�߻� ����
    readonly float damping = 10f;

    [Header("������")]
    readonly float reloadTime = 2f;
    readonly int maxBullet = 10; //�ִ� �Ѿ� ��
    int currBullet = 10;
    bool isReload = false;
    WaitForSeconds wsReload;

    public bool isFire = false;
    public AudioClip fireSfx;
    public AudioClip reloadSfx;

    //�Ѿ� �߻� ����
    public GameObject bullet;
    public Transform firePos;

    //�ѱ�ȭ�� ����
    public MeshRenderer muzzleFlash;

    void Start()
    {
        playerTr = GameObject.FindGameObjectWithTag("PLAYER").GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        animator = GetComponent<Animator>();
        audio = GetComponent<AudioSource>();

        wsReload = new WaitForSeconds(reloadTime);

        muzzleFlash.enabled = false;
    }


    void Update()
    {
        if (!isReload && isFire) //�߻� ���¶�� // &&�Ҷ� �󵵼� ������ �տ�.
        {
            if (Time.time > nextFire)
            {
                //�Ѿ� �߻� �Լ� ȣ�� 
                Fire();
                nextFire = Time.time + fireRate + Random.Range(0f, 0.3f);
            }
            // A - B = B�� A�� �ٶ󺸴� ����
            Quaternion rot = Quaternion.LookRotation(playerTr.position - enemyTr.position);

            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, Time.deltaTime * damping);
        }

    }

    void Fire()
    {
        //�ѱ� ȭ�� �ڷ�ƾ �Լ� ȣ��
        StartCoroutine(ShowMuzleFlash());

        animator.SetTrigger(hashFire);
        audio.PlayOneShot(fireSfx, 1f);

        GameObject _bullet = Instantiate(bullet, firePos.position, firePos.rotation);

        Destroy(_bullet, 3f);

        currBullet--;
        //���� �Ѿ� ������ ���ؼ� ������ ��������
        // = ������ ������ ���Ͽ� ���ϰ�� true �ƴϸ� false
        isReload = (currBullet % maxBullet == 0);
        if(isReload)
        {
            //������ �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(Reload());
        }
       
    }

    IEnumerator ShowMuzleFlash()
    {
        muzzleFlash.enabled = true;

        //�����÷��� ���带 0~360�� ȸ�� ��Ű������
        Quaternion rot = Quaternion.Euler(Vector3.forward * Random.Range(0, 360));
        muzzleFlash.transform.localRotation = rot;

        //�����÷����� Scale ���� xyz ��� 1 ~ 2�� �ø���
        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1f, 2f);

        //�ؽ�ó ������ �����ϱ�
        //Random.Range�� ���ؼ� 0 �Ǵ� 1���� �����µ�
        //0.5�� ���ؼ� 0 �Ǵ� 0.5 ���� �������� ���
        Vector2 offset = new Vector2(Random.Range(0, 2), Random.Range(0, 2)) * 0.5f;
        //�����÷����� ��Ƽ������ offset ���� ����
        //��Ȯ���� ��Ƽ������ �����ϴ� Shader�� ������Ƽ���� ����
        muzzleFlash.material.SetTextureOffset("_MainTex", offset);

        //���ð��� 0.05 ~ 0.2�� ���� �����ϰ� ����
        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        muzzleFlash.enabled = false;
    }

    IEnumerator Reload()
    {
        animator.SetTrigger(hashReload);
        audio.PlayOneShot(reloadSfx, 1f);

        //������ �ϴ� ���� ����� �纸
        //�ִϸ��̼� ���� �ð����� ��ٷ��ִ°�
        yield return wsReload;

        currBullet = maxBullet;
        isReload = false;
    }

}
