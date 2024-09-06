using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDamage : MonoBehaviour
{
    [SerializeField] private readonly string bulletTag = "BULLET";
    [SerializeField] private GameObject bloodEffect;
    public float hp = 100f;

    public Vector3 hpbarOffset = new Vector3(0f, 2.2f, 0f);
    public Image hpbarimg;
    public GameObject hpbar;
    private void OnEnable()
    {
        StartCoroutine(SetHpBar());
    }

    IEnumerator SetHpBar()
    {
        yield return new WaitForSeconds(0.2f); //pc속도에 맞춰 시간 조정

        if (this.gameObject.CompareTag("ENEMY"))
            hpbar = ObjectPoolingManager.poolingManager.GetE_Hpbar(); //오브젝트 풀링매니저에서 생성한 hpbar중 비활성화된 가장 인덱스가 낮은 오브젝트를 가져온다.
        else if (this.gameObject.CompareTag("SWAT"))
            hpbar = ObjectPoolingManager.poolingManager.GetS_Hpbar();

        hpbarimg = hpbar.GetComponentsInChildren<Image>()[1]; // 가져온 오브젝트의 1인덱스의 속한 오브젝트의 이미지 컴포넌트를 가져온다.
        var _hpbar = hpbar.GetComponent<EnemyHpBar>(); //hpbar를 소환하는 스크립트를 가져온다.

        _hpbar.targetTr = this.gameObject.transform; // 이 스크립트를 가진 에너미의 위치를 targetTr에 저장한다.
        _hpbar.offset = hpbarOffset; // hpbar 소환 스크립트의 offset변수에 hpbarOffset 좌표를 넣는다.

        hpbarimg.fillAmount = 1f; //hpbar 채우는 코드 위치 옮김
        hpbarimg.color = Color.red; //hp바 색 다시 빨강으로 초기화
        hp = 100f;

        hpbar.SetActive(true); //hpbar 활성화
    }

    void Start()
    {
        bloodEffect = Resources.Load("Effects/BulletImpactFleshBigEffect") as GameObject;
    }
    #region 프로젝타일 방식의 충돌 감지 isTrigger 체크된 경우 OnTriggerEnter
    //private void OnCollisionEnter(Collision col)
    //{
    //    if(col.collider.CompareTag(bulletTag))
    //    {
    //        col.gameObject.SetActive(false);
    //        // 맞은 위치를 넘김  Collision 구조체안에 contacts라는 배열이 있다.
    //        ShowBloodEffect(col);
    //        curhp -= col.gameObject.GetComponent<BulletCtrl>().damage;
    //        curhp = Mathf.Clamp(curhp, 0f, 100f);
    //        if (curhp <= 0f)
    //            Die();
    //    }

    //}
    #endregion

    void OnDamage(object[] _params)
    {
        ShowBloodEffect((Vector3)_params[0]);
        hp -= (float)_params[1];
        hp = Mathf.Clamp(hp, 0f, 100f);
        hpbarimg.fillAmount = hp / 100.0f;
        if (hp <= 0f)
        {
            //hpbarimg.GetComponentsInChildren<Image>()[1].color = Color.clear;
            Die();
        }
    }
    void Die()
    {
       // Debug.Log("사망!");
        GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
        GameManager.G_instance.KillScore();
    }
    private void ShowBloodEffect(Vector3 col)
    {
        Vector3 pos = col; //위치
        Vector3 _normal = col.normalized; //방향
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, _normal);
        GameObject blood = Instantiate(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }
}
