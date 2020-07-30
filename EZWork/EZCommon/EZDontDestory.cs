using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EZDontDestory : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
