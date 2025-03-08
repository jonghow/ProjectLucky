using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.Xml.Serialization;
using UnityEditor;
using System.Linq;
using GlobalGameDataSpace;

public class ChrActXMLInfo
{
    List<CharacterActionDamage> _Lt_ActDamages;// 데미지 트리거
    List<CharacterActionEffect> _Lt_ActEffects; // 이펙트 트리거
    List<CharacterActionSound> _Lt_ActSounds; // 사운드 트리거

    private string _mStr_ActionName;
    private string _mStr_FileName;
    public bool _mb_IsLoop;

    public ChrActXMLInfo(XmlNode _node)
    {
        InitializeInfo();

        foreach(XmlNode _innerNode in _node.ChildNodes)
        {
            switch(_innerNode.Name)
            {
                case "DamageObject":
                    CharacterActionDamage _actDamage = new CharacterActionDamage(_innerNode);
                    _Lt_ActDamages.Add(_actDamage);
                    break;
                case "EffectObject":
                    CharacterActionEffect _actEffect = new CharacterActionEffect(_innerNode);
                    _Lt_ActEffects.Add(_actEffect);
                    break;
                case "SoundObject":
                    CharacterActionSound _actSound = new CharacterActionSound(_innerNode);
                    _Lt_ActSounds.Add(_actSound);
                    break;
                case "DamageObject_Projectile":
                    CharacterActionDamage _actDamage_ProjectileArrow = new CharacterActionDamage(_innerNode);
                    _Lt_ActDamages.Add(_actDamage_ProjectileArrow);
                    break;
                case "DamageObject_Fireball":
                    CharacterActionDamage _actDamage_ProjectileFireball = new CharacterActionDamage(_innerNode);
                    _Lt_ActDamages.Add(_actDamage_ProjectileFireball);
                    break;
            }
        }

        SortLists();
    }

    private void InitializeInfo()
    {
        if(_Lt_ActDamages == null) _Lt_ActDamages = new List<CharacterActionDamage>();
        if(_Lt_ActEffects == null) _Lt_ActEffects = new List<CharacterActionEffect>();
        if(_Lt_ActSounds == null) _Lt_ActSounds = new List<CharacterActionSound>();
    }

    private void SortLists()
    {
        // Damage 기준으로 정렬
        SortList(_Lt_ActDamages, (item1, item2) => item1.Frame.CompareTo(item2.Frame));
        SortList(_Lt_ActEffects, (item1, item2) => item1.Frame.CompareTo(item2.Frame));
        SortList(_Lt_ActSounds, (item1, item2) => item1.Frame.CompareTo(item2.Frame));
    }
    private void SortList<T>(List<T> list, Comparison<T> comparison)
    {
        list.Sort(comparison);
    }

    public List<CharacterActionBase> GetActXMLInfoByFrame(int _frame)
    {
        List<CharacterActionBase> _ret = new List<CharacterActionBase>();

        _ret.AddRange(GetActXMLInfoByFrame(_Lt_ActDamages, rhs => rhs.Frame == _frame));
        _ret.AddRange(GetActXMLInfoByFrame(_Lt_ActEffects, rhs => rhs.Frame == _frame));
        _ret.AddRange(GetActXMLInfoByFrame(_Lt_ActSounds, rhs => rhs.Frame == _frame));

        return _ret;
    }

    private List<T> GetActXMLInfoByFrame<T>(List<T> _list ,Predicate<T> _pred)
    {
        List<T> _ret = _list.FindAll(_pred);
        return _ret;
    }
}

public class GameDB_BaseData { }
public class GameDB_CharacterInfo : GameDB_BaseData
{
    public int _mi_CharacterID;
    public int _mi_CharacterName;
    public int _mi_CharacterStatSet;
    public int _mi_CharacterSkillSet;
    public int _mi_Freshness;
    public int _mi_Dia;
    public EntityGrade _me_Grade;

    public string CharacterName => $""; // 나중에 추가할 것

