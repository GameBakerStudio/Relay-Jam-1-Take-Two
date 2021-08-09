using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiSourceManager : MonoBehaviour
{
    [SerializeField] public List<OscillatorSource> managedOscillators;
    [SerializeField] float playbackSpeed = 1f;
    [SerializeField] bool loop = false;
    [SerializeField] [Range(0,1)] public float masterGain = 1;
    private double timer;
    public bool playing { get; private set; }
    public bool paused { get; private set; }
    private double lastTickDSPTime;

    private int tickCount;
    private int longestInstructionCount;

    public void Play()
    {
        longestInstructionCount = 0;
        foreach (var oscillator in managedOscillators)
        {
            if(oscillator.gameObject.activeSelf && oscillator.InstructionCount > longestInstructionCount)
            {
                longestInstructionCount = oscillator.InstructionCount;
            }
        }

        managedOscillators.ForEach(x => x.InitTrack(this));

        Reset();
        PlayAll();
    }

    public void Pause()
    {
        managedOscillators.ForEach(x => x.Pause());
        playing = false;
        paused = true;
    }

    public void Resume()
    {
        managedOscillators.ForEach(x => x.Resume());
        playing = true;
        paused = false;
    }

    public void Stop()
    {
        managedOscillators.ForEach(x => x.Pause());
        Reset();
        playing = false;
        paused = false;
    }

    private void PlayAll() => managedOscillators.ForEach(x => x.Play());

    private void Reset()
    {
        lastTickDSPTime = AudioSettings.dspTime;
        tickCount = 0;
        playing = true;
        paused = false;
    }

    private void Update()
    {
        if (!playing) return;

        float tickRate = 1/ (8f * playbackSpeed);
        timer = AudioSettings.dspTime - lastTickDSPTime;
        
        if(timer >= tickRate)
        {
            managedOscillators.ForEach(x => x.Tick());
            tickCount++;

            // Might want to change how this increments later. Weird consistency issues.
            lastTickDSPTime = AudioSettings.dspTime;
        }

        if (tickCount >= longestInstructionCount)
        {
            if(loop)
            {
                Play();
                return;
            }
            else
            {
                playing = false;
                return;
            }
        }
    }
}
