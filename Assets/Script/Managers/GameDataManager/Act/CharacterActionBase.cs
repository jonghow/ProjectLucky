using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CharacterActionBase
{
    protected int _mi_ID;
    protected int _mi_Frame;
    protected string _mStr_ActType;

    public int ID { get { return _mi_ID; } }
    public int Frame { get { return _mi_Frame; } }
    public string ActType { get { return _mStr_ActType;} }
}