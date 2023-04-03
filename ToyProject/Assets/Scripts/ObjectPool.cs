using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<T> where T:Component
{
    private Queue<T> pool;
    private T sample;
    private Transform parent;
    public ObjectPool(T _sample, Transform _parent)
    {
        sample = _sample;
        parent = _parent;
        pool = new Queue<T>();
    }

    public T Get()
    {
        T newT = null;
        if (pool.Count == 0)
        {
            newT = Object.Instantiate(sample, parent);
        }
        else
            newT = pool.Dequeue();
        return newT;
    }
    public void Push(T _usedT)
    {
        if(pool.Count >=5)
        {
            Object.Destroy(_usedT.gameObject);
        }
        else
        {
            _usedT.gameObject.transform.SetParent(parent);
            _usedT.gameObject.SetActive(false);
            pool.Enqueue(_usedT);
        }
    }
}
