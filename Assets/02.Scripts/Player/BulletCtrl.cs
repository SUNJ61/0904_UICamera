using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletCtrl : MonoBehaviour
{
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform tr;
    [SerializeField] private TrailRenderer trailRenderer;
    public float Speed = 1500f;
    public float damage = 25f;
    void Awake()
    {
        tr = transform;
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();

        //Destroy(this.gameObject,3.0f);
        Invoke("BulletDisable", 2.0f);
    }
    void BulletDisable()
    {
        this.gameObject.SetActive(false);
    }
    private void OnEnable() //오브젝트가 활성화 될때 자동호출
    {
        damage = GameManager.G_instance.gameData.damage;
        GameManager.OnItemChange += UpdateSetup;
        rb.AddForce(tr.forward * Speed);
    }
    void  UpdateSetup()
    {
        damage = GameManager.G_instance.gameData.damage;
    }
    private void OnDisable()//오브젝트가 비활성화 될때 자동호출
    {
        trailRenderer.Clear();
       //tr.position  = Vector3.zero; 
        tr.rotation = Quaternion.identity;
        rb.Sleep(); // 리지디 바디 작동 중지 
    }
}
