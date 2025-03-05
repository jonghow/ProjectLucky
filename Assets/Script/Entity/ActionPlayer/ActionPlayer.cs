using DTR_Extension;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine;
using System.Text;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ActionPlayer : MonoBehaviour 
{
    [SerializeField] long _ml_ownerUID;
    [SerializeField] int _mi_ownerTID;

    [SerializeField] Animator _animator;
    public string _mStr_actClipName;

    Dictionary<string, ActionInfoBase> _mDict_ActInfo;

    ActionInfoBase _m_CurActionInfo; // �׽�Ʈ�� �̰͸� ����� ��
    ActionInfoBase _m_PrevActionInfo;

    Entity _m_CachedOwnerEntity;
    AnimatorOverrideController _m_OverrideContoller;
    private const string MotionA = "MotionA";
    private const string MotionB = "MotionB";

    public void InitDataContainer()
    {
        if (_mDict_ActInfo == null) _mDict_ActInfo = new Dictionary<string, ActionInfoBase>();
    }
    public void SetOwnerUID(long _ownerUID, int _ownerTID)
    {
        InitDataContainer();

        _ml_ownerUID = _ownerUID;
        _mi_ownerTID = _ownerTID;
        EntityManager.GetInstance().GetEntity(_ml_ownerUID, out _m_CachedOwnerEntity);

        LoadAnimOverrideController();
        SettingActInfos();
    }
    public void LoadAnimOverrideController()
    {
        ResourceType _resourceType = ResourceType.PlayerAnimationController;
        int _entityTID = 0;

        switch (_m_CachedOwnerEntity._me_Division)
        {
            case EntityDivision.Player:
                _resourceType = ResourceType.PlayerAnimationController;
                _entityTID = _m_CachedOwnerEntity.Controller._mi_EntityTID;
                break;
            case EntityDivision.Enemy:
                _resourceType = ResourceType.EnemyAnimationController;
                _entityTID = _m_CachedOwnerEntity.Controller._mi_EntityTID;
                break;
            case EntityDivision.MealFactory:
                _resourceType = ResourceType.MealFactoryAnimationController;
                _entityTID = _m_CachedOwnerEntity.Controller._mi_EntityTID;
                return;
            case EntityDivision.Neutrality:
                break;
            case EntityDivision.Deco:
                break;
        }

        ResourceManager.GetInstance().GetResource(_resourceType, _entityTID, true, (overriceController) =>
        {
            _m_OverrideContoller = GameObject.Instantiate(overriceController) as AnimatorOverrideController;
            _animator.runtimeAnimatorController = _m_OverrideContoller;
        });

        //StringBuilder _sb = new StringBuilder();
        //_sb.Append($"ActOc_");
        //string _toConversionActData = string.Empty;

        //switch (_m_CachedOwnerEntity._me_Division)
        //{
        //    case EntityDivision.Player:
        //        var _characterActData = ((CHARACTER_ACT_DATA)_m_CachedOwnerEntity.CharacterID);
        //        _toConversionActData = (_characterActData).ToString().ToLower();
        //        break;
        //    case EntityDivision.Enemy:
        //        var _enemyActData = ((ENEMY_ACT_DATA)_m_CachedOwnerEntity.CharacterID);
        //        _toConversionActData = (_enemyActData.ToString().ToLower());
        //        break;
        //    case EntityDivision.MealFactory:
        //        return;
        //    case EntityDivision.Neutrality:
        //        break;
        //    case EntityDivision.Deco:
        //        break;
        //}

        //_sb.Append($"{_toConversionActData}");

        //string _key = _sb.ToString();

        //Addressables.LoadAssetAsync<AnimatorOverrideController>(_key).Completed += (op) =>
        //{
        //    if (op.Status == AsyncOperationStatus.Succeeded)
        //    {
        //        _m_OverrideContoller = op.Result;
        //        _animator.runtimeAnimatorController = _m_OverrideContoller;
        //    }
        //};
    }
    public void SettingActInfos()
    {
        EntityDivision _eDivision = _m_CachedOwnerEntity._me_Division;

        if (_eDivision > EntityDivision.Enemy)
        {
            UnityLogger.GetInstance().LogFuncFailed(GetType().Name, $"SettingActInfos", $"���� Character , Enemy Ÿ�� �̿ܿ� ��Ʈ������ �ȵǵ��� �մϴ�. �����丮�� ���Ŀ� �ؾ��ؿ�");
            return;
        }
        // ���� �۾��� �ȵǾ �ӽ� ó��

        Dictionary<string,ChrActXMLInfo> _originActInfo = GameDataManager.GetInstance().GetActionInfo(_eDivision, _mi_ownerTID);

        foreach(var _actInfo in _originActInfo)
        {
            string _actName = _actInfo.Key.ToUpper();
            ChrActXMLInfo _chrXmlInfo = _actInfo.Value;
            ActionInfoBase _act = null;

            switch (_actName)
            {
                case "IDLE":
                    _act = new ActionInfoIdle();
                    _act.SetActInfo(_chrXmlInfo, _ml_ownerUID);
                    break;
                case "MOVE":
                    _act = new ActionInfoMove();
                    _act.SetActInfo(_chrXmlInfo, _ml_ownerUID);
                    break;
                case "ATTACK_L":
                    _act = new ActionInfoAttack();
                    _act.SetActInfo(_chrXmlInfo, _ml_ownerUID );
                    break;
                case "ATTACK_R":
                    _act = new ActionInfoAttack();
                    _act.SetActInfo(_chrXmlInfo, _ml_ownerUID);
                    break;
                case "DEAD":
                    _act = new ActionInfoDead();
                    _act.SetActInfo(_chrXmlInfo, _ml_ownerUID);
                    break;
                default:
                    break;
            }

            _mDict_ActInfo.Add(_actName, _act);
        }
    }
    public void PlayAnimation(string _clipName)
    {
        if (_clipName.IsNullOrEmpty()) return;
        if (_animator == null) return;

        _mStr_actClipName = _clipName;
        _animator.Play($"{_clipName}");
    }
    public bool IsPlayingEqualAnimation(string _clipName)
    {
        for (int i = 0; i < _animator.layerCount; i++)  // ��� ���̾� üũ
        {
            AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(i);
            if (clipInfo.Length > 0 && clipInfo[0].clip.name == _clipName)
            {
                return true;  // �ش� ���̾�� �ִϸ��̼��� ���� ���̸� true ��ȯ
            }
        }
        return false;
    }

    public bool IsEndAnimation(string _clipName)
    {
        for (int i = 0; i < _animator.layerCount; i++)  // ��� ���̾� üũ
        {
            AnimatorClipInfo[] clipInfo = _animator.GetCurrentAnimatorClipInfo(i);
            if (clipInfo.Length > 0 && clipInfo[0].clip.name == _clipName)
            {
                AnimatorStateInfo _info = _animator.GetCurrentAnimatorStateInfo(i);
                float normalTime = _info.normalizedTime;
                if (normalTime >= 1f)
                    return true;

                return false;  // �ش� ���̾�� �ִϸ��̼��� ���� ���̸� true ��ȯ
            }
        }
        return false;
    }


    public void PlayAnimationBaseOverride(EntityDivision _eDivision,  int _jobId, string _actName)
    {
        if (_actName.IsNullOrEmpty()) return;
        if (GameDataManager.GetInstance().IsValidActionInfo(_eDivision, _jobId, _actName) == false) return;

        _mStr_actClipName = _actName.ToUpper();

        if(_m_CurActionInfo != null)
        {
            // ���� �ִϸ��̼� ��ɿ� ������ Unitask�� �����Ѵ�.
            _m_CurActionInfo.StopExecute();
        }
        _m_CurActionInfo = _mDict_ActInfo[_mStr_actClipName];

        AnimationManager.GetInstance().GetAnimationClip((AnimationCategory)_eDivision, _jobId, $"{_actName}", (obj) =>
        {
            var _animClip = obj as AnimationClip;

            _m_OverrideContoller[MotionB] = _animClip;
            _animator.runtimeAnimatorController = _m_OverrideContoller;
            
            int _totalFrame = Mathf.FloorToInt(_animClip.frameRate * _animClip.length);

            _m_CurActionInfo.DoExecute(_animator, _totalFrame);

            //_animator.CrossFade(MotionB, 0.1f); // �ִϸ��̼� �ε巴�� ��ȯ
            _animator.Play(MotionB, 2,0f); // �ִϸ��̼� ���� ��ȯ
        });
    }

    public void ClearActionInfos()
    {
        foreach(var pair in _mDict_ActInfo)
        {
            pair.Value.DestoryExecute();
        }
    }
}
