using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using GlobalGameDataSpace;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;

public class NavigationVisualizer : MonoBehaviour
{
    [SerializeField] bool _onVisualize = false;

    public void Start()
    {
        DontDestroyOnLoad(this);
    }

    private void OnDrawGizmos()
    {
        if (_onVisualize == false)
            return;

        else
        {
            OnDrawPlayerMap();
            OnDrawRivalMap();
        }
    }

    public void OnDrawPlayerMap()
    {
        MapManager.GetInstance().GetNavigationElements(out var _nvGrid);

        if (_nvGrid == null) return;

        foreach (var elementPair in _nvGrid)
        {
            var _element = elementPair.Value;
            if (_element == null) continue;

            if (_element._mb_IsEnable == true)
            {
                var _vPos = _element._mv3_Pos;
                var _scaleX = Defines.DefaultScaleX; // 0.5f
                var _scaleY = Defines.DefaultScaleY; // // 0.5f

                Gizmos.color = Color.magenta;
                Gizmos.DrawWireCube(_vPos, new Vector3(_scaleX, _scaleY, 0f));
            }
            else
            {
                var _vPos = _element._mv3_Pos;
                var _scaleX = Defines.DefaultScaleX; // 0.5f
                var _scaleY = Defines.DefaultScaleY; // // 0.5f

                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(_vPos, new Vector3(_scaleX, _scaleY, 0f));
            }
        }

        OnDrawSelectedElement();
    }

    public void OnDrawRivalMap()
    {
        // ¾Æ·¡´Â Rival Map
        RivalMapManager.GetInstance().GetNavigationElements(out var _rivalNvGrid);

        if (_rivalNvGrid == null) return;

        foreach (var elementPair in _rivalNvGrid)
        {
            var _element = elementPair.Value;
            if (_element == null) continue;

            if (_element._mb_IsEnable == true)
            {
                var _vPos = _element._mv3_Pos;
                var _scaleX = Defines.DefaultScaleX; // 0.5f
                var _scaleY = Defines.DefaultScaleY; // // 0.5f

                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(_vPos, new Vector3(_scaleX, _scaleY, 0f));
            }
            else
            {
                var _vPos = _element._mv3_Pos;
                var _scaleX = Defines.DefaultScaleX; // 0.5f
                var _scaleY = Defines.DefaultScaleY; // // 0.5f

                Gizmos.color = Color.cyan;
                Gizmos.DrawWireCube(_vPos, new Vector3(_scaleX, _scaleY, 0f));
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        //OnDrawSelectedElement();
    }

    public void OnDrawSelectedElement()
    {
        var _element = MapManager.GetInstance().SelectedElement;
        if (_element == null) return;

        var _vPos = _element._mv3_Pos;
        var _scaleX = Defines.DefaultScaleX; // 0.5f
        var _scaleY = Defines.DefaultScaleY; // // 0.5f

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(_vPos, new Vector3(_scaleX, _scaleY, 0f));
    }
}

