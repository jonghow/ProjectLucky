using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public partial class PlayerManager
{
    public static PlayerManager Instance;
    public static PlayerManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new PlayerManager();
        }

        return Instance;
    }

    public PlayerManager()
    {
        _mi_Gold = 100;
    }
}