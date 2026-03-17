using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class CubeMovement : MonoBehaviour
{
    public float moveSpeed = 5f;      
    public float jumpForce = 5f;      
    public Camera playerCamera;       

    private Rigidbody rb;
    private bool isGrounded = true;
    private string lastKey = "";
    public bool selectionMode = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Bloquear rotación para que nunca gire al moverse
        rb.freezeRotation = true;

        if (playerCamera == null)
            playerCamera = Camera.main;
    }

    void Update()
    {
        if (selectionMode) return;
        // --- Movimiento con teclas ---
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
        {
            Move("W", "S");
        }
        else if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
        {
            Move("S", "W");
        }
        else if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
        {
            Move("A", "D");
        }
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
        {
            Move("D", "A");
        }

        // --- Salto ---
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    void Move(string currentKey, string oppositeKey)
    {
        // Direcciones relativas a la cámara
        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        // Obtener dirección del input
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");
        Vector3 moveDir = (forward * v + right * h).normalized;

        // Mover suavemente sin usar física (sin AddForce)
        rb.MovePosition(rb.position + moveDir * moveSpeed * Time.deltaTime);

        lastKey = currentKey;
    }

    void FixedUpdate()
    {
        // Detectar suelo
        isGrounded = Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }
}
