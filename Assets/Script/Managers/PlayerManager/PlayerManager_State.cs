using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public partial class PlayerManager
{
    public Action<int> _onCB_AlertBoss;
    public void Command_AlertBoss(int _bossIndex)
    {
        _onCB_AlertBoss?.Invoke(_bossIndex);
    }

}