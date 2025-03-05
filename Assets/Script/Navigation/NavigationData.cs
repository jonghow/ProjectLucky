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
    /// NOTICE :: Navigation의 위치를 관리하는 클래스 입니다. 
    /// 실제 이동 F G H 코스트는 NavigationState가 관리할 것입니다.
    /// </summary>
    public Vector2Int _mv2_Index;
    public Vector3 _mv3_Pos;

    public bool _mb_IsEnable;
    public NavigationElement() { } // XML 데이터 로드때문에 필요, XML 생성 시 기본 생성자 인스턴스해서 작업한다.
    public NavigationElement(Vector2Int v2Int_Index, Vector3 v3_Pos)
    {
        _mv2_Index = v2Int_Index;
        _mv3_Pos = v3_Pos;
        _mb_IsEnable = true;
    }
    public void Dispose()
    {
        GC.SuppressFinalize(this); // 가비지 컬렉터에 의한 최종화 호출을 방지
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
    public int _mi_GCost; // 시작점 - 현재까지의 가중치
    public int _mi_HCost; // 노드의 위치 - 목표의 가중치

    public int FCost { get { return _mi_GCost + _mi_HCost; } }

    public NavigationElement _m_Parent; // 노드 추적용

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


