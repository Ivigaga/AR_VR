using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchCharacter : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        // Obtiene el Animator del mismo GameObject
        animator = GetComponent<Animator>();

        // Si el Animator no está en este objeto, intenta buscarlo en hijos
        if (animator == null)
            animator = GetComponentInChildren<Animator>();
    }

    private void OnMouseDown()
    {
        Debug.Log("touché");

        if (animator != null)
        {
            animator.SetTrigger("Attack");
        }
        else
        {
            Debug.LogWarning("No se encontró un Animator en el personaje.");
        }
    }
}
