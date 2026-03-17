using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ProjectileHit : MonoBehaviour
{
    [Header("Configuración")]
    public int maxHoyos = 3; 

    [Header("Interfaz de Usuario")]
    public TextMeshProUGUI resetCounterText;

    private static int totalResets = 0;

    private void Start()
    {
        UpdateCounterDisplay();
    }

    private void OnCollisionEnter(Collision collision)
    {
        // En lugar de comparar con una variable, comparamos con el Tag
        if (collision.gameObject.CompareTag("Diana"))
        {
            totalResets++; 

            if (totalResets >= maxHoyos)
            {
                FinalizarJuego();
            }
            else
            {
                ResetScene();
            }
        }
    }

    private void UpdateCounterDisplay()
    {
        if (resetCounterText != null)
        {
            resetCounterText.text = "Hoyo: " + (totalResets + 1) + "/" + maxHoyos;
        }
    }

    private void ResetScene()
    {
        // IMPORTANTE: Si vas a cargar la escena de menú, cambia esto por el nombre de tu menú
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void FinalizarJuego()
    {
        Debug.Log("Cerrando aplicación...");
        Application.Quit();
        
        // Si quieres que vaya al menú en lugar de cerrarse, usa:
        // SceneManager.LoadScene("NombreDeTuEscenaMenu");
    }
}