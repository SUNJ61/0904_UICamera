using System.IO; // 파일 입출력을 위한 네임스페이스
using System.Runtime.Serialization.Formatters.Binary;
// 바이너리 포맷을 위한 네임스페이스 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DataInfo;
public class DataManager : MonoBehaviour
{
    [SerializeField] string dataPath; //저장경로
    public void Initialize() //저장 경로를 초기화 하기위한 함수
    {
        dataPath = Application.persistentDataPath + "/gameData3.dat";
        // 파일 저장 경로와 파일명 지정 
    }
    public void Save(GameData gameData)
    {   
        // 바이너리 파일 포맷을 위한 BinaryFormatter 생성
        BinaryFormatter bf = new BinaryFormatter();
        // 데이터 저장을 위한 파일 생성 
        FileStream file = File.Create(dataPath);
        //파일에 저장할 클래스에 데이터 할당
        GameData data = new GameData();
        data.killCount = gameData.killCount;
        data.hp = gameData.hp;
        data.speed = gameData.speed;
        data.damage = gameData.damage;
        data.equipItem = gameData.equipItem;
         // 직렬화 과정을 거친다.
        bf.Serialize(file, data);
        file.Close(); //파일 스트림을 닫는다.
        // 안닫고 다음 로직을 진행 한다. 메모리가 상당히 많이 차지 한채로
        // 게임을 진행 해야 하므로 반드시 닫아야 한다.
    }
    public GameData Load()
    {        //존재 유무 유효성검사 
        if(File.Exists(dataPath))
        {
            //파일이 존재 하는 경우 데이터 불러오기 
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(dataPath, FileMode.Open);   
            GameData data = (GameData)bf.Deserialize(file);
                                 //역직렬화 
             file.Close();
            return data;
        }
        else
        {
            //파일이 없는 경우 기본값을 반환
            GameData data = new GameData();
            return data;
        }


    }
}
