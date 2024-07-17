using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

//구조체는 메모리의 스택 영역에 할당 
//성능이 클래스보다 좋음
//빠른 계산 및 활용이 필요할 경우 쓰면 좋음
[System.Serializable]
public struct PlayerSfx //구조체
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
    //현재 사용중인 무기 확인용 변수
    public WeaponType currWeapon = WeaponType.RIFLE;

    public PlayerSfx playerSfx;
    AudioSource _audio;

    //카메라가 흔드는 스크립트 가져오기
    Shake shake;

    public GameObject bulletPrefab;
    public Transform firePos;
    public ParticleSystem cartridge; //탄피 파티클시스템용 변수

    private ParticleSystem muzzleFlash;

    public Image magazineImg;
    public Text magazineText;

    public int maxBullet = 10;
    public int reamainingBullet = 10;
    public float reloadTime = 2f;
    bool isReloading = false;

    public Sprite[] weaponIcons;
    public Image weaponImage;

    [Header("자동공격관련")] //*
    public bool isFire = false;
    float nextFire;
    public float fireRate = 0.1f;

    int enemyLayer;
    int obstacleLayer;
    int layerMask;

    void Start()
    {
        //firePos의 자식오브젝트 중에서 ParticleSystem 컴포넌트 획득
        //유니티의 모든 오브젝트는 상대성을 지님
        //따라서 스크립트의 위치나 오브젝트의 위치가 매우 중요함
        muzzleFlash = firePos.GetComponentInChildren<ParticleSystem>();
        _audio = GetComponent<AudioSource>();
        shake = GameObject.Find("CameraRig").GetComponent<Shake>();

        //레이어값 추출해서 저장 *
        enemyLayer = LayerMask.NameToLayer("ENEMY");
        obstacleLayer = LayerMask.NameToLayer("OBSTACLE"); //*
        //두 레이어 결합
        //레이어를 2개 이상 병합 할 때는 | (OR 비트 연산자) 이용
        layerMask = 1 << enemyLayer | 1 << obstacleLayer;
    }

    // Update is called once per frame
    void Update()
    {
        //자동사냥 검출 레이저 색상표시*
        Debug.DrawRay(firePos.position, firePos.forward * 20f, Color.red);

        //UI 등을 클릭 또는 터치 하게되면 True 아니면 False
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }

        //레이캐슽트 방식(자동공격) //*
        RaycastHit hit;

        //Raycast는 충돌 유무만 판단 실제 충돌 객체 정보는
        //RaycastHit 에 전달됨 
        //이 때 ouy 으로 출력되는 값을 전달받기 위한 변수를 미리 선언
        //Raycast(레이 발사위치, 레이 발사 방향, 충돌한 객체 정보 반환받을 변수, 레이 사거리, 검출 레이어)
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 20f, layerMask))
        {
            //enemyLayer에 의해서 검출이 되면
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

        //GetMouseButton 은 마우스 누르고 있는 동안 지속 발생
        //GetMouseButtonDown 은 누르는 순간 1번만
        //GetMouseButtonUp 은 때는 순간 1번만
        //0은 좌클릭 1은 우클릭
        if (!isReloading && Input.GetMouseButtonDown(0))
        {
            reamainingBullet--; //총알소모 

            //공격 메소드 호출
            Fire();

            if (reamainingBullet == 0)
            {
                //재장전 코루틴 함수 호출
                StartCoroutine(Reloading());
            }
        }
    }

    void Fire()
    {
        StartCoroutine(shake.ShakeCamera());
        //총알 프리팹을 총구의 위치와 회전값을 가지고 동적 생성
        // Instantiate(bulletPrefab, firePos.position, firePos.rotation);

        //위의 동적생성을 사용하지 않고 오브젝트풀 사용
        //싱글턴 기법을 활용해서 오브젝트풀의
        //놀고 있는 총알 가져오기
        var _bullet = GameManager.instance.GetBullet();
        if (_bullet != null)
        {
            _bullet.transform.position = firePos.position;
            _bullet.transform.rotation = firePos.rotation;
            _bullet.SetActive(true);
        }

        //파티클 재생
        cartridge.Play();
        muzzleFlash.Play();
        //발사 사운드 재생
        FireSfx();

        magazineImg.fillAmount = (float)reamainingBullet / (float)maxBullet;
        //남은 총알 수 텍스트 갱신용 함수 호출

        UpdateBulletText();
    }

    void FireSfx()
    {
        //현재 들고 있는 무기의 enum 값을 int 로 변환해서 재생하고자 하는 무기의 오디오 클립을 가져옴
        var _sfx = playerSfx.fire[(int)currWeapon];
        //지정된 음원을, 1(100%) 볼륨으로 재생
        _audio.PlayOneShot(_sfx, 1f);
    }

    IEnumerator Reloading()
    {
        isReloading = true;
        _audio.PlayOneShot(playerSfx.reload[(int)currWeapon], 1f);

        //재장전 음원의 길이 + 0.3초 만큼 대기
        yield return new WaitForSeconds(playerSfx.reload[(int)currWeapon].length + 0.3f);

        isReloading = false;
        magazineImg.fillAmount = 1f;
        reamainingBullet = maxBullet;
        //남은 총알 수 텍스트 갱신용 함수 호출
        UpdateBulletText();
    }
    void UpdateBulletText()
    {
        string str = string.Format("<color=#ff0000>{0}</color>/{1}", reamainingBullet, maxBullet);

        //다른방법
        string str2 = $"<color=#ff0000>{reamainingBullet}</color>/{maxBullet}";
        string str3 = "<color=#ff0000>" + reamainingBullet + "</color>/" + maxBullet;

        magazineText.text = str;
    }

    public void OnChangeWeapon() //무기 탭 교체 버튼클릭 기능 메서드
    {
        currWeapon++;
        currWeapon = (WeaponType)((int)currWeapon % 2);
        weaponImage.sprite = weaponIcons[(int)currWeapon];
    }
}
