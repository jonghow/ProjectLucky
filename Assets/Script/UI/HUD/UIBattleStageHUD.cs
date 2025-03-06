using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Cysharp.Threading.Tasks;
using System.Threading;
using GlobalGameDataSpace;

public class UIBattleStageHUD : MonoBehaviour
{
    /// <summary>
    /// Bottom
    /// </summary>
    [SerializeField] UIBattleStageHUD_MyHandInfo _m_MyHandInfo;

    private void OnEnable()
    {
    }
    private void OnDisable()
    {
       
    }

    public void On_ClickSpawn()
    {
        FindEnableEntityGroups(out var _nvRet);
        SpawnEntity(_nvRet);
    }

    public void FindEnableEntityGroups(out NavigationElement _nvRet)
    {
        _nvRet = null;
        EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Player, 1, out EntitiesGroup _ret);

        Vector3 _myPosition = _ret.transform.position;
        var _selectedNavigation = MapManager.GetInstance().GetMyNavigationByPos3D(_myPosition);
    }

    public void SpawnEntity(NavigationElement _selectedNavigation)
    {
        if (_selectedNavigation == null)
        {
            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"SpawnEntity", $"_selectedNavigation is NULL");
            return;
        }

        //UnityLogger.GetInstance().Log($"[StructureBuildCommand]BuildStructure Success");

        var _handCard = PlayerManager.GetInstance().GetSelectedHandCardItem();
        if (_handCard.GetCardType() != HandCardType.SpawnEntity) return;

        Vector2Int _n2_NavIdx = _selectedNavigation._mv2_Index;
        Vector3 _v3_position = _selectedNavigation._mv3_Pos;

        int spawnID = int.Parse(_handCard.GetUpgradeValue());

        UserEntityFactory _spawner = new UserEntityFactory();
        _ = _spawner.CreateEntity(spawnID, _v3_position, (obj) =>
        {

        });

        //PlayerManager.GetInstance().GetSelectedShadow().OffVisualizers();

        InputManager.GetInstance().PopInputState();
        InputManager.GetInstance().PushInputState(InputState.NormalState);
    }

}
