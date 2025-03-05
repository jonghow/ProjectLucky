using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public abstract class InputCommandBase
{
    public abstract void Initialize();
    public abstract void Detect();
}