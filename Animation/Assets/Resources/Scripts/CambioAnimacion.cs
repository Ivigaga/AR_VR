using UnityEngine;
using System.Collections;

public class CambioAnimacion : MonoBehaviour
{
    [System.Serializable]
    public class PasoDeBaile
    {
        public string boolName; // nombre del booleano
        public float tiempo;    // segundos para activarlo
    }

    public PasoDeBaile[] pasos;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        StartCoroutine(ControlarPasos());
    }

    IEnumerator ControlarPasos()
    {
        foreach (var paso in pasos)
        {
            yield return new WaitForSeconds(paso.tiempo);
            animator.SetBool(paso.boolName, true);
        }
        yield return new WaitForSeconds(0.1f);
        ReiniciarAnimacion();
    }

    public void ReiniciarAnimacion()
    {
        // Apaga todos los booleanos antes de reiniciar
        foreach (var paso in pasos)
        {
            animator.SetBool(paso.boolName, false);
        }


        StopAllCoroutines();
        StartCoroutine(ControlarPasos());
        animator.SetBool("Restart", true);
    }
}
