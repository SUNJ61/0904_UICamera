using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoveBullet : MonoBehaviour
{
    [SerializeField] private GameObject hitspark;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] private readonly string bulletTag = "BULLET";
    [SerializeField] private readonly string E_bulletTag = "E_BULLET";
    [SerializeField] private FireCtrl fireCtrl;
    void Start()
    {
        source = GetComponent<AudioSource>();
        fireCtrl = GameObject.FindWithTag("Player").GetComponent<FireCtrl>();
        hitspark = Resources.Load("HitSpark") as GameObject;
        hitClip = Resources.Load("Sounds/hitMetal") as AudioClip;
    }
    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag(E_bulletTag))
            
        {
            //Destroy(collision.gameObject);
            collision.gameObject.SetActive(false);
            Vector3 hitPos = collision.transform.position;
            //Vector3 firePos = (fireCtrl.firePos.position - hitPos).normalized;
            Quaternion rot = Quaternion.FromToRotation(Vector3.forward, hitPos.normalized);
            GameObject spark = Instantiate(hitspark, hitPos, rot);
            Destroy(spark, 1f);
            //source.PlayOneShot(hitClip, 1f);
            SoundManager.S_instance.PlaySound(hitPos, hitClip);
        }
    }
    void OnDamage(object[] _params)
    {
        Vector3 hitPos = (Vector3) _params[0];
        Vector3 hitNormal = (Vector3)_params[0];
        hitNormal = (fireCtrl.firePos.position - hitPos).normalized;
        //Vector3 firePos = (fireCtrl.firePos.position - hitPos).normalized;
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, hitNormal);
        GameObject spark = Instantiate(hitspark, hitPos, rot);
        Destroy(spark, 1f);
        //source.PlayOneShot(hitClip, 1f);
        SoundManager.S_instance.PlaySound(hitPos, hitClip);

    }

}
