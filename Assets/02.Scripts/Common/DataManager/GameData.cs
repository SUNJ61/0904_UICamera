using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace DataInfo
{
    [System.Serializable]
    public class GameData  
    {
        public int killCount = 0;
        public float hp = 120f;
        public float damage = 25f;
        public float speed = 6.0f;
        public List<Item> equipItem = new List<Item>();
     
    }
    [System.Serializable]
    public class Item
    {
        public enum ItemType { HP=0,SPEED,GRENADE,DAMAGE} //아이템 종류 선언
        public enum ItemCalc { VALUE, PERSENT} //아이템 계산 방식 
        public ItemType itemType; //아이템 종류
        public ItemCalc itemCalc; // 아이템 계산 종류
        public string name; //아이템 이름
        public string desc; //아이템 소개
        public float value; // 계산 값


    }

}
