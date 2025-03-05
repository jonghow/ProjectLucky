using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathManager
{
    public static PathManager Instance;

    public static PathManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new PathManager();
        }

        return Instance;
    }
    public void GetGameDataPath(out string _ret)
    {
        _ret = Application.dataPath + "/ResourceData/99.GameData";
    }

    public string assetPath_Prefab = $"../Object/";
}
