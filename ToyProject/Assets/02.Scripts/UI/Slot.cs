using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot : MonoBehaviour
{
    [SerializeField]
    RectTransform tr;
    Rect rc;

    public Rect RC
    {
        get
        {
            rc.x = tr.position.x - tr.rect.width * 0.5f;
            rc.y = tr.position.y + tr.rect.height * 0.5f;
            return rc;
        }
    }
    void Start()
    {
        rc.x = tr.transform.position.x - tr.rect.width / 2;
        rc.y = tr.transform.position.y + tr.rect.height / 2;
        rc.xMax = tr.rect.width;
        rc.yMax = tr.rect.height;
        rc.width = tr.rect.width;
        rc.height = tr.rect.height;
    }

    public bool IsInRect(Vector2 _pos)
    {
        if (_pos.x >= RC.x && _pos.x <= RC.x + RC.width && _pos.y >= RC.y - RC.height && _pos.y <= RC.y)
        {
            return true;
        }
        return false;
    }
}
