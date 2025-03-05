using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

/// <summary>
/// 인풋 매니저 시스템 입니다. 
/// 
/// </summary>
public class InputManager : MonoBehaviour
{
    public static InputManager Instance;
    public static InputManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("InputManager");
            Instance = obj.AddComponent<InputManager>();
            Instance.InitCommand();
        }

        return Instance;
    }
    public void Awake()
    {
        InitCommand();
        DontDestroyOnLoad(this);
    }
    public void InitCommand()
    {
        if (_mDict_InputInfoCommand != null) return;

        _mDict_InputInfoCommand = new Dictionary<InputState, List<InputCommandBase>>();
        _mSt_NowUseCommand = new Stack<KeyValuePair<InputState,List<InputCommandBase>>>();

        for (int i = 0; i < (int)InputState.StateMax; ++i)
        {
            if(!_mDict_InputInfoCommand.ContainsKey((InputState)i))
            {
                _mDict_InputInfoCommand.Add((InputState)i, new List<InputCommandBase>());
            }

            SettingByState((InputState)i);
        }
    }
    public void SettingByState(InputState _eInputState)
    {
        List<InputCommandBase> _Lt_Cmd = new List<InputCommandBase>();
        InputCommandBase _cmdBase = null;

        switch (_eInputState)
        {
            case InputState.NormalState:

                _cmdBase = new EntitySelectCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);
                // Entity Select

                _cmdBase = new CameraMoveCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);
                // Camera Move

                break;
            case InputState.StructureBuildState:

                _cmdBase = new CameraMoveCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);
                // Camera Move

                _cmdBase = new StructureBuildCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);
                // Structure Build

                _cmdBase = new StateBackCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                break;
            case InputState.SelectEntityState:

                _cmdBase = new EntitySelectCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);
                // Entity Drag

                _cmdBase = new GroundClickCheckCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                _cmdBase = new CameraMoveCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                _cmdBase = new EntityMoveOrderCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                _cmdBase = new StateBackCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                break;
            case InputState.SelectCookCardState:

                _cmdBase = new CameraMoveCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                _cmdBase = new CookMealKitSelectCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                _cmdBase = new StateBackCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                break;

            case InputState.SelectStructureState:

                _cmdBase = new EntitySelectCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);
                // Entity Drag

                _cmdBase = new GroundClickCheckCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                _cmdBase = new CameraMoveCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                _cmdBase = new StateBackCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                break;

            case InputState.SelectSpawnCardState:

                _cmdBase = new CameraMoveCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                _cmdBase = new SpawnEntityCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);
                // Structure Build

                _cmdBase = new StateBackCommand();
                _cmdBase.Initialize();
                _Lt_Cmd.Add(_cmdBase);

                break;

            default:
                break;
        }


        for(int i = 0; i < _Lt_Cmd.Count; ++i)
            _mDict_InputInfoCommand[(InputState)_eInputState].Add(_Lt_Cmd[i]);
    }
    public void PushInputState(InputState _eInputState)
    {
        var _inputCommand = new KeyValuePair<InputState, List<InputCommandBase>>(_eInputState, _mDict_InputInfoCommand[_eInputState]);

        if (!IsEqualUseInputSystem(_eInputState))
            _mSt_NowUseCommand.Push(_inputCommand);

        _mCB_OnChangeInputSystem?.Invoke();
        InvokeByEnterInputState(_eInputState);
    }
    public void InvokeByEnterInputState(InputState eInputState)
    {
        switch (eInputState)
        {
            case InputState.NormalState:
                _mCB_OnEnterNormalState?.Invoke();
                break;
            case InputState.StructureBuildState:
                _mCB_OnEnterStructureBuildState?.Invoke();
                break;
            case InputState.SelectEntityState:
                _mCB_OnEnterSelectEntityState?.Invoke();
                break;
            case InputState.SelectStructureState:
                _mCB_OnEnterSelectStructureState?.Invoke();
                break;
            case InputState.SelectCookCardState:
                _mCB_OnEnterSelectCookCardState?.Invoke();
                break;
            case InputState.SelectSpawnCardState:
                _mCB_OnEnterSelectSpawnEntityCardState?.Invoke();
                break;

            case InputState.UIOpenState:
                _mCB_OnEnterUIOpenState?.Invoke();
                break;
            case InputState.StateMax:
                break;
            default:
                break;
        }
    }
    public void InvokeByReleaseInputState(InputState eInputState)
    {
        switch (eInputState)
        {
            case InputState.NormalState:
                _mCB_OnReleaseNormalState?.Invoke();
                break;
            case InputState.StructureBuildState:
                _mCB_OnReleaseStructureBuildState?.Invoke();
                break;
            case InputState.SelectEntityState:
                _mCB_OnReleaseSelectEntityState?.Invoke();
                break;
            case InputState.SelectStructureState:
                _mCB_OnReleaseSelectStructureState?.Invoke();
                break;
            case InputState.SelectCookCardState:
                _mCB_OnReleaseSelectCookCardState?.Invoke();
                break;
            case InputState.SelectSpawnCardState:
                _mCB_OnReleaseSelectSpawnEntityCardState?.Invoke();
                break;

            case InputState.UIOpenState:
                _mCB_OnReleaseUIOpenState?.Invoke();
                break;
            case InputState.StateMax:
                break;
            default:
                break;
        }
    }
    public void PopInputState()
    {
        var popCommand = _mSt_NowUseCommand.Pop();

        if (_mSt_NowUseCommand.IsEmpty())
            _mSt_NowUseCommand.Push(popCommand); // 1개는 무조건 있어야한다.

        _mCB_OnChangeInputSystem?.Invoke();
        InvokeByReleaseInputState(popCommand.Key);
    }

    public Action _mCB_OnChangeInputSystem;

    public Action _mCB_OnEnterNormalState;
    public Action _mCB_OnReleaseNormalState;

    public Action _mCB_OnEnterStructureBuildState;
    public Action _mCB_OnReleaseStructureBuildState;

    public Action _mCB_OnEnterSelectEntityState;
    public Action _mCB_OnReleaseSelectEntityState;

    public Action _mCB_OnEnterSelectStructureState;
    public Action _mCB_OnReleaseSelectStructureState;

    public Action _mCB_OnEnterSelectCookCardState;
    public Action _mCB_OnReleaseSelectCookCardState;

    public Action _mCB_OnEnterSelectSpawnEntityCardState;
    public Action _mCB_OnReleaseSelectSpawnEntityCardState;

    public Action _mCB_OnEnterUIOpenState;
    public Action _mCB_OnReleaseUIOpenState;

    public bool IsEqualUseInputSystem(InputState eInputState)
    {
        if (_mSt_NowUseCommand.IsNull()) return true; 
        // true 일때는 안넣도록 예외 처리

        if (_mSt_NowUseCommand.IsEmpty()) return false;

        if (_mSt_NowUseCommand.Peek().Key != eInputState) return false;

        return true;
    }

    public List<InputCommandBase> GetNowUseInputCommandList() => _mSt_NowUseCommand.Peek().Value;
    public InputState GetNowUseInputState() => _mSt_NowUseCommand.Peek().Key;

    private Stack<KeyValuePair<InputState,List<InputCommandBase>>> _mSt_NowUseCommand; // 실제로 사용하는 커맨드 시스템
    private Dictionary<InputState, List<InputCommandBase>> _mDict_InputInfoCommand; // 인풋 커맨드 시스템 정보
}