using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;
using UnityEngine.UI;

public class UIBattleStageHUD_StructureInfo : StageBase
{
    private Entity _m_CachedEntity;

    #region Character Portrait
    [SerializeField] Image _m_Img_Portrait;
    #endregion

    #region Character HP 
    [SerializeField] UICommonHPBar _m_HpBar;
    #endregion

    #region Character Ability _ Skill

    #endregion

    #region Character Ability _ Stat

    [SerializeField] UICommonStat _m_Stat_Attack;
    [SerializeField] UICommonStat _m_Stat_Shield;
    [SerializeField] UICommonStat _m_Stat_AttackRange;
    [SerializeField] UICommonStat _m_Stat_CriticalRate;
    [SerializeField] UICommonStat _m_Stat_MoveSpeed;

    #endregion

    public void ProcActivationCardList(bool isActive)
    {
        this.gameObject.SetActive(isActive);
    }

    private void OnEnable()
    {
        if (_m_CachedEntity != null)
        {
            _m_CachedEntity.Controller._onCB_HitProcess -= RefreshStructureInfo;
            _m_CachedEntity.Controller._onCB_HitProcess += RefreshStructureInfo;
        }

        RefreshStructureInfo();
    }

    private void OnDisable()
    {
        if (_m_CachedEntity != null)
        {
            _m_CachedEntity.Controller._onCB_HitProcess -= RefreshStructureInfo;
        }
    }

    public void RefreshStructureInfo()
    {
        _m_CachedEntity = PlayerManager.GetInstance().GetSelectedEntity();

        if(_m_CachedEntity != null)
        {
            _m_HpBar.SetHP(_m_CachedEntity.Info.HP, _m_CachedEntity.Info.MaxHP);

            _m_Stat_Attack.SetStat(StatType.Attack, _m_CachedEntity.Info.Status.STR);
            _m_Stat_Shield.SetStat(StatType.Shield, _m_CachedEntity.Info.Status.GUT);
            _m_Stat_AttackRange.SetStat(StatType.AttackRange, _m_CachedEntity.Info.AttackRange);
            _m_Stat_CriticalRate.SetStat(StatType.CriticalRate, _m_CachedEntity.Info.Status.DEX);
            _m_Stat_MoveSpeed.SetStat(StatType.MoveSpeed, _m_CachedEntity.Info.MoveSpeed);
        }
    }
}
