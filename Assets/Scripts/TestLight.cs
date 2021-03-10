using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestLight : MonoBehaviour {

    public GameObject go;

    private bool flag;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!flag)
        {
            go.SetActive(true);
        }
    }
    
}
