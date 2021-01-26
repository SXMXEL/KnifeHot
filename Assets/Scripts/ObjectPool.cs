using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class ObjectPool<T> : MonoBehaviour where T : MonoBehaviour
{
    public T Prefab;

    private readonly List<T> _freeList = new List<T>();
    private readonly List<T> _usedList = new List<T>();
    private Transform _container;
    private Transform CurrentContainer => _container != null ? _container : transform;

    public virtual void Init(Transform container)
    {
        _container = container;
    }

    protected T GetObject()
    {
        T pooledObject;
        if (_freeList.Any())
        {
            pooledObject = _freeList.First();
            _freeList.Remove(pooledObject);
        }
        else
        {
            pooledObject = Instantiate(Prefab, CurrentContainer);
        }

        _usedList.Add(pooledObject);
        pooledObject.gameObject.SetActive(true);

        return pooledObject;
    }

    protected void ReturnObject(T pooledObject)
    {
        if (!_usedList.Contains(pooledObject))
        {
            Debug.LogError(pooledObject.name + " is not related to this ObjectPool.");
            return;
        }

        _usedList.Remove(pooledObject);
        _freeList.Add(pooledObject);
        pooledObject.transform.SetParent(CurrentContainer);
        pooledObject.gameObject.SetActive(false);
    }
}