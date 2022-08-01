using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Tilemaps;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Single;

public class MovementController : MonoBehaviour, ISaveable
{
    private static MovementController instance;

    private static readonly Matrix<float> ISO_BASIS = DenseMatrix.OfArray(new float[,]
    {
        { Mathf.Cos(26.57f * Mathf.Deg2Rad), Mathf.Cos(153.43f * Mathf.Deg2Rad) },
        { Mathf.Sin(26.57f * Mathf.Deg2Rad), Mathf.Sin(153.43f * Mathf.Deg2Rad) }
    });

    private static readonly Matrix<float> ROTATION_MATRIX = DenseMatrix.OfArray(new float[,]
    {
        { 0.5f, 0.5f },
        { -0.5f, 0.5f }
    });

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
        Vector<float> delta = ISO_BASIS.Multiply(
            ROTATION_MATRIX.Multiply(
                DenseVector.OfArray(new float[] { axis.x, axis.y })
                )
            );

        rb.velocity = new Vector2(delta[0], delta[1]).normalized * speed;

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
        Debug.Log($"Saved MovementController position {transform.position.x}, {transform.position.y}, {transform.position.z}");
    }

    public void LoadData()
    {
        if (SaveFile.file.HasData(gameObject, typeof(MovementController), "position"))
        {
            float[] position = SaveFile.file.Load<float[]>(gameObject, typeof(MovementController), "position");
            transform.position = new Vector3(position[0], position[1], position[2]);

            Debug.Log($"Position was {new Vector3(position[0], position[1], position[2])}");
        }
    }
}
