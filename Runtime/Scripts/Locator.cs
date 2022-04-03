using System;
using System.Collections.Generic;
using UnityEngine;

public class Locator : MonoBehaviour
{
    public static Locator Global { get; private set; }

    public bool IsGlobal;

    private Locator parent;
    private Dictionary<Type, object> items = new Dictionary<Type, object>();

    private void Awake()
    {
        if (IsGlobal)
        {
            if (Global == null)
            {
                Global = this;
            }
            else
            {
                Debug.LogError("[Locator] There can only be one global locator.", this);
                IsGlobal = false;
            }
        }
        if (!IsGlobal && transform.parent != null)
        {
            parent = transform.parent.GetComponentInParent<Locator>();
        }
    }

    public T Locate<T>() where T : class
    {
        Type type = typeof(T);
        if (items.ContainsKey(type))
        {
            return (T)items[typeof(T)];
        }
        if (parent != null)
        {
            T item = parent.Locate<T>();
            if (item != null)
            {
                return item;
            }
            
        }
        if (!IsGlobal)
        {
            return Global.Locate<T>();
        }
        Debug.LogError($"[Locator] Object of type '{type}' could not be located.", this);
        return default;
    }

    public void Register<T>(T item) where T : class
    {
        Type type = typeof(T);
        if (!items.ContainsKey(type))
        {
            items.Add(typeof(T), item);
        }
        else
        {
            Debug.LogError($"[Locator] The type '{type}' has already been registered with this locator.", this);
        }
    }

    public void Unregister<T>(T item) where T : class
    {
        items.Remove(typeof(T));
    }

    public static Locator Find(GameObject gameObject)
    {
        Locator locator = gameObject.GetComponentInParent<Locator>();
        if (locator != null)
        {
            return locator;
        }
        else
        {
            if (Global == null)
            {
                Debug.LogError("[Locator] No locator found. Consider adding a global locator.", gameObject);
            }
            return Global;
        }
    }
}