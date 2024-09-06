using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//플레이가 되었을 때 배럴 색상이 랜덤 하게 만들기 
//5번 배럴이 총알에 맞으면 폭파 물리현상을 구현하기 
public class BarrelCtrl : MonoBehaviour
{
    [SerializeField] private GameObject Effect; //폭파 이펙트 
    [SerializeField] private Texture[] textures;
    [SerializeField] private MeshRenderer meshRenderer;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private int hitCount = 0;
    [SerializeField] private AudioClip ExpClip;
    [SerializeField] private MeshFilter meshFilter;
    [SerializeField] private Mesh[] meshes;
    private readonly string bulletTag = "BULLET";
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        meshRenderer = GetComponent<MeshRenderer>();
        meshFilter = GetComponent<MeshFilter>();
        meshes = Resources.LoadAll<Mesh>("Meshes");
        textures = Resources.LoadAll<Texture>("BarrelTextures");
        meshRenderer.material.mainTexture = textures[Random.Range(0,textures.Length)];
        Effect = Resources.Load("ExpEffect") as GameObject;
        ExpClip = Resources.Load("Sounds/grenade_exp_1") as AudioClip;
    }
    #region 프로젝타일 방식의 충돌감지 
    //private void OnCollisionEnter(Collision col)
    //{
    //    if(col.gameObject.CompareTag(bulletTag))
    //    {
    //        if(++hitCount ==5)
    //        {

    //            ExplosionBarrel();
    //        }
    //    }

    //}
    #endregion

    void OnDamage(object[] _params)
    {
        Vector3 firePos = (Vector3)_params[0]; //발사위치
        Vector3 hitPos = (Vector3)_params[1]; // 맞은 위치 
        // 맞은 좌표에서 발사위치 빼면 = 거리와 방향을 구할 수 있다.
        Vector3 incomeVector = hitPos - firePos; //Ray의 각도를 구하기 위해
        // 전문 용어로는 입사 벡터라고 한다.
        incomeVector = incomeVector.normalized; //입사벡터를 정규화 벡터로 변경
        //Ray의 hit 좌표에 입사벡터의 각도로 힘을 생성 
        GetComponent<Rigidbody>().AddForceAtPosition(incomeVector * 1500f,
            hitPos);
        // 어떤 지점에 힘을 모아서 물리가 생성되게 하려고 할때 호출 되는 메서드

        if (++hitCount == 5)
            ExplosionBarrel();
        
    }
    void ExplosionBarrel()
    {
        Camera.main.GetComponent<CameraShake>().TurnOn();
        //폭파 이펙트 효과
        GameObject eff = Instantiate(Effect,transform.position,Quaternion.identity);
        Destroy(eff,2.0f);

        Collider[] Cols = Physics.OverlapSphere(transform.position, 20f, 1 << 7 | 1<<12);
             //배럴 자기자신 위치에서 20반경에 배럴 레이어만 Cols라는 배열에 담는다
        foreach(Collider col in Cols)
        {
            Rigidbody rigidbody = col.GetComponent<Rigidbody>();
            if(rigidbody != null)
            {
                rigidbody.mass = 1.0f; //배럴 무게를 가볍게 
                rigidbody.AddExplosionForce(1000, transform.position, 20f, 1200f);
                //리지디바디 클래스 폭파 함수는 AddExplosionForce(폭파력, 위치 ,반경,위로 솟구치는 힘)
                col.gameObject.SendMessage("ExpDie");
            }
            Invoke("BarrelMassChange", 3.0f);
        }
        SoundManager.S_instance.PlaySound(transform.position, ExpClip);
        //찌그러진 메쉬를 적용
        int idx = Random.Range(0, meshes.Length);
        meshFilter.sharedMesh = meshes[idx];
        GetComponent<MeshCollider>().sharedMesh = meshes[idx];
    }
    void BarrelMassChange()
    {
        Collider[] Cols = Physics.OverlapSphere(transform.position, 20f, 1 << 7);
        //배럴 자기자신 위치에서 20반경에 배럴 레이어만 Cols라는 배열에 담는다
        foreach (Collider col in Cols)
        {
            Rigidbody rigidbody = col.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.mass = 60.0f; 
             
            }


        }
    }
}
