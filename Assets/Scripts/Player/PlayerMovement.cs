using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    Rigidbody playerBody;
    public float movementSpeed = 1.0f;
    public GameObject testobj;
    Vector3 mousePos;
    // Start is called before the first frame update
    void Start()
    {
      playerBody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        mousePos = InputManager.Instance.GetPositionUsingRay();
        if (Input.GetMouseButtonDown(0)) // if we click our mouse 
        {


            playerBody.transform.position = InputManager.Instance.GetPositionUsingRay();
            //playerBody.transform.position = Vector3.Lerp(transform.position, mousePos, movementSpeed * Time.deltaTime);         }
        }
    }

    
}
