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

public class UIPopupTutorial : MonoBehaviour
{
    [SerializeField] GameObject[] _mArr_ObjTutorial;

    [SerializeField] GameObject[] _mArr_ObjGuidePage1;
    [SerializeField] GameObject[] _mArr_ObjGuidePage2;

    int _mi_CurGuideNumber = 0;
    int _mi_Step = 0;

    public void SetPopup(int _guideNumber)
    {
        _mi_CurGuideNumber = _guideNumber;
        _mi_Step = 0;
        _mArr_ObjTutorial[_guideNumber].gameObject.SetActive(true);

        if( _guideNumber == 0 )
            _mArr_ObjGuidePage1[_mi_Step].gameObject.SetActive(true);
        else
            _mArr_ObjGuidePage2[_mi_Step].gameObject.SetActive(true);
    }

    public void OnClickNext()
    {
        CloseGuidePage();

        if(_mi_CurGuideNumber == 0)
            _mArr_ObjGuidePage1[_mi_Step].SetActive(true);
        else
            _mArr_ObjGuidePage2[_mi_Step].SetActive(true);
    }

    public void OnConfirm()
    {
        AllTutorialClose();
        AllPageClose();
    }

    public void CloseGuidePage()
    {
        if(_mi_CurGuideNumber == 0)
        {
            _mArr_ObjGuidePage1[_mi_Step].SetActive(false);
        }
        else
        {
            _mArr_ObjGuidePage2[_mi_Step].SetActive(false);
        }

        ++_mi_Step;
    }

    public void AllPageClose()
    {
        if (_mi_CurGuideNumber == 0)
        {
            for(int i = 0; i <  _mArr_ObjGuidePage1.Length; ++i)
                _mArr_ObjGuidePage1[i].SetActive(false);
        }
        else
        {
            for (int i = 0; i < _mArr_ObjGuidePage2.Length; ++i)
                _mArr_ObjGuidePage2[i].SetActive(false);
        }
    }

    public void AllTutorialClose()
    {
        for (int i = 0; i < _mArr_ObjTutorial.Length; ++i)
            _mArr_ObjTutorial[i].SetActive(false);
    }

}

