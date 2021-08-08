using UnityEngine;

public enum EWaveType { Sin, ClippedSin, Square, Saw, Triangle, Noise, FM}

public class Wave
{
    protected double phase;
    protected double time;
    protected float frequency;
    protected double increment;
    protected int note;

    public void SetFreq(float freq)
    {
        this.frequency = freq;
    }

    public virtual float[] Sample(int note, int bufferLength, int channels = 2, float pitchBend = 0)
    {
        this.note = note;
        
        var buffer = new float[bufferLength];

        frequency = Freq(note);
        frequency += pitchBend;
        SetIncrement();

        for (int i = 0; i < bufferLength; i += channels)
        {
            IncrementPhase();
            buffer[i] = AtPoint();

            if (channels == 2)
            {
                buffer[i + 1] = buffer[i];
            }

            LoopPhase();
        }
        return buffer;
    }

    public void SetIncrement()
    {
        increment = frequency * 2f * Mathf.PI / OscillatorSource._samplingFrequency;
    }

    public void IncrementPhase()
    {
        phase += increment;
    }

    public void LoopPhase()
    {
        if (phase > Mathf.PI * 2f)
        {
            phase -= Mathf.PI * 2f;
        }
    }

    public virtual float AtPoint()
    {
        return SinOf((float)phase);
    }

    #region Standard waveforms

    public static float SinOf(double phase)
    {
        return Mathf.Sin((float)phase);
    }

    public static float SquareOf(float phase)
    {
        if (Mathf.Sin((float)phase) >= 0)
        {
            return .6f;
        }
        else
        {
            return -.6f;
        }
    }

    #endregion

    public static float Freq(int note)
    {
        return 440 * Mathf.Pow(2, note / 12f);
    }

    public static float Freq(int? note)
    {
        return 440 * Mathf.Pow(2, (float)note / 12f);
    }
}

[System.Serializable]
public class WaveSin : Wave
{
    public override float AtPoint()
    {
        return SinOf(phase);
    }
}

public class WaveSquare : Wave
{
    public override float AtPoint()
    {
        return GetSquare();
    }

    public float GetSquare()
    {
        return Wave.SquareOf((float)phase);
    }
}

public class WaveSaw : Wave
{
    public override float AtPoint()
    {
        return (float)(phase % (2 * Mathf.PI) / 2f)/2f - .5f;
    }
}

public class WaveNoise: Wave
{
    System.Random rand = new System.Random();

    public override float AtPoint()
    {
        phase = rand.NextDouble() * 2 * Mathf.PI;
        return base.AtPoint();
    }
}

public class WaveTriangle : Wave
{
    public override float AtPoint()
    {
        return (float)((double)Mathf.PingPong((float)phase, 1.0f)) * 2f - .5f;
    }
}

public class WaveClippedSin : Wave
{
    public override float AtPoint()
    {
        var clippedSin = Wave.SinOf(phase) * 2.5f;
        clippedSin = Mathf.Clamp(clippedSin, -1, 1);
        return clippedSin;
    }
}

public class WaveFM : Wave
{
    public float modIndex = 500;
    private WaveSin modWave = new WaveSin();

    public override float AtPoint()
    {
        return SinOf(phase);
    }

    public override float[] Sample(int note, int bufferLength, int channels = 2, float pitchBend = 0)
    {
        modWave.SetFreq(1111);
        this.note = note;
        
        var buffer = new float[bufferLength];

        for (int i = 0; i < bufferLength; i += channels)
        {
            
            modWave.SetIncrement();
            frequency = Freq(note) + pitchBend + modWave.AtPoint() * modIndex;
            SetIncrement();
            

            IncrementPhase();
            modWave.IncrementPhase();

            buffer[i] = AtPoint();

            if (channels == 2)
            {
                buffer[i + 1] = buffer[i];
            }
        }

        LoopPhase();
        modWave.LoopPhase();

        return buffer;
    }
}