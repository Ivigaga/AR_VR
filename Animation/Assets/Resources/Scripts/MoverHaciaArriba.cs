using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoverHaciaArriba : MonoBehaviour
{
    public float distancia = 5f;      // Distancia a mover
    public float duracion = 1f;       // Tiempo que tarda en moverse
    public float delay = 2f;           // Delay antes de empezar el movimiento

    private Vector3 posicionInicial;
    private Vector3 posicionFinal;

    void Start()
    {
        posicionInicial = transform.position;
        posicionFinal = posicionInicial + Vector3.up * distancia;
        StartCoroutine(MoverConDelay());
    }

    IEnumerator MoverConDelay()
    {
        // Espera el tiempo del delay
        yield return new WaitForSeconds(delay);

        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracion)
        {
            tiempoTranscurrido += Time.deltaTime;
            float t = tiempoTranscurrido / duracion;
            transform.position = Vector3.Lerp(posicionInicial, posicionFinal, t);
            yield return null;
        }

        // Asegura que llegue exactamente a la posición final
        transform.position = posicionFinal;
    }
}

