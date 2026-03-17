using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuButtons : MonoBehaviour
{
    public string targetScene="TargetScene";
    public string GPSSCene="GPSScene";
   
    public void loadTarget(){
        SceneManager.LoadScene(targetScene);
    }

    public void loadGSPS(){
        SceneManager.LoadScene(GPSSCene);
    }
}
