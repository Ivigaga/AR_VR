using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraBeatMover : MonoBehaviour
{
    [Header("Referencias")]
    public CinemachineVirtualCamera virtualCamera;
    public AudioSource musicSource;

    [Header("Configuración del ritmo")]
    public float bpm = 120f;        // Ritmo de la música
    public float moveDistance = 0.5f; // Cuánto se mueve la cámara hacia adelante
    public float returnSpeed = 4f;  // Qué tan rápido vuelve a su posición original

    private float beatInterval;
    private float timer;
    private Vector3 originalPos;
    private bool movingForward = false;

    void Start()
    {
        if (virtualCamera == null)
            virtualCamera = GetComponent<CinemachineVirtualCamera>();

        originalPos = virtualCamera.transform.localPosition;
        beatInterval = 60f / bpm;
    }

    void Update()
    {
        if (musicSource != null && !musicSource.isPlaying)
            return;

        timer += Time.deltaTime;
        if (timer >= beatInterval)
        {
            timer -= beatInterval;
            movingForward = true; // activa el movimiento en cada beat
        }

        if (movingForward)
        {
            // Mueve hacia adelante
            virtualCamera.transform.localPosition = Vector3.Lerp(
                virtualCamera.transform.localPosition,
                originalPos + virtualCamera.transform.forward * moveDistance,
                Time.deltaTime * returnSpeed
            );

            // Si ya se acercó bastante al destino, empieza a volver
            if (Vector3.Distance(virtualCamera.transform.localPosition, originalPos + virtualCamera.transform.forward * moveDistance) < 0.05f)
                movingForward = false;
        }
        else
        {
            // Vuelve a la posición original
            virtualCamera.transform.localPosition = Vector3.Lerp(
                virtualCamera.transform.localPosition,
                originalPos,
                Time.deltaTime * returnSpeed
            );
        }
    }
}
