using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems; // <-- AÑADIDO: Necesario para EventTrigger
using UnityEngine.Events;     // <-- AÑADIDO: Necesario para UnityAction

namespace DigitalRuby.PyroParticles
{
    public class FireAttack : MonoBehaviour
    {
        public GameObject[] Prefabs;

        private GameObject currentPrefabObject;
        private FireBaseScript currentPrefabScript;
        private int currentPrefabIndex;
        
        private void UpdateEffect()
        {

        }

        // --- MODIFICADO ---
        // Añadimos 'spawnPoint'. Si es null, usa 'transform'.
        private void BeginEffect(Transform target = null, Transform spawnPoint = null)
        {
            // --- NUEVO: Determinar el transform de origen ---
            Transform originTransform = (spawnPoint != null) ? spawnPoint : transform;
            // ----------------------------------------------

            Vector3 pos;
            
            // --- MODIFICADO: Usamos 'originTransform' en lugar de 'transform' ---
            float yRot = originTransform.rotation.eulerAngles.y;
            Vector3 forwardY = Quaternion.Euler(0.0f, yRot, 0.0f) * Vector3.forward;
            Vector3 forward = originTransform.forward;
            Vector3 right = originTransform.right;
            Vector3 up = originTransform.up;
            // ------------------------------------------------------------------
            
            Quaternion rotation = Quaternion.identity;
            
            currentPrefabObject = GameObject.Instantiate(Prefabs[currentPrefabIndex]);

            // --- !!! CÓDIGO NUEVO AÑADIDO !!! ---
            // Llamamos a la función para configurar el EventTrigger
            SetupFireballColliderClick(currentPrefabObject);
            // -----------------------------------

            currentPrefabScript = currentPrefabObject.GetComponent<FireConstantBaseScript>();

            if (currentPrefabScript == null)
            {
                currentPrefabScript = currentPrefabObject.GetComponent<FireBaseScript>();
                if (currentPrefabScript.IsProjectile)
                {
                    // --- LÓGICA DE APUNTADO Y SPAWN MODIFICADA ---
                    pos = originTransform.position + forward; 
                    if (target == null)
                    {
                        rotation = originTransform.rotation;
                    }
                    else
                    {
                        Vector3 directionToTarget = target.position - pos;
                        rotation = Quaternion.LookRotation(directionToTarget);
                    }
                    // --- FIN DE LA LÓGICA MODIFICADA ---
                }
                else
                {
                    pos = originTransform.position + (forwardY * 10.0f);
                }
            }
            else
            {
                pos = originTransform.position + (forwardY * 5.0f);
                rotation = originTransform.rotation;
                pos.y = 0.0f;
            }

            FireProjectileScript projectileScript = currentPrefabObject.GetComponentInChildren<FireProjectileScript>();
            if (projectileScript != null)
            {
                projectileScript.ProjectileCollisionLayers &= (~UnityEngine.LayerMask.NameToLayer("FireLayer"));
            }

            currentPrefabObject.transform.position = pos;
            currentPrefabObject.transform.rotation = rotation;
        }

        /// <summary>
        /// Configura el EventTrigger en el hijo "FireballCollider" del objeto instanciado.
        /// </summary>
        /// <param name="prefabInstance">El GameObject "Fireball" que se acaba de instanciar.</param>
        private void SetupFireballColliderClick(GameObject prefabInstance)
        {
            // 1. Encontrar el objeto "FireballCollider"
            Transform colliderTransform = prefabInstance.transform.Find("FireballCollider");
            if (colliderTransform == null)
            {
                Debug.LogError("No se pudo encontrar el hijo 'FireballCollider' en el prefab " + prefabInstance.name);
                return;
            }
            GameObject colliderObject = colliderTransform.gameObject;

            // Asegurarse de que tiene un collider
            if (colliderObject.GetComponent<Collider>() == null)
            {
                Debug.LogWarning($"El objeto '{colliderObject.name}' no tiene Collider. Añadiendo BoxCollider por defecto.");
                colliderObject.AddComponent<BoxCollider>();
            }

            // 2. Obtener o añadir el componente EventTrigger
            EventTrigger trigger = colliderObject.GetComponent<EventTrigger>();
            if (trigger == null)
            {
                trigger = colliderObject.AddComponent<EventTrigger>();
            }

            // 3. Obtener el script de la Main Camera
            GameObject mainCameraObject = GameObject.FindWithTag("MainCamera");
            if (mainCameraObject == null)
            {
                Debug.LogError("No se encontró 'MainCamera'. Asegúrate de que tu cámara tiene ese tag.");
                return;
            }

            // --- ¡IMPORTANTE! Reemplaza 'Magic' con el nombre de tu script ---
            Magic magicScript = mainCameraObject.GetComponent<Magic>();
            if (magicScript == null)
            {
                Debug.LogError($"'MainCamera' no tiene el script 'Magic'.");
                return;
            }

            // 4. Buscar o crear la entrada de PointerClick
            EventTrigger.Entry entry = trigger.triggers.Find(e => e.eventID == EventTriggerType.PointerClick);
            if (entry == null)
            {
                entry = new EventTrigger.Entry();
                entry.eventID = EventTriggerType.PointerClick;
                
                // Asegurarse de inicializar la lista si es nula
                if (trigger.triggers == null) 
                {
                    trigger.triggers = new System.Collections.Generic.List<EventTrigger.Entry>();
                }
                
                trigger.triggers.Add(entry);
            }

            // Limpiar listeners anteriores para evitar duplicados
            entry.callback.RemoveAllListeners();

            // 5. --- ¡ESTA ES LA LÍNEA CORRECTA Y DEFINITIVA! ---
            // Esto añade la llamada a la función en tiempo de ejecución.
            // Funciona en el editor Y en el build final.
            // No aparecerá en la lista "Runtime Only" del Inspector, ¡pero funcionará!
            entry.callback.AddListener((eventData) => { magicScript.StartCurrent(); });

            Debug.Log($"EventTrigger PointerClick configurado en '{colliderObject.name}' para llamar a 'Magic.StartCurrent()'.");
        }

        // --- MODIFICADO ---
        // Añadimos el parámetro 'spawnPoint' y se lo pasamos a BeginEffect
        public void StartCurrent(Transform target = null, Transform spawnPoint = null)
        {
            StopCurrent();
            BeginEffect(target, spawnPoint);
        }

        private void StopCurrent()
        {
            if (currentPrefabScript != null && currentPrefabScript.Duration > 10000)
            {
                currentPrefabScript.Stop();
            }
            currentPrefabObject = null;
            currentPrefabScript = null;
        }
    }

    // --- !!! CLASE DE EJEMPLO !!! ---
    // Este es un ejemplo del script que deberías tener en tu MainCamera.
    // public class NombreDeTuScriptEnLaCamara : MonoBehaviour
    // {
    //     public void NombreDeTuFuncionPublica()
    //     {
    //         Debug.Log("¡Se ha hecho click en la Fireball!");
    //         // Aquí pones la lógica que quieras ejecutar
    //     }
    // }
    // ---------------------------------
}