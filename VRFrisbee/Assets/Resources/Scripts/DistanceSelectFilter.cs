using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Filtering;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DistanceSelectFilter : MonoBehaviour, IXRSelectFilter
{
    public float maxGrabDistance = 3.0f;

    // Propiedad obligatoria de la interfaz
    public bool canProcess => true;

    // Firma exacta requerida por XRI 3.1.2
    public bool Process(IXRSelectInteractor interactor, IXRSelectInteractable interactable)
    {
        float distance = Vector3.Distance(interactor.transform.position, interactable.transform.position);
        
        // Solo permite seleccionar si está cerca
        return distance <= maxGrabDistance;
    }
}