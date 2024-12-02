using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleMovement : MonoBehaviour
{
    /// <summary>
    /// The goal and logic of this movement script is very simply. We move forward, if we run into a wall we move backward
    /// We keep flipping directions over an over, no detection for player or obstacles. 
    /// </summary>
    [SerializeField] float moveSpeed = 5f;
    Rigidbody enemyBody;
    // bool isAlive; // Don't need to check for alive status
    // but if you decide to add logic related to health then this will be helpful.
    void Start()
    {
        enemyBody = GetComponent<Rigidbody>();
    }

    
    void Update()
    {
        enemyBody.velocity = new Vector3 (moveSpeed, 0, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Ran into wall");
        moveSpeed = -moveSpeed;
        flipFacingDirection();
    }

    private void flipFacingDirection()
    {
        // Turn around. 
        transform.Rotate (Vector3.forward  * Time.deltaTime);
    }
}
