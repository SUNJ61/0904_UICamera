using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolingManager : MonoBehaviour
{
    public static ObjectPoolingManager poolingManager;
    [SerializeField] private GameObject bulletPrefab; //�Ѿ� ������
    [SerializeField] private GameObject E_bulletPrefab; //�Ѿ� ������
    private int maxPool = 3; //������Ʈ Ǯ�� ���� �� ���� 
    public List<GameObject> bulletPoolList;
    public List<GameObject> E_bulletPoolList;
    [Header("EnemyObjectPool")]
    public GameObject EnemyPrefab;
    public List<GameObject> EnemyPoolList;
    public List<Transform> SpawnPointList;
    [Header("swatObjectPool")]
    public GameObject swatPrefab;
    public List<GameObject> swatPoolList;
    [Header("EnemyHpbar")]
    public GameObject EnemyHpbar;
    public List<GameObject> E_HpbarList;
    [Header("swatHpbar")]
    public List<GameObject> S_HpbarList;

    private readonly string Hpbar_Name = "EnemyHpBar";
    void Awake() //Awake-> OnEnable-> Start
    {
        if (poolingManager == null)
            poolingManager = this;
        else if (poolingManager != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);

        bulletPrefab = Resources.Load("Bullet") as GameObject;
        E_bulletPrefab = Resources.Load("E_Bullet") as GameObject;
        EnemyPrefab = Resources.Load<GameObject>("Enemy");
        EnemyHpbar = Resources.Load<GameObject>(Hpbar_Name);
        swatPrefab = Resources.Load<GameObject>("swat");
        CreateBulletPool(); //������Ʈ Ǯ�� ���� �Լ� 
        CreateE_BulletPool();
        CreateEnemyPool();
        CreateE_HpbarPool();
        CreateswatPool();
        CreateS_HpbarPool();
    }
    private void Start()
    {
        var spawnPoint = GameObject.Find("SpawnPoints");
        if (spawnPoint != null)
            spawnPoint.GetComponentsInChildren<Transform>(SpawnPointList);

         SpawnPointList.RemoveAt(0);
        if (SpawnPointList.Count > 0)
        {
            StartCoroutine(CreateEnemy());
            StartCoroutine(Createswat());
        }
        
    }

    private void CreateEnemyPool()
    {
        GameObject EnemyGroup = new GameObject("EnemyGroup");
        for (int i = 0; i < maxPool; i++)
        {
            var enemyObj = Instantiate(EnemyPrefab, EnemyGroup.transform);
            enemyObj.name = $"{(i + 1).ToString()} ��";
            enemyObj.SetActive(false);
            EnemyPoolList.Add(enemyObj);
        }
    }
    private void CreateswatPool()
    {
        GameObject swayGroup = new GameObject("swayGroup");
        for (int i = 0; i < maxPool; i++)
        {
            var swatObj = Instantiate(swatPrefab, swayGroup.transform);
            swatObj.name = $"{(i + 1).ToString()} ��";
            swatObj.SetActive(false);
            swatPoolList.Add(swatObj);
        }
    }

    void CreateBulletPool()
    {               //���� ������Ʈ ���� 
        GameObject playerBulletGroup = new GameObject("PlayerBulletGroup");
        for(int i =0;i< 10;i++)
        {
            var _bullet = Instantiate(bulletPrefab,playerBulletGroup.transform);
            _bullet.name = $"{(i+1).ToString()} ��";
            _bullet.SetActive(false);
            bulletPoolList.Add(_bullet);
        }

    }
    void CreateE_BulletPool()
    {               //���� ������Ʈ ���� 
        GameObject EnemyBulletGroup = new GameObject("Enemy_BulletGroup");
        for (int i = 0; i < 20; i++)
        {
            var E_bullet = Instantiate(E_bulletPrefab, EnemyBulletGroup.transform);
            E_bullet.name = $"{(i + 1).ToString()} ��";
            E_bullet.SetActive(false);
            E_bulletPoolList.Add(E_bullet);
        }

    }
    public GameObject GetBulletPool()
    {
        for(int i = 0; i< bulletPoolList.Count;i++)
        {      
            //��Ȱ�� �Ǿ��ٸ� activeSelf�� Ȱ��ȭ ��Ȱ�� ���θ� �˷���
            if (bulletPoolList[i].activeSelf ==false)
            {
                return bulletPoolList[i];
            }
        }
        return null;
    }
    public GameObject E_GetBulletPool()
    {
        for (int i = 0; i < E_bulletPoolList.Count; i++)
        {
            //��Ȱ�� �Ǿ��ٸ� activeSelf�� Ȱ��ȭ ��Ȱ�� ���θ� �˷���
            if (E_bulletPoolList[i].activeSelf == false)
            {
                return E_bulletPoolList[i];
            }
        }
        return null;
    }
    IEnumerator CreateEnemy()
    {
        while (!GameManager.G_instance.isGameOver)
        {
            yield return new WaitForSeconds(3f);
            if (GameManager.G_instance.isGameOver) yield break;
              //������ ���� �Ǹ� �ڷ�ƾ�� ���� �ؼ� ���� ��ƾ�� ���� ���� ����
            foreach(GameObject _enemy in EnemyPoolList)
            {
                if(_enemy.activeSelf ==false)
                {
                    int idx = Random.Range(0,SpawnPointList.Count-1);
                    _enemy.transform.position = SpawnPointList[idx].position;
                    _enemy.transform.rotation = SpawnPointList[idx].rotation;
                    _enemy.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    IEnumerator Createswat()
    {
        while (!GameManager.G_instance.isGameOver)
        {
            yield return new WaitForSeconds(3f);
            if (GameManager.G_instance.isGameOver) yield break;
            //������ ���� �Ǹ� �ڷ�ƾ�� ���� �ؼ� ���� ��ƾ�� ���� ���� ����
            foreach (GameObject _swat in swatPoolList)
            {
                if (_swat.activeSelf == false)
                {
                    int idx = Random.Range(0, SpawnPointList.Count - 1);
                    _swat.transform.position = SpawnPointList[idx].position;
                    _swat.transform.rotation = SpawnPointList[idx].rotation;
                    _swat.gameObject.SetActive(true);
                    break;
                }
            }
        }
    }

    void CreateE_HpbarPool()
    {
        Transform Enemy_UI = GameObject.Find("Canvas-EnemyUI").transform;
        GameObject E_HpbarGroup = new GameObject("Enemy_HpbarGroup");
        E_HpbarGroup.transform.parent = Enemy_UI;
        E_HpbarGroup.transform.localPosition = Vector3.zero;
        E_HpbarGroup.transform.localScale = Vector3.one;
        for (int i = 0; i < 10; i++)
        {
            var E_hpbar = Instantiate(EnemyHpbar,E_HpbarGroup.transform);
            E_hpbar.name = $"{(i + 1).ToString()}��° Hpbar";
            E_hpbar.SetActive(false);
            E_HpbarList.Add(E_hpbar);
        }
    }

    public GameObject GetE_Hpbar()
    {
        for (int i = 0; i < E_HpbarList.Count; i++)
        {
            if (E_HpbarList[i].activeSelf == false)
            {
                return E_HpbarList[i];
            }
        }
        return null;
    }

    void CreateS_HpbarPool()
    {
        Transform Enemy_UI = GameObject.Find("Canvas-EnemyUI").transform;
        GameObject S_HpbarGroup = new GameObject("swat_HpbarGroup");
        S_HpbarGroup.transform.parent = Enemy_UI;
        S_HpbarGroup.transform.localPosition = Vector3.zero;
        S_HpbarGroup.transform.localScale = Vector3.one;
        for (int i = 0; i < 10; i++)
        {
            var S_hpbar = Instantiate(EnemyHpbar, S_HpbarGroup.transform);
            S_hpbar.name = $"{(i + 1).ToString()}��° Hpbar";
            S_hpbar.SetActive(false);
            S_HpbarList.Add(S_hpbar);
        }
    }

    public GameObject GetS_Hpbar()
    {
        for (int i = 0; i < S_HpbarList.Count; i++)
        {
            if (S_HpbarList[i].activeSelf == false)
            {
                return S_HpbarList[i];
            }
        }
        return null;
    }
}
