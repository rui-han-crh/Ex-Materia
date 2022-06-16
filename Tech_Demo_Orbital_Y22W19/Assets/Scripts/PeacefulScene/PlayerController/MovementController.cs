using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MovementController : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private int speed;

    [SerializeField]
    private Tilemap groundReference;

    private KeyboardControls keyboardControls;
    private Rigidbody2D rb;
    

    private void OnEnable()
    {
        keyboardControls?.Enable();
    }

    private void OnDisable()
    {
        keyboardControls?.Disable();
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
