using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEditor;

public class UITitle : MonoBehaviour
{
    public enum TitleState 
    {
        None,
        Open,
        Open_Ing,
        Open_Ing_Loop,
        Close
    }

    [SerializeField] Animator _animator;

    [SerializeField] TitleState _me_TitleState;

    public bool _mb_OnClickStart;
    public bool _mb_StartOnLoadBattleStage;
    public bool _IsPlayInitBGM;

    [SerializeField] UICredit _m_Credit;

    public void Start()
    {
        _me_TitleState = TitleState.None;
        _mb_OnClickStart = false;
        _mb_StartOnLoadBattleStage = false;

        _m_Credit.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        _IsPlayInitBGM = false;
    }

    public void OnDisable()
    {
        _me_TitleState = TitleState.None;
        _mb_OnClickStart = false;
        _mb_StartOnLoadBattleStage= false;
    }

    private void Update()
    {
        if(_IsPlayInitBGM == false && SoundManager.HasInstance() == true && SoundManager.GetInstance()._isLoadingComplete == true)
        {
            _IsPlayInitBGM = true;
            SoundManager.GetInstance().PlayBGM($"SoundBGM_Lobby");
        }

        CheckAnimation();
    }

    public void CheckAnimation()
    {
        if(_mb_OnClickStart == true)
        {
            _animator.Play($"Close");
            _me_TitleState = TitleState.Close;
        }

        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        float normalizedTime = stateInfo.normalizedTime % 1; // 루프 애니메이션인 경우 % 1로 제한

        switch (_me_TitleState)
        {
            case TitleState.None:
                _animator.Play($"Open");
                _me_TitleState = TitleState.Open;
                break;
            case TitleState.Open:
                // Normalized Time 확인 (0.0 ~ 1.0)
                if (normalizedTime >= 0.99f)
                {
                    _animator.Play($"Ing");
                    _me_TitleState = TitleState.Open_Ing;
                }

                break;
            case TitleState.Open_Ing:
                // Normalized Time 확인 (0.0 ~ 1.0)
                if (normalizedTime >= 0.99f)
                {
                    _animator.Play($"Ing_Loop");
                    _me_TitleState = TitleState.Open_Ing_Loop;
                }
                break;
            case TitleState.Open_Ing_Loop:
                if (normalizedTime >= 0.99f && _mb_OnClickStart == true)
                {
                    _animator.Play($"Close");
                    _me_TitleState = TitleState.Close;
                }
                break;
            case TitleState.Close:
                if (normalizedTime >= 0.99f && !_mb_StartOnLoadBattleStage)
                {
                    StartLoadBattleStage();
                }
                break;
            default:
                break;
        }
    }

    public void OnClick_Start()
    {
        if (_mb_StartOnLoadBattleStage == true)
            return;

        _mb_OnClickStart = true;

        SoundManager.GetInstance().StopBGM($"SoundBGM_Lobby");
    }
    public void OnClick_Credit()
    {
        if (_mb_StartOnLoadBattleStage == true)
            return;

        if (_m_Credit != null)
            _m_Credit.gameObject.SetActive(true);

        //SoundManager.GetInstance().PlaySFX($"");
    }
    public void OnClick_Quit()
    {
        if (_mb_StartOnLoadBattleStage == true)
            return;

        EditorApplication.isPlaying = false;
        Application.Quit();
    }
    public void StartLoadBattleStage()
    {
        _mb_StartOnLoadBattleStage = true;
        UnityLogger.GetInstance().Log($"Scene Load Start");

        _ = SceneLoadManager.GetInstance().OnLoadScene($"StageScene", 1001);
    }
}

