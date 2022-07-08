using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;

public class MovementController : MonoBehaviour, ISaveable
{
    private static MovementController instance;

    public static MovementController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MovementController>();
            }
            return instance;
        }
    }

    [SerializeField]
    private Animator animator;

    [SerializeField]
    private float speed;

    [SerializeField]
    private Tilemap groundReference;

    private KeyboardControls keyboardControls;
    private Rigidbody2D rb;

    private Action<InputAction.CallbackContext> stop = _ => { };


    public void OnEnable()
    {
        Debug.Log("Enabled movement controller");
        keyboardControls?.Enable();
    }

    public void OnDisable()
    {
        Debug.Log("Disabled movement controller");
        keyboardControls?.Disable();
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        keyboardControls = new KeyboardControls();
        keyboardControls.Mouse.Move.performed += ctx => Move(ctx.ReadValue<Vector2>());

        keyboardControls.Mouse.Move.canceled += _ => Stop();
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

    public void SaveData()
    {
        SaveFile.file.Save(gameObject, typeof(MovementController), "position", 
            new float[] { transform.position.x, transform.position.y, transform.position.z });
    }

    public void LoadData()
    {
        if (SaveFile.file.HasData(gameObject, typeof(MovementController), "position"))
        {
            float[] position = SaveFile.file.Load<float[]>(gameObject, typeof(MovementController), "position");
            transform.position = new Vector3(position[0], position[1], position[2]);
        }
    }
}
