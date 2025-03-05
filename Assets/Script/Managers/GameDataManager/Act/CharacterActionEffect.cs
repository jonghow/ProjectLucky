using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using DTR_Extension;
using GlobalGameDataSpace;

public class CharacterActionEffect : CharacterActionBase
{
    public string _mStr_EffectFileName;
    public Vector3 _mv3_Position;
    public Vector3 _mv3_Rotation;
    public Vector3 _mv3_Scale;
    public ActEffectMoveType _me_EffectMoveType;

    public CharacterActionEffect(XmlNode _node)
    {
        _mStr_ActType = _node.Name;

        _mi_ID = int.Parse(_node.Attributes["ID"].Value);
        _mi_Frame = int.Parse(_node.Attributes["Frame"].Value);

        _mStr_EffectFileName = _node.Attributes["EffectFileName"].Value;

        _mv3_Position = _node.Attributes["Position"].Value.ToVector3(new char[2] { '(',')'}, ',');
        _mv3_Rotation = _node.Attributes["Rotation"].Value.ToVector3(new char[2] { '(',')'}, ',');
        _mv3_Scale = _node.Attributes["Scale"].Value.ToVector3(new char[2] { '(',')'}, ',');
    }
}