using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UVJoyStick : UIView ,IDragHandler, IEndDragHandler
{
    [SerializeField]
    RectTransform center;
    [SerializeField]
    Image joystick;
    [SerializeField]
    ControlUnit mob;
    float radius;
    private void Start()
    {
        radius = center.rect.width;
        Debug.Log(center.position);
    }
    
    Vector2 dir;
    Vector3 GetDirection() { return new Vector3(dir.x, 0, dir.y); }
    private void OnEnable()
    {
        InputManager.Instance.StartControl(GetDirection);
    }
    private void OnDisable()
    {
        InputManager.Instance.Clear();
    }
    public void OnDrag(PointerEventData eventData)
    {
        dir = (eventData.position - (Vector2)center.position).normalized;
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
        dir = Vector2.zero;
        joystick.rectTransform.position = center.position;
    }
    
}
