using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeColor : MonoBehaviour
{
    // Referencia al cubo que controlas (así sabremos qué material copiar)
    public GameObject controlledCube;

    private Material originalMaterial;
    private bool isChanged = false;

    void Start()
    {
        controlledCube = GameObject.Find("Player");
        // Guardamos el material original de este cubo
        originalMaterial = GetComponent<Renderer>().material;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C) && controlledCube != null)
        {
            Renderer myRenderer = GetComponent<Renderer>();
            Renderer controlledRenderer = controlledCube.GetComponent<Renderer>();

            if (!isChanged)
            {
                // Cambiar al material del cubo controlado
                myRenderer.material = controlledRenderer.material;
                isChanged = true;
                Debug.Log($"{gameObject.name} ha copiado el material de {controlledCube.name}");
            }
            else
            {
                // Volver al material original
                myRenderer.material = originalMaterial;
                isChanged = false;
                Debug.Log($"{gameObject.name} ha vuelto a su material original");
            }
        }
    }
}

