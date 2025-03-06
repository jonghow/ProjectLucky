using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using System.Threading;

public class GroundClickCheckCommand : InputCommandBase
{
    private Camera _m_MainCamera;
    [SerializeField] EntitiesGroup _m_CachedSelectedEntitiesGroup;

    public override void Initialize()
    {
        _m_MainCamera = Camera.main;
    }

    public override void Detect()
    {
        if (_m_MainCamera == null)
        {
            _m_MainCamera = Camera.main;
        }

        _m_CachedSelectedEntitiesGroup = PlayerManager.GetInstance().GetSelectedEntity();

        if (_m_CachedSelectedEntitiesGroup == null)
            return;

        GroundClickCheck();
    }

    public void GroundClickCheck()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (CheckOverrideButtons())
                return;

            Vector3 _mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);

            List<EntitiesGroup> _Lt_Groups = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Player);

            float _magDistance = 0.5f; // �ּ� �Ÿ�

            EntitiesGroup _m_NearEntityGroup = null;
            if (_Lt_Groups != null)
            {
                for (int i = 0; i < _Lt_Groups.Count; ++i)
                {
                    EntitiesGroup _curEntity = _Lt_Groups[i];

                    if (Vector2.SqrMagnitude(_curEntity.Pos3D - _mousePos) <= _magDistance)
                    {
                        _m_NearEntityGroup = _curEntity;
                        _magDistance = Vector2.SqrMagnitude(_m_NearEntityGroup.Pos3D - _mousePos);
                    }
                }
            }

            if (_m_NearEntityGroup == null)
                ReleaseState();
        }
    }

    public void ReleaseState()
    {
        PlayerManager.GetInstance().ClearSelectedEntity();
        SetSelectedCircle();
        InputManager.GetInstance().PopInputState();
        InputManager.GetInstance().PushInputState(InputState.NormalState);
    }

    public bool CheckOverrideButtons()
    {
        if (PlayerManager.GetInstance().GetSelectedEntity() == null)
            return false;

        if ((MouseTopButtonCheck() && MouseBottomButtonCheck()))
            return false;

        return true;
    }

    public bool MouseTopButtonCheck()
    {
        Vector3 _mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
        _mousePos.z = 0;

        float restrict = 0.5f;

        var gObj = GameObject.Find("SelectedGroups");
        var component = gObj.GetComponent<SelectCircle>();
        Vector3 pivot = component.GetPivotPos3D();

        Vector3 pivotToWorld = _m_MainCamera.ScreenToWorldPoint(pivot);
        pivotToWorld.y += 1;

        if (MathUtility.CheckOverV3MagnitudeDistance(_mousePos, pivotToWorld, restrict))
            return true;

        return false;
    }

    public bool MouseBottomButtonCheck()
    {
        Vector3 _mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
        _mousePos.z = 0;

        float restrict = 0.5f;

        var gObj = GameObject.Find("SelectedGroups");
        var component = gObj.GetComponent<SelectCircle>();
        Vector3 pivot = component.GetPivotPos3D();

        Vector3 pivotToWorld = _m_MainCamera.ScreenToWorldPoint(pivot);
        pivotToWorld.y -= 1;

        if (MathUtility.CheckOverV3MagnitudeDistance(_mousePos, pivotToWorld, restrict))
            return true;

        return false;
    }

    public void SetSelectedCircle()
    {
        var gObj = GameObject.Find("SelectedGroups");
        var component = gObj.GetComponent<SelectCircle>();
        component.SetOwnerEntity(PlayerManager.GetInstance().GetSelectedEntity());
    }
}