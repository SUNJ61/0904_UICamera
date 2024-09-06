using DataInfo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager G_instance;
    public bool isGameOver= false;
    public CanvasGroup inventoryCG;
    public Text killTXT;
    public int killCount = 0;
    [Header("DataManager")]
    [SerializeField]
    DataManager dataManager;
    //public GameData gameData;
    public GameDataObject gameData;
    //인벤토리 아이템이 변경 되었을 때 발생 시킬 이벤트 정의
    public delegate void ItemChangeDelegate();
    public static event ItemChangeDelegate OnItemChange;
    [SerializeField] private GameObject slotList;
    public GameObject[] itemObjects;

    void Awake()
    {
        if (G_instance == null)
            G_instance = this;
        else if (G_instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
         
        dataManager = GetComponent<DataManager>();
        dataManager.Initialize();

        inventoryCG = GameObject.Find("Inventory").GetComponent<CanvasGroup>();
        killTXT  = GameObject.Find("Canvas-UI").transform.GetChild(7).GetComponent<Text>();
        OnInventoryOpen(false);
        LoadGameData();
    }
    void LoadGameData() //게임 초기 데이터 로드
    {              //플레이어 프리퍼런스 
                   //killCount = PlayerPrefs.GetInt("KILLCOUNT", 0);
                   //키값을 예약을 한다.
        //GameData data = dataManager.Load();
        //gameData.hp = data.hp;
        //gameData.damage = data.damage;
        //gameData.speed = data.speed;
        //gameData.killCount = data.killCount;
        //gameData.equipItem = data.equipItem;
        //ScriptableObject는 전역적으로 접근 할 수 있기에
        //별도로 로드하는 과정이 필요 없다.
        if (gameData.equipItem.Count > 0)
            InventorySetUp();

        killTXT.text = "<color=#00ff00>KILL:</color> " + "<color=#ff0000>" + gameData.killCount.ToString("0000") + "</color>";
    }
    void InventorySetUp()
    {
        var slots = slotList.GetComponentsInChildren<Transform>();
        for(int i =0; i< gameData.equipItem.Count; i++)
        {
            for(int j =1;j< slots.Length; j++)
            {     //Slot 하위에 다른 아이템이 있으면 다음 인덱스로 바로 넘어감
                if (slots[j].childCount > 0) continue;
                int itemIndex = (int)gameData.equipItem[i].itemType;
                //보유한 아이템의 종류에 따라 인덱스를 추출
                itemObjects[itemIndex].GetComponent<Transform>().SetParent(slots[j].transform);
                //아이템의 부모는 슬롯 게임오브젝트 가 된다.
                itemObjects[itemIndex].GetComponent<ItemInfo>().itemData = gameData.equipItem[i];
                //아이템의 ItemInfo 클래스의 itemData에 로드한 gameData.equipItem[i] 데이터 값을 저장
                break;
                
            }
        }

    }
    void SaveGameData()
    {
        //dataManager.Save(gameData);
        //.asset 파일에 데이터 저장 
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameData);
#endif
    }
    //인벤토리에 아이템을 추가 했 을때 데이터 정보를 업데이트 하는 함수
    public void AddItem(Item item)
    {      // 보유 아이템에 같은 아이템이면 추가하지 않고 빠져나감
        if (gameData.equipItem.Contains(item)) return;
        gameData.equipItem.Add(item);
        //아이템을 GameData.equipItem 배열에 추가 
        switch(item.itemType)
        {
            case Item.ItemType.HP:
                 //아이템 계산 방식에 따라 처리 
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.hp += item.value;
                else
                    gameData.hp += gameData.hp * item.value;

                break;
            case Item.ItemType.DAMAGE:
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.damage += item.value;
                else
                    gameData.damage += gameData.damage * item.value;

                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.speed += item.value;
                else
                    gameData.speed += gameData.speed * item.value;
                break;
            case Item.ItemType.GRENADE:

                break;


        }
        OnItemChange();
        //아이템이 변경된 것을 실시간으로 반영 하기 위해
        //이벤트를 발생 시킴
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameData);
#endif
    }
    public void RemoveItem(Item item) 
    {
        //인벤토리 UI에서 특정 아이템을 빼면 뺀 아이템이 적용되기 위해
        gameData.equipItem.Remove(item);
        switch (item.itemType)
        {
            case Item.ItemType.HP:
                //아이템 계산 방식에 따라 처리 
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.hp -= item.value;
                else
                    gameData.hp = gameData.hp  /(1.0f+item.value);

                break;
            case Item.ItemType.DAMAGE:
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.damage -= item.value;
                else
                    gameData.damage = gameData.damage / (1.0f+ item.value);

                break;
            case Item.ItemType.SPEED:
                if (item.itemCalc == Item.ItemCalc.VALUE)
                    gameData.speed -= item.value;
                else
                    gameData.speed = gameData.speed /(1.0f+ item.value);
                break;
            case Item.ItemType.GRENADE:

                break;


        }
        OnItemChange();
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(gameData);
#endif

    }


    public bool isPaused =false;
    public void OnPauseClick()
    {
        isPaused = !isPaused;
        Time.timeScale = (isPaused) ? 0.0f : 1f;
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        var scripts = playerObj.GetComponents<MonoBehaviour>();
          //플레이어에 있는 MonoBehaviour 상속 하는 스크립트들 
        foreach (var script in scripts)
        {
            script.enabled = !isPaused;
        }
        var canvasGroup = GameObject.Find("Panel-Weapon").GetComponent<CanvasGroup>();
        canvasGroup.blocksRaycasts =!isPaused;
    }
   
    public void OnInventoryOpen(bool isOpened)
    {
       
        Time.timeScale = (isOpened) ? 0f : 1f;
        inventoryCG.alpha = (isOpened) ? 1.0f : 0.0f;
        inventoryCG.interactable = isOpened;
        inventoryCG.blocksRaycasts=isOpened;

    }
    public void KillScore()
    {
        //++killCount;
        ++gameData.killCount;
        killTXT.text = "<color=#00ff00>KILL:</color> " + "<color=#ff0000>" + gameData.killCount.ToString("0000") + "</color>";
        //PlayerPrefs.SetInt("KILLCOUNT", killCount);
        //킬수를 저장 한다. 
        //PlayerPrefs 은 보안성이 없다.
        //실무에서 쓴다고 한다면  암호화 해서 저장 해야 한다.

    }
    private void OnDisable()
    {
       // PlayerPrefs.DeleteKey("KILLCOUNT");
        
    }
    private void OnApplicationQuit()
    {
        SaveGameData();
    }
}
