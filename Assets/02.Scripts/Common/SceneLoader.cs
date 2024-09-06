using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public CanvasGroup fadeCG; //페이드 인 적용 하기 위해
    [Range(0.5f, 2.0f)] public float fadeDuration = 1.0f;
    public Dictionary<string,LoadSceneMode> loadScenes = new Dictionary<string,LoadSceneMode>();
    void InitSceneInfo()
    {
        loadScenes.Add("Level_1", LoadSceneMode.Additive);
        loadScenes.Add("BattleFieldScene",LoadSceneMode.Additive);

    }
    IEnumerator Start()
    {
        //fadeCG = GetComponent<CanvasGroup>();
        fadeCG.alpha = 1.0f;// 처음 알파값 불투명
        InitSceneInfo(); //씬 합치기 
        
        foreach (var loadscene in loadScenes )
        {
            yield return StartCoroutine(LoadScene(loadscene.Key,loadscene.Value));
        }

        StartCoroutine(Fade(0.0f));
    }
    IEnumerator LoadScene(string sceneName,LoadSceneMode mode)
    {
        //비동기 방식으로 씬을 로드 하고 로드가 완료 될 때까지 대기함 
        yield return SceneManager.LoadSceneAsync(sceneName,mode);
        //호출 된 씬을 활성화 
        Scene loadedScene = SceneManager.GetSceneAt(SceneManager.sceneCount-1);
        SceneManager.SetActiveScene(loadedScene);

    }
    IEnumerator Fade(float finalAlpha)
    {
        // 라이트 맵이 깨지는 것을 방지 하기 위해 스테이지 씬을 활성화 
        SceneManager.SetActiveScene(SceneManager.GetSceneByName("Level_1"));
        fadeCG.blocksRaycasts= true;
        //절대값 함수로 백분율 계산  
        float fadeSpeed = Mathf.Abs(fadeCG.alpha - finalAlpha) / fadeDuration;
                                   //1.0f         0.0f
        while(!Mathf.Approximately(fadeCG.alpha, finalAlpha))
        {       //아직 불투명 하다면 
            fadeCG.alpha = Mathf.MoveTowards(fadeCG.alpha,finalAlpha,fadeSpeed *Time.deltaTime);
                           //점진적으로 부드럽게 투명하게 변한다.
            yield return null; //  한프레임 다음에 

        }
      
        fadeCG.blocksRaycasts = false;
        // 비동기적(독립적)으로 FadeIn이 완료 된 후  SceneLoader은 삭제 (Unload)
        SceneManager.UnloadSceneAsync("SceneLoader");
    }    
}
