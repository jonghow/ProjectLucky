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
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"Detect", $"UIKeyCommand ���� �ƹ��� Ű �Լ��� �����Ͽ� ������� �ʾҽ��ϴ�. Ȯ�����ּ���.");
            return;
        }
    }
}