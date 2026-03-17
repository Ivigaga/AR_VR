using UnityEngine;
using TMPro; // Necesario para TextMeshPro
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ThrowCounterUI : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("El componente de texto donde se verá el número")]
    public TextMeshProUGUI countText;
    
    [Tooltip("El objeto a contar. Si está en este mismo objeto, puedes dejarlo vacío.")]
    public XRGrabInteractable objectToCount;

    [Header("Ajustes de Texto")]
    public string prefix = "Lanzamientos: ";

    private static int throwCount = 0;

    private void Awake()
    {
        // Si no se asignó el objeto por parámetro, lo buscamos en el mismo GameObject
        if (objectToCount == null)
        {
            objectToCount = GetComponent<XRGrabInteractable>();
        }

        // Suscribirse al evento de soltar el objeto
        if (objectToCount != null)
        {
            objectToCount.selectExited.AddListener(UpdateUI);
        }

        // Inicializar el texto al empezar
        UpdateTextDisplay();
    }

    private void OnDestroy()
    {
        // Limpieza de eventos al destruir el objeto
        if (objectToCount != null)
        {
            objectToCount.selectExited.RemoveListener(UpdateUI);
        }
    }

    // Este método se ejecuta cada vez que sueltas el objeto
    private void UpdateUI(SelectExitEventArgs args)
    {
        throwCount++;
        UpdateTextDisplay();
    }

    private void UpdateTextDisplay()
    {
        if (countText != null)
        {
            countText.text = prefix + throwCount.ToString();
        }
    }
}