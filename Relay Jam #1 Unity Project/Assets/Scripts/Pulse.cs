using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pulse : MonoBehaviour
{
    [SerializeField] private float _pulseWidth = .5f;
    [SerializeField] private float _minScale = .5f;
    [SerializeField] private float _maxScale = 1f;
    [SerializeField] private float _rotationSpeed = 1f;

    void Update()
    {
        float scaleMod = Mathf.PingPong(Time.time * _pulseWidth, 1);
        float scale = Mathf.SmoothStep(_minScale, _maxScale, scaleMod);
        transform.localScale = scale * Vector3.one;
        transform.Rotate(Vector3.one * _rotationSpeed * Time.deltaTime);
    }
}
