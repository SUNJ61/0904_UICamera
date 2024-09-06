using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IDragHandler,IBeginDragHandler,IEndDragHandler
{
    [SerializeField]
    private Transform itemTr;
    [SerializeField]
    private Transform inventoryTr;
    [SerializeField]
    private Transform itemList;
    [SerializeField]
    private CanvasGroup canvasGroup;
    public static GameObject draggingItem = null;
    DragDirction dragDirction;
    IEnumerator Start()
    {
        inventoryTr = GameObject.Find("Inventory").transform;
        itemTr = GetComponent<Transform>();
        yield return new WaitForSeconds(0.5f);
        itemList = GameObject.Find("ItemList ").transform;
        canvasGroup = GetComponent<CanvasGroup>();
        dragDirction = GetComponent<DragDirction>();
    }
    public void OnDrag(PointerEventData eventData)
    {        //드래그 이벤트
         itemTr.position = Input.mousePosition;
         // 드래그 이벤트가 발생하면 아이템의 위치를 
         //마우스 커서의 위치로 변경
    }
    // 드래그를 시작 할 때 한 번 호출 되는 이벤트 
    public void OnBeginDrag(PointerEventData eventData)
    {
       this.transform.SetParent(inventoryTr);
        //드래그가 시작되면  부모 오브젝트는 인벤토리가 된다.
        draggingItem = this.gameObject;
        //드래그가 시작되면 드래그 되는 아이템 정보를 저장
        canvasGroup.blocksRaycasts = false;
        //드래그가 시작 되었을 때 다른 UI이벤트를 받지 않게 하기 
    }
    //드래그가 끝났다면 호출 되는 이벤트 함수 
    public void OnEndDrag(PointerEventData eventData)
    {   
        //드래그가 종료 되면 드래그 아이템을 null로 변경
        draggingItem = null;
        canvasGroup.blocksRaycasts = true;
        //부모가 인벤토리라면 
        if(itemTr.parent == inventoryTr )
        {        //아이템은 아이템리스트가 부모가 된다.
            
            itemTr.SetParent(itemList.transform);
           
           GameManager.G_instance.RemoveItem(GetComponent<ItemInfo>().itemData);
               //인벤토리 슬롯에 올라간 아이템을 뺐다면 
          
          
        }

    }
}
