using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DemoButton : MonoBehaviour
{
    [SerializeField] GameObject playVisuals;
    [SerializeField] MultiSourceManager multiSourceManager;

    public void OnClick()
    {
        if (!multiSourceManager.playing && !multiSourceManager.paused)
        {
            playVisuals.SetActive(false);
            multiSourceManager.Play();
        }
        else if(!multiSourceManager.playing && multiSourceManager.paused)
        {
            playVisuals.SetActive(false);
            multiSourceManager.Resume();
        }
        else if(multiSourceManager.playing)
        {
            playVisuals.SetActive(true);
            multiSourceManager.Pause();
        }
    }
}
