using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/*
1. ��ĳ������ ���� ��ǥ�� �о ��ũ�� ��ǥ�� ����.
2. �ش� ��ũ�� ��ǥ�� �о, ĵ���� ���� ��ǥ�� ����.
3. ĵ�������� ��ǥ�� �ٽ� ��ĳ������ ���� ��ǥ�� �����Ѵ�. (�ش� ��ǥ�� HpBar�� ��ȯ���� ����ٳ����.)
*/
public class EnemyHpBar : MonoBehaviour
{
    [SerializeField]
    private Camera UI_Camera;
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private RectTransform rectParent;
    [SerializeField]
    private RectTransform rectHp;

    public Transform targetTr; // UI�� ����ٴ� Ÿ�� ��ġ

    public Vector3 offset = Vector3.zero; // targetTr��ġ���� hpbar�� ������ ��ġ�� �����ϴ� ����
    void Start()
    {
        canvas = GameObject.Find("Canvas-EnemyUI").GetComponent<Canvas>();
        UI_Camera = canvas.worldCamera;
        rectParent = canvas.GetComponent<RectTransform>();
        rectHp = this.gameObject.GetComponent<RectTransform>();
    }
    void LateUpdate() //ĳ���Ͱ� �����̰� UI�� ���󰡾��ϹǷ�
    {
        var screenPos = Camera.main.WorldToScreenPoint(targetTr.position + offset); // hpbar�� ���� ���� ��ǥ�� ��ũ�� ��ǥ�� ��ȯ

        if(screenPos.z < 0f) //���� ��ǥ�� 3���� ��ũ�� ��ǥ�� 2�����̴�. ��, �Ѿ�� �� ������ǥ�� Z���� 0���� ������ ī�޶� ���ʿ� ��ġ�� ���� ��Ÿ����.
        {// UI_Camera�� �ش� �� ĳ������ UI�� �ٶ󺸷��� �ϴ� ������ �߻��Ѵ�. �̷��� ������ ���� ���� �������� �Ѿ���� ���� ��ǥ�� z���� ����� ����� �ش�.
            //screenPos.z *= -1.0f; //����� �ڵ�
            screenPos *= -1.0f; //����� �ڵ�
        }

        Vector2 screenPoint = new Vector2(screenPos.x, screenPos.y);

        var localPos = Vector2.zero; //�Ʒ��� �ڵ忡�� ��ũ�� ��ǥ�� ������ǥ�� ��ȯ�Ͽ� �ش� ������ �����Ѵ�.
        RectTransformUtility.ScreenPointToLocalPointInRectangle //��ũ�� ��ǥ�� �ٽ� ���� ��ǥ�� �����Ѵ�.
            (rectParent, screenPos, UI_Camera, out localPos);  //  ī�޶�(UI_Camera)�� �����ִ� ĵ����(rectParent)�� ��ũ�� ��ǥ(screenPos)�� ������ǥ�� ��ȯ�Ͽ� �����Ѵ�. (localPos�� ����) 

        rectHp.localPosition = localPos; //hpbar�� ��ġ�� �����Ѵ�.
    }
}
