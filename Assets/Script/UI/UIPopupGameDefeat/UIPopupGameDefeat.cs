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
using UnityEngine.SceneManagement;

public class UIPopupGameDefeat : MonoBehaviour
{
    [SerializeField] GameObject _m_ObjMain;

    public void SetPopup()
    {
        _m_ObjMain.gameObject.SetActive(true);
    }

    public void OnClick_GoTitle()
    {
        EntityManager.GetInstance().ClearEntity();
        InputManager.GetInstance().PopInputState();
        InputManager.GetInstance().PushInputState(InputState.NormalState);

        _ = SceneLoadManager.GetInstance().OnLoadTitleScene();

        SoundManager.GetInstance().StopBGM($"SoundBGM_Stage");
    }

    public void OnClick_QuitGame()
    {
        EntityManager.GetInstance().ClearEntity();
        SoundManager.GetInstance().StopBGM($"SoundBGM_Stage");

        EditorApplication.isPlaying = false;
        Application.Quit();
    }
}

