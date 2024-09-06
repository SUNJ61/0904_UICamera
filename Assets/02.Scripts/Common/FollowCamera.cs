using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] private Transform target; //따라 다닐 대상
    [SerializeField] private float Height = 5.0f; //카메라 높이
    [SerializeField] private float distance = 7.0f; // 타겟과의 거리
    [SerializeField] private float movedamping = 10f; // 카메라가 이동 회전시  떨림을 부드럽게 완화 하는 값
    [SerializeField] private float rotdamping = 15f;
    [SerializeField] private Transform CamTr; // 자기자신 트랜스폼
    [SerializeField] private float targetOffset = 2.0f; // 타켓에서의 카메라 높이값
    [Header("Camera Obstacle Move")]
    public float maxHeight = 12f; // 카메라가 장애물에 가려지면 올라갈 높이
    public float castOffset = 2.0f;// 주인공 머리 위의 높이 
    public float originHeight;
    IEnumerator Start()
    {
        CamTr = GetComponent<Transform>();
        yield return new WaitForSeconds(0.5f);
        Height = 5f;
        originHeight = Height; 
        // 카메라가 위로 이동 후 원래 높이로 돌아오기 위해
    }
    void Update()
    {
        if(target ==null) return;
        Vector3 castTarget = target.position + (target.up * castOffset);
                              //타겟(플레이어)위치에서 1만큼 올림
        Vector3 castDir = (castTarget - CamTr.position).normalized;
                           //타켓 위치에  카메라 위치를 빼면 방향과 거리가 구해짐
        RaycastHit hit;
        if(Physics.Raycast(CamTr.position, castDir, out hit,Mathf.Infinity))
        {
            if(!hit.collider.CompareTag("Player")) //플레이어에 광선에 맞지 않았다면 
            {
                Height = Mathf.Lerp(Height,maxHeight,Time.deltaTime *5f);
                         //수학적인 선형보간 함수 
            }
           else //  맞았다면 
            {
                Height = Mathf.Lerp(Height, originHeight, Time.deltaTime * 5f);
            }
        }


    }
    void LateUpdate()
    {               
       
                //타겟 포지션에서  distance만큼 뒤에 위치      +  Height 높이 만큼 위에 위치 
        var CamPos = target.position - (target.forward * distance) + (target.up * Height);
        CamTr.position = Vector3.Slerp(CamTr.position, CamPos, Time.deltaTime * movedamping);
                //곡면 보간  (자기자신 위치에서 , Campos 까지 ,damping 시간 만큼 부드럽게 움직임
        CamTr.rotation = Quaternion.Slerp(CamTr.rotation,target.rotation, Time.deltaTime * rotdamping);
        //곡면 보간으로 회전 (자기자신로테이션에서 ,타겟 로테이션 까지 , damping 시간 만큼 부드럽게 회전함
        CamTr.LookAt(target.position +(target.up *targetOffset));
                     //타겟 포지션에서  2만큼 위로 올림
    }
    private void OnDrawGizmos() // 씬화면에서 색상이나 선을 그려주는 함수 콜백함수
    {
        //CamTr.position = transform.position;
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(target.position + (target.up * targetOffset), 0.1f);
                 //색상
        //Gizmos.DrawLine(target.position +(target.up *targetOffset),CamTr.position);
               //선
    }
}
