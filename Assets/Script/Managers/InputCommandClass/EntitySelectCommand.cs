using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;

/// <summary>
/// Mercenary Entity �� �����ϴ� Ŀ�ǵ��Դϴ�.
/// </summary>
public class EntitySelectCommand : InputCommandBase
{
    private Camera _m_MainCamera;
    public override void Initialize()
    {
        _m_MainCamera = Camera.main;
    }

    public override void Detect()
    {
        if(_m_MainCamera == null)
        {
            _m_MainCamera = Camera.main;
        }

        if (Input.GetMouseButtonDown(0)) // ���콺 Ŭ��
        {
            if (CheckOverrideButtons())
                return;

            // ���ǻ� �����Ϳ��� ���� �ڵ�
            Vector3 _mousePos = _m_MainCamera.ScreenToWorldPoint(Input.mousePosition);
            _mousePos.z = 0;

            List<EntitiesGroup> _Lt_Groups = EntityManager.GetInstance().NewGetEntityGroups(EntityDivision.Player);

            float _magDistance = 0.5f; // �ּ� �Ÿ�

            if (_Lt_Groups != null)
            {
                for (int i = 0; i < _Lt_Groups.Count; ++i)
                {
                    EntitiesGroup _curEntity = _Lt_Groups[i];

                    if (Vector3.SqrMagnitude(_curEntity.Pos3D - _mousePos) <= _magDistance)
                    {
                        PlayerManager.GetInstance().SetSelectedEntity(_curEntity);
                        _magDistance = Vector2.SqrMagnitude(_curEntity.Pos3D - _mousePos);
                    }
                }
            }

            if (PlayerManager.GetInstance().GetSelectedEntity() != null)
            {
                SetSelectedCircle();
                ChangeInputStateToSelectEntity();
            }
        }
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

    public void ChangeInputStateToSelectEntity()
    {
        InputManager.GetInstance().PopInputState();
        InputManager.GetInstance().PushInputState(InputState.SelectEntityState);
    }

    public void SetSelectedCircle()
    {
        var gObj = GameObject.Find("SelectedGroups");
        var component = gObj.GetComponent<SelectCircle>();
        component.SetOwnerEntity(PlayerManager.GetInstance().GetSelectedEntity());
    }
}