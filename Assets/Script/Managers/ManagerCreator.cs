using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManagerCreator : MonoBehaviour
{
    protected CancellationTokenSource _onLoadedCancellationToken;

    private void Awake()
    {
        ManagerContainer.GetInstance();
    }
}