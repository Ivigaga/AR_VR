using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

///--------------------------------
///   Author: Victor Alvarez, Ph.D.
///   University of Oviedo, Spain
///--------------------------------

public class GroundRestart : MonoBehaviour {

    private void OnTriggerEnter()
    {
        SceneManager.LoadScene("GameMenu");
    }
}
