using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CoinCount : MonoBehaviour
{
    public AudioSource audioSource = null;
    public AudioClip audioClip = null;  
    
    // Variable estática compartida por todas las instancias
    public static int sharedScore = 0;
    
    // Variable local para inicializar el score solo una vez
    public int initialScore = 0;
    
    public Text scoreText; 
    // Start is called before the first frame update
    void Start()
    {
        // Solo inicializar el score compartido si no ha sido inicializado
        // y si este objeto tiene un valor inicial configurado
        if (sharedScore == 0 && initialScore > 0)
        {
            sharedScore = initialScore;
        }
        
        // Actualizar la UI al iniciar
        updateScore();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Coin"))
        {
            other.gameObject.SetActive(false);
            
            // Reproducir sonido solo si hay AudioSource configurado
            if (audioSource != null && audioClip != null)
            {
                audioSource.PlayOneShot(audioClip);
            }
            
            // Decrementar el score compartido
            sharedScore--;
            updateScore();
            
            // Verificar si el juego terminó
            if (sharedScore <= 0)
                SceneManager.LoadScene("FinalScene");
        }
    }
    
    private void updateScore()
    {
        // Solo actualizar el texto si hay scoreText configurado
        if (scoreText != null)
        {
            scoreText.text = "Coins left: " + sharedScore;
        }
    }
    
    // Método estático para acceder al score desde otros scripts
    public static int GetScore()
    {
        return sharedScore;
    }
    
    // Método estático para modificar el score desde otros scripts
    public static void SetScore(int newScore)
    {
        sharedScore = newScore;
    }
    
    // Método estático para decrementar el score desde otros scripts
    public static void DecrementScore()
    {
        sharedScore--;
    }
}
