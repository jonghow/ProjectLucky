using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using GlobalGameDataSpace;
using static UnityEditor.PlayerSettings;
using UnityEngine.UIElements;

public class SpawnerVisualizer : MonoBehaviour
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
            SpawnerManager.GetInstance().GetSpawnerDatas(out var _datas);

            if (_datas == null) return;

            foreach (var elementPair in _datas)
            {
                SpawnerBase _spawnerBase = (elementPair.Value) as SpawnerBase;
                if (_spawnerBase == null) continue;

                var _vPos = _spawnerBase._mv3_Pos;
                var _scaleX = 0.5f;
                var _scaleY = 0.5f;

                Gizmos.color = Color.blue;
                Gizmos.DrawWireSphere(_vPos, _scaleX);
            }

            OnDrawSelectedElement();
        }
    }

    private void OnDrawGizmosSelected()
    {
        //OnDrawSelectedElement();
    }

    public void OnDrawSelectedElement()
    {
        SpawnerBase _spawnerBase = (SpawnerManager.GetInstance().SelectedSpawnerInfo) as SpawnerBase;
        if (_spawnerBase == null) return;

        var _vPos = _spawnerBase._mv3_Pos;
        var _scaleX = 0.5f;
        var _scaleY = 0.5f;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_vPos, _scaleX);
    }
}

