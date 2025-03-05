using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;
using System.Threading;

public class UIKeyCommand : InputCommandBase
{
    public override void Initialize()
    {
    }

    public override void Detect()
    {
        if(Input.anyKeyDown)
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"Detect", $"UIKeyCommand 에서 아무런 키 함수를 정의하여 사용하지 않았습니다. 확인해주세요.");
            return;
        }
    }
}