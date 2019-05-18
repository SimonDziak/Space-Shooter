﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Boundary
{
    public float xMin, xMax, zMin, zMax;
}

public class PlayerController : MonoBehaviour
{
    public float moveHorizontal;
    public float moveVertical;

    private Rigidbody rb;
    public float speed;
    public float tilt;
    public Boundary boundary;

    // projectile
    public GameObject projectile;
    private GameObject newProjectile;
    public Transform shotSpawn;
    public float fireDelta = 0.25f;
    private float nextFire = 0.05f;
    private float myTime = 0.0f;

    // Face mouse function
    public Transform CharacterTransform;
    public float RotationSmoothingCoef;

    // audio
    private AudioSource audiosource;

    private void Update()
    {
        myTime = myTime + Time.deltaTime;
        shotSpawn.eulerAngles = new Vector3(0.0f, shotSpawn.eulerAngles.y, 0.0f); // fixes bullets falling through the ground

        if (Input.GetButton("Fire1") && myTime > nextFire)
        {
            nextFire = myTime + fireDelta;
            shotSpawn.rotation = Quaternion.Euler(shotSpawn.rotation.eulerAngles.x, shotSpawn.rotation.eulerAngles.y, 0.0f);
            newProjectile = Instantiate(projectile, shotSpawn.position, shotSpawn.rotation);
            audiosource.Play();

            // create code here that animates the newProjectil

            nextFire = nextFire - myTime;
            myTime = 0.0F;
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        audiosource = GetComponent<AudioSource>();

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        moveHorizontal = Input.GetAxis("Horizontal");
        moveVertical = Input.GetAxis("Vertical");
        faceMouse();

        Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
        rb.AddForce(movement * speed);
        
        //Boundary
        rb.position = new Vector3
        (
            Mathf.Clamp(rb.position.x, boundary.xMin, boundary.xMax),
            0.0f,
            Mathf.Clamp(rb.position.z, boundary.zMin, boundary.zMax)
        );
        
    }
    public void faceMouse() // i basically have zero idea what this code does
    {
        var groundPlane = new Plane(Vector3.up, -CharacterTransform.position.y);
        var mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        float hitDistance;

        if (groundPlane.Raycast(mouseRay, out hitDistance))
        {
            var lookAtPosition = mouseRay.GetPoint(hitDistance);
            var targetRotation = Quaternion.LookRotation(lookAtPosition - CharacterTransform.position, Vector3.up);
            var rotation = Quaternion.Lerp(CharacterTransform.rotation, targetRotation, RotationSmoothingCoef);
            rotation.z = rb.velocity.x * -tilt;
            //Debug.Log("RB Velocity X" + rb.velocity.x * -tilt);
            //Debug.Log("rotation: " + rotation);
            CharacterTransform.rotation = rotation;
        }
    }
}