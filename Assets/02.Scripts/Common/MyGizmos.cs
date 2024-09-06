using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyGizmos : MonoBehaviour
{
    public enum Type { NORMAL,WAYPOINT}
    private const string wayPointFile = "Enemy";
    public Type type = Type.NORMAL;
    public Color _color;
    public float _radius;
    void Start()
    {
       
    }
    private void OnDrawGizmos() //씬화면에서 색상이나 선을 그려주는 함수
    {                           //유니티 지원 함수 
        if (type == Type.NORMAL)
        {
            Gizmos.color = _color;
            Gizmos.DrawSphere(transform.position, _radius);
        }
        else
        {
            Gizmos.color = _color;
            Gizmos.DrawIcon(transform.position + Vector3.up * 1.0f, wayPointFile, true);
                                      // 위치                      , 파일명    , 스케일 적용여부
            Gizmos.DrawWireSphere(transform.position, _radius);
        }
    }

}
