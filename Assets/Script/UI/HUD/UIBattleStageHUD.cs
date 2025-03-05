using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;

public class UIBattleStageHUD : MonoBehaviour
{
    ///<summary>
    /// Top
    ///</summary>
    [SerializeField] UIBattleStageHUD_FreshnessInfo _m_FreshnessInfo; 
    
    ///<summary>
    /// Left Top
    ///</summary>
    [SerializeField] UIBattleStageHUD_StageInfo _m_StageInfo;

    /// <summary>
    /// Bottom
    /// </summary>
    [SerializeField] UIBattleStageHUD_MyHandInfo _m_MyHandInfo;
    [SerializeField] UIBattleStageHUD_StructureBuild _m_StructuBuild; // 원하는 곳에 지어달라는 그런 UI 띄울것임
    [SerializeField] UIBattleStageHUD_MercenaryInfo _m_MercernaryInfo;
    [SerializeField] UIBattleStageHUD_StructureInfo _m_StructureInfo;

    private void OnEnable()
    {
        if(HandCardManager.GetInstance() != null)
        {
            HandCardManager.GetInstance()._mCB_DrawHandCard -= RefreshCardList;
            HandCardManager.GetInstance()._mCB_DrawHandCard += RefreshCardList;

            HandCardManager.GetInstance()._mCB_UseHandCard -= RefreshCardList;
            HandCardManager.GetInstance()._mCB_UseHandCard += RefreshCardList;
        }

        if(InputManager.GetInstance() != null)
        {
            InputManager.GetInstance()._mCB_OnEnterNormalState -= ShowCardList;
            InputManager.GetInstance()._mCB_OnEnterNormalState += ShowCardList;
            // Enter Normal

            InputManager.GetInstance()._mCB_OnReleaseNormalState -= HideCardList;
            InputManager.GetInstance()._mCB_OnReleaseNormalState += HideCardList;
            // Release Normal

            InputManager.GetInstance()._mCB_OnEnterStructureBuildState -= ShowStructBuildGuide;
            InputManager.GetInstance()._mCB_OnEnterStructureBuildState += ShowStructBuildGuide;
            // Enter StructBuild

            InputManager.GetInstance()._mCB_OnReleaseStructureBuildState -= HideStructBuildGuide;
            InputManager.GetInstance()._mCB_OnReleaseStructureBuildState += HideStructBuildGuide;
            // Release StructBuild

            InputManager.GetInstance()._mCB_OnEnterSelectEntityState -= ShowEntityInfo;
            InputManager.GetInstance()._mCB_OnEnterSelectEntityState += ShowEntityInfo;
            // Enter Selected Entity

            InputManager.GetInstance()._mCB_OnReleaseSelectEntityState -= HideEntityInfo;
            InputManager.GetInstance()._mCB_OnReleaseSelectEntityState += HideEntityInfo;
            // Release Selected Entity

            InputManager.GetInstance()._mCB_OnEnterSelectStructureState -= ShowStructureInfo;
            InputManager.GetInstance()._mCB_OnEnterSelectStructureState += ShowStructureInfo;
            // Enter Select Structure

            InputManager.GetInstance()._mCB_OnReleaseSelectStructureState -= HideStructureInfo;
            InputManager.GetInstance()._mCB_OnReleaseSelectStructureState += HideStructureInfo;
            // Release Select Structure

            InputManager.GetInstance()._mCB_OnEnterSelectCookCardState -= ShowStructBuildGuide;
            InputManager.GetInstance()._mCB_OnEnterSelectCookCardState += ShowStructBuildGuide;
            // Enter Select CookCard

            InputManager.GetInstance()._mCB_OnReleaseSelectCookCardState -= HideStructBuildGuide;
            InputManager.GetInstance()._mCB_OnReleaseSelectCookCardState += HideStructBuildGuide;
            // Release Select CookCard

            InputManager.GetInstance()._mCB_OnEnterSelectSpawnEntityCardState -= ShowStructBuildGuide;
            InputManager.GetInstance()._mCB_OnEnterSelectSpawnEntityCardState += ShowStructBuildGuide;
            // Enter Select SpawnEntityCard

            InputManager.GetInstance()._mCB_OnReleaseSelectSpawnEntityCardState -= HideStructBuildGuide;
            InputManager.GetInstance()._mCB_OnReleaseSelectSpawnEntityCardState += HideStructBuildGuide;
            // Release Select SpawnEntityCard
        }

        RefreshCardList();
    }
    private void OnDisable()
    {
        if (HandCardManager.GetInstance() != null)
        {
            HandCardManager.GetInstance()._mCB_DrawHandCard -= RefreshCardList;
            HandCardManager.GetInstance()._mCB_UseHandCard -= RefreshCardList;
        }

        if (InputManager.GetInstance() != null)
        {
            InputManager.GetInstance()._mCB_OnEnterNormalState -= ShowCardList;
            // Enter Normal

            InputManager.GetInstance()._mCB_OnReleaseNormalState -= HideCardList;
            // Release Normal

            InputManager.GetInstance()._mCB_OnEnterStructureBuildState += ShowStructBuildGuide;
            // Enter StructBuild

            InputManager.GetInstance()._mCB_OnReleaseStructureBuildState += HideStructBuildGuide;
            // Release StructBuild

            InputManager.GetInstance()._mCB_OnEnterSelectEntityState -= ShowEntityInfo;
            // Enter Selected Entity

            InputManager.GetInstance()._mCB_OnReleaseSelectEntityState -= HideEntityInfo;
            // Release Selected Entity

            InputManager.GetInstance()._mCB_OnEnterSelectStructureState -= ShowStructureInfo;
            // Enter Select Structure

            InputManager.GetInstance()._mCB_OnReleaseSelectStructureState -= HideStructureInfo;
            // Release Select Structure

            InputManager.GetInstance()._mCB_OnEnterSelectCookCardState -= ShowStructBuildGuide;
            // Enter Select CookCard 

            InputManager.GetInstance()._mCB_OnReleaseSelectCookCardState -= HideStructBuildGuide;
            // Release Select CookCard 

            InputManager.GetInstance()._mCB_OnEnterSelectSpawnEntityCardState -= ShowStructBuildGuide;
            // Enter SpawnEntity Card 

            InputManager.GetInstance()._mCB_OnReleaseSelectSpawnEntityCardState -= HideStructBuildGuide;
            // Release SpawnEntity Card 
        }
    }
    public void RefreshCardList()
    {
        _m_MyHandInfo.UpdateHandCards();
    }

