using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

///--------------------------------
///   Author: Victor Alvarez, Ph.D.
///   University of Oviedo, Spain
///--------------------------------

public class Moveball : MonoBehaviour {

    private Rigidbody rigidBody;

    public int ballSpeed = 100;

    public int jumpSpeed = 60;

    private bool ballistouching = false;

    public AudioSource audioSource= null;
    public AudioClip audioClip= null;  

    public int score = 0;

    public Text scoreText; 


	void Start () {
        rigidBody = GetComponent<Rigidbody>();
	}
	
	void Update () {
        float hmove = Input.GetAxis("Horizontal");
        float vmove = Input.GetAxis("Vertical");

        // ball moves with arrow keys
        Vector3 ballmove = new Vector3(hmove,0.0f,vmove);
        rigidBody.AddForce(ballmove*Time.deltaTime*ballSpeed);

        // ball jumps with space bar (only when touching)
        if (Input.GetKey(KeyCode.Space) && ballistouching)
        {
            Vector3 balljump = new Vector3(0.0f,jumpSpeed,0.0f);
            rigidBody.AddForce(balljump);
            ballistouching = false;
        }
    }

    private void OnCollisionStay (Collision collision)
    {
        ballistouching = true;
    }

    private void OnTriggerEnter (Collider other)
    {
        if (other.gameObject.CompareTag("CoinTag"))
        {
            other.gameObject.SetActive(false);
            audioSource.PlayOneShot(audioClip);
            score--;
            updateScore();
            if (score == 0)
               SceneManager.LoadScene("FinalScene");
        }
    }

    private void updateScore()
    {
        scoreText.text = "Coins left: " + score;
    }
}
