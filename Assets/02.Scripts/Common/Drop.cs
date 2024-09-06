using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DataInfo;
public class Drop : MonoBehaviour,IDropHandler
{
   // public int isDropCount = 0;
    public void OnDrop(PointerEventData eventData)
    {        //자식 오브젝트가 없다면 
        if(transform.childCount == 0)
            Drag.draggingItem.transform.SetParent(transform,false);
            //드래그 아이템 부모 오브젝트는 Slot 자기 자신이 된다.
        Item item = Drag.draggingItem.GetComponent<ItemInfo>().itemData;
        //드래그 되고 있는 오브젝트의 ItemInfo 클래스의 itemData를 대입
        // 인벤토리 UI에서 올라간 아이템을 item 에 대입
        //Drag.draggingItem.GetComponent<Drag>().isDropping = true;
        GameManager.G_instance.AddItem(item);
        //++isDropCount;
        
    }

}
