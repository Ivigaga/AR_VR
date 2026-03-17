using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;

public class TargetDistanceUI : MonoBehaviour
{
    [Header("Referencias")]
    public Transform targetObject; 
    public GameObject uiContainer; 
    public TextMeshProUGUI textComponent; 

    [Header("Configuración de Escala")]
    [Tooltip("Este es tu parámetro 'x'. Ajusta el tamaño visual del texto.")]
    public float scaleMultiplier = 0.002f; // Un valor pequeño suele ser mejor para World Space Canvas

    private Transform playerCamera;
    private bool isBeingPointed = false;

    void Start()
    {
        playerCamera = Camera.main.transform;
        
        if (uiContainer != null) uiContainer.SetActive(false);
    }

    void Update()
    {
        if (isBeingPointed && targetObject != null)
        {
            // 1. Cálculo de distancia
            float distance = Vector3.Distance(playerCamera.position, targetObject.position);
            textComponent.text = $"Distancia: {distance:F2}m";

            // 2. Cambio de tamaño proporcional (Tu nueva lógica: x * distancia)
            float currentScale = scaleMultiplier * distance;
            uiContainer.transform.localScale = new Vector3(currentScale, currentScale, currentScale);

            // 3. Rotación "Billboard"
            Vector3 directionToPlayer = playerCamera.position - uiContainer.transform.position;
            
            if (directionToPlayer != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToPlayer);
                uiContainer.transform.rotation = targetRotation * Quaternion.Euler(0, 180, 0);
            }
        }
    }

    public void OnPointerEnter()
    {
        isBeingPointed = true;
        uiContainer.SetActive(true);
    }

    public void OnPointerExit()
    {
        isBeingPointed = false;
        uiContainer.SetActive(false);
    }
}