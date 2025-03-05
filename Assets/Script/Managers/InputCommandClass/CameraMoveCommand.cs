using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Unity.Burst.CompilerServices;
using TMPro;

public class CameraMoveCommand : InputCommandBase
{
    [SerializeField] Camera _m_MainCam;
    [SerializeField] Transform _mTr_Cam;
    [SerializeField] Vector3 _mv3_CamPos;
    [SerializeField] float _mf_MoveSpeed;

    [SerializeField] Vector3 _mv3_CameraMinBound;
    [SerializeField] Vector3 _mv3_CameraMaxBound;

    public override void Initialize()
    {
        _m_MainCam = Camera.main;
        _mTr_Cam = _m_MainCam.GetComponent<Transform>();
        _mf_MoveSpeed = 10f;
        _mv3_CameraMinBound = new Vector3(-20f, 8f, 0f);
        _mv3_CameraMaxBound = new Vector3(15f, -8f, 0f);
    }

    public override void Detect()
    {
        if(_m_MainCam == null)
            _m_MainCam = Camera.main;

        if(_mTr_Cam == null)
            _mTr_Cam = _m_MainCam.GetComponent<Transform>();

        float _horizontal = Input.GetAxisRaw($"Horizontal");
        float _vertical = Input.GetAxisRaw($"Vertical");

        Vector3 _v3_Dir = new Vector3(_horizontal, _vertical, 0f).normalized;

        Vector3 PO = _mTr_Cam.transform.position;
        Vector3 AT = _v3_Dir * _mf_MoveSpeed * Time.deltaTime;

        Vector3 P1 = PO + AT;

        P1.x = Mathf.Clamp(P1.x, _mv3_CameraMinBound.x, _mv3_CameraMaxBound.x);
        P1.y = Mathf.Clamp(P1.y, _mv3_CameraMaxBound.y, _mv3_CameraMinBound.y);

        //targetPosition.x = Mathf.Clamp(targetPosition.x, minBounds.x, maxBounds.x);
        //targetPosition.y = Mathf.Clamp(targetPosition.y, minBounds.y, maxBounds.y);

        _mTr_Cam.transform.position = P1;

        //UnityLogger.GetInstance().LogWarning($"[CameraMoveCommand]Detect h : {_horizontal} , v : {_vectical}"); 
    } 
}
