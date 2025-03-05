using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using static UnityEditor.Progress;

public partial class PlayerManager
{
    private Entity _selectedEntity;
    public void SetSelectedEntity(Entity _item) 
    {
        PrevSelectedEntityClear();
        SetNextSeletedEntityEvent(_item);

        _selectedEntity = _item;
    }

    private void SetNextSeletedEntityEvent(Entity _item)
    {
        _item.Controller._onCB_DiedProcess -= () =>
        {
            InputManager.GetInstance().PopInputState();
            InputManager.GetInstance().PushInputState(InputState.NormalState);

            var gObj = GameObject.Find("SelectedArrow");
            var component = gObj.GetComponent<SelectedArrow>();
            component.SetOwnerEntity(PlayerManager.GetInstance().GetSelectedEntity());
        };

        _item.Controller._onCB_DiedProcess += () =>
        {
            InputManager.GetInstance().PopInputState();
            InputManager.GetInstance().PushInputState(InputState.NormalState);

            var gObj = GameObject.Find("SelectedArrow");
            var component = gObj.GetComponent<SelectedArrow>();
            component.SetOwnerEntity(PlayerManager.GetInstance().GetSelectedEntity());
        };
    }

    private void PrevSelectedEntityClear()
    {
        if(_selectedEntity != null)
        {
            _selectedEntity.Controller._onCB_DiedProcess -= () =>
            {
                InputManager.GetInstance().PopInputState();
                InputManager.GetInstance().PushInputState(InputState.NormalState);

                var gObj = GameObject.Find("SelectedArrow");
                var component = gObj.GetComponent<SelectedArrow>();
                component.SetOwnerEntity(PlayerManager.GetInstance().GetSelectedEntity());
            };
        }
    }

    public Entity GetSelectedEntity() => _selectedEntity;
    public void ClearSelectedEntity() 
    {
        _selectedEntity.Controller._onCB_DiedProcess -= () => 
        {
            InputManager.GetInstance().PopInputState();
            InputManager.GetInstance().PushInputState(InputState.NormalState);

            var gObj = GameObject.Find("SelectedArrow");
            var component = gObj.GetComponent<SelectedArrow>();
            component.SetOwnerEntity(PlayerManager.GetInstance().GetSelectedEntity());
        };
        _selectedEntity = null; 
    }
}