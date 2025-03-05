using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;
using System.Threading;

public class StateBackCommand : InputCommandBase
{
    public override void Initialize()
    {
    }

    public override void Detect()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            InputManager.GetInstance().PopInputState();
            InputManager.GetInstance().PushInputState(InputState.NormalState);
        }
    }
}