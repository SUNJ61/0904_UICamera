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
        yield return new WaitForSeconds(0.2f); //pc�ӵ��� ���� �ð� ����

        if (this.gameObject.CompareTag("ENEMY"))
            hpbar = ObjectPoolingManager.poolingManager.GetE_Hpbar(); //������Ʈ Ǯ���Ŵ������� ������ hpbar�� ��Ȱ��ȭ�� ���� �ε����� ���� ������Ʈ�� �����´�.
        else if (this.gameObject.CompareTag("SWAT"))
            hpbar = ObjectPoolingManager.poolingManager.GetS_Hpbar();

        hpbarimg = hpbar.GetComponentsInChildren<Image>()[1]; // ������ ������Ʈ�� 1�ε����� ���� ������Ʈ�� �̹��� ������Ʈ�� �����´�.
        var _hpbar = hpbar.GetComponent<EnemyHpBar>(); //hpbar�� ��ȯ�ϴ� ��ũ��Ʈ�� �����´�.

        _hpbar.targetTr = this.gameObject.transform; // �� ��ũ��Ʈ�� ���� ���ʹ��� ��ġ�� targetTr�� �����Ѵ�.
        _hpbar.offset = hpbarOffset; // hpbar ��ȯ ��ũ��Ʈ�� offset������ hpbarOffset ��ǥ�� �ִ´�.

        hpbarimg.fillAmount = 1f; //hpbar ä��� �ڵ� ��ġ �ű�
        hpbarimg.color = Color.red; //hp�� �� �ٽ� �������� �ʱ�ȭ
        hp = 100f;

        hpbar.SetActive(true); //hpbar Ȱ��ȭ
    }

    void Start()
    {
        bloodEffect = Resources.Load("Effects/BulletImpactFleshBigEffect") as GameObject;
    }
    #region ������Ÿ�� ����� �浹 ���� isTrigger üũ�� ��� OnTriggerEnter
    //private void OnCollisionEnter(Collision col)
    //{
    //    if(col.collider.CompareTag(bulletTag))
    //    {
    //        col.gameObject.SetActive(false);
    //        // ���� ��ġ�� �ѱ�  Collision ����ü�ȿ� contacts��� �迭�� �ִ�.
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
       // Debug.Log("���!");
        GetComponent<EnemyAI>().state = EnemyAI.State.DIE;
        GameManager.G_instance.KillScore();
    }
    private void ShowBloodEffect(Vector3 col)
    {
        Vector3 pos = col; //��ġ
        Vector3 _normal = col.normalized; //����
        Quaternion rot = Quaternion.FromToRotation(Vector3.forward, _normal);
        GameObject blood = Instantiate(bloodEffect, pos, rot);
        Destroy(blood, 1.0f);
    }
}
