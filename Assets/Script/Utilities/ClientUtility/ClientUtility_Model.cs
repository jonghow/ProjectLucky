using GlobalGameDataSpace;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class ClientUtility
{
    //public static void ConvertJobIDToAddressableKey(int _jobTID, out string _modelName)
    //{
    //    _modelName = $"PCModel_";

    //    if (_jobTID == 0) return;
    //    else if (_jobTID == 1) _modelName += $"SwordMan";
    //    else if (_jobTID == 2) _modelName += $"SwordMan";
    //    else if (_jobTID == 3) _modelName += $"SwordMan";
    //    else _modelName += $"SwordMan";
    //}
    public static void ConvertJobIDToAddressableKey(int _jobTID, out string _modelName)
    {
        _modelName = $"Character_";

        if (_jobTID == 0) return;
        else
        {
            _modelName += String.Format("{0:00}", _jobTID);
        }
    }

    //public static void ConvertMobIDToAddressableKey(int _mobTID, out string _modelName)
    //{
    //    _modelName = $"NPCModel_";

    //    if (_mobTID == 0) return;
    //    else if (_mobTID == 1) _modelName += $"WeakGoblin";
    //    else if (_mobTID == 2) _modelName += $"NormalGoblin";
    //    else if (_mobTID == 3) _modelName += $"StrongGoblin";
    //    else _modelName += $"NoNamed";
    //}

    public static void ConvertMobIDToAddressableKey(int _mobTID, out string _modelName)
    {
        _modelName = $"Monster_";

        if (_mobTID == 0) return;
        else
        {
            _modelName += String.Format("{0:00}", _mobTID);
        }
    }
    public static void ConvertProjectileIDToAddressableKey(int _projectileID, out string _modelName)
    {
        _modelName = $"WO_Projectile_";

        if (_projectileID == 0) return;
        else
        {
            _modelName += String.Format("{0:00}", _projectileID);
        };
    }

    public static void ConvertOrderDisplayIDToAddressableKey(int _projectileID, out string _modelName)
    {
        _modelName = $"WO_OrderDisplay_";

        if (_projectileID == 0) return;
        else
        {
            _modelName += String.Format("{0:00}", _projectileID);
        };
    }
    public static void ConvertMealFactoryIDToAddressableKey(int _mobTID, out string _modelName)
    {
        _modelName = $"MealFactory_";

        if (_mobTID == 0) return;
        else
        {
            _modelName += String.Format("{0:00}", _mobTID);
        }
    }

    public static void ConvertSpawnerIDToAddressableKey(int _mobTID, out string _modelName)
    {
        _modelName = $"Spawner_";

        if (_mobTID == 0) return;
        else
        {
            _modelName +=  _mobTID;
        }
    }
    public static void ConvertMealHandCardIDToAddressableKey(int _mobTID, out string _modelName)
    {
        _modelName = $"HandCardSO_";

        if (_mobTID == 0) return;
        else
        {
            _modelName += String.Format("{0:00}", _mobTID);
        }
    }
    public static void ConvertPlayerAnimationControllerToAddressableKey(int _entityTID, out string _modelName)
    {
        _modelName = $"ActOc_";

        if (_entityTID == 0) return;
        else
        {
            _modelName += ((CHARACTER_ACT_DATA)_entityTID).ToString().ToLower();
        }
    }
    public static void ConvertEnemyAnimationControllerToAddressableKey(int _entityTID, out string _modelName)
    {
        _modelName = $"ActOc_";

        if (_entityTID == 0) return;
        else
        {
            _modelName += ((ENEMY_ACT_DATA)_entityTID).ToString().ToLower();
        }
    }
    public static void ConvertMealFactoryAnimationControllerToAddressableKey(int _entityTID, out string _modelName)
    {
        _modelName = $"ActOc_";

        if (_entityTID == 0) return;
        else
        {
            _modelName += ((CHARACTER_ACT_DATA)_entityTID).ToString().ToLower();
        }
    }
    public static void ConvertHUDToAddressableKey(int _stageID, out string _modelName)
    {
        _modelName = $"UI_";

        if (_stageID == 1001)
        {
            _modelName += "BattleHUD";
        }
        else
        {
        }
    }
    public static void ConvertUIWOToAddressableKey(int _uiID, out string _modelName)
    {
        _modelName = $"WO_UI_";
        _modelName += String.Format("{0:00}", _uiID);
    }

    public static void ConvertAtlasKeyToAddressableKey(int _uiID, out string _modelName)
    {
        _modelName = $"Atlas_";
        _modelName += String.Format("{0:00}", _uiID);
    }
}