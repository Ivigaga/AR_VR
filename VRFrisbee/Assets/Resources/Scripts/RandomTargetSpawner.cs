using UnityEngine;

public class RandomTargetSpawner : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject dianaPrefab;
    public Transform playerTransform;

    [Header("Límites del Cuadrado")]
    public float minX = 50f;
    public float maxX = 950f;
    public float minZ = 50f;
    public float maxZ = 950f;
    public float fixedY = 0.0001f;

    [Header("Rango de Distancia")]
    public float minDistanceFromPlayer = 150f;
    public float maxDistanceFromPlayer = 300f; // Nuevo límite máximo

    void Start()
    {
        if (playerTransform == null && Camera.main != null)
        {
            playerTransform = Camera.main.transform;
        }

        SpawnTarget();
    }

    void SpawnTarget()
    {
        Vector3 finalPosition = Vector3.zero;
        bool positionValid = false;
        int safetyBreak = 0; 

        while (!positionValid && safetyBreak < 200) // Aumentamos intentos por si el rango es estrecho
        {
            float randomX = Random.Range(minX, maxX);
            float randomZ = Random.Range(minZ, maxZ);
            finalPosition = new Vector3(randomX, fixedY, randomZ);

            float distance = Vector3.Distance(finalPosition, playerTransform.position);

            // Ahora comprueba que esté entre el mínimo y el máximo
            if (distance >= minDistanceFromPlayer && distance <= maxDistanceFromPlayer)
            {
                positionValid = true;
            }
            
            safetyBreak++;
        }

        if (dianaPrefab != null && positionValid)
        {
            Instantiate(dianaPrefab, finalPosition, Quaternion.identity);
        }
        else if (!positionValid)
        {
            Debug.LogWarning("No se encontró una posición válida en 200 intentos. Revisa los límites.");
        }
    }
}