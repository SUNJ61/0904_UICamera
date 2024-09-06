using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
[System.Serializable]
public struct PlayerSound
{
    public AudioClip[] fire;
    public AudioClip[] reload;
}

public class FireCtrl : MonoBehaviour
{
    public enum WeaponType
    {
        RIFLE = 1,SHOTGUN = 2
    }
    public PlayerSound playerSound;
    public WeaponType curType = WeaponType.SHOTGUN;
    public Transform firePos;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip fireclip;
    [SerializeField] private Player player;
    [SerializeField] private ParticleSystem muzzleFlash;
    private readonly string EnemyTag = "ENEMY";
    private readonly string swatTag = "SWAT";
    private readonly string BarrelTag = "BARREL";
    private readonly string WallTag = "WALL";
    private const float DIST = 20f;
    public Image magazineImg; //탄창 이미지
    public Text magazineText; // 남은 총알 수 text UI
    public float reloadTime = 2.0f; // 재장전 시간
    public bool isReloading = false;  // 재장전 여부 
    public int maxBullet = 10; //총알 최대수
    public int curBullet = 10; //현재 총알 수 
    public Sprite[] weaponIcons;
    public Image weaponImg;
   
    public bool isFire = false;
    private float nextFire; // 다음 발사 시간을 저장 할 변수
    public float fireRate = 0.1f; //총알 발사 간격
    private int enemyLayer; //적 레이어 번호를 받을 변수
    private int swatLayer;
    private int boxLayer;
    private int barrelLayer;
    private int layerMask;
    void Start()
    {
        magazineImg = GameObject.Find("Canvas-UI").transform.GetChild(1)
            .GetChild(2).GetComponent<Image>();
        magazineText = GameObject.Find("Canvas-UI").transform.GetChild(1).
            GetChild(0).GetComponent<Text>();
        weaponImg = GameObject.Find("Canvas-UI").transform.GetChild(3)
            .GetChild(0).GetComponent<Image>();
        player = GetComponent<Player>();
        source = GetComponent<AudioSource>();
        fireclip = Resources.Load("Sounds/p_ak_1_1") as AudioClip;
        weaponIcons = Resources.LoadAll<Sprite>("WeaponIcons");
        muzzleFlash.Stop();
        enemyLayer = LayerMask.NameToLayer("ENEMY");
        swatLayer = LayerMask.NameToLayer("SWAT");
        barrelLayer = LayerMask.NameToLayer("BARREL");
        boxLayer = LayerMask.NameToLayer("BOXES");
        layerMask = 1<< enemyLayer | 1<< barrelLayer | 1 << boxLayer | 1 << swatLayer;
                       
       
    }

    
    void Update()
    {
        RaycastHit hit;
        // 광선에 맞은 오브젝트의 위치와 거리 정보가 있는 구조체
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, 25f, layerMask))
            isFire = (hit.collider.CompareTag(EnemyTag) | hit.collider.CompareTag(swatTag));
        else
            isFire = false;


       if (EventSystem.current.IsPointerOverGameObject())
            return;

         if(!isReloading && isFire && !player.isRunning)
        {
            if(Time.time >nextFire)
            {
                --curBullet;
                Fire();
                if(curBullet==0)
                {
                    StartCoroutine(Reloading());
                }
                nextFire = Time.time + fireRate;
            }
        }



        //UI에 특정 이벤트가 발생 되면 빠져 나간다.
        //Debug.DrawRay(firePos.position, firePos.forward * 100f, Color.red);

        #region 마우스 왼쪽 버튼으로 발사 
        if (Input.GetMouseButtonDown(0)&&!isReloading)
        {   
            if(!player.isRunning)
            {
                --curBullet; // 총알 수가 감소 
                Fire();
                muzzleFlash.Play();
                if (curBullet == 0)
                    StartCoroutine(Reloading());
            }
              
        }
        else if (Input.GetMouseButtonUp(0))
        {
            muzzleFlash.Stop();
        }
        #endregion
    }
    void Fire()
    {
        #region projectile movement  방식

        ////오브젝트 풀링 방식이 아닐 때 
        ////Instantiate(bulletPrefab,firePos.position,firePos.rotation);
        ////오브젝트 풀링 방식 일때 로직 
        //var _bullet = ObjectPoolingManager.poolingManager.GetBulletPool();
        //if(_bullet != null)
        //{
        //    _bullet.transform.position = firePos.position;
        //    _bullet.transform.rotation = firePos.rotation;
        //    _bullet.SetActive(true);
        //}
        #endregion
        RaycastHit hit; //광선이 오브젝트 맞으면 충돌지점이나
                        // 거리등을 알려주는 광선 구조체
                        //광선을 쏘았을 때 맞았는 가 여부
        if (Physics.Raycast(firePos.position, firePos.forward, out hit, DIST))
        {       //태그검사 
            if (hit.collider.gameObject.tag == EnemyTag)
            {
                
                object[] _params = new object[2];
                _params[0] = hit.point; //첫번째 배열에는 맞은 위치를 전달
                _params[1] = 25f; // 두번째 배열에는 데미지 값을 전달
                // 광선에 맞은 오브젝트의 함수를 호출 하면서 매개변수 값을 전달
                hit.collider.gameObject.SendMessage("OnDamage", _params,
                       SendMessageOptions.DontRequireReceiver);
            }
            if (hit.collider.gameObject.tag == swatTag)
            {

                object[] _params = new object[2];
                _params[0] = hit.point; //첫번째 배열에는 맞은 위치를 전달
                _params[1] = 25f; // 두번째 배열에는 데미지 값을 전달
                // 광선에 맞은 오브젝트의 함수를 호출 하면서 매개변수 값을 전달
                hit.collider.gameObject.SendMessage("OnDamage", _params,
                       SendMessageOptions.DontRequireReceiver);
            }
            if (hit.collider.gameObject.tag == WallTag)
            {
                object[] _params = new object[2];
                _params[0] = hit.point;
                hit.collider.gameObject.SendMessage("OnDamage", _params,
                       SendMessageOptions.DontRequireReceiver);

            }
            if (hit.collider.gameObject.tag == BarrelTag)
            {
                object[] _params = new object[2];
                _params[0] = firePos.position; //발사위치
                _params[1] = hit.point; // 맞은 위치 
                hit.collider.gameObject.SendMessage("OnDamage", _params,
                       SendMessageOptions.DontRequireReceiver);

            }

        }
        source.PlayOneShot(fireclip, 1.0f);
        magazineImg.fillAmount = (float)curBullet / (float)maxBullet;
        UpdateBulletText();
    }

    private void UpdateBulletText()
    {
        //(남은 총알 수 /최대 총알 수) 표시 
        magazineText.text = string.Format("<color=#ff0000>{0}</color> / {1} ",
            curBullet, maxBullet);
    }
    public void OnChangeWeapon()
    {
        curType = (WeaponType)((int)++curType % 2);
         
        weaponImg.sprite = weaponIcons[(int)curType];
         //스프라이트 이미지 교체 

    }

    IEnumerator Reloading()
    {
        isReloading = true;

        SoundManager.S_instance.PlaySound(transform.position, playerSound.reload[(int)curType]);
        yield return
            new WaitForSeconds(playerSound.reload[(int)curType].length + 0.3f);
                                   //리로드 사운드 길이보다 0.3 만큼
        isReloading = false;
        magazineImg.fillAmount = 1.0f; //다 쏘고 나서 빈상태를 다시 채움
        curBullet = maxBullet;
        UpdateBulletText();

    }
}
