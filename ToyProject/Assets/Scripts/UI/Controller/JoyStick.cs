using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class JoyStick : MonoBehaviour ,IDragHandler, IEndDragHandler
{
    [SerializeField]
    RectTransform center;
    [SerializeField]
    Image joystick;
    float radius;
    private void Start()
    {
        radius = center.rect.width;
        Debug.Log(center.position);
    }
    
    public void UpdateJoystickHandle(Vector2 _dir)
    {
        
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 dir = eventData.position - center.anchoredPosition;

        float distance = Vector2.Distance(eventData.position, center.position);
        if (radius < distance)
        {
            float angle = Mathf.Atan2(eventData.position.y - center.position.y, eventData.position.x - center.position.x); // 입력 위치와 원의 중심 사이의 각도

            joystick.rectTransform.position = new Vector2(center.position.x + radius * Mathf.Cos(angle), center.position.y + radius * Mathf.Sin(angle));
        }
        else
        {
            joystick.rectTransform.position = eventData.position;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        joystick.rectTransform.position = center.position;
    }
}
