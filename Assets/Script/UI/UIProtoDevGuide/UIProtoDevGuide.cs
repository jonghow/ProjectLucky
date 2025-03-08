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

public class UIProtoDevGuide : MonoBehaviour
{
    public void Cheat_Stage1DataLoad()
    {
        UnityEngine.Debug.Log($"[BattleStage] Cheat_Stage1DataLoad");

        _ = MapManager.GetInstance().LoadStage(1001);
    }

    public void Cheat_RivalStageDataLoad()
    {
        UnityEngine.Debug.Log($"[BattleStage] Cheat_RivalStageDataLoad 1002");

        _ = RivalMapManager.GetInstance().LoadStage(1002);
    }

    public void Cheat_RivalPlayerTurnONAI()
    {
        UnityEngine.Debug.Log($"[BattleStage] Cheat_RivalStageDataLoad 1002");

        RivalPlayerAIManager.GetInstance().GetRavalPlayer(out var _rival);
        _rival.TurnOnAI();
    }

    public void Cheat_Stage1DataRemove()
    {
        UnityEngine.Debug.Log($"[BattleStage] Cheat_Stage1DataRemove");

        MapManager.GetInstance().ClearMapNavigation();
    }
    public void Cheat_Stage1SpawnerLoad()
    {
        UnityEngine.Debug.Log($"[BattleStage] Cheat_Stage1SpawnerLoad");

        _ = SpawnerManager.GetInstance().LoadSpawner(1000,0);
    }
    public void Cheat_InstancePlayerTeamObject()
    {
        // 현재 자리에서 우리팀의 오브젝트 생성
        UnityEngine.Debug.Log($"[BattleStage] Cheat_InstancePlayerTeamObject");

        //var spawner = new UserEntityFactory();
        //spawner.CreateEntity(1);

    }
    public void Cheat_InstanceEnemyTeamObject()
    {
        // 현재 자리에서 상대팀 오브젝트 생성
        UnityEngine.Debug.Log($"[BattleStage] Cheat_InstanceEnemyTeamObject");

        //var spawner = new RivalEntityFactory();
        //spawner.CreateEntity(1);
    }
    public void Cheat_Unitask_ModelLoading()
    {
        UnityEngine.Debug.Log($"[BattleStage] Cheat_Unitask_ModelLoading");
        Stopwatch sw = new Stopwatch();
        sw.Start();

        ResourceManager.GetInstance().GetResource(ResourceType.Effect, 0, true, (ret) =>
        {
            sw.Stop();
            UnityEngine.Debug.Log($"[BattleStage] Load Complete time : {sw.ElapsedMilliseconds}");

            GameObject obj = GameObject.Instantiate(ret) as GameObject;
            obj.transform.position = new Vector3(0f, 0f, 0f);
        });
    }
    public void Cheat_PlayerTeamTurnONAI()
    {
        List<System.Tuple<long, Entity>> _playerList;
        EntityManager.GetInstance().GetEntityList(EntityDivision.Player, out _playerList);

        for (int i = 0; i < _playerList.Count; i++)
        {
            _playerList[i].Item2.Controller.TurnOnAI();
        }
    }
    public void Cheat_PlayerTeamTurnOFFAI()
    {
        List<System.Tuple<long, Entity>> _playerList;
        EntityManager.GetInstance().GetEntityList(EntityDivision.Player, out _playerList);

        for (int i = 0; i < _playerList.Count; i++)
        {
            _playerList[i].Item2.Controller.TurnOffAI();
        }
    }
    public void Cheat_GetUpgradeCard()
    {
        int _drawCardCount = 1;
        HandCardManager.GetInstance().CommandDrawCard(_drawCardCount);
    }
    public void Cheat_SpawnStart()
    {
        SpawnerManager.GetInstance().CommandAllSpawnStart();
    }
    public void Cheat_SpawnStopAndClear()
    {
        SpawnerManager.GetInstance().CommandAllSpawnStop();
    }

    public void Cheat_sot()
    {
        HandCardManager.GetInstance().CommandGetCardByID(5);
    }

    public void Cheat_SpawnCard(int _cardID)
    {
        HandCardManager.GetInstance().CommandGetCardByID(_cardID);
    }

    public void OnClick_ApplyFreshness(int _freshness)
    {
        PlayerManager.GetInstance().AddFreshness(_freshness);
    }

    public void Update()
    {
    }
}

