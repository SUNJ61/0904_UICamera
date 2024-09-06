using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class UIManager : MonoBehaviour
{
    public void OnClickPlayBtn()
    {
        //SceneManager.LoadScene("Level_1");
        //SceneManager.LoadScene("BattleFieldScene", LoadSceneMode.Additive);
        SceneManager.LoadScene("SceneLoader");
        //LoadSceneMode.Additive 기존씬을 삭제 하지 않고 추가 해서 새로운 씬을 로드한다.
        //LoadSceneMode.Single 기존에 로드된 씬을 모두 삭제후 새로운 씬을 로드 한다.
    }
    public void OnClickQuitBtn()
    {
#if UNITY_EDITOR //유니티 에디터에서는 
        //실행중인 애플리케이션 종료
        EditorApplication.isPlaying = true; //유니티에서 종료 
#else
    Application.Quit(); // 출시 하려고 빌드 한 화면에서 종료 유저에서 종료 
#endif

    }

}
