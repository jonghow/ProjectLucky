using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using UnityEngine.AddressableAssets;
using System.IO;
using GlobalGameDataSpace;
using System.Text;
using Cysharp.Threading.Tasks;
using System.Threading;

public class AnimationManager
{


    /// <summary>
    /// Animation Clip 들을 가지고 있는 Manager 입니다.
    /// </summary>

    private static AnimationManager Instance;

    public static AnimationManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new AnimationManager();
        }

        return Instance;
    }

    public void Initialize()
    {
        _m_Dict_CacheCharacterAnimationClip = new Dictionary<int, Dictionary<string, AnimationClip>>();
        _m_Dict_CacheMonsterAnimationClip = new Dictionary<int, Dictionary<string, AnimationClip>>();
        _m_Dict_CacheMealFactoryAnimationClip = new Dictionary<int, Dictionary<string, AnimationClip>>();
    }

    public AnimationManager()
    {
        ClearUnitaskToken();
        _ = InitLoadDatas();
    }

    CancellationTokenSource _cancellationToken;
    bool _isLoaded;
    private void ClearUnitaskToken()
    {
        if (_cancellationToken != null)
        {
            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
            _cancellationToken = null;
        }

        _cancellationToken = new CancellationTokenSource();
    }

    /// <summary>
    /// 게임 애니메이션에 관련한 데이터를 로드합니다.
    /// </summary>
    private async UniTask InitLoadDatas()
    {
        Initialize();

        await PreLoadCharcterAnimation();
        // Character

        await PreLoadMonsterAnimation();
        // Monster 

        await PreLoadMealFactoryAnimation();
        // MealFactory

        _isLoaded = true;
    }

    public async UniTask PreLoadCharcterAnimation()
    {
        StringBuilder _sb = new StringBuilder();

        int _index = 0;
        bool _isLoaded = false;

        for (int i = (int)CHARACTER_ACT_DATA.RiceCakeMercenary; i < (int)CHARACTER_ACT_DATA.MAX; ++i)
        {
            if (!( i == 1 || i == 2 || i == 14 || i == 3 || i == 17 || i == 8 || i == 9 || i == 23)) continue;

            if (!_m_Dict_CacheCharacterAnimationClip.ContainsKey(i))
                _m_Dict_CacheCharacterAnimationClip.Add(i, new Dictionary<string, AnimationClip>());

            while (_index < CHARACTER_ANIMATIONS.Length)
            {
                _isLoaded = false;
                _sb.Clear();
                _sb.Append($"Character{String.Format("{0:00}", i)}_{CHARACTER_ANIMATIONS[_index].ToUpper()}");
                string _animationKey = _sb.ToString();

                Addressables.LoadAssetAsync<UnityEngine.Object>(_animationKey).Completed += (op) =>
                {
                    if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
                    {
                        AnimationClip clip = op.Result as AnimationClip;
                        if (clip != null)
                        {
                            lock (_m_Dict_CacheCharacterAnimationClip)
                            {
                                if (!_m_Dict_CacheCharacterAnimationClip[i].ContainsKey(_animationKey))
                                {
                                    _m_Dict_CacheCharacterAnimationClip[i].Add(_animationKey, clip);
                                }
                            }
                        }
                        else
                        {
                            Debug.LogError($"로드된 애니메이션 클립이 null입니다: {_animationKey}");
                        }

                        _isLoaded = true;
                        Addressables.Release(op);
                    }
                };
                await UniTask.WaitUntil(() => _isLoaded == true);
                ++_index;
            }
        }
    }


    public async UniTask PreLoadMonsterAnimation()
    {
        StringBuilder _sb = new StringBuilder();

        int _index = 0;
        bool _isLoaded = false;

        for (int i = (int)ENEMY_ACT_DATA.SpoiledSlime; i < (int)ENEMY_ACT_DATA.MAX; ++i)
        {
            if (!_m_Dict_CacheMonsterAnimationClip.ContainsKey(i))
                _m_Dict_CacheMonsterAnimationClip.Add(i, new Dictionary<string, AnimationClip>());

            while (_index < MONSTER_ANIMATIONS.Length)
            {
                _isLoaded = false;
                _sb.Clear();
                _sb.Append($"Monster{String.Format("{0:00}", i)}_{MONSTER_ANIMATIONS[_index].ToUpper()}");
                string _animationKey = _sb.ToString();

                Addressables.LoadAssetAsync<UnityEngine.Object>(_sb.ToString()).Completed += (op) =>
                {
                    AnimationClip clip = op.Result as AnimationClip;
                    if (clip != null)
                    {
                        lock (_m_Dict_CacheMonsterAnimationClip)
                        {
                            if (!_m_Dict_CacheMonsterAnimationClip[i].ContainsKey(_animationKey))
                            {
                                _m_Dict_CacheMonsterAnimationClip[i].Add(_animationKey, clip);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"로드된 애니메이션 클립이 null입니다: {_animationKey}");
                    }

                    _isLoaded = true;
                    Addressables.Release(op);
                };

                await UniTask.WaitUntil(() => _isLoaded == true);
                ++_index;
            }
        }
    }
    public async UniTask PreLoadMealFactoryAnimation()
    {
        StringBuilder _sb = new StringBuilder();

        int _index = 0;
        bool _isLoaded = false;

        for (int i = (int)MEALFACTORY_ACT_DATA.CookMealFactory; i < (int)MEALFACTORY_ACT_DATA.MAX; ++i)
        {
            if (!_m_Dict_CacheMealFactoryAnimationClip.ContainsKey(i))
                _m_Dict_CacheMealFactoryAnimationClip.Add(i, new Dictionary<string, AnimationClip>());

            while (_index < MEALFACTORY_ANIMATIONS.Length)
            {
                _isLoaded = false;

                _sb.Clear();
                _sb.Append($"MealFactory{String.Format("{0:00}", i)}_{MEALFACTORY_ANIMATIONS[_index].ToUpper()}");
                string _animationKey = _sb.ToString();

                Addressables.LoadAssetAsync<UnityEngine.Object>(_sb.ToString()).Completed += (op) =>
                {
                    AnimationClip clip = op.Result as AnimationClip;
                    if (clip != null)
                    {
                        lock (_m_Dict_CacheMealFactoryAnimationClip)
                        {
                            if (!_m_Dict_CacheMealFactoryAnimationClip[i].ContainsKey(_animationKey))
                            {
                                _m_Dict_CacheMealFactoryAnimationClip[i].Add(_animationKey, clip);
                            }
                        }
                    }
                    else
                    {
                        Debug.LogError($"로드된 애니메이션 클립이 null입니다: {_animationKey}");
                    }

                    _isLoaded = true;
                    Addressables.Release(op);
                };

                await UniTask.WaitUntil(() => _isLoaded == true);
                ++_index;
            }
        }
    }

    public static readonly string[] CHARACTER_ANIMATIONS = new string[5]
    {
        $"IDLE",$"MOVE",$"ATTACK_L",$"ATTACK_R",$"DEAD"
    };

    public static readonly string[] MONSTER_ANIMATIONS = new string[5]
    {
        $"IDLE",$"MOVE",$"ATTACK_L",$"ATTACK_R",$"DEAD"
    };

    public static readonly string[] MEALFACTORY_ANIMATIONS = new string[5]
    {
        $"Idle",$"Cooking",$"OverHeating",$"Complete",$"Destroy"
    };

    private Dictionary<int, Dictionary<string, AnimationClip>> _m_Dict_CacheCharacterAnimationClip;
    private Dictionary<int, Dictionary<string, AnimationClip>> _m_Dict_CacheMonsterAnimationClip;
    private Dictionary<int, Dictionary<string, AnimationClip>> _m_Dict_CacheMealFactoryAnimationClip;

    public void GetAnimationClip(AnimationCategory _eCategory, int _entityTID, string _motionName, Action<UnityEngine.Object> _callback)
    {
        Dictionary<string, AnimationClip> _dict_FindTarget = null;
        switch (_eCategory)
        {
            case AnimationCategory.Character:
            case AnimationCategory.Rival:
                _eCategory = AnimationCategory.Character;
                _m_Dict_CacheCharacterAnimationClip.TryGetValue(_entityTID, out _dict_FindTarget);
                break;
            case AnimationCategory.Monster:
                _m_Dict_CacheMonsterAnimationClip.TryGetValue(_entityTID, out _dict_FindTarget);
                break;
            case AnimationCategory.Factory:
                _m_Dict_CacheMealFactoryAnimationClip.TryGetValue(_entityTID, out _dict_FindTarget);
                break;
            default:
                break;
        }

        if (_dict_FindTarget.TryGetValue(_motionName, out var _retClip))
        {
            _callback?.Invoke(_retClip);
            return;
        }

        StringBuilder _sb = new StringBuilder();
        _sb.Append(_eCategory.ToString());
        _sb.Append($"{String.Format("{0:00}", _entityTID)}_");
        _sb.Append(_motionName);

        string _animationKey = _sb.ToString();

        // 만약에 없다면 로드를 해주어야 한다.
        Addressables.LoadAssetAsync<UnityEngine.Object>(_animationKey).Completed += (op) =>
        {
            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                var _loadedObject = op.Result;
                _callback?.Invoke(_loadedObject);

                var _animationClip = _loadedObject as AnimationClip;

                if (!_dict_FindTarget.ContainsKey(_motionName))
                    _dict_FindTarget.Add(_motionName, _animationClip);
            }
        };
    }
}
