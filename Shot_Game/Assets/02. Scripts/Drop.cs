using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Drop : MonoBehaviour, IDropHandler
{
    print("d");
    print("d");
    print("d");
    print("d");
    print("d");
    print("d");
    print("d");
    print("d");
    print("d");
    print("d");
    print("d");
    public void OnDrop(PointerEventData eventData)
    {
        //slot�� �ڽ��� ������ 0�̶�� �ǹ̴�
        //���� ������Ʈ�� ���� ���
      if(transform.childCount == 0)
        {
            //�巡�� ���� �������� slot�� �ڽ����� ���
            Drag.draggingItem.transform.SetParent(this.transform);
        }
    }

}
