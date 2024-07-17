using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//����ü�� �޸��� ���� ������ �Ҵ� 
//������ Ŭ�������� ����
//���� ��� �� Ȱ���� �ʿ��� ��� ���� ����
[System.Serializable]
public struct PlayerSfx //����ü
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    public enum WeaponType
    {
        RIFLE = 0,
        SHOTGUN

    }
    //���� ������� ���� Ȯ�ο� ����
    public WeaponType currWeapon = WeaponType.RIFLE;

    public PlayerSfx playerSfx;
    AudioSource _audio;

    //ī�޶� ���� ��ũ��Ʈ ��������
    Shake shake;

    public GameObject bulletPrefab;
    public Transform firePos;
    public ParticleSystem cartridge; //ź�� ��ƼŬ�ý��ۿ� ����

    private ParticleSystem muzzleFlash;

    public Image magazineImg;
    public Text magazineText;

    public int maxBullet = 10;
    public int reamainingBullet = 10;
    public float reloadTime = 2f;
    bool isReloading = false;

    public Sprite[] weaponIcons;
    public Image weaponImage;

    [Header("�ڵ����ݰ���")] //*
    public bool isFire = false;
    float nextFire;
    public float fireRate = 0.1f;

    int enemyLayer;
    int obstacleLayer;
    int layerMask;

    void Start()
    {
        //firePos�� �ڽĿ�����Ʈ �߿��� ParticleSystem ������Ʈ ȹ��
        //����Ƽ�� ��� ������Ʈ�� ��뼺�� ����
        //���� ��ũ��Ʈ�� ��ġ�� ������Ʈ�� ��ġ�� �ſ� �߿���
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();

        //���̾ �����ؼ� ���� *
        enemyLayer = LayerMask.NameToLayer("ENEMY");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE"); //*
        //�� ���̾� ����
        //���̾ 2�� �̻� ���� �� ���� | (OR ��Ʈ ������) �̿�
        layerMask = 1 << enemyLayer | 1 << obstacleLayer;
    }

    // Update is called once per frame
    void Update()
    {
        //�ڵ���� ���� ������ ����ǥ��*
        Debug.DrawRay(firePos.position, firePos.forward * 20f, Color.red);

        //UI ���� Ŭ�� �Ǵ� ��ġ �ϰԵǸ� True �ƴϸ� False
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //����ĳ��Ʈ ���(�ڵ�����) //*
        RaycastHit hit;

        //Raycast�� �浹 ������ �Ǵ� ���� �浹 ��ü ������
        //RaycastHit �� ���޵� 
        //�� �� ouy ���� ��µǴ� ���� ���޹ޱ� ���� ������ �̸� ����
        //Raycast(���� �߻���ġ, ���� �߻� ����, �浹�� ��ü ���� ��ȯ���� ����, ���� ��Ÿ�, ���� ���̾�)
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f, layerMask))
        {
            //enemyLayer�� ���ؼ� ������ �Ǹ�
            isFire = hit.collider.CompareTag("ENEMY");
        }
        else
        {
            isFire = false;
        }

        if (!isReloading && isFire)
        {
            if (Time.time > nextFire)
            {
                reamainingBullet--;
                Fire();
                if (reamainingBullet == 0)
                {
                    StartCoroutine(Reloading());
                }
                nextFire = Time.time + fireRate;
            }
        }

        //GetMouseButton �� ���콺 ������ �ִ� ���� ���� �߻�
        //GetMouseButtonDown �� ������ ���� 1����
        //GetMouseButtonUp �� ���� ���� 1����
        //0�� ��Ŭ�� 1�� ��Ŭ��
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            reamainingBullet--; //�Ѿ˼Ҹ� 

            //���� �޼ҵ� ȣ��
            Fire();

            if (reamainingBullet == 0)
            {
                //������ �ڷ�ƾ �Լ� ȣ��
                StartCoroutine(Reloading());
            }
        }
    }

    void Fire()
    {
        StartCoroutine(shake.ShakeCamera());
        //�Ѿ� �������� �ѱ��� ��ġ�� ȸ������ ������ ���� ����
        // Instantiate(bulletPrefab, firePos.position, firePos.rotation);

        //���� ���������� ������� �ʰ� ������ƮǮ ���
        //�̱��� ����� Ȱ���ؼ� ������ƮǮ��
        //��� �ִ� �Ѿ� ��������
        var _bullet = GameManager.instance.GetBullet();
        if (_bullet != null)
        {
            _bullet.transform.position = firePos.position;
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }

        //��ƼŬ ���
        cartridge.Play();
        muzzleFlash.Play();
        //�߻� ���� ���
        FireSfx();

        magazineImg.fillAmount = (float)reamainingBullet / (float)maxBullet;
        //���� �Ѿ� �� �ؽ�Ʈ ���ſ� �Լ� ȣ��

        UpdateBulletText();
    }

    void FireSfx()
    {
        //���� ��� �ִ� ������ enum ���� int �� ��ȯ�ؼ� ����ϰ��� �ϴ� ������ ����� Ŭ���� ������
        var _sfx = playerSfx.fire[(int)currWeapon];
        //������ ������, 1(100%) �������� ���
        _audio.PlayOneShot(_sfx, 1f);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1f);

        //������ ������ ���� + 0.3�� ��ŭ ���
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.3f);

        isReloading = false;
        magazineImg.fillAmount = 1f;
        reamainingBullet = maxBullet;
        //���� �Ѿ� �� �ؽ�Ʈ ���ſ� �Լ� ȣ��
        UpdateBulletText();
    }
    void UpdateBulletText()
    {
        string str = string.Format("<color=#ff0000>{0}</color>/{1}", reamainingBullet, maxBullet);

        //�ٸ����
        string str2 = $"<color=#ff0000>{reamainingBullet}</color>/{maxBullet}";
        string str3 = "<color=#ff0000>" + reamainingBullet + "</color>/" + maxBullet;

        magazineText.text = str;
    }

    public void OnChangeWeapon() //���� �� ��ü ��ưŬ�� ��� �޼���
    {
        currWeapon++;
        currWeapon = (WeaponType)((int)currWeapon % 2);
        weaponImage.sprite = weaponIcons[(int)currWeapon];
    }
}
