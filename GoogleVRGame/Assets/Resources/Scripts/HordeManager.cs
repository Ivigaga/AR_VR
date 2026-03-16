using UnityEngine;
using UnityEngine.SceneManagement; // Para cargar escenas
using TMPro; // Necesario para TextMeshPro

public class HordeManager : MonoBehaviour
{
    // Arrastra el objeto EnemyCounterText (TextMeshPro) aquí en el Inspector
    public TextMeshProUGUI enemyCounterText;

    // Etiqueta para identificar enemigos
    public string enemyTag = "Enemy";

    // Escena a cargar cuando no queden enemigos
    public string victoryScene;

    private bool hasWon = false; // Para evitar cargar la escena múltiples veces

    void Update()
    {
        UpdateEnemyCounter();
    }

    // Actualiza el texto en la UI contando solo enemigos VIVOS (con HealthSystem activo)
    void UpdateEnemyCounter()
    {
        if (enemyCounterText != null)
        {
            GameObject[] allEnemies = GameObject.FindGameObjectsWithTag(enemyTag);
            int aliveCount = 0;

            foreach (GameObject enemy in allEnemies)
            {
                // Contar solo si tiene HealthSystem habilitado (está vivo)
                HealthSystem health = enemy.GetComponent<HealthSystem>();
                if (health != null && health.enabled)
                {
                    aliveCount++;
                }
            }

            enemyCounterText.text = "Enemigos: " + aliveCount;

            // Si no quedan enemigos vivos y no ha ganado aún, cargar escena de victoria
            if (aliveCount == 0 && !hasWon && !string.IsNullOrEmpty(victoryScene))
            {
                hasWon = true;
                Debug.Log("¡Todos los enemigos derrotados! Cargando escena: " + victoryScene);
                SceneManager.LoadScene(victoryScene);
            }
        }
    }
}