using Cysharp.Threading.Tasks;
using DTR_Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using System.Xml;
using UnityEngine.Pool;

public class PooledObjectCoinCountTag : PooledBase, IPoolBase
{
    [SerializeField] Animator _m_Animator;
    [SerializeField] TextMeshProUGUI _m_Text;

    [SerializeField] string[] _mStr_AnimStyle;

    public void SetData(float _coin)
    {
        Regist();

        int _randomStyle = UnityEngine.Random.Range(0, _mStr_AnimStyle.Length);
        _m_Animator.Play(_mStr_AnimStyle[_randomStyle], 0,0);
        _m_Text.text = $"+ {_coin}";

        UIManager.GetInstance().GetCoinCountPivot(out var _pivotObject);
        this.transform.SetParent(_pivotObject.transform);
        this.transform.localPosition = Vector3.zero;
    }
    public void Regist()
    {
        this.gameObject.SetActive(true);

        _me_PooledType = PooledObject.WO;
        _me_PooledInnerType = PooledObjectInner.WO_CoinCountTag;
    }

    public void Release()
    {
        var _IPooledBase = this as IPoolBase;
        PoolingManager.GetInstance().CollectObject(_me_PooledType, _me_PooledInnerType, ref _IPooledBase);
    }

    public void Update()
    {
        UpdateTime();
    }

    private void UpdateTime()
    {
        AnimatorStateInfo _stateInfo = _m_Animator.GetCurrentAnimatorStateInfo(0);
        if (_stateInfo.normalizedTime >= 0.99f)
        {
            Release();
        }
    }
    public void ClearData()
    {

    }
}
