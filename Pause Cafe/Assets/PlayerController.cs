using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float speed=10;
    private Animator animatorPlayer;

    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        animatorPlayer = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            Debug.Log("courir");
            animatorPlayer.SetBool("isRunning", true);


        }
        else
        {
            Debug.Log("immobile");
            animatorPlayer.SetBool("isRunning", false);

        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            animatorPlayer.Play("Roulade");
            rb.AddForce(transform.forward * 5, ForceMode.VelocityChange);

        }


        if (Input.GetKeyDown(KeyCode.V))
        {
            animatorPlayer.Play("Attaque3");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            animatorPlayer.Play("AttackJump");
        }


  //      animatorPlayer.SetBool("isRoulade", false);

        /*       else
               {
                   animatorPlayer.SetBool("isRunning", false);
               }*/

        float axeX =Input.GetAxis("Horizontal");
        float axeZ=Input.GetAxis("Vertical");

        this.transform.Translate(Vector3.right*Time.deltaTime*speed* axeX);
        this.transform.Translate(Vector3.forward * Time.deltaTime * speed * axeZ);

    }
}
