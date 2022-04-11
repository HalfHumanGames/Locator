using System;
using System.Collections.Generic;
using UnityEngine;

public class Locator : MonoBehaviour
{
    public static Locator Global { get; private set; }

    public bool IsGlobal;
    public Component[] Components;

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
        if (Components != null && Components.Length > 0)
        {
            for (int i = 0; i < Components.Length; i++)
            {
                if (Components[i] != null)
                {
                    Register(Components[i]);
                }
            }
        }
    }

    public T Get<T>() where T : class
    {
        Type type = typeof(T);
        if (items.ContainsKey(type))
        {
            return (T)items[typeof(T)];
        }
        if (parent != null)
        {
            T item = parent.Get<T>();
            if (item != null)
            {
                return item;
            }
            
        }
        if (!IsGlobal)
        {
            return Global.Get<T>();
        }
        Debug.LogError($"[Locator] Object of type '{type}' could not be located.", this);
        return default;
    }

    public void Register(object item)
    {
        RegisterAs(item, item.GetType());
    }

    public void RegisterAs<T>(T item) where T : class
    {
        RegisterAs(item, typeof(T));
    }

    public void RegisterAs(object item, Type type)
    {
        if (!items.ContainsKey(type))
        {
            items.Add(type, item);
        }
        else
        {
            Debug.LogError($"[Locator] The type '{type}' has already been registered with this locator.", this);
        }
    }

    public void Unregister(object item)
    {
        Unregister(item.GetType());
    }

    public void Unregister<T>() where T : class
    {
        Unregister(typeof(T));
    }

    public void Unregister(Type type)
    {
        items.Remove(type);
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

    public static T GetGlobal<T>() where T : class
    {
        return Global != null ? Global.Get<T>() : null;
    }
}