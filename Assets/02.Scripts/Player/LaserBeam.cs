using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class LaserBeam : MonoBehaviour
{
    private Transform tr;
    private LineRenderer line;
    [SerializeField] private Transform firePos;
    [SerializeField] private FireCtrl fireCtrl;
    [SerializeField] private Player player;
    void Start()
    {
        tr = transform;
        line = GetComponent<LineRenderer>();
        line.useWorldSpace = false;
        line.enabled = false;
        firePos = transform.GetComponentsInParent<Transform>()[1];
        fireCtrl = GetComponentInParent<FireCtrl>();
        player = GetComponentInParent<Player>();
       
    }
    void Update()
    {
        if(Time.timeScale <= 0f) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
           // UI 위에 마우스 포인터나 손가락이 올려져 있다면 빠져 나간다.
        //광선을 미리 생성
        Ray ray = new Ray(firePos.position, tr.forward);
        RaycastHit hit;
      
        Debug.DrawRay(ray.origin, ray.direction*100f ,Color.blue );
        if (fireCtrl.isFire && !fireCtrl.isReloading&& !player.isRunning )
        {
            line.SetPosition(0, tr.InverseTransformPoint(ray.origin));

            if (Physics.Raycast(ray, out hit, 100f))
            {
                line.SetPosition(1, tr.InverseTransformPoint(hit.point));
            }
            else //맞지 않았을 때 끝점을 100으로 잡는다. 
            {
                line.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100f)));

            }
            StartCoroutine(ShowRaserBeam());
        }
        #region 마우스 왼쪽버튼 눌러서 라인 렌더러 나오게하기 
        if (Input.GetMouseButtonDown(0) && !fireCtrl.isReloading)
        {    //라인 랜더러의 첫번째 점의 위치 설정
            line.SetPosition(0, tr.InverseTransformPoint(ray.origin));
                                    //월드좌표 포지션을 로컬좌표 포지션으로 변경
            // 어떤 물체에 광선이 맞았을 때의 위치를 Line Renderer의 끝점으로 설정
            if(Physics.Raycast(ray,out hit,100f))
            {
                line.SetPosition(1,tr.InverseTransformPoint(hit.point));
            }
            else //맞지 않았을 때 끝점을 100으로 잡는다. 
            {                                                
                line.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(100f)));
                                    
            }
            StartCoroutine(ShowRaserBeam());
        }
        #endregion

    }
    IEnumerator ShowRaserBeam()
    {
        line.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.01f,0.2f));
        line.enabled = false;
    }
}
