using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    [Header("������ ���� ����")]
    public Transform[] points;
    public GameObject enemy;
    public float createTime = 2f; //���� �ֱ�
    public int maxEnemy = 10; //�ִ� ���� ��

    public bool isGameOver = false;

    [Header("������ƮǮ ���� ����")]
    public GameObject bulletPrefab;
    //������ƮǮ�� �ִ� ������
    public int maxPool = 10;
    //������Ʈ Ǯ�� ����Ʈ
    public List<GameObject> bulletPool = new List<GameObject>();

    //�̱��� ������ Ȱ���Ͽ� �ش� ��ũ��Ʈ�� �����ϱ����� ���� 
    public static GameManager instance = null;

    public CanvasGroup inventoryCG; //*

    [HideInInspector]
    public int killCount;
    public Text killCountTxt;

    void LoadGameData() //*
    {
        //������ ������ ����ҿ� ����� ���� �����´�.
        //�̶� ������ �Ǽ��� �ʱⰪ�� �������൵ ��
        //GetInt("Ű��, �ʱⰪ")
        killCount = PlayerPrefs.GetInt("KILL_COUNT", 0);
        killCountTxt.text = "KILL " + killCount.ToString("0000");
    }

    public void InKillCount() //*
    {
        killCount++;
        killCountTxt.text = "KILL " + killCount.ToString("0000");
        //����ҿ� KILL_COUNT ��� Ű ������ killCount ���� ����
        PlayerPrefs.SetInt("KILL_COUNT", killCount);
    }

    private void Awake()
    {
        if (instance == null) //����ڵ�
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(this.gameObject); //���� ���� instance�� ����
        }

        //�� ������ �߻��ص� �ش� ���� ������Ʈ �ı�����
        //������ ����� �� ���� �� ��
        DontDestroyOnLoad(this.gameObject);

        //����� ���� ������ �ҷ�����
        LoadGameData(); //*

        //������Ʈ Ǯ ���� �Լ� ȣ��
        CreateObjectPool();
    }

    void Start()
    {
        OnInventoryOpen(false);

        points = GameObject.Find("SpawnPointGroup").GetComponentsInChildren<Transform>();
        if (points.Length > 0)
        {
            //���� �ڷ�ƾ �Լ� ȣ��
            StartCoroutine(CreateEnemy());
        }
    }

    IEnumerator CreateEnemy()
    {
        while (!isGameOver)
        {
            //ENEMY �±׸� ���� ������Ʈ�� �˻��ؼ� ����(ũ��) ���� ����
            int enemyCount = GameObject.FindGameObjectsWithTag("ENEMY").Length;
            if (enemyCount < maxEnemy)
            {
                //�����ֱ⸸ŭ ���
                yield return new WaitForSeconds(createTime);

                //����� ������ ����Ʈ�� ���� ��ŭ ������ ���� ����
                //�ش� �ε����� ���Ͽ� ���� ��ġ�� �����ϰ� ������
                int idx = Random.Range(1, points.Length);

                Instantiate(enemy, points[idx].position, points[idx].rotation);
            }
            else //�������� Enemy�� ���� Max�� ���� ���� ��
            {
                yield return null;
            }
        }
    }

    //������ƮǮ ���� �޼ҵ�
    public void CreateObjectPool()
    {
        //ObjectPools ��� �̸��� �������Ʈ�� ����
        GameObject objectPools = new GameObject("ObjectPools");

        //Ǯ�� ������ŭ �Ѿ��� �����ϱ� ���� �ݺ���
        for (int i = 0; i < maxPool; i++)
        {
            //�Ѿ� �������� �������� �ϸ鼭
            //������ ������ ObjectPools�� �ڽ����� �ٷ� �־���
            var bullet = Instantiate<GameObject>(bulletPrefab, objectPools.transform);
            bullet.name = "Bullet_" + i.ToString("00");
            //�Ѿ� ��Ȱ��ȭ
            bullet.SetActive(false);
            //������ƮǮ(����Ʈ)�� ������ �Ѿ� �߰�
            bulletPool.Add(bullet);
        }
    }

    //������Ʈ Ǯ���� ����ִ� �Ѿ� ��� ��ȯ
    public GameObject GetBullet()
    {
        for (int i = 0; i < bulletPool.Count; i++)
        {
            //Ǯ���� ������ �Ѿ��� ���°� Active False�� ���
            if (!bulletPool[i].activeSelf)
            {
                return bulletPool[i];
            }
        }
        return null;
    }

    bool isPaused; //*
    public void OnPauseClick()
    {
        isPaused = !isPaused;

        //bool ������ true�� 0 �ƴϸ� 1
        //timeScale�� 1�� �������� �۾����� �������� Ŀ���� ������
        //0�̵Ǹ� �Ͻ����̳� �ִ� 4�̻� ������ �ʴ°��� ����
        //����� ���� �߿� �� ��Ÿ ����
        Time.timeScale = (isPaused) ? 0f : 1f;

        var playerObj = GameObject.FindGameObjectWithTag("PLAYER");
        //�÷��̾ �߰��� ��ũ��Ʈ ��� ��������
        //MonoBehaviour�� ���� ��ũ��Ʈ ���δ� ������
        var scripts = playerObj.GetComponents<MonoBehaviour>();

        //�Ͻ� ���� �� �� ��� ��ũ��Ʈ ����
        //�Ͻ� ���� �����Ǹ� �ٽ� ����
        foreach (var script in scripts)
        {
            script.enabled = !isPaused;
        }

        //���� ��ü UI�� CanvasGroup�� �����ϱ� ���� �ڵ� �߰�
        var canvasGroup = GameObject.Find("Panel_Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts = !isPaused;
    }

    public void OnInventoryOpen(bool isOpened) //*
    {
        inventoryCG.alpha = (isOpened) ? 1f : 0;
        //������ 0�� �Ǿ UI�� ������ �ʴ���
        //����ĳ��Ʈ�� ���� ��ġ �̺�Ʈ�� �߻��ϱ� ������
        //�Ʒ� �ڵ带 ���ؼ� ��ġ �̺�Ʈ �����ϵ��� ����
        inventoryCG.interactable = isOpened;
        inventoryCG.blocksRaycasts = isOpened;
    }
}
