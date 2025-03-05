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
        else if (_jobTID == 1) _chrName += $"���";
        else if (_jobTID == 2) _chrName += $"�ü�";
        else if (_jobTID == 3) _chrName += $"������";
        else if (_jobTID == 4) _chrName += $"N��° ĳ����";
        else _chrName += $"N��° ĳ����";
    }



}