using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour 
where T : Singleton<T>
{
    private static T m_instance;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<T>();
            }
            return m_instance;
        }
    }

    public virtual void Awake()
    {
        if (m_instance !=null && m_instance!= this)
        {
            Destroy(gameObject);
            return;
        }
        if (m_instance == null)
        {
            m_instance = FindObjectOfType<T>();
            DontDestroyOnLoad(m_instance.gameObject);
        }
    }
}