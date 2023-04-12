using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class JoyStick : MonoBehaviour ,IDragHandler, IEndDragHandler,IPointerClickHandler
{
    [SerializeField]
    RectTransform center;
    [SerializeField]
    Image joystick;
    [SerializeField]
    GameObject mob;
    float radius;
    private void Start()
    {
        radius = center.rect.width;
        Debug.Log(center.position);
    }
    
    public void UpdateJoystickHandle(Vector2 _dir)
    {
        
    }
    Vector2 DIR;
    private void Update()
    {
        mob.transform.position += new Vector3(DIR.x,0,DIR.y) * Time.deltaTime;
    }
    public void OnDrag(PointerEventData eventData)
    {
        DIR = (eventData.position - (Vector2)center.position).normalized;

        

        float distance = Vector2.Distance(eventData.position, center.position);
        if (radius < distance)
        {
            float angle = Mathf.Atan2(eventData.position.y - center.position.y, eventData.position.x - center.position.x); // �Է� ��ġ�� ���� �߽� ������ ����

            joystick.rectTransform.position = new Vector2(center.position.x + radius * Mathf.Cos(angle), center.position.y + radius * Mathf.Sin(angle));
        }
        else
        {
            joystick.rectTransform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        DIR = Vector2.zero;
        joystick.rectTransform.position = center.position;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log(eventData.position+  "Ŭ�� ������");
        Debug.Log(center.transform.position+  "���� ������ : transform.position");
        Debug.Log(center.anchoredPosition+  "���� ������ : anchoredPosition");
        Debug.Log(center.position+  "���� ������ : rectPosition");

    }
}
