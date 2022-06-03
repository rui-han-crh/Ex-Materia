using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T>: MonoBehaviour where T : MonoBehaviour
{
    public static T instance { get; private set; }



    // Start is called before the first frame update
    public void Awake()
    {
        if (instance == null)
            instance = (T)FindObjectOfType(typeof(T));
        else
            Destroy(gameObject);
    }


}
