using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///--------------------------------
///   Author: Victor Alvarez, Ph.D.
///   University of Oviedo, Spain
///--------------------------------

public class Camerafollows : MonoBehaviour {

    public GameObject ballsphere;
    private Vector3 distance;

	// Use this for initialization
	void Start () {
        distance = transform.position - ballsphere.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
        transform.position = distance + ballsphere.transform.position;
	}
}
