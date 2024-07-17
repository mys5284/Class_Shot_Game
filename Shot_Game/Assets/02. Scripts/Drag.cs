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


    //드래그 인터페이스의 메소드
    //인터페이스가 가지고 있는 메소드는 반드시 자식 클래스에서 구현 해줘야함
    //오버라이딩임!!
    public void OnDrag(PointerEventData eventData)
    {
        itemTr.position = Input.mousePosition;
    }

    //드래그가 시작할 때 호출되는 메소드
    public void OnBeginDrag(PointerEventData eventData)
    {
        this.transform.SetParent(inventoryTr);
        //드래그가 시작될 때 드래그되는 아이템 정보 저장
        draggingItem = this.gameObject;

        //드래그가 시작되면 다른 UI 이벤트를 받지 않도록 설정
        canvasGroup.blocksRaycasts = false;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //드래그가 끝날 때 드래그 아이템을 null로 설정
        draggingItem = null;
        canvasGroup.blocksRaycasts = true;

        if(itemTr.parent == inventoryTr)
        {
            itemTr.SetParent(itemListTr);
        }
    }
}
