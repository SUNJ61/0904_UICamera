using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private Vector3 PosCamera;
    [SerializeField] private Quaternion RotCamera;
    [SerializeField] private float shakeTime;
    public bool isShake = false;
   
    void Start()
    {
        
    }
    void Update()
    {
        if (isShake)
        {
            float x = Random.Range(-0.1f, 0.1f);
            float y = Random.Range(-0.1f, 0.1f);
            Camera.main.transform.position += new Vector3(x, y, 0f);
            Vector3 ShakeRot = 
                new Vector3 (0f, 0f, Random.Range(-0.0001f * Time.deltaTime,0.001f * Time.deltaTime));
            Camera.main.transform.rotation = Quaternion.Euler(ShakeRot);
            if(Time.time - shakeTime > 0.5f) 
            {
                isShake = false;
                Camera.main.transform.position = PosCamera;
                Camera.main.transform.rotation = RotCamera; 
                //shakeTime = Time.time;
            }
        }
        
    }
    public void TurnOn()
    {
        isShake = true;
        shakeTime = Time.time;
        PosCamera = Camera.main.transform.position;
        RotCamera = Camera.main.transform.rotation;
    }
}
