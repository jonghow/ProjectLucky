using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using static UnityEditor.Progress;

public partial class PlayerManager
{
    private EntitiesGroup _selectedEntity;
    public void SetSelectedEntity(EntitiesGroup _item) 
    {
        SetNextSeletedEntityEvent(_item);

        _selectedEntity = _item;
    }

    private void SetNextSeletedEntityEvent(EntitiesGroup _item)
    {

    }

    public EntitiesGroup GetSelectedEntity() => _selectedEntity;
    public void ClearSelectedEntity() 
    {
        _selectedEntity = null; 
    }
}