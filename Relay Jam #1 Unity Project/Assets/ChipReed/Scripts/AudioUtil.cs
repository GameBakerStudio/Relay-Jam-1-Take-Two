using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using System.Diagnostics;

public class AudioUtil
{
    private static string HelperPath = $"{Application.dataPath}/ChipReed/Editor/BeepHelper/ChipReedBeepHelper.exe";
    public static void PlayBeepProcess(int pitch, int duration)
    {

#if UNITY_EDITOR_WIN

        if (!ChipReedSettings._previewBeeps) return;
        ProcessStartInfo startInfo = new ProcessStartInfo(HelperPath);
        startInfo.Arguments = $"{pitch} {duration}";
        System.Diagnostics.Process.Start(startInfo);

#endif

    }
}
