using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotion : MonoBehaviour
{

    public CharacterController controller;

    // variables for character speed and the force of gravity 
    public float walkspeed;
    public float sprintspeed;
    public float crouchspeed;
    public float gravity;
    public float heightofjump;
    public float crouchingheight = 1.25f;
    public float initialheight = 1.98f;


    public Transform GroundCheck;
    public float distancetoground = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;

    bool isgrounded;
    public bool isSprinting;
    public bool isCrouching; 

   

    void Start()
    {

    }

    
    void Update()
    {

    
        var speed = walkspeed;

        //jumping
        isgrounded = Physics.CheckSphere(GroundCheck.position, distancetoground, groundMask);


        if (isgrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        float x = Input.GetAxis ("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * speed * Time.deltaTime);

        if(Input.GetButtonDown("Jump") && isgrounded)
        {
            velocity.y = Mathf.Sqrt(heightofjump * -2f * gravity);
        }
        




        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

        //....
        if (Input.GetKey(KeyCode.LeftShift) && isCrouching == false)
        {
            isSprinting = true;
        }
        else
        {
            isSprinting = false;
        }

        if(isSprinting == true)
        {
            speed = sprintspeed;
            controller.Move(move * speed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            isCrouching = true;
            controller.height = crouchingheight;
            controller.Move(move * crouchspeed * Time.deltaTime);
        }
        else
        {
            controller.height = initialheight;
            isCrouching = false;
        }
    }
}