    public GameDB_CharacterInfo(XmlNode _node)
    {
        int.TryParse(_node["ID"].InnerText.ToString(), out _mi_CharacterID);
        int.TryParse(_node["Name"].InnerText.ToString(), out _mi_CharacterName);
        int.TryParse(_node["StatSet"].InnerText.ToString(), out _mi_CharacterStatSet);
        int.TryParse(_node["Freshness"].InnerText.ToString(), out _mi_Freshness);

        string _grade = _node["Grade"].InnerText.ToString();
        _me_Grade = (EntityGrade)Enum.Parse(typeof(EntityGrade), _grade);

        int.TryParse(_node["Dia"].InnerText.ToString(), out _mi_Freshness);
    }
}
public class GameDB_CharacterStat
{
    public int _mi_StatID;

    public int _mi_StatStr;
    public int _mi_StatDex;
    public int _mi_StatWis;
    public int _mi_StatGuts;
    public int _mi_StatMen;
    public int _mi_StatMaxHp;
    public float _mf_AttackRange;
    public GameDB_CharacterStat(XmlNode _node)
    {
        int.TryParse(_node["ID"].InnerText.ToString(), out _mi_StatID);
        int.TryParse(_node["Str"].InnerText.ToString(), out _mi_StatStr);
        int.TryParse(_node["Dex"].InnerText.ToString(), out _mi_StatDex);
        int.TryParse(_node["Wis"].InnerText.ToString(), out _mi_StatWis);
        int.TryParse(_node["Guts"].InnerText.ToString(), out _mi_StatGuts);
        int.TryParse(_node["Men"].InnerText.ToString(), out _mi_StatMen);
        int.TryParse(_node["MaxHP"].InnerText.ToString(), out _mi_StatMaxHp);
        float.TryParse(_node["AttackRange"].InnerText.ToString(), out _mf_AttackRange);
    }
}
public class GameDB_StringCommon: GameDB_BaseData
{
    public int _mi_ID;
    public string _mStr_Common;

    public GameDB_StringCommon(XmlNode _node)
    {
        int.TryParse(_node["ID"].InnerText.ToString(), out _mi_ID);
        ConvertCountry(_node);
    }

    public void ConvertCountry(XmlNode _node)
    {
        switch(Application.systemLanguage)
        {
            case SystemLanguage.English:
                _mStr_Common = _node["EN"].InnerText;
                break;
            case SystemLanguage.Korean:
                _mStr_Common = _node["KR"].InnerText;
                break;
            case SystemLanguage.Japanese:
                _mStr_Common = _node["JP"].InnerText;
                break;
            case SystemLanguage.Chinese:
                _mStr_Common = _node["CN"].InnerText;
                break;
            default:
                UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"ConvertCountry", $"지원하지 않는 언어 입니다.");
                EditorApplication.isPlaying = false;
                break;
        }
    }
}
public class GameDB_MealRecipe : GameDB_BaseData
{
    public int _mi_ID;
    public int _mi_MealKitID;
    public int _mi_TechTree;
    public string _mStr_Recipe;

    public int[] Arr_Recipe
    {
        get
        {
            int[] _ret = null;

            if(_mStr_Recipe.Contains('|'))
                _ret = _mStr_Recipe.Split('|').Select(int.Parse).ToArray();
            else
                _ret = new int[1] { int.Parse(_mStr_Recipe) };

            return _ret;
        }
    }

    public GameDB_MealRecipe(XmlNode _node)
    {
        int.TryParse(_node["ID"].InnerText.ToString(), out _mi_ID);
        int.TryParse(_node["MealKitID"].InnerText.ToString(), out _mi_MealKitID);
        int.TryParse(_node["TechTree"].InnerText.ToString(), out _mi_TechTree);

        _mStr_Recipe = _node["RecipeList"].InnerText.ToString();
    }
}
public class GameDB_MealKitInfo : GameDB_BaseData
{
    public int _mi_ID;
    public int _mi_NameID;
    public float _mf_CookTime;
    public float _mf_HeatingTime;
    public int _mi_CreateCount;
    public int _mi_CreateLimit;
    public int _mi_CreateEntityID;

