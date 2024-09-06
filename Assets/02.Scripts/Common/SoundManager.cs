using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager S_instance;

    public float SoundVolumn = 1.0f; //사운드 볼륨
    public bool isSoundMute = false; // 음소거 여부 
   
    void Awake()
    {
        if (S_instance == null) S_instance = this;
        else if (S_instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
    public void PlaySound(Vector3 pos ,AudioClip clip)
    {
        if (isSoundMute) return; // 음소거 일때 이 함수를 빠져나감 

        GameObject soundObj = new GameObject("Sound!");
                  //오브젝트 동적 생성
        soundObj.transform.position = pos; //소리는 위치를 전달 받음
        AudioSource audioSource = soundObj.AddComponent<AudioSource>();
        // 오디오소스  컴퍼넌트 가 없다면.
        // AddComponent<AudioSource>()로 오디오 소스를 부착 할 수 있다.
        audioSource.clip = clip;
        audioSource.minDistance = 20f;
        audioSource.maxDistance = 50f;
        audioSource.volume = SoundVolumn;
        audioSource.Play();
        Destroy(soundObj,clip.length);
    }

}
