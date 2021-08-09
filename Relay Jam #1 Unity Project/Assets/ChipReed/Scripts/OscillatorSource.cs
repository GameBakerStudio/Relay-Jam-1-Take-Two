using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class OscillatorSource : MonoBehaviour
{
    [SerializeField] bool reverb; //These don't work yet
    [SerializeField] float reverbFactor = .01f; //These don't work yet

    [SerializeField] [Range(0,.3f)] private float gain = .1f; // The gain of the oscillator
    [SerializeField] private float volume = 1; // The volume of the current note instruction

    [SerializeField] private float oscillatorBend; // The bend applied to the overall pitch
    [SerializeField] private int octaveShift;
    [SerializeField] private float currentBend = 0; // The current pitch bend instruction for the oscillator

    [SerializeField] EWaveType wave; // Debug inspector wave input
    [SerializeField] NoteRoll noteRollSource; // The note roll the oscillator reads from

    private int? currentPitch = 3; // The base pitch of the current note instruction

    private float interpVolume = 1; // The interpolated volume used to scale the gain
    private const float interpSpeed = .01f; // The interpolation step limit in place to prevent transients

    public bool playing;
    public double currentTrackTime;

    public double startDspTime { get; private set; }
    public static readonly double _samplingFrequency = 48000;
    
    private List<Wave> inputs = new List<Wave>(); // The source wave functions for generating the new buffer
    
    [HideInInspector] public float[] lastBuffer = new float[0]; // The buffer from the last read

    private MultiSourceManager myManager;
    private bool onNullNote;

    public int InstructionCount {
        get 
        {
            if (noteRollSource == null) return 0;
            return noteRollSource.InstructionCount;
        }
    }

    [SerializeField] int debugPitch;

    private void Start()
    {
        SetWave();
    }

    private void OnValidate()
    {
        SetWave();
    }

    public NoteRoll GetNoteRoll() => noteRollSource;

    private void SetWave()
    {
        inputs.Clear();
        switch (wave)
        {
            case EWaveType.Sin:
                inputs.Add(new WaveSin());
                gameObject.name = $"Osc. Source (Sin)";
                break;
            case EWaveType.Square:
                inputs.Add(new WaveSquare());
                gameObject.name = $"Osc. Source (Square)";
                break;
            case EWaveType.Saw:
                inputs.Add(new WaveSaw());
                gameObject.name = $"Osc. Source (Saw)";
                break;
            case EWaveType.FM:
                inputs.Add(new WaveFM());
                gameObject.name = $"Osc. Source (FM)";
                break;
            case EWaveType.Noise:
                inputs.Add(new WaveNoise());
                gameObject.name = $"Osc. Source (Noise)";
                break;
            case EWaveType.Triangle:
                inputs.Add(new WaveTriangle());
                gameObject.name = $"Osc. Source (Triangle)";
                break;
            case EWaveType.ClippedSin:
                inputs.Add(new WaveClippedSin());
                gameObject.name = $"Oscillator Source (Clipped Sin)";
                break;
            default:
                break;
        }
        if(noteRollSource?.track != null)
        {
            gameObject.name += $" {noteRollSource.track.name}";
        }
    }

    public void Tick()
    {
        noteRollSource.Tick();
    }

    public void InitTrack(MultiSourceManager manager)
    {
        myManager = manager;
        noteRollSource.Init(this);
    }

    public void Play()
    {
        SetWave();
        startDspTime = AudioSettings.dspTime;

        noteRollSource.ResetToStart();
        noteRollSource.StartRead(AudioSettings.dspTime);

        playing = true;
    }

    public void Pause()
    {
        playing = false;
        noteRollSource.StopRead();
    }

    public void Resume()
    {
        playing = true;
        noteRollSource.ContinueRead();
    }

    public void SetPitch(int? pitch)
    {
        currentPitch = pitch;
    }

    public void SetBend(float bend)
    {
        this.currentBend = bend;
    }

    public void SetVolume(float vol)
    {
        this.volume = vol;
    }

    public void SetNullNoteInstruction(bool onNullNote)
    {
        this.onNullNote = onNullNote;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        if (!playing) return;

        noteRollSource.Read();

        currentTrackTime = AudioSettings.dspTime - startDspTime;

        var buffer = new float[data.Length];

        if (!onNullNote && currentPitch != null)
        {
            for (int input = 0; input < inputs.Count; input++)
            {
                var tempBuffer = inputs[input].Sample((int)currentPitch + (octaveShift * 12), data.Length, channels, currentBend + oscillatorBend);
                for (int j = 0; j < data.Length; j++)
                {
                    buffer[j] += tempBuffer[j];
                }
            }
        }

        for (int i = 0; i < buffer.Length; i++)
        {
            interpVolume = Mathf.MoveTowards(interpVolume, volume, interpSpeed);
            buffer[i] *= gain * interpVolume * myManager.masterGain;
        }

        
        //Reverb, VERY WIP
        /*
        var loopGain = reverbFactor;
        var delayMilliseconds = 5;
        var delaySamples = (int)((float)delayMilliseconds * (_samplingFrequency/1000f));
        if (reverb)
        {
            for (int i = 0; i < buffer.Length - delaySamples; i++)
            {
                buffer[i + delaySamples] += (buffer[i] * loopGain);
            }
        }
        */

        Array.Copy(buffer, data, buffer.Length);

        if(lastBuffer == null || lastBuffer.Length < buffer.Length) { lastBuffer = new float[buffer.Length]; }
        Array.Copy(buffer, lastBuffer, buffer.Length);
    }

    private float CalculatePerLoopGain(float delayTime, float reverbTime)
    {
        return (1f / (Mathf.Pow(10, (3f / (delayTime * reverbTime)))));
    }

}
