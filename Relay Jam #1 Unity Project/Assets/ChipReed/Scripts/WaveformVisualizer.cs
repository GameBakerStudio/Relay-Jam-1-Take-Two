using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class WaveformVisualizer : MonoBehaviour
{
    [SerializeField] MultiSourceManager multiSourceManager;
    private LineRenderer lineRenderer;
    private float horizontalCrunch = .002f;
    private int ticker;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.SetPositions(new Vector3[] { transform.position, transform.position + Vector3.right * 10 });
    }

    void FixedUpdate()
    {
        if (multiSourceManager == null) return;
        if (!multiSourceManager.playing) return;
        lineRenderer.positionCount = multiSourceManager.managedOscillators[0].lastBuffer.Length / 2;

        for(int i = 0; i < lineRenderer.positionCount; i++)
        {
            Vector3 position = transform.position + new Vector3(i * horizontalCrunch, 0, 0);
            foreach (var oscillator in multiSourceManager.managedOscillators)
            {
                if (!oscillator.gameObject.activeSelf || oscillator.lastBuffer == null || oscillator.lastBuffer.Length < 1)
                {
                    continue;
                }
                position = position + new Vector3(0, oscillator.lastBuffer[i * 2], 0);
            }
            lineRenderer.SetPosition(i, position);
        }
    }
}
