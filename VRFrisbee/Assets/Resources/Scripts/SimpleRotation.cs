using UnityEngine;

public partial class SimpleRotation : MonoBehaviour
{
    [Header("Configuración de Rotación")]
    [Tooltip("Velocidad de giro en grados por segundo")]
    public float rotationSpeed = 50.0f;

    [Tooltip("Eje de rotación (por defecto es el eje Y)")]
    public Vector3 rotationAxis = Vector3.up;

    void Update()
    {
        // Multiplicamos por Time.deltaTime para que la rotación sea constante
        // independientemente de los FPS (fotogramas por segundo).
        transform.Rotate(rotationAxis * rotationSpeed * Time.deltaTime);
    }
}