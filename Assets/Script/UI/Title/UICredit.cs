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

public class UICredit : MonoBehaviour
{
    [SerializeField] Animator _animator;

    public void OnEnable()
    {
        _animator.Play($"Credit",0,0f);
    }

    public void OnDisable()
    {
        _animator.StopPlayback();
    }

    public void OnClick_Close()
    {
        this.gameObject.SetActive(false);
    }
}

