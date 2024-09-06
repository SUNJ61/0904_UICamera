using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
1. 적캐릭터의 월드 좌표를 읽어서 스크린 좌표로 변경.
2. 해당 스크린 좌표를 읽어서, 캔버스 위에 좌표로 변경.
3. 캔버스위의 좌표를 다시 적캐릭터의 로컬 좌표로 변경한다. (해당 좌표의 HpBar를 소환시켜 따라다녀야함.)
*/
public class EnemyHpBar : MonoBehaviour
{
    [SerializeField]
    private Camera UI_Camera;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RectTransform rectParent;
    [SerializeField]
    private RectTransform rectHp;

    public Transform targetTr; // UI가 따라다닐 타겟 위치

    public Vector3 offset = Vector3.zero; // targetTr위치에서 hpbar가 부착될 위치를 조절하는 변수
    void Start()
    {
        canvas = GameObject.Find("Canvas-EnemyUI").GetComponent<Canvas>();
        UI_Camera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = this.gameObject.GetComponent<RectTransform>();
    }
    void LateUpdate() //캐릭터가 움직이고 UI가 따라가야하므로
    {
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset); // hpbar를 붙일 월드 좌표를 스크린 좌표로 변환

        if(screenPos.z < 0f) //월드 좌표는 3차원 스크린 좌표는 2차원이다. 즉, 넘어올 때 월드좌표의 Z값이 0보다 작으면 카메라 뒤쪽에 위치한 적을 나타낸다.
        {// UI_Camera가 해당 적 캐릭터의 UI를 바라보려고 하는 현상이 발생한다. 이러한 현상을 막기 위한 로직으로 넘어오는 월드 좌표의 z값을 양수로 만들어 준다.
            //screenPos.z *= -1.0f; //강사님 코드
            screenPos *= -1.0f; //강사님 코드
        }

        Vector2 screenPoint = new Vector2(screenPos.x, screenPos.y);

        var localPos = Vector2.zero; //아래의 코드에서 스크린 좌표를 로컬좌표로 변환하여 해당 변수에 저장한다.
        RectTransformUtility.ScreenPointToLocalPointInRectangle //스크린 좌표를 다시 로컬 좌표로 변경한다.
            (rectParent, screenPos, UI_Camera, out localPos);  //  카메라(UI_Camera)가 보고있는 캔버스(rectParent)의 스크린 좌표(screenPos)를 로컬좌표로 변환하여 저장한다. (localPos에 저장) 

        rectHp.localPosition = localPos; //hpbar의 위치를 변경한다.
    }
}
