using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class StageTrace
{
    static string trace;
    static int nb;
    static string dir = "D:\\Unity\\_trace";
    static public void Trace(string trace)
    {
#if UNITY_EDITOR
        StageTrace.trace += "\n" + trace;

        System.IO.File.WriteAllText(dir + "\\trace_" + nb + ".txt", StageTrace.trace);
#endif
    }
    static public void CreateTrace()
    {

#if UNITY_EDITOR
        StageTrace.trace += "---------- START TRACE : " + Time.time + " ----------";
        nb = DirCount(new DirectoryInfo(dir));
#endif
    }
    static public int DirCount(DirectoryInfo d)
    {

#if UNITY_EDITOR
        int i = 0;
        // Add file sizes.
        FileInfo[] fis = d.GetFiles();
        foreach (FileInfo fi in fis)
        {
            if (fi.Name.Contains("trace") && fi.Extension.Contains("txt"))
                i++;
        }
        return i;
#else
        return 0;
#endif
    }
}
