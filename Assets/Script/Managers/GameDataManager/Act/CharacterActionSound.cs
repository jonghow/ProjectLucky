using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using DTR_Extension;

public class CharacterActionSound : CharacterActionBase
{
    public string _mStr_SoundFileName;
    public string _mStr_VoiceLanguage;

    public bool _mb_IsVoice;

    public CharacterActionSound(XmlNode _node)
    {
        _mStr_ActType = _node.Name;

        _mi_ID = int.Parse(_node.Attributes["ID"].Value);
        _mi_Frame = int.Parse(_node.Attributes["Frame"].Value);

        _mStr_SoundFileName = _node.Attributes["SoundFileName"].Value;

        _mStr_VoiceLanguage = _node.Attributes["Langauge"].Value;

        string ss = _node.Attributes["IsVoice"].Value;
        //_mb_IsVoice = bool.Parse(_node.Attributes["IsVoice"].Value);
    }
}