using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using GlobalGameDataSpace;

[Serializable]
public class NavigationElement : IDisposable
{
    /// <summary>
    /// NOTICE :: Navigation�� ��ġ�� �����ϴ� Ŭ���� �Դϴ�. 
    /// ���� �̵� F G H �ڽ�Ʈ�� NavigationState�� ������ ���Դϴ�.
    /// </summary>
    public Vector2Int _mv2_Index;
    public Vector3 _mv3_Pos;

    public bool _mb_IsEnable;
    public NavigationElement() { } // XML ������ �ε嶧���� �ʿ�, XML ���� �� �⺻ ������ �ν��Ͻ��ؼ� �۾��Ѵ�.
    public NavigationElement(Vector2Int v2Int_Index, Vector3 v3_Pos)
    {
        _mv2_Index = v2Int_Index;
        _mv3_Pos = v3_Pos;
        _mb_IsEnable = true;
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this); // ������ �÷��Ϳ� ���� ����ȭ ȣ���� ����
    }
}

[Serializable]
public class NavigationElementListWrap
{
    public List<NavigationElement> List = new List<NavigationElement>();

    public int _row;
    public int _col;
}

public class NavigationState
{
    public int _mi_GCost; // ������ - ��������� ����ġ
    public int _mi_HCost; // ����� ��ġ - ��ǥ�� ����ġ

    public int FCost { get { return _mi_GCost + _mi_HCost; } }

    public NavigationElement _m_Parent; // ��� ������

    public NavigationState()
    {
        OnInitialize();
    }

    public void OnInitialize()
    {
        _mi_GCost = int.MaxValue;
        _mi_HCost = 0;
        _m_Parent = null;
    }
}


