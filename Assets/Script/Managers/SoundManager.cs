using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using UnityEngine.AddressableAssets;
using static Unity.VisualScripting.Member;

public class SoundManager 
{
    public static SoundManager Instance; // 싱글톤 인스턴스

    public static SoundManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new SoundManager();
        }

        return Instance;
    }

    public static bool HasInstance()
    {
        return Instance != null;
    }

    public SoundManager()
    {
        _isLoaded = false;
        _isLoadingComplete = false;

        ClearUnitaskToken();

        _dict_AudioSources = new Dictionary<SoundType, Dictionary<string, List<AudioSource>>>();

        //_ = InitLoadDatas();
     }

    public async UniTask InitLoadDatas()
    {
        await InitLoadSourceDatas();

        _onLoadComplete?.Invoke();
    }

    public void ClearUnitaskToken()
    {
        if (_cancellationToken != null)
        {
            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
            _cancellationToken = null;
        }

        _cancellationToken = new CancellationTokenSource();
    }

    public Action _onLoadComplete;
    CancellationTokenSource _cancellationToken;
    public bool _isLoaded;
    public bool _isLoadingComplete;

    /// <summary>
    /// 게임 사운드에 관련한 데이터를 로드합니다.
    /// </summary>
    private async UniTask InitLoadSourceDatas()
    {
        // BGM _ Lobby
        string _soundKey = $"SoundBGM_Lobby";

        if (!_dict_AudioSources.ContainsKey(SoundType.BGM))
            _dict_AudioSources.Add(SoundType.BGM, new Dictionary<string, List<AudioSource>>());

        _isLoaded = false;

        Addressables.LoadAssetAsync<UnityEngine.Object>(_soundKey).Completed += (op) =>
        {
            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                GameObject _objNewInstance = GameObject.Instantiate(op.Result as GameObject);
                AudioSource _source = _objNewInstance.GetComponent<AudioSource>();

                if (_source != null)
                {
                    lock (_dict_AudioSources)
                    {
                        if (!_dict_AudioSources[SoundType.BGM].ContainsKey(_soundKey))
                        {
                            _dict_AudioSources[SoundType.BGM].Add(_soundKey, new List<AudioSource>());
                        };

                        _dict_AudioSources[SoundType.BGM][_soundKey].Add(_source);
                    }
                }
                else
                {
                    Debug.LogError($"로드된 오디오 클립이 null입니다: {_soundKey}");
                }

                _isLoaded = true;
                Addressables.Release(op);
            }
        };
        await UniTask.WaitUntil(() => _isLoaded == true);

        // BGM  _ Stage
        _soundKey = $"SoundBGM_Stage";
        _isLoaded = false;

        Addressables.LoadAssetAsync<UnityEngine.Object>(_soundKey).Completed += (op) =>
        {
            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                GameObject _objNewInstance = GameObject.Instantiate(op.Result as GameObject);
                AudioSource _source = _objNewInstance.GetComponent<AudioSource>();

                if (_source != null)
                {
                    lock (_dict_AudioSources)
                    {
                        if (!_dict_AudioSources[SoundType.BGM].ContainsKey(_soundKey))
                        {
                            _dict_AudioSources[SoundType.BGM].Add(_soundKey, new List<AudioSource>());
                        };

                        _dict_AudioSources[SoundType.BGM][_soundKey].Add(_source);
                    }
                }
                else
                {
                    Debug.LogError($"로드된 오디오 클립이 null입니다: {_soundKey}");
                }

                _isLoaded = true;
                Addressables.Release(op);
            }
        };
        await UniTask.WaitUntil(() => _isLoaded == true);

        // BGM SFX  _ ArrowShot
        if (!_dict_AudioSources.ContainsKey(SoundType.SFX))
            _dict_AudioSources.Add(SoundType.SFX, new Dictionary<string, List<AudioSource>>());

        _soundKey = $"SoundSFX_ArrowShot";
        _isLoaded = false;

        Addressables.LoadAssetAsync<UnityEngine.Object>(_soundKey).Completed += (op) =>
        {
            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                GameObject _objNewInstance = GameObject.Instantiate(op.Result as GameObject);
                AudioSource _source = _objNewInstance.GetComponent<AudioSource>();

                if (_source != null)
                {
                    lock (_dict_AudioSources)
                    {
                        if (!_dict_AudioSources[SoundType.SFX].ContainsKey(_soundKey))
                        {
                            _dict_AudioSources[SoundType.SFX].Add(_soundKey, new List<AudioSource>());
                        };

                        _dict_AudioSources[SoundType.SFX][_soundKey].Add(_source);
                    }
                }
                else
                {
                    Debug.LogError($"로드된 오디오 클립이 null입니다: {_soundKey}");
                }

                _isLoaded = true;
                Addressables.Release(op);
            }
        };

        await UniTask.WaitUntil(() => _isLoaded == true);

        // BGM SFX  _ Fireball
        _soundKey = $"SoundSFX_Fireball";
        _isLoaded = false;

        Addressables.LoadAssetAsync<UnityEngine.Object>(_soundKey).Completed += (op) =>
        {
            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                GameObject _objNewInstance = GameObject.Instantiate(op.Result as GameObject);
                AudioSource _source = _objNewInstance.GetComponent<AudioSource>();

                if (_source != null)
                {
                    lock (_dict_AudioSources)
                    {
                        if (!_dict_AudioSources[SoundType.SFX].ContainsKey(_soundKey))
                        {
                            _dict_AudioSources[SoundType.SFX].Add(_soundKey, new List<AudioSource>());
                        };
                        _dict_AudioSources[SoundType.SFX][_soundKey].Add(_source);
                    }
                }
                else
                {
                    Debug.LogError($"로드된 오디오 클립이 null입니다: {_soundKey}");
                }

                _isLoaded = true;
                Addressables.Release(op);
            }
        };
        await UniTask.WaitUntil(() => _isLoaded == true);


        // BGM SFX  _ Punch
        _soundKey = $"SoundSFX_Punch";
        _isLoaded = false;

        Addressables.LoadAssetAsync<UnityEngine.Object>(_soundKey).Completed += (op) =>
        {
            if (op.Status == UnityEngine.ResourceManagement.AsyncOperations.AsyncOperationStatus.Succeeded)
            {
                GameObject _objNewInstance = GameObject.Instantiate(op.Result as GameObject);
                AudioSource _source = _objNewInstance.GetComponent<AudioSource>();

                if (_source != null)
                {
                    lock (_dict_AudioSources)
                    {
                        if (!_dict_AudioSources[SoundType.SFX].ContainsKey(_soundKey))
                        {
                            _dict_AudioSources[SoundType.SFX].Add(_soundKey, new List<AudioSource>());
                        };
                        _dict_AudioSources[SoundType.SFX][_soundKey].Add(_source);
                    }
                }
                else
                {
                    Debug.LogError($"로드된 오디오 클립이 null입니다: {_soundKey}");
                }

                _isLoaded = true;
                Addressables.Release(op);
            }
        };
        await UniTask.WaitUntil(() => _isLoaded == true);
    }
    public async UniTask InitCollectObjects(GameObject _parent)
    {
        string _soundKey = $"SoundBGM_Lobby";
        AudioSource _source = _dict_AudioSources[SoundType.BGM][_soundKey][0];
        _source.transform.SetParent(_parent.transform);

        _soundKey = $"SoundBGM_Stage";
        _source = _dict_AudioSources[SoundType.BGM][_soundKey][0];
        _source.transform.SetParent(_parent.transform);

        _soundKey = $"SoundSFX_ArrowShot";
        _source = _dict_AudioSources[SoundType.SFX][_soundKey][0];
        _source.transform.SetParent(_parent.transform);

        lock(_dict_AudioSources)
        {
            for (int i = 0; i < 30; ++i)
            {
                GameObject _newInstance = GameObject.Instantiate(_source.gameObject);
                AudioSource _audioSource = _newInstance.GetComponent<AudioSource>();

                _dict_AudioSources[SoundType.SFX][_soundKey].Add(_audioSource);
                _audioSource.transform.SetParent(_parent.transform);
            }
        }

        await UniTask.Yield(PlayerLoopTiming.Update);

        _soundKey = $"SoundSFX_Fireball";
        _source = _dict_AudioSources[SoundType.SFX][_soundKey][0];
        _source.transform.SetParent(_parent.transform);

        lock (_dict_AudioSources)
        {
            for (int i = 0; i < 30; ++i)
            {
                GameObject _newInstance = GameObject.Instantiate(_source.gameObject);
                AudioSource _audioSource = _newInstance.GetComponent<AudioSource>();

                _dict_AudioSources[SoundType.SFX][_soundKey].Add(_audioSource);
                _audioSource.transform.SetParent(_parent.transform);
            }
        }

        await UniTask.Yield(PlayerLoopTiming.Update);

        _soundKey = $"SoundSFX_Punch";
        _source = _dict_AudioSources[SoundType.SFX][_soundKey][0];
        _source.transform.SetParent(_parent.transform);

        lock (_dict_AudioSources)
        {
            for (int i = 0; i < 30; ++i)
            {
                GameObject _newInstance = GameObject.Instantiate(_source.gameObject);
                AudioSource _audioSource = _newInstance.GetComponent<AudioSource>();

                _dict_AudioSources[SoundType.SFX][_soundKey].Add(_audioSource);
                _audioSource.transform.SetParent(_parent.transform);
            }
        }

        await UniTask.Yield(PlayerLoopTiming.Update);

        _isLoaded = true;
        _isLoadingComplete = true;
    }

    public Dictionary<SoundType, Dictionary<string, List<AudioSource>>> _dict_AudioSources;

    // 배경 음악 재생 메서드
    public void PlayBGM(string _bgmKey)
    {
        if (!_dict_AudioSources.ContainsKey(SoundType.BGM))
            return;

        if (!_dict_AudioSources[SoundType.BGM].ContainsKey(_bgmKey))
            return;

        AudioSource _source = _dict_AudioSources[SoundType.BGM][_bgmKey][0];

        _source.loop = true; // 반복 재생
        _source.volume = 1f; // 볼륨
        _source.Play();
    }

    public void StopBGM(string _bgmKey)
    {
        if (!_dict_AudioSources.ContainsKey(SoundType.BGM))
            return;

        if (!_dict_AudioSources[SoundType.BGM].ContainsKey(_bgmKey))
            return;

        AudioSource _source = _dict_AudioSources[SoundType.BGM][_bgmKey][0];
        _source.Stop();
    }

    public void PlaySFX(string _sfxKey)
    {
        if (!_dict_AudioSources.ContainsKey(SoundType.SFX))
            return;

        if (!_dict_AudioSources[SoundType.SFX].ContainsKey(_sfxKey))
            return;

        AudioSource audioSource = null;

        for (int i = 0; i < _dict_AudioSources[SoundType.SFX][_sfxKey].Count; ++i)
        {
            var Element = _dict_AudioSources[SoundType.SFX][_sfxKey][i];
            if (Element == null) continue;
            if (Element.isPlaying == true) continue;

            audioSource = _dict_AudioSources[SoundType.SFX][_sfxKey][i];
            break;
        }

        audioSource.loop = false; // 반복 재생
        audioSource.volume = 1f; // 볼륨
        audioSource.Play();
    }
}