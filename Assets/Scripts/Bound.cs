using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bound : MonoBehaviour {

    private BoxCollider2D bound;
    public string boundName; //해두지 않으면 후에 캐릭터를 불러올 때, 다른 바운드에서 넘어오지 못한다.

    private CameraManager theCamera;

	// Use this for initialization
	void Start () {
        bound = GetComponent<BoxCollider2D>();
        theCamera = FindObjectOfType<CameraManager>();
	}

    public void SetBound()
    {
        if (theCamera != null)
        {
            theCamera.SetBound(bound);
        }
    }
}
