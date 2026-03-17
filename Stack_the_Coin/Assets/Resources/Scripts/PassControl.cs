using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Controla el paso y apilamiento de cubos. Permite seleccionar y mover stacks de cubos como grupo.
/// </summary>
[RequireComponent(typeof(Collider))]
public class PassControl : MonoBehaviour
{
    [Header("Configuración de Detección")]
    public string targetTag = "Cube";
    public float topNormalThreshold = 0.5f;
    public float heightTolerance = 0.05f;
    public float searchRadius = 0.6f;
    public float verticalStep = 1.0f;

    private Collider myCollider;
    private GameObject currentCube;
    private bool groupControlActive = false;

    public GameObject controlledCube;
    private Material playerMaterial;
    private float qKeyDownTime = 0f;
    private float holdThreshold = 0.15f;
    private Material standardMaterial;

    // Estado del Stack
    private List<GameObject> stackedCubes = new List<GameObject>();

    // Estado de Selección
    private bool isSelectingStack = false;
    private int selectedCount = 1;

    void Start()
    {
        controlledCube = GameObject.Find("Player");
        myCollider = GetComponent<Collider>();

        Renderer r = controlledCube.GetComponent<Renderer>();
        if (r != null)
            playerMaterial = r.material;

        standardMaterial = new Material(Shader.Find("Standard"));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            qKeyDownTime = Time.time;
        }

