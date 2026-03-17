using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

///--------------------------------
///   Authors: Victor Alvarez, Ph.D. Paula Diaz Alvarez
///   University of Oviedo, Spain
///--------------------------------

public class MoveBall : MonoBehaviour
{
    
    private Rigidbody rb;
    public int ballspeed = 0;
    public int jumpspeed = 0;
    public int numberCoins = 0;
    public string nextScene;

    private bool ballistouching = true;
    public AudioSource audioSource;
    public AudioClip audioClip;
    private int score;
    public Text scoreText;

    private string lastKey="";

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
        score = numberCoins;
        updateScore();
	}
	
	// Update is called once per frame
	void Update () {
    
        if(Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)){
            applyForce("W","S");
        } else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            applyForce("A","D");
        } else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
            applyForce("S","W");
        } else {
            applyForce("D","A");
        }

        if (Input.GetKey (KeyCode.Space) && ballistouching )
        {
            Vector3 balljump = new Vector3(0.0f, 6.0f, 0.0f);
            //rb.AddForce(balljump * jumpspeed*Time.deltaTime);
            rb.AddForce(balljump * jumpspeed);
            ballistouching = false;
        }
    }

    void applyForce(string currentKey, string oppositeKey) {
        float hmove = Input.GetAxis("Horizontal");
        float vmove = Input.GetAxis("Vertical");
        Vector3 ballmove = new Vector3(hmove,0.0f,vmove);

        if(lastKey.Equals(oppositeKey)) {
                rb.AddForce(ballmove*ballspeed*2);
            } else {
                rb.AddForce(ballmove*ballspeed);
            }
            lastKey=currentKey;
    }

    // void Update () {
    //     float hmove = Input.GetAxis("Horizontal");
    //     float vmove = Input.GetAxis("Vertical");

    //     Vector3 ballmove = new Vector3(hmove,0.0f,vmove);

    //     //rb.AddForce(ballmove*ballspeed*Time.deltaTime);
    //     if(Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)){
    //         rb.AddForce(ballmove*ballspeed*2);
    //     } else {
    //         rb.AddForce(ballmove*ballspeed);
    //     }

    //     if (Input.GetKey (KeyCode.Space) && ballistouching )
    //     {
    //         Vector3 balljump = new Vector3(0.0f, 6.0f, 0.0f);
    //         //rb.AddForce(balljump * jumpspeed*Time.deltaTime);
    //         rb.AddForce(balljump * jumpspeed);
    //         ballistouching = false;
    //     }
        
        
    // }

    private void OnCollisionStay(Collision collision)
    {
        ballistouching = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("CoinTag"))
        { 
            other.gameObject.SetActive(false); //Desactivo la moneda
            audioSource.PlayOneShot(audioClip); //Reproduzco un sonido
            score--;
            updateScore();
            if (score == 0) //Si llegue a 0 acabé el juego
                SceneManager.LoadScene(nextScene);
        }
    }

    private void updateScore()
    {
        scoreText.text = "Coins: " + score;
    }
}
