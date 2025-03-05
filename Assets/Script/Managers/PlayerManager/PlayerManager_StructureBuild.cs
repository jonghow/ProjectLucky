using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;

public partial class PlayerManager
{
    private HandCardItem _selectedItem;
    public void SetSelectedHandCardItem(HandCardItem _item) { _selectedItem = _item; }
    public HandCardItem GetSelectedHandCardItem() => _selectedItem;
    public void ClearSelectedHandCardItem() { _selectedItem = null; }

    // ¼³Ä¡ ÀÜ»ó
    private SelectedShadow _selectedShadow;
    public void SetSelectedShadow(HandCardItem _item) 
    {
        if(_selectedShadow == null)
        {
            GameObject _obj_Shadow = GameObject.Find($"SelectedShadowGroup");
            _selectedShadow = _obj_Shadow.GetComponent<SelectedShadow>();
        }
        
        _selectedItem = _item;
        _selectedShadow.SetSelectedHandItem(_item);
    }
    public SelectedShadow GetSelectedShadow() => _selectedShadow;
}