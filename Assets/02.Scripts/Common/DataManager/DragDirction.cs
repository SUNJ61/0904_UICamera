using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DataInfo;
public class DragDirction : MonoBehaviour,IPointerDownHandler,IPointerUpHandler
{
    private Vector2 startPosition;
    public Transform ItemListTr;
    public Transform ItemTr;
    public bool isDown = false;
    void Start()
    {
        ItemListTr = GameObject.Find("ItemList ").transform;
        ItemTr = GetComponent<Transform>();
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        startPosition = Input.mousePosition;
        //startPosition = Input.touches[0].position; //모바일 플랫폼이면 
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Vector2 endPosition = Input.mousePosition;
        Vector2 direction = endPosition - startPosition;

        // 아래쪽으로 드래그했는지 판별
        if (direction.y < 0 )
        {
          
            Debug.Log("아래쪽으로 드래그했습니다.");
           
            isDown = true;
        }
        else
        {
            Debug.Log("다른 방향으로 드래그했습니다.");
            isDown = false;
        }
    }
}
