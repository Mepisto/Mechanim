using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonoBehaviourSingleton<T> : MonoBehaviour where T : MonoBehaviourSingleton<T>
{
    private static T s_instance = null;

    protected static bool s_isDestroyed = false;

    public static T Instance
    {
        get
        {
            if (s_isDestroyed)
            {
                return null;
            }

            if (s_instance == null)
            {
                s_instance = FindObjectOfType<T>() as T;
                if (s_instance != null)
                {
                    s_instance.Init();
                }

                if (s_instance == null)
                {
                    var obj = new GameObject("[" + typeof(T).ToString() + "]");
                    s_instance = obj.AddComponent<T>();
                    s_instance.Init();
                }
            }

            return s_instance;
        }
    }

    protected virtual void Init() { }

    protected virtual void Start() { }

    protected virtual void Awake() { }

    protected virtual void OnApplicationQuit()
    {
        s_instance = null;
        s_isDestroyed = true;
    }
}
    