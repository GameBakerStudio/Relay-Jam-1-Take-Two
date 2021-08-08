using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class NoteRoll : MonoBehaviour
{
    private OscillatorSource oscillator;
    //private List<NoteInstruction> noteInstructions;

    [SerializeField] public NoteRollTrackAuthoring track;
    private List<LWNoteInstruction> instructions = new List<LWNoteInstruction>();

    private double trackStartDspTime;
    public double trackDspTime;
    private bool reading;
    private int noteIndex = 0;

    private double lastNoteDspStart;
    private LWNoteInstruction lastNonNullInstruction;

    public int InstructionCount
    {
        get
        {
            if (instructions == null) return 0;
            return instructions.Count;
        }
    }

    public void Init(OscillatorSource oscillator)
    {
        #region Old
        /*
        double currentDspTime = 0;

        for (int i = 0; i < noteInstructions.Count; i++)
        {
            noteInstructions[i] = noteInstructions[i].SetDspTime(currentDspTime);
            noteInstructions[i] = noteInstructions[i].TryParseASPN();
            currentDspTime += noteInstructions[i].duration;
        }
        */
        #endregion

        this.oscillator = oscillator;
        instructions = track?.MergePartitions();
    }

    public int GetInstructionCountNoMerge()
    {
        int count = 0;
        if (track == null) return 0;
        foreach (var partition in track.partitions)
        {
            count += partition.instructions.Count;
        }
        return count;
    }

    public static float PitchFromInt(int val)
    {
        return 440 * Mathf.Pow(2, val / 12f);
    }

    public void ResetToStart()
    {
        noteIndex = 0;
        lastNonNullInstruction = null;
        lastAD = null;
        lastVibratoDepth = 0;
        lastVibratoFreq = 0;
        oscillator.SetVolume(0);

        if(instructions != null)
            oscillator.SetPitch(instructions[0].ASPN2Int());
    }

    public void Tick()
    {
        if (!reading) return;

        noteIndex++;
        if (!reading || noteIndex >= instructions.Count) return;

        ResetLastNoteDsp();
        CheckNullInstruction();
    }

    private void CheckNullInstruction()
    {
        if (instructions == null || instructions.Count < 1) return;
        if (instructions[noteIndex].HasNote())
        {
            lastNonNullInstruction = instructions[noteIndex];
        }
    }

    public void StartRead(double trackDspTime)
    {
        this.trackStartDspTime = trackDspTime;
        reading = true;

        ResetToStart();

        CheckNullInstruction();
        oscillator.SetNullNoteInstruction(false);
        lastNoteDspStart = 0;
    }

    public void StopRead()
    {
        reading = false;
    }

    public void ContinueRead()
    {
        reading = true;
    }

    public void Read()
    {
        if (!reading || instructions == null) return;
        trackDspTime = AudioSettings.dspTime - trackStartDspTime;

        if (noteIndex >= instructions.Count)
        {
            oscillator.SetNullNoteInstruction(true);
            reading = false;
            Debug.LogError("Done reading");
            return;
        }

        SetADEnvelopeParams();
        SetOscilatorParams();
    }

    private void ResetLastNoteDsp()
    {
        if(instructions[noteIndex].ASPN2Int() != null)
            lastNoteDspStart = trackDspTime;
    }

    private float lastVibratoDepth;
    private float lastVibratoFreq;
    private void SetOscilatorParams()
    {
        double timeSinceLastNote = trackDspTime - lastNoteDspStart;
        var instruction = instructions[noteIndex];

        if (lastNonNullInstruction == null) return;

        if (instruction.vibratoDepth != 0)
        {
            lastVibratoDepth = instruction.vibratoDepth;
        }

        if (instruction.vibratoFrequency != 0)
        {
            lastVibratoFreq = instruction.vibratoFrequency;
        }

        int? noteInt = lastNonNullInstruction.ASPN2Int();
        oscillator.SetPitch(noteInt);

        float bendAmt = 0;
        if (lastNonNullInstruction.HasBendTarget() && noteInt != null) 
        {
            float pitchDiff = Wave.Freq(lastNonNullInstruction.bendTarget.ASPN2Int()) - Wave.Freq(noteInt);
            if (lastNonNullInstruction.bendTime > 0)
            {
                bendAmt = Mathf.Lerp(pitchDiff, 0, (float)timeSinceLastNote / lastNonNullInstruction.bendTime);
            }
        }

        float vibratoOffset = Mathf.Sin((float)trackDspTime * lastVibratoFreq) * lastVibratoDepth;
        oscillator.SetBend(-bendAmt + vibratoOffset);
    }

    private AD_SO lastAD;
    private void SetADEnvelopeParams()
    {
        double timeSinceLastNote = trackDspTime - lastNoteDspStart;
        var currentNote = instructions[noteIndex];

        if(currentNote.ADEnvelopeID != null)
        {
            lastAD = currentNote.ADEnvelopeID;
        }


        if (lastAD == null) { return; }

        float vol;
        if (timeSinceLastNote < lastAD.timeA)
        {
            vol = Mathf.Lerp(lastAD.levelStart, lastAD.levelA, (float)timeSinceLastNote / lastAD.timeA);
        }
        else
        {
            vol = Mathf.Lerp(lastAD.levelA,
                lastAD.levelD,
                ((float)timeSinceLastNote - lastAD.timeA) / lastAD.timeD);
        }
        if(!float.IsNaN(vol * instructions[noteIndex].velocity))
            oscillator.SetVolume(vol * instructions[noteIndex].velocity);
    }


    [System.Serializable]
    public class LWNoteInstruction
    {
        [SerializeField] public string ASPN;
        [SerializeField] public AD_SO ADEnvelopeID;

        [SerializeField] public float vibratoFrequency;
        [SerializeField] public float vibratoDepth;

        [SerializeField] public string bendTarget;
        [SerializeField] public float bendTime;

        [SerializeField] public float velocity = 1;

        public bool HasNote() => !string.IsNullOrEmpty(ASPN);
        
        public bool HasBendTarget() => !string.IsNullOrEmpty(bendTarget);

        public int? ASPN2Int()
        {
            var input = ASPN;

            if (string.IsNullOrEmpty(input)) return null;

            string[] split = input.Split(',');

            if (split.Length != 2)
            {
                throw new System.Exception("Please format ASPN inputs like so: 'c#,3' ");
            }

            if (int.TryParse(split[1], out int octave) && octave >= 0 && octave <= 10)
            {
                if (conversionTables.TryGetValue(split[0].ToUpper(), out var array))
                {
                    return array[octave];
                }
                else
                {
                    throw new System.Exception($"ASPN value {split[0].ToUpper()} not found");
                }
            }

            return null;
        }

        public static int? ASPN2IntInternal(string input)
        {
            if (string.IsNullOrEmpty(input)) return null;

            string[] split = input.Split(',');

            if (split.Length != 2)
            {
                throw new System.Exception("Please format ASPN inputs like so: 'c#,3' ");
            }

            if (int.TryParse(split[1], out int octave) && octave >= 0 && octave <= 10)
            {
                if (conversionTables.TryGetValue(split[0].ToUpper(), out var array))
                {
                    return array[octave];
                }
                else
                {
                    throw new System.Exception($"ASPN value {split[0].ToUpper()} not found");
                }
            }

            return null;
        }
    }


    [System.Serializable]
    public struct NoteInstruction
    {
        //1-12 at a time
        public int? note;
        public string ASPN;
        public double duration;
        public AD_SO ADEnvelope;

        [SerializeField] public float vibratoFrequency;
        [SerializeField] public float vibratoDepth;

        [SerializeField] public string bendTarget;
        [SerializeField] public float bendTime;

        [HideInInspector] public double dspTimeStart;
        [HideInInspector] public double dspTimeEnd;


        public NoteInstruction SetDspTime(double dspTimeStart)
        {
            this.dspTimeStart = dspTimeStart;
            this.dspTimeEnd = dspTimeStart + duration;
            return this;
        }

        public NoteInstruction TryParseASPN()
        {
            if (string.IsNullOrEmpty(ASPN)) return this;

            this.note = ASPN2Int(ASPN);
            return this;
        }

        public float Pitch()
        {
            return 440 * Mathf.Pow(2, (float)note / 12f);
        }

        public int? ASPN2Int(string input)
        {
            string[] split = input.Split(',');

            if(split.Length != 2)
            {
                throw new System.Exception("Please format ASPN inputs like so: 'C#,3' ");
            }

            if(int.TryParse(split[1], out int octave) && octave >= 0 && octave <= 10)
            {
                if (conversionTables.TryGetValue(split[0].ToUpper(), out var array))
                {
                    return array[octave];
                }
                else
                {
                    throw new System.Exception($"ASPN value {split[0].ToUpper()} not found");
                }
            }

            return null;
        }

        /*
         * 
Octave	 0	 1	 2	 3	 4	5	6	7	8	9	10
    C	-57	-45	-33	-21	-9	3	15	27	39	51	63
    C♯  -56	-44	-32	-20	-8	4	16	28	40	52	64
    D	-55	-43	-31	-19	-7	5	17	29	41	53	65
    D#	-54	-42	-30	-18	-6	6	18	30	42	54	66
    E	-53	-41	-29	-17	-5	7	19	31	43	55	67
    F	-52	-40	-28	-16	-4	8	20	32	44	56	68
    F#	-51	-39	-27	-15	-3	9	21	33	45	57	69
    G	-50	-38	-26	-14	-2	10	22	34	46	58	70
    G#	-49	-37	-25	-13	-1	11	23	35	47	59	71
    A	-48	-36	-24	-12	0	12	24	36	48	60	72
    A#  -47	-35	-23	-11	1	13	25	37	49	61	73
    B	-46	-34	-22	-10	2	14	26	38	50	62	74
         */
    }

    #region old
    /*
    List<NotationTable> tables = new List<NotationTable>()
    {
        new NotationTable("C", new int[] { -57   -45, -33, -21, -9,  3,   15,  27,  39,  51,  63}),

        new NotationTable("C#", new int[] {-56,  -44, -32, -20, -8,  4,   16,  28,  40,  52,  64}),
        new NotationTable("Db", new int[] {-56,  -44, -32, -20, -8,  4,   16,  28,  40,  52,  64}),

        new NotationTable("D", new int[] { -55,  -43, -31, -19, -7,  5,   17,  29,  41,  53,  65}),

        new NotationTable("D#", new int[] {-54,  -42, -30, -18, -6,  6,   18,  30,  42,  54,  66}),
        new NotationTable("Eb", new int[] {-54,  -42, -30, -18, -6,  6,   18,  30,  42,  54,  66}),

        new NotationTable("E", new int[] { -53,  -41, -29, -17, -5,  7,   19,  31,  43,  55,  67}),

        new NotationTable("F", new int[] { -52,  -40, -28, -16, -4,  8,   20,  32,  44,  56,  68}),

        new NotationTable("F#", new int[] {-51,  -39, -27, -15, -3,  9,   21,  33,  45,  57,  69}),
        new NotationTable("Gb", new int[] {-51,  -39, -27, -15, -3,  9,   21,  33,  45,  57,  69}),

        new NotationTable("G", new int[] { -50,  -38, -26, -14, -2,  10,  22,  34,  46,  58,  70}),

        new NotationTable("G#", new int[] {-49,  -37, -25, -13, -1,  11,  23,  35,  47,  59,  71}),
        new NotationTable("Ab", new int[] {-49,  -37, -25, -13, -1,  11,  23,  35,  47,  59,  71}),

        new NotationTable("A", new int[] { -48,  -36, -24, -12, 0,   12,  24,  36,  48,  60,  72}),

        new NotationTable("A#", new int[] {-47,  -35, -23, -11, 1,   13,  25,  37,  49,  61,  73}),
        new NotationTable("Bb", new int[] {-47,  -35, -23, -11, 1,   13,  25,  37,  49,  61,  73}),

        new NotationTable("B", new int[] { -46,  -34, -22, -10, 2,   14,  26,  38,  50,  62,  74})

    };
    */
    #endregion

    // Probably faster this way
    public static readonly Dictionary<string, int[]> conversionTables = new Dictionary<string, int[]>()
    {
        { "C", new int[] { -57   -45, -33, -21, -9,  3,   15,  27,  39,  51,  63}},
        { "C#", new int[] {-56,  -44, -32, -20, -8,  4,   16,  28,  40,  52,  64} },
        { "DB", new int[] {-56,  -44, -32, -20, -8,  4,   16,  28,  40,  52,  64} },
        { "D", new int[] { -55,  -43, -31, -19, -7,  5,   17,  29,  41,  53,  65} },
        { "D#", new int[] {-54,  -42, -30, -18, -6,  6,   18,  30,  42,  54,  66} },
        { "EB", new int[] {-54,  -42, -30, -18, -6,  6,   18,  30,  42,  54,  66} },
        { "E", new int[] { -53,  -41, -29, -17, -5,  7,   19,  31,  43,  55,  67} },
        { "F", new int[] { -52,  -40, -28, -16, -4,  8,   20,  32,  44,  56,  68} },
        { "F#", new int[] {-51,  -39, -27, -15, -3,  9,   21,  33,  45,  57,  69} },
        { "GB", new int[] {-51,  -39, -27, -15, -3,  9,   21,  33,  45,  57,  69} },
        { "G", new int[] { -50,  -38, -26, -14, -2,  10,  22,  34,  46,  58,  70} },
        { "G#", new int[] {-49,  -37, -25, -13, -1,  11,  23,  35,  47,  59,  71} },
        { "AB", new int[] {-49,  -37, -25, -13, -1,  11,  23,  35,  47,  59,  71} },
        { "A", new int[] { -48,  -36, -24, -12, 0,   12,  24,  36,  48,  60,  72} },
        { "A#", new int[] {-47,  -35, -23, -11, 1,   13,  25,  37,  49,  61,  73} },
        { "BB", new int[] {-47,  -35, -23, -11, 1,   13,  25,  37,  49,  61,  73} },
        { "B", new int[] { -46,  -34, -22, -10, 2,   14,  26,  38,  50,  62,  74} }

    };

    //unused after implementing dictionary
    struct NotationTable
    {
        int[] numericValues;
        string qualifiedASPN;

        public NotationTable(string qualifiedASPN, int[] numericValues)
        {
            this.qualifiedASPN = qualifiedASPN;
            this.numericValues = numericValues;
        }
    }

    public enum ArgType { bend, vibratoDepth, vibratoWidth}
}

[System.Serializable] public enum ENoteDuration { sixteenth, eighth, quarter, half, whole }

public static class Extensions
{
    public static int? ASPN2Int(this string str)
    {
        return NoteRoll.LWNoteInstruction.ASPN2IntInternal(str);
    }
}