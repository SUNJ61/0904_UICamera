using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class EnemyAI : MonoBehaviour
{    
    public enum State //열거형 상수 
    {
        PTROL=0,TRACE,ATTACK,DIE
    }
    public State state = State.PTROL;
    [SerializeField] private Transform playerTr; // 거리를 재기위해 선언
    [SerializeField] private Transform enemyTr; //거리를 재기위해 선언
    [SerializeField] private Animator animator;
    [SerializeField] private EnemyFOV enemyFOV;
    // 공격 거리 추적 거리 
    public float attackDist = 5.0f;
    public float traceDist = 10f;
    public bool isDie = false; //사망 여부 판단
    private WaitForSeconds ws;
    private EnemyMoveAgent moveAgent;
    private EnemyFire enemyFire;
    // 애니메이터 컨트롤러에 정의 한 파라미터의 해시값을 정수로 미리 추출
    private readonly int hashMove = Animator.StringToHash("IsMove");
    private readonly int hashSpeed = Animator.StringToHash("moveSpeed");
    private readonly int hashDie = Animator.StringToHash("DieTrigger");
    private readonly int hashDieIdx = Animator.StringToHash("DieIdx");
    private readonly int hashOffset = Animator.StringToHash("Offset");
    private readonly int hashWalkSpeed = Animator.StringToHash("WalkSpeed");
    private readonly int hashPlayerDie = Animator.StringToHash("PlayerDieTrigger");
    private readonly int hashexpDie = Animator.StringToHash("ExpDieTrigger");

    private int check_obj;
    void Awake()
    {
        animator = GetComponent<Animator>();
        moveAgent = GetComponent<EnemyMoveAgent>();
        var player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        playerTr = player.GetComponent<Transform>();
        enemyTr = GetComponent<Transform>();
        enemyFire = GetComponent<EnemyFire>();
        ws = new WaitForSeconds(0.3f);
        enemyFOV = GetComponent<EnemyFOV>();

        if (this.gameObject.CompareTag("ENEMY")) //오브젝트의 원래 태그를 기억하기 위해 선언
            check_obj = 0;
        else if (this.gameObject.CompareTag("SWAT"))
            check_obj = 1;
    }
    private void OnEnable() //오브젝트 활성화 될때 마다 호출
    {
        Damage.OnPlayerDie += OnPlayerDie;
        // 이벤트 연결 
        animator.SetFloat(hashOffset, Random.Range(0.2f, 1.0f));
        animator.SetFloat(hashWalkSpeed, Random.Range(1.0f, 2.0f));
        StartCoroutine(CheckState());
        StartCoroutine(Action());
    }
    IEnumerator CheckState()
    {
        yield return new WaitForSeconds(1.0f);
        // 오브젝트 풀에 생성시 다른 스크립트의 초기화를 위해 대기 
        while(!isDie)
        {
            if(state == State.DIE) yield break;
            //사망 상태이면 코루틴 함수를 종료 시킴
            float dist = (playerTr.position - enemyTr.position).magnitude;
            if (dist <= attackDist)
            {
                if (enemyFOV.isViewPlayer())
                    state = State.ATTACK; // 장애물이 없으면 공격 모드 
                else 
                    state = State.TRACE;
            }
                
            else if (enemyFOV.isTracePlayer())
                state = State.TRACE;
            else
                state = State.PTROL;

            yield return ws;
        }
    }
    IEnumerator Action()
    {
        while (!isDie)
        {
            yield return ws;
            switch(state)
            {
                case State.PTROL:
                    enemyFire.isFire = false;
                    moveAgent.patrolling = true;
                    animator.SetBool(hashMove, true);
                    break;
                case State.ATTACK:
                    if(enemyFire.isFire==false)
                      enemyFire.isFire = true;
                    moveAgent.Stop();
                    animator.SetBool(hashMove, false);
                    break;
                case State.TRACE:
                    enemyFire.isFire =false;
                    moveAgent.traceTarget = playerTr.position;
                    animator.SetBool(hashMove, true);
                    break;
                case State.DIE:
                    EnemyDie();
                    break;
            }

        }


    }

    private void EnemyDie()
    {
        isDie = true;
        moveAgent.Stop();
        enemyFire.isFire = false;
        animator.SetTrigger(hashDie);
        animator.SetInteger(hashDieIdx, Random.Range(0, 2));
        GetComponent<Rigidbody>().isKinematic = true;//물리제거
        GetComponent<CapsuleCollider>().enabled = false;//캡슐 콜라이더 비활성화
        //GetComponent<EnemyHpBar>().gameObject.SetActive(false); //죽이면 hp바 꺼짐, 강사님 코드 -> ObjectPoolPush안에 있음
        GetComponent<EnemyDamage>().hpbar.SetActive(false); //내가 수정한 코드
        gameObject.tag = "Untagged"; //사망 했다면 태그빼줌
        state = State.DIE;
        StartCoroutine(ObjectPoolPush());
    }
    void ExpDie()
    {
        if (isDie) return;
        GameManager.G_instance.KillScore();
        moveAgent.Stop();
        enemyFire.isFire = false;
        GetComponent<Rigidbody>().isKinematic = true;//물리제거
        GetComponent<CapsuleCollider>().enabled = false;//캡슐 콜라이더 비활성화
        //GetComponent<EnemyHpBar>().gameObject.SetActive(false); //죽이면 hp바 꺼짐
        GetComponent<EnemyDamage>().hpbar.SetActive(false); //내가 수정한 코드
        gameObject.tag = "Untagged"; //사망 했다면 태그빼줌
        state = State.DIE;
        isDie = true;
        animator.SetTrigger(hashexpDie);
        StartCoroutine(ObjectPoolPush());
        
    }
    IEnumerator ObjectPoolPush()
    {
        yield return new WaitForSeconds(3f);

        isDie = false;
        GetComponent<Rigidbody>().isKinematic = false;//물리가 있게
        GetComponent<CapsuleCollider>().enabled = true;//캡슐 콜라이더 활성화
        //GetComponent<EnemyDamage>().hpbarimg.fillAmount = 1f; //강사님 코드
        //GetComponent<EnemyDamage>().hpbarimg.color = Color.red; //hp바 색 다시 빨강으로 초기화
        //GetComponent<EnemyDamage>().hp = 100f; //강사님 코드

        if (check_obj == 0)
            gameObject.tag = "ENEMY"; // 오브젝트가 활성화 되기전 태그이름을 원래대로
        else if (check_obj == 1)
            gameObject.tag = "SWAT";

        gameObject.SetActive(false);
        state = State.PTROL;
    }
    void OnPlayerDie()
    {
        StopAllCoroutines(); //모든 코루틴 중지
        //애니메이션 재생
        animator.SetTrigger(hashPlayerDie);
        GameManager.G_instance.isGameOver = true;
    }


    void Update()
    {
        animator.SetFloat(hashSpeed,moveAgent.speed);
    }
    private void OnDisable()
    {
        Damage.OnPlayerDie -= OnPlayerDie;
                   // 이벤트 연결 해제 
    }
}