    public void ShowCardList()
    {
        _m_MyHandInfo.ProcActivationCardList(true);
    }

    public void HideCardList()
    {
        _m_MyHandInfo.ProcActivationCardList(false);
    }

    public void ShowStructBuildGuide()
    {
        _m_StructuBuild.ProcActivationCardList(true);
    }

    public void HideStructBuildGuide()
    {
        _m_StructuBuild.ProcActivationCardList(false);
    }

    public void ShowEntityInfo()
    {
        _m_MercernaryInfo.ProcActivationCardList(true);
    }
    public void HideEntityInfo()
    {
        _m_MercernaryInfo.ProcActivationCardList(false);
    }
    public void ShowStructureInfo()
    {
        _m_StructureInfo.ProcActivationCardList(true);
    }
    public void HideStructureInfo()
    {
        _m_StructureInfo.ProcActivationCardList(false);
    }

    public void OnClick_CardReroll()
    {
        int _cost = 50;

        if (!PlayerManager.GetInstance().IsEnougnFreshness(_cost))
            return;

        if (HandCardManager.GetInstance().EmptyCardInventory())
            return;

        PlayerManager.GetInstance().UseFreshness(_cost);
        HandCardManager.GetInstance().CommandRerollCard();
    }

    public void OnClick_BattleStart()
    {
        //EntityManager.GetInstance().GetEntityList(EntityDivision.MealFactory, out var _entity);
        //if (_entity == null || _entity.Count == 0)
        //    return;

        //bool _EnableStart = false;
        //for(int i = 0; i < _entity.Count; ++i)
        //{
        //    if (_entity[i].Item2.CharacterID == 3)
        //    {
        //        _EnableStart = true;
        //        break;
        //    }
        //}

        //if (_EnableStart == false)
        //    return;
        // 임시

        SceneLoadManager.GetInstance().GetStage(out var _stage);
        
        if(_stage is BattleStage _battleStage)
        {
            _battleStage.GetStageMachine(out var _stageMachine);
            _stageMachine.GetCurrentState(out var _currentState);

            if (_currentState is BattleReadyState)
            {
                _stageMachine.SetState(new BattleState(_stageMachine));
                // 전투 상태로 변경 

                SpawnerManager.GetInstance().CommandAllSpawnStart();
                // 모든 몬스터 스폰 

                List<System.Tuple<long, Entity>> _playerList;
                EntityManager.GetInstance().GetEntityList(EntityDivision.Player, out _playerList);

                for (int i = 0; i < _playerList.Count; i++)
                {
                    _playerList[i].Item2.Controller.TurnOnAI();
                }
                // 모든 Entity AI ON
            }
        }
    }

    public void OnClick_DrawCard()
    {
        int _cost = 30;

        if (!PlayerManager.GetInstance().IsEnougnFreshness(_cost))
            return;

        if (!HandCardManager.GetInstance().IsEnoughCardSpace())
            return;

        PlayerManager.GetInstance().UseFreshness(_cost);
        HandCardManager.GetInstance().CommandDrawCard(1);
    }
}
