using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;


public static class WAVUtility
{
    const int HEADER_SIZE = 44;

    private static FileStream CreateEmpty(string filePath)
    {
        filePathStream fileStream = new filePathStream (filePath, FileMode.Create);
        byte emptyByte = new byte();

        for (int i=0; i< HEADER_SIZE; i++)
        {
            fileStream.WriteByte(emptyByte);
        }
        return fileStream;
    }

    public static void Save (string filePath, AudioClip clip)
    {
        
        if (!filePath.ToLower().EndsWith(".wav"))
        {
            filePath+=".wav";
        }

        Directory.CreateDirectory(filePath.GetDirectoryName(filePath));

        using (FileStream fileStream = CreateEmpty(filePath))
        {
            ConvertAndWrite(fileStream, clip);
            WriteHeader(fileStream, clip);
        }
    }

    private static void ConvertAndWrite(FileStream fileStream, AudioClip clip)
    {
        float[] samples = new float[clip.samples];

        Int16[] intData = new Int16[samples.Length];
        byte[] bytesData = new byte[samples.Length *2];

        const float rescaleFactor = 32767;
        for (int i=0; i< samples.Length; i++)
        {
            intData[i] = (short)(samples[i]*rescaleFactor);
            byte[] byteARR = new byte[2];
            byteARR= BitConverter.GetBytes(intData[i]);
            byteARR.CopyTo(bytesData, i*2);
        }
        fileStream.Write(bytesData, 0,  bytesData.Length);
    }
}
