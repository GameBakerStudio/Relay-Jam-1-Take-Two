using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{


    public bool isLeftPressed;
    // This is a script Reeds wrote for me (me as in DiscordDynne) for another project feel free to edit.
    [SerializeField] Rigidbody2D rb2d;
    [SerializeField] float followSpeed = 30;
    private Camera mainCam;
    void Start()
    {

        mainCam = Camera.main;
    }

    void FixedUpdate()
    {
        if (isLeftPressed)
        {
            Vector3 mousePosition = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCam.transform.position.z));
            mousePosition.z = 0;

            Vector3 vecToMouse = mousePosition - transform.position;
            rb2d.velocity = vecToMouse * followSpeed;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isLeftPressed = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isLeftPressed = false;
        }
        
    }


}