    public GameDB_MealKitInfo(XmlNode _node)
    {
        int.TryParse(_node["ID"].InnerText.ToString(), out _mi_ID);
        int.TryParse(_node["NameID"].InnerText.ToString(), out _mi_NameID);
        float.TryParse(_node["CookTime"].InnerText.ToString(), out _mf_CookTime);
        float.TryParse(_node["OverHeatingTime"].InnerText.ToString(), out _mf_HeatingTime);
        int.TryParse(_node["EntityCreateCount"].InnerText.ToString(), out _mi_CreateCount);
        int.TryParse(_node["EntityCreateLimit"].InnerText.ToString(), out _mi_CreateLimit);
        int.TryParse(_node["CreateEntityID"].InnerText.ToString(), out _mi_CreateEntityID);
    }
}
public class GameDB_BuildingInfo : GameDB_BaseData
{
    public int _mi_ID;
    public string _mStr_BuildGrid;
    public int _mi_StatSet;

    public GridDirectionGroup[] _meAr_BuildGrid;

    public GameDB_BuildingInfo(XmlNode _node)
    {
        int.TryParse(_node["ID"].InnerText.ToString(), out _mi_ID);
        int.TryParse(_node["StatSet"].InnerText.ToString(), out _mi_StatSet);
        ParseGrid(_node);
    }

    public void ParseGrid(XmlNode _node)
    {
        _mStr_BuildGrid = _node["BuildGrid"].InnerText.ToString();

        if (_mStr_BuildGrid.Contains('|'))
        {
            _meAr_BuildGrid = _mStr_BuildGrid.Split('|').Select(x => (GridDirectionGroup)Enum.Parse(typeof(GridDirectionGroup), x)).ToArray();
        }
        else
        {
            _meAr_BuildGrid = new GridDirectionGroup[1] { (GridDirectionGroup)Enum.Parse(typeof(GridDirectionGroup), _mStr_BuildGrid) };
        }
    }
}

public class GameDB_BuildingStat : GameDB_BaseData
{
    public int _mi_StatID;

    public int _mi_StatStr;
    public int _mi_StatDex;
    public int _mi_StatWis;
    public int _mi_StatGuts;
    public int _mi_StatMen;
    public int _mi_StatMaxHp;
    public float _mf_AttackRange;
    public GameDB_BuildingStat(XmlNode _node)
    {
        int.TryParse(_node["ID"].InnerText.ToString(), out _mi_StatID);
        int.TryParse(_node["Str"].InnerText.ToString(), out _mi_StatStr);
        int.TryParse(_node["Dex"].InnerText.ToString(), out _mi_StatDex);
        int.TryParse(_node["Wis"].InnerText.ToString(), out _mi_StatWis);
        int.TryParse(_node["Guts"].InnerText.ToString(), out _mi_StatGuts);
        int.TryParse(_node["Men"].InnerText.ToString(), out _mi_StatMen);
        int.TryParse(_node["MaxHP"].InnerText.ToString(), out _mi_StatMaxHp);
        float.TryParse(_node["AttackRange"].InnerText.ToString(), out _mf_AttackRange);
    }
}

public class GameDB_DrawHandCardInfo : GameDB_BaseData
{
    public int _mi_ID;
    private int _mi_CardNameID;
    private int _mi_CardType;
    public float _mf_CardValue;
    public HandCardType UpgradeCardType => (HandCardType)_mi_CardType;
    public float UpgradeCardValue => _mf_CardValue;
    public string CardName => GameDataManager.GetInstance().GetCommonString(_mi_CardNameID);

    public GameDB_DrawHandCardInfo(XmlNode _node)
    {
        int.TryParse(_node["ID"].InnerText.ToString(), out _mi_ID);
        int.TryParse(_node["CardNameID"].InnerText.ToString(), out _mi_CardNameID);
        int.TryParse(_node["CardType"].InnerText.ToString(), out _mi_CardType);
        float.TryParse(_node["CardValue"].InnerText.ToString(), out _mf_CardValue);
    }
}



