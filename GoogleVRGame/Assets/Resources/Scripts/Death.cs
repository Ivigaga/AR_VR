using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Death : HealthSystem
{
    public string deathScene;

    protected override void Die()
    {
        Debug.Log(gameObject.name + " ha muerto. Intentando cargar escena: " + deathScene);
        
        if (!string.IsNullOrEmpty(deathScene))
        {
            Debug.Log("Cargando escena de muerte: " + deathScene);
            SceneManager.LoadScene(deathScene);
        }
        else
        {
            Debug.LogError("¡No hay escena de muerte asignada en " + gameObject.name + "!");
        }
    }
}
