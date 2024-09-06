using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFOV : MonoBehaviour
{
    public float viewRange = 15.0f; //적캐릭터의 추적 사정 거리 
    [Range(0, 360)]
    public float viewAngle = 120f; //적캐릭터의 시야각
    [SerializeField] private Transform enemyTr;
    [SerializeField] private Transform playerTr;
    [SerializeField] private int playerLayer;
    [SerializeField] private int boxLayer;
    [SerializeField] private int layerMask;
    [SerializeField] private int barrelLayer;
    void Start()
    {
        enemyTr = GetComponent<Transform>();
        playerTr = GameObject.FindWithTag("Player").GetComponent<Transform>();

        playerLayer = LayerMask.NameToLayer("PLAYER");
        boxLayer = LayerMask.NameToLayer("BOXES");
        barrelLayer = LayerMask.NameToLayer("BARREL");
        layerMask = 1 << playerLayer | 1 << boxLayer | 1 << barrelLayer;
    }
    public Vector3 CirclePoint(float angle) //원주의 시작점을 알기위해 
    { 
        //로컬 좌표계 기준으로 설정하기 위해서 적캐릭의 Y회전값을 더함 
        angle += transform.eulerAngles.y;
        return new Vector3(Mathf.Sin(angle * Mathf.Deg2Rad), 0f
            , Mathf.Cos(angle * Mathf.Deg2Rad));
        // 반지름이 1인 단위원으로 가정하면 X축 sin =x/1 이란 식이 성립하고
        //  따라서 x =sin 이되고 마찬가지로 z축 cos  =z/1 이기에 z= cos 성립
        //Mathf.Deg2Rad는 변환상수 로서 (PI*2)/360 과 같다.
        //일반 각도를 라디안으로 변환
        //그반대도 있다.Mathf.Rad2Deg 라디안을 일반각도로 변환
    }
    public bool isTracePlayer() //플레이어를 추적해야 하는 지 판단 함수
    {
        bool isTrace = false;
         // 15 반경 안에서 적 포지션에서 플레이어가  있는지는 추출 
        Collider[] colls = Physics.OverlapSphere(enemyTr.position,
            viewRange, 1 << playerLayer);
        if(colls.Length ==1) //플레이어가 범위 안에 있다고 판단
        {
            // 적캐릭터와 주인공 사이의 방향 벡터를 계산 함 
            Vector3 dir= (playerTr.position - enemyTr.position).normalized;
            // 적 캐릭터의 시야각에 들어왔다고 판단
            if(Vector3.Angle(enemyTr.forward,dir) <viewAngle *0.5f)
            {
                isTrace = true;
            }
        }
        return isTrace;
    }
    public bool isViewPlayer() //플레이어를 보았다면 공격 하기 위한 판단 함수 
    {
        bool isView = false;
        RaycastHit hit;

        Vector3 dir = (playerTr.position - enemyTr.position).normalized;
        //레이캐스트를 쏘아서 장애물이 있는 지 판단
        if(Physics.Raycast(enemyTr.position, dir, out hit,viewRange ,layerMask))
        {
            isView = (hit.collider.CompareTag("Player"));
        }
        return isView;


    }
}
