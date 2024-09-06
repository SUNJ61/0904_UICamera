using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwatFire : MonoBehaviour
{
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private Animator animator;
    [SerializeField] private Transform playerTr;//방향을 구하기 위해서
    [SerializeField] private Transform enemyTr;//방향을 구하기 위해서
    [SerializeField] private Transform firePos;

    private readonly int hashFire = Animator.StringToHash("FireTrigger");
    private readonly int hashReload = Animator.StringToHash("ReloadTrigger");
    private float nextFire = 0.0f; // 다음 시간에 발사할 시간 계산용 변수
    private readonly float fireRate = 0.1f; // 총알 발사 간격
    private readonly float damping = 10.0f; // 플레이어를 향해 회전할 속도 
    public bool isFire = false;

    [Header("Reload")]
    [SerializeField] private readonly float reloadTime = 2.0f; //재장전 시간
    [SerializeField] private readonly int maxBullet = 10; //10발일때 재장전 하기위한 Max값
    [SerializeField] private int curBullet = 0; //현재 총알 수 
    [SerializeField] private bool isReload = false; // 재장전 여부
    [SerializeField] private WaitForSeconds reloadWs; // 스타트 코루틴에서 시간 정할 변수
    [SerializeField] private AudioClip reloadClip; //재장전 사운드 
    public MeshRenderer muzzleFlash;

    void Start()
    {
        firePos = transform.GetChild(2).GetChild(0).GetChild(0).transform;
        muzzleFlash = transform.GetChild(2).GetChild(0).GetChild(0).GetChild(0).GetComponent<MeshRenderer>();
        muzzleFlash.enabled = false;
        animator = GetComponent<Animator>();
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindGameObjectWithTag("Player").transform;
        fireClip = Resources.Load("Sounds/p_m4_1_1") as AudioClip;
        reloadClip = Resources.Load<AudioClip>("Sounds/p_reload 1_1");
        curBullet = maxBullet;
        reloadWs = new WaitForSeconds(reloadTime);
    }
    void Update()
    {
        if (GameManager.G_instance.isGameOver) return;
        if (isFire && !isReload)
        {
            if (Time.time >= nextFire)
            {
                Fire();
                nextFire = Time.time + fireRate + Random.Range(0.0f, 0.3f);
            }
            Vector3 playerNormal = playerTr.position - enemyTr.position;
            Quaternion rot = Quaternion.LookRotation(playerNormal.normalized);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, damping * Time.deltaTime);
        }
    }

    void Fire()
    {
        var E_bullet = ObjectPoolingManager.poolingManager.E_GetBulletPool();

        E_bullet.transform.position = firePos.position;
        E_bullet.transform.rotation = firePos.rotation;

        E_bullet.SetActive(true);

        animator.SetTrigger(hashFire);
        SoundManager.S_instance.PlaySound(firePos.position, fireClip);

        isReload = (--curBullet % maxBullet) == 0;
        if (isReload)
        {
            StartCoroutine(Reloading());
        }
        StartCoroutine(ShowMuzzleFlash());
    }

    IEnumerator Reloading()
    {
        animator.SetTrigger(hashReload);//재장전 애니메이션 실행
        SoundManager.S_instance.PlaySound(transform.position, reloadClip);
        //재장전 사운드 실행
        yield return reloadWs; //재장전 시간만큼 대기하는 동안 제어권 양보

        curBullet = maxBullet; // 현재 총알을 다시 10발로 
        isReload = false;   //재장전 불변수는 false로 

    }
    IEnumerator ShowMuzzleFlash()
    {
        muzzleFlash.enabled = true;

        muzzleFlash.transform.localScale = Vector3.one * Random.Range(1.5f, 2.3f);
        Quaternion rot = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        muzzleFlash.transform.localRotation = rot;

        yield return new WaitForSeconds(Random.Range(0.05f, 0.2f));

        muzzleFlash.enabled = false;

    }
}
