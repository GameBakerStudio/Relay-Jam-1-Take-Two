using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomHandler : MonoBehaviour
{
    public static RoomHandler activeRoom = null;

    // This script is used to switch rooms to create a nice room effect (I defininitly didnt follow along with a tutorial)
    public CinemachineVirtualCamera vcam;
    public Transform spawnPoint;

    Camera mainCam;

    private void Awake()
    {
        //Use the main camera size
        mainCam = Camera.main;
        vcam.m_Lens.OrthographicSize = mainCam.orthographicSize;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            Debug.Log("AAAA");
            activeRoom = this;
            vcam.gameObject.SetActive(true);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger)
        {
            vcam.gameObject.SetActive(false);
        }
    }
}
