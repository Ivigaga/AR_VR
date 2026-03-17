using System.Collections.Generic;
using UnityEngine;

public class StackMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 5.5f;
    public Camera playerCamera;
    public bool selectionMode = false;

    public float castSkin = 0.05f;
    public LayerMask collisionMask = ~0;

    private struct CubeData
    {
        public GameObject go;
        public Rigidbody rb;
        public Collider col;
    }

    private readonly List<CubeData> group = new List<CubeData>();
    private bool groupActive = false;

    public void SetStack(List<GameObject> cubes)
    {
        group.Clear();

        foreach (var cube in cubes)
        {
            var rb = cube.GetComponent<Rigidbody>();
            var col = cube.GetComponent<Collider>();

            if (rb != null && col != null)
            {
                rb.freezeRotation = true;
                group.Add(new CubeData { go = cube, rb = rb, col = col });
            }
        }

        if (playerCamera == null)
            playerCamera = Camera.main;

        groupActive = group.Count > 0;
    }

    public void ClearStack()
    {
        group.Clear();
        groupActive = false;
    }

    void Update()
    {
        if (!groupActive || playerCamera == null || selectionMode) return;

        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        forward.y = 0f; right.y = 0f;
        forward.Normalize(); right.Normalize();

        float h = (Input.GetKey(KeyCode.A) ? -1f : 0f) + (Input.GetKey(KeyCode.D) ? 1f : 0f);
        float v = (Input.GetKey(KeyCode.S) ? -1f : 0f) + (Input.GetKey(KeyCode.W) ? 1f : 0f);

        Vector3 moveDir = (forward * v + right * h);
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();

        if (moveDir != Vector3.zero && CanMoveGroup(moveDir, moveSpeed * Time.deltaTime))
        {
            Vector3 delta = moveDir * moveSpeed * Time.deltaTime;
            foreach (var c in group)
            {
                c.rb.MovePosition(c.rb.position + delta);
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && IsGroupGrounded())
        {
            foreach (var c in group)
            {
                c.rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }

    private bool CanMoveGroup(Vector3 dir, float dist)
    {
        foreach (var c in group)
        {
            Vector3 size = c.col.bounds.size;
            Vector3 halfExtents = size * 0.5f - Vector3.one * castSkin;
            float castDistance = dist + castSkin;
            Vector3 origin = c.col.bounds.center;
            Quaternion orientation = c.rb.rotation;

            if (Physics.BoxCast(origin, halfExtents, dir.normalized, out RaycastHit hit, orientation, castDistance, collisionMask, QueryTriggerInteraction.Ignore))
            {
                foreach (var other in group)
                {
                    if (hit.collider != null && hit.collider.gameObject == other.go)
                        goto ContinueNext;
                }
                return false;
            }

            ContinueNext: ;
        }
        return true;
    }

    private bool IsGroupGrounded()
    {
        foreach (var c in group)
        {
            float rayDist = 1.1f;
            if (!Physics.Raycast(c.rb.position, Vector3.down, rayDist, collisionMask, QueryTriggerInteraction.Ignore))
                return false;
        }
        return true;
    }
}
