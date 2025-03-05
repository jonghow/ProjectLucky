using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using System;

public class AsyncLazy<T>
{
    private readonly Lazy<UniTask<T>> _lazyTask;

    public AsyncLazy(Func<UniTask<T>> factory)
    {
        // UniTask.Run�� ������� �ʰ�, �ٷ� factory�� ����
        _lazyTask = new Lazy<UniTask<T>>(factory);
    }

    public UniTask<T> Value => _lazyTask.Value;
}

