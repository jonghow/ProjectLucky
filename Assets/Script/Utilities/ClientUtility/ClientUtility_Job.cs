using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class ClientUtility
{
    public static void ConvertJobNameToJobID(int _jobTID, out string _jobName)
    {
        _jobName = $"";

        if (_jobTID == 0) return;
        else if (_jobTID == 1) _jobName += $"SwordMan";
        else if (_jobTID == 2) _jobName += $"Archer";
        else if (_jobTID == 3) _jobName += $"Mage";
        else _jobName += $"None";
    }
    public static void ConvertChrNameToJobID(int _jobTID, out string _chrName)
    {
        _chrName = $"";

        if (_jobTID == 0) return;
        else if (_jobTID == 1) _chrName += $"기사";
        else if (_jobTID == 2) _chrName += $"궁수";
        else if (_jobTID == 3) _chrName += $"마법사";
        else if (_jobTID == 4) _chrName += $"N번째 캐릭터";
        else _chrName += $"N번째 캐릭터";
    }



}