using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SceneLogic
{
    private static string GetMusicPath()
    {
        return Application.streamingAssetsPath + "/levels/"+ PlayerPrefs.GetInt("Level");
    }

    public static AudioClip GetAudioFile()
    {
        WWW w = new WWW(GetMusicPath() + ".mp3");
        return NAudioPlayer.FromMp3Data(w.bytes);
    }

    
    public static void SaveLevelToFile<T>(T toSave)
    {
        if (!File.Exists(GetMusicPath() + ".bytes"))
        {
            File.Create(GetMusicPath() + ".bytes");
        }

        var json = JsonUtility.ToJson(toSave);
        File.WriteAllText(GetMusicPath()  + ".bytes", json);
    }

    public static void LoadDataFromFile<T>(T toLoad)
    {
        if (!File.Exists(GetMusicPath()  + ".bytes"))
            return;
        

        var json = File.ReadAllText(GetMusicPath()  + ".bytes");
        JsonUtility.FromJsonOverwrite(json, toLoad);
    }
}
