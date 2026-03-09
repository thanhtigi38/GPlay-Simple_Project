using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance;
    public bool m_DontDestroyOnLoad = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this as T;

            if (transform.parent == null && m_DontDestroyOnLoad)
            {
                DontDestroyOnLoad(this.gameObject);
            }
        }
        else
        {
            if (this != Instance)
            {
                DestroyImmediate(this.gameObject);
            }
            return;
        }

        OnAwake();
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// This method is called just after the singleton construction.
    /// Override it to perform the initial setup.
    /// </summary>
    protected virtual void OnAwake()
    {
        
    }
}
