using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using System;
using UnityEngine.UI;

public class App : MonoBehaviour
{
    public Action OnRoomEnter;
    public static bool acceptingMoveInput { get; set; } = false;

    protected CinemachineVirtualCamera currentCam;
    protected static App _singleton;
    protected bool ready = false;

    public static readonly float deathDelay = 2f;
    public static readonly float sceneTransitionLength = 1.5f;

    [SerializeField] private Image _transitionQuad;
    [SerializeField] private MarkerManager _markerManager;

    private void Awake()
    {
        Init();
    }

    protected void Init()
    {
        if (_singleton == null)
        {
            _singleton = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        ready = true;
    }

    private IEnumerator Start()
    {
        yield return FadeToFromWhite(false);
    }

    public static void DoSceneTransition(AsyncOperation operation)
    {
        if(App._singleton == null || _singleton._transitionQuad == null) 
        { 
            operation.allowSceneActivation = true; 
            return; 
        }
        _singleton.StartCoroutine(_singleton.SceneTransitionCoroutine(operation));
    }

    private IEnumerator SceneTransitionCoroutine(AsyncOperation operation)
    {
        _transitionQuad.enabled = true;
        yield return FadeToFromWhite(true);
        yield return new WaitForSeconds(.5f);
        operation.allowSceneActivation = true;
    }

    private IEnumerator FadeToFromWhite(bool fadeToWhite)
    {
        if (App._singleton == null || _singleton._transitionQuad == null) { yield break; }
        _transitionQuad.enabled = true;
        float elapsed = 0;
        while (elapsed < sceneTransitionLength)
        {
            elapsed += Time.deltaTime;
            _transitionQuad.color = new Color(1, 1, 1, fadeToWhite ? elapsed / sceneTransitionLength : 1 - elapsed / sceneTransitionLength);
            yield return null;
        }
        _transitionQuad.enabled = fadeToWhite;
    }

    public static void EnterRoom(CinemachineVirtualCamera cam)
    {
        if (_singleton == null || !_singleton.ready) return;
        _singleton.currentCam = cam;
        _singleton.OnRoomEnter?.Invoke();
    }

    public static void SpawnDeathMarker(Vector3 position)
    {
        if(_singleton._markerManager == null) { return; }
        _singleton._markerManager.SpawnMarker(position);
    }
}
