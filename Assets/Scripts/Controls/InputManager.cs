#define USE_NEW_INPUT_SYSTEM 
// preprocessor directive aka variable file for compiler, this will be used for input system. 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
    private PlayerInputActions playerActions;

    private void Awake()
    {
        // error check for more than one instance, hence singleton, we'll delete the extra
        if (Instance != null)
        {
            Debug.Log("There's more than one UnitActionSystem! " + transform + "_" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        playerActions = new PlayerInputActions();
        playerActions.Player.Enable();
    }
    public Vector2 GetMouseScreenPosition()
    {
#if USE_NEW_INPUT_SYSTEM // if defined when code compiles it will run code in if, otherwise it will run code in else.  running on compile time instead of run time
        return Mouse.current.position.ReadValue();
#else
        return Input.mousePosition;
#endif
    }


    public Vector2 GetCameraMoveVector()
    {
/*#if USE_NEW_INPUT_SYSTEM
        Debug.Log(playerActions.Player.CameraMovement.ReadValue<Vector2>());
       return playerActions.Player.CameraMovement.ReadValue<Vector2>();
#else*/
        Vector3 inputMoveDir = new Vector2(0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.y = +10f;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.y = -2f;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -10f;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = +10f;
        }
        return inputMoveDir;

    }

    public float GetCameraRotateAmount()
    {
/*#if USE_NEW_INPUT_SYSTEM
        return playerActions.Player.CameraRotate.ReadValue<float>();*/

        float rotateAmount = 0f;

        if (Input.GetKey(KeyCode.Q))
        {
            rotateAmount = +1f;
        }
        if (Input.GetKey(KeyCode.E))
        {
            rotateAmount = -1f;
        }

        return rotateAmount;

    }

    public float GetCameraZoomAmount()
    {
/*#if USE_NEW_INPUT_SYSTEM
        return playerActions.Player.CameraZoom.ReadValue<float>();*/

        float zoomAmount = 0f;
        if (Input.mouseScrollDelta.y > 0)
        {
            zoomAmount = -1f;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            zoomAmount = 1f;
        }

        return zoomAmount;

    }

}
