using UnityEngine;
using Framework.Tools;
using System.Text;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private const string NAME = "Singleton";
    private static T instance;

    public static T Instance
    {
        get
        {
            if (null == instance)
            {
                instance = FindObjectOfType<T>();
                if (null == instance)
                {
                    GameObject go = GameObject.Find(NAME);
                    if (null == go)
                    {
                        go = new GameObject(NAME);
                        DontDestroyOnLoad(go);
                    }
//                    instance = Utility.AddChild(new GameObject(typeof(T).Name), go.transform).AddComponent<T>();
                }
            }
            return instance;
        }
    }

    void Awake()
    {
        instance = this as T;
    }

    protected virtual void OnEnable()
    {
        instance = this as T;
    }
    
    protected virtual void OnDisable()
    {
        if (null != instance) instance = null;
    }
}
