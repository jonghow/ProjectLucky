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
    public bool _mb_OnClickStart;
    public bool _mb_StartOnLoadBattleStage;
    public bool _IsPlayInitBGM;

    [SerializeField] Button _mBtn_Start;

    public void Start()
    {
        _mb_OnClickStart = false;
        _mb_StartOnLoadBattleStage = false;
    }

    public void OnEnable()
    {
        _IsPlayInitBGM = false;
    }

    public void OnDisable()
    {
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

        _mBtn_Start.interactable = ManagerContainer.GetInstance()._isLoadingComplete;
    }

    public void OnClick_Start()
    {
        if (_mb_StartOnLoadBattleStage == true)
            return;

        _mb_OnClickStart = true;

        SoundManager.GetInstance().StopBGM($"SoundBGM_Lobby");
        StartLoadBattleStage();
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

        _ = SceneLoadManager.GetInstance().OnLoadScene($"LuckyStageScene", 1001);
    }
}

