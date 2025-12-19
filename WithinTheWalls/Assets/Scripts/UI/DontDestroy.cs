using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    [HideInInspector]
    public string uniqueID;
    
    private void Awake()
    {
        uniqueID = name + "_" + transform.position.ToString();
    }

    void Start()
    {
        for(int i = 0; i < Object.FindObjectsOfType<DontDestroy>().Length; i++)
        {
            if(Object.FindObjectsOfType<DontDestroy>()[i] != this)
            {
                if(Object.FindObjectsOfType<DontDestroy>()[i].uniqueID == uniqueID)
                {
                    Destroy(gameObject);
                }
            }
        }

        DontDestroyOnLoad(gameObject);
    }
}