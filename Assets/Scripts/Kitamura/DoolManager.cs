using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoolManager : MonoBehaviour
{
    private static DoolManager instance;
    
    private void Awake()
    { 
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
