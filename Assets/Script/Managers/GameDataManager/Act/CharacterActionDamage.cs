using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CharacterActionDamage : CharacterActionBase
{
    public string _mStr_AttachNode;

    public CharacterActionDamage(XmlNode _node)
    {
        _mStr_ActType = _node.Name;

        _mi_ID = int.Parse(_node.Attributes["ID"].Value);
        _mi_Frame = int.Parse(_node.Attributes["Frame"].Value);
        _mStr_AttachNode = _node.Attributes["AttachNode"].Value;
    }
}