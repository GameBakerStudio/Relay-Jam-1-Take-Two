using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseFollow : MonoBehaviour
{
    // This is a script Reeds wrote for me (me as in DiscordDynne) for another project feel free to edit.

    public bool isLeftPressed;
    [SerializeField] float followSpeed = 30;

    Rigidbody2D rb;
    Camera mainCam;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        mainCam = Camera.main;
    }

    void FixedUpdate()
    {
        if (isLeftPressed)
        {
            Vector3 mousePosition = mainCam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, -mainCam.transform.position.z));
            mousePosition.z = 0;
            Vector3 vecToMouse = mousePosition - transform.position;

            //Changed to addForce for more physicy movement
            rb.AddForce(vecToMouse * followSpeed);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
            isLeftPressed = true;
        if (Input.GetMouseButtonUp(0))
            isLeftPressed = false;
    }
}