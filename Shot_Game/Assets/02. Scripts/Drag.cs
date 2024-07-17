using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    Transform itemTr;
    Transform inventoryTr;
    public static GameObject draggingItem = null;

    Transform itemListTr;
    CanvasGroup canvasGroup;

    void Start()
    {
        itemTr = GetComponent<Transform>();
        inventoryTr = GameObject.Find("Inventory").GetComponent<Transform>();

        itemListTr = GameObject.Find("ItemList").GetComponent<Transform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }


    //�巡�� �������̽��� �޼ҵ�
    //�������̽��� ������ �ִ� �޼ҵ�� �ݵ�� �ڽ� Ŭ�������� ���� �������
    //�������̵���!!
    public void OnDrag(PointerEventData eventData)
    {
        itemTr.position = Input.mousePosition;
    }

    //�巡�װ� ������ �� ȣ��Ǵ� �޼ҵ�
    public void OnBeginDrag(PointerEventData eventData)
    {
        this.transform.SetParent(inventoryTr);
        //�巡�װ� ���۵� �� �巡�׵Ǵ� ������ ���� ����
        draggingItem = this.gameObject;

        //�巡�װ� ���۵Ǹ� �ٸ� UI �̺�Ʈ�� ���� �ʵ��� ����
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //�巡�װ� ���� �� �巡�� �������� null�� ����
        draggingItem = null;
        canvasGroup.blocksRaycasts = true;

        if(itemTr.parent == inventoryTr)
        {
            itemTr.SetParent(itemListTr);
        }
    }
}
