using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Tilemap groundReference;

    private KeyboardControls keyboardControls;
    private Rigidbody2D rb;
    

    public void OnEnable()
    {
        Debug.Log("Enabled movement controller");
        keyboardControls?.Enable();

        if (SaveFile.file.HasData(gameObject, typeof(MovementController), "position"))
            transform.position = SaveFile.file.Load<Vector3>(gameObject, typeof(MovementController), "position");
    }

    public void OnDisable()
    {
        Debug.Log("Disabled movement controller");
        keyboardControls?.Disable();
        SaveFile.file.Save(gameObject, typeof(MovementController), "position", transform.position);
    }

    private void Awake()
    {
        keyboardControls = new KeyboardControls();
        keyboardControls.Mouse.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());
        keyboardControls.Mouse.Move.canceled += _ => Stop();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Move(Vector2 axis)
    {
        rb.velocity = axis * speed;
        animator.SetBool("isMoving", true);
        animator.SetFloat("xDirection", axis.x);
        animator.SetFloat("yDirection", axis.y);
    }

    public void Stop()
    {
        rb.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);
    }
}
