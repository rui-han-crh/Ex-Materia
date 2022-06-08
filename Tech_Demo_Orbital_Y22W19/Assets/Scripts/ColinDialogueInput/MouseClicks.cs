using UnityEngine.InputSystem;
using UnityEngine;


/*
 * this has to be attatched to something in the scene 
 * Preferably the GameManager
 */


public class MouseClicks : MonoBehaviour
{
    /*
     * need to serialize the gameCam for this
     */
    [SerializeField]
    private Camera gameCamera;
    private InputAction click;

    void Awake()
    {
        click = new InputAction(binding: "<Mouse>/leftButton");
        click.performed += ctx => {
            RaycastHit hit;
            Vector3 coor = Mouse.current.position.ReadValue();
            if (Physics.Raycast(gameCamera.ScreenPointToRay(coor), out hit))
            {
                hit.collider.GetComponent<IClickable>()?.OnClick();
            }
        };
        click.Enable();
    }
}