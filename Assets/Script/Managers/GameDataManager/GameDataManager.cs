using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;

public partial class GameDataManager
{
    public static GameDataManager Instance;
    public static GameDataManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new GameDataManager();
        }

        return Instance;
    }

    public GameDataManager()
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
    /// 게임에 관련한 데이터를 로드합니다.
    /// </summary>
    private async UniTask InitLoadDatas()
    {
        InitializeCharacterPartial();
        await UTaskInitEntityXmlLoad();
        await UTask_Load_GameDBCharacterDatas(); // Info, Stat
        // Character

        InitializeStringPartial();
        await UTask_Load_StringCommon();
        // String 

        InitializeMealRecipePartial();
        await UTask_Load_MealRecipe();
        // MealRecipe

        InitializeMealKitPartial();
        await UTask_Load_MealKitInfo();
        // MealKit

        InitializeBuildingPartial();
        await UTask_Load_GameDBBuildingDatas();
        // Building Info

        InitializeDrawHandCardPartial();
        await UTask_Load_GameDBDarwHandCardDatas();
        //HandCardData

        _isLoaded = true;
    }
}