        // Solo permitir modo de selección manual si ya hay un stack activo
        if (Input.GetKey(KeyCode.E) && currentCube != null && groupControlActive)
        {
            if (!isSelectingStack && Time.time - qKeyDownTime > holdThreshold)
            {
                // Si solo queda el Player en el stack, deshacer el stack automáticamente
                if (stackedCubes.Count == 1 && stackedCubes[0] == controlledCube)
                {
                    ToggleFullStack(); // Deshacer el stack
                    return;
                }

                // Entrar en modo de selección para modificar el stack existente
                selectedCount = stackedCubes.Count; // Empezar con el stack completo actual
                isSelectingStack = true;
                UpdateSelectionVisuals();

                var stackMover = controlledCube.GetComponent<StackMovement>();
                var playerMover = controlledCube.GetComponent<CubeMovement>();

                // Verificar que playerMover no sea null antes de usarlo
                if (stackMover != null)
                {
                    stackMover.selectionMode = true;
                }
                if (playerMover != null)
                {
                    playerMover.selectionMode = true;  // Cambiado a true para bloquear el movimiento
                    playerMover.enabled = false;       // Agregado para asegurar que se deshabilite el movimiento
                }

                Debug.Log("Modo selección de stack ACTIVADO - Modificando stack existente");
            }

            if (isSelectingStack)
            {
                if (Input.GetKeyDown(KeyCode.S))
                {
                    selectedCount = Mathf.Min(selectedCount + 1, stackedCubes.Count);
                    UpdateSelectionVisuals();
                }
                else if (Input.GetKeyDown(KeyCode.W))
                {
                    selectedCount = Mathf.Max(selectedCount - 1, 1);
                    UpdateSelectionVisuals();
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            if (isSelectingStack)
            {
                if (stackedCubes.Count > 0)
                {
                    selectedCount = Mathf.Clamp(selectedCount, 1, stackedCubes.Count);
                    
                    // Si solo queda el Player seleccionado, deshacer el stack completamente
                    if (selectedCount == 1 && stackedCubes[0] == controlledCube)
                    {
                        StackMovement stackMover = controlledCube.GetComponent<StackMovement>();
                        CubeMovement playerMover = controlledCube.GetComponent<CubeMovement>();

                        // Limpiar y destruir el StackMovement
                        if (stackMover != null)
                        {
                            stackMover.ClearStack();
                            Destroy(stackMover);
                        }

                        // Reactivar el movimiento del jugador
                        if (playerMover != null)
                        {
                            playerMover.selectionMode = false;
                            playerMover.enabled = true;
                        }

                        // Restaurar materiales
                        foreach (GameObject cube in stackedCubes)
                        {
                            Renderer cubeRenderer = cube.GetComponent<Renderer>();
                            if (cubeRenderer != null && standardMaterial != null && cube != controlledCube)
                                cubeRenderer.material = standardMaterial;
                        }

                        // Resetear masa de los cubos a 50
                        ResetCubeMass(stackedCubes);

                        groupControlActive = false;
                        stackedCubes.Clear();
                        Debug.Log("Stack deshecho - Solo quedaba el Player");
                    }
                    else
                    {
                        // Confirmar selección con múltiples cubos
                        List<GameObject> finalStack = stackedCubes.GetRange(0, selectedCount);

                        StackMovement stackMover = controlledCube.GetComponent<StackMovement>();
                        CubeMovement playerMover = controlledCube.GetComponent<CubeMovement>();

                        if (stackMover == null)
                            stackMover = controlledCube.AddComponent<StackMovement>();

                        stackMover.SetStack(finalStack);
                        stackMover.selectionMode = false;
                        
                        if (playerMover != null)
                        {
                            playerMover.selectionMode = false;
                            playerMover.enabled = false;
                        }

                        // Cambiar masa de cubos seleccionados a la del Player
                        SetCubeMassToPlayer(finalStack);
                        
                        // Resetear masa de cubos no seleccionados a 50
                        List<GameObject> nonSelectedCubes = new List<GameObject>();
                        for (int i = selectedCount; i < stackedCubes.Count; i++)
                        {
                            nonSelectedCubes.Add(stackedCubes[i]);
                        }
                        ResetCubeMass(nonSelectedCubes);

                        groupControlActive = true;
                        Debug.Log($"Stack confirmado con {selectedCount} cubos");
                    }
                }
                else
                {
                    Debug.LogWarning("No hay cubos en la pila para seleccionar");
                }
                isSelectingStack = false;

            }
            else
            {
                if (Time.time - qKeyDownTime <= holdThreshold && currentCube != null)
                {
                    ToggleFullStack();
                }
            }
        }
    }

    void ToggleFullStack()
    {
        stackedCubes = FindStackedCubes(currentCube);
        if (!stackedCubes.Contains(controlledCube))
            stackedCubes.Insert(0, controlledCube);

        StackMovement stackMover = controlledCube.GetComponent<StackMovement>();
        CubeMovement playerMover = controlledCube.GetComponent<CubeMovement>();

        if (!groupControlActive)
        {
            if (stackMover == null)
                stackMover = controlledCube.AddComponent<StackMovement>();

            stackMover.SetStack(stackedCubes);
            groupControlActive = true;

            if (playerMover != null)
                playerMover.enabled = false;

            foreach (GameObject cube in stackedCubes)
            {
                Renderer cubeRenderer = cube.GetComponent<Renderer>();
                if (cubeRenderer != null && playerMaterial != null)
                    cubeRenderer.material = playerMaterial;
            }

            // Cambiar masa de los cubos a la del Player
            SetCubeMassToPlayer(stackedCubes);

            Debug.Log("Control de grupo ACTIVO (tap corto).");
        }
        else
        {
            if (AllRigidbodiesStopped(stackedCubes))
            {
                if (stackMover != null)
                {
                    stackMover.ClearStack();
                    Destroy(stackMover);
                }

                groupControlActive = false;

                if (playerMover != null)
                    playerMover.enabled = true;

                foreach (GameObject cube in stackedCubes)
                {
                    Renderer cubeRenderer = cube.GetComponent<Renderer>();
                    if (cubeRenderer != null && standardMaterial != null && cube != controlledCube)
                        cubeRenderer.material = standardMaterial;
                }

                // Resetear masa de los cubos a 50
                ResetCubeMass(stackedCubes);
                
                stackedCubes.Clear();

                Debug.Log("Control de grupo DESACTIVADO (tap corto).");
            }
            else
            {
                Debug.Log("No puedes desactivar el stack hasta que todos estén quietos.");
            }
        }
    }

    void UpdateSelectionVisuals()
    {
        for (int i = 0; i < stackedCubes.Count; i++)
        {
            Renderer cubeRenderer = stackedCubes[i].GetComponent<Renderer>();
            if (cubeRenderer != null)
            {
                cubeRenderer.material = i < selectedCount ? playerMaterial : standardMaterial;
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!collision.gameObject.CompareTag(targetTag))
            return;

        bool isOnTop = false;

        foreach (ContactPoint cp in collision.contacts)
        {
            if (cp.normal.y > topNormalThreshold)
            {
                isOnTop = true;
                break;
            }
        }

        if (!isOnTop)
        {
            float myMinY = myCollider.bounds.min.y;
            float otherMaxY = collision.collider.bounds.max.y;

            if (myMinY >= otherMaxY - heightTolerance)
                isOnTop = true;
        }

        if (isOnTop)
        {
            currentCube = collision.gameObject;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject == currentCube)
        {
            currentCube = null;
            stackedCubes.Clear();
        }
    }

    List<GameObject> FindStackedCubes(GameObject topCube)
    {
        List<GameObject> result = new List<GameObject>();
        GameObject current = topCube;

        while (current != null)
        {
            result.Add(current);

            Vector3 origin = current.transform.position;
            RaycastHit hit;

            if (Physics.Raycast(origin, Vector3.down, out hit, verticalStep + heightTolerance))
            {
                if (hit.collider != null && hit.collider.gameObject.CompareTag(targetTag))
                {
                    float currentMinY = current.GetComponent<Collider>().bounds.min.y;
                    float otherMaxY = hit.collider.bounds.max.y;

                    if (Mathf.Abs(currentMinY - otherMaxY) <= heightTolerance)
                    {
                        current = hit.collider.gameObject;
                        continue;
                    }
                }
            }

            current = null;
        }

        return result;
    }

    bool AllRigidbodiesStopped(List<GameObject> cubes)
    {
        foreach (var cube in cubes)
        {
            Rigidbody rb = cube.GetComponent<Rigidbody>();
            if (rb != null && rb.velocity.magnitude > 0.05f)
                return false;
        }
        return true;
    }

    void SetCubeMassToPlayer(List<GameObject> cubes)
    {
        // Obtener la masa del Player
        Rigidbody playerRb = controlledCube.GetComponent<Rigidbody>();
        float playerMass = playerRb != null ? playerRb.mass : 1f;

        foreach (GameObject cube in cubes)
        {
            if (cube != controlledCube) // No cambiar la masa del Player
            {
                Rigidbody rb = cube.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.mass = playerMass;
                }
            }
        }
    }

    void ResetCubeMass(List<GameObject> cubes)
    {
        foreach (GameObject cube in cubes)
        {
            if (cube != controlledCube) // No cambiar la masa del Player
            {
                Rigidbody rb = cube.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.mass = 500f;
                }
            }
        }
    }
}
