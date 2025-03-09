using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public partial class PlayerManager
{
    public Action<int> _onCB_ChangeEnemyCount;

    private int _mi_EnemyCount; 

    public void AddEnemyCount(int _gold)
    {
        _mi_EnemyCount += _gold;
        _onCB_ChangeEnemyCount?.Invoke(_mi_EnemyCount);
    }
    public int GetEnemyCount() => _mi_EnemyCount;
}