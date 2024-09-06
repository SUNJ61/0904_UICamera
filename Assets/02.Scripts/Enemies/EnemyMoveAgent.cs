using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyMoveAgent : MonoBehaviour
{
    //패트롤 지점을 담기 위한 List 제네릭(일반형) 변수 
    public List<Transform> wayPointList;
    public int nexIdx = 0;// 다음 순찰 지점의 배열 인덱스 값
    [SerializeField] private NavMeshAgent agent;
    private readonly float patrolSpeed = 1.5f;
    private readonly float traceSpeed = 4.0f;
    private float damping = 1.0f; //회전 할 때의 속도를 조절 하는 계수
    private Transform enemyTr;
    private bool _patrolling;
    public bool patrolling //프로 퍼티 
    {
        get { return _patrolling; } 
        set 
        { 
            _patrolling = value; 
              if (_patrolling)
              {
                agent.speed = patrolSpeed;
                damping = 1.0f;
                MovewayPoint();
              }
         }
    }
    private Vector3 _traceTarget;
    public Vector3 traceTarget
    {
        get { return _traceTarget; }
        set
        {
            _traceTarget = value;
            agent.speed = traceSpeed;
            damping = 7.0f;
            TraceTarget(_traceTarget);
        }
    }
    public float speed
    {                 //NavmeshAgent 속도 
        get { return agent.velocity.magnitude; }
    }
    void Start()
    {
        enemyTr = GetComponent<Transform>();
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;
        agent.updateRotation = false;
        //에이전트를 이용해서 회전하는 기능을 비활성화 이유 부드럽지가 않음
        var group = GameObject.Find("WayPointGroup");
        // 하이라키에 있는 오브젝트명 이 WayPointGroup 를 찾아서 대입
        if (group != null )
        {         // 하위 오브젝트의 트랜스폼을 wayPointList에 다 담는다.
            group.GetComponentsInChildren<Transform>(wayPointList);
            wayPointList.RemoveAt(0); //첫번째 인덱스는 삭제 
        }
        nexIdx = Random.Range(0,wayPointList.Count);
        MovewayPoint();

    }
    void Update()
    {
        if (agent.isStopped == false)
        {      
            //NavMeshAgent 가야 할 방향 벡터를 쿼터니언 타입의 각도로 변환
            Quaternion rot = Quaternion.LookRotation(agent.desiredVelocity);
            enemyTr.rotation = Quaternion.Slerp(enemyTr.rotation, rot, 
                                          Time.deltaTime * damping);
            //보간 함수를 이용해서 점진적으로 부드럽게 회전 시킴
        }
        if (_patrolling==false) return;
        //다음 도착지점이 0.5보다 작거나 같다면 
        if(agent.remainingDistance <=0.5f)
        {

            //nexIdx = ++nexIdx % wayPointList.Count;
            nexIdx = Random.Range(0, wayPointList.Count);
            MovewayPoint();
        }

    }
    void MovewayPoint()
    {        //최단 경로 계산이 끝나지 않거나 길을 잃어버린경우
        if (agent.isPathStale) return;
        // 경로 지점의 추적대상 =  리스트에 담았던 트랜스폼
        agent.destination = wayPointList[nexIdx].position;
        agent.isStopped = false;
    }
    private void TraceTarget(Vector3 pos)
    {
        if(agent.isPathStale) return;
        agent.destination = pos;
        agent.isStopped = false;
        
    }
    public void Stop()
    {
        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        _patrolling = false;
    }
}
