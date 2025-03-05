using Cysharp.Threading.Tasks;
using DTR_Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using TMPro;
using System.Xml;

public class PooledObjectDamageTag : PooledBase, IPoolBase
{
    [SerializeField] Animator _m_Animator;
    [SerializeField] TextMeshProUGUI _m_Text;

    [SerializeField] string[] _mStr_AnimStyle;

    public void SetData(Vector3 _pos, float _damage)
    {
        this.transform.position = _pos;

        int _randomStyle = UnityEngine.Random.Range(0, _mStr_AnimStyle.Length);
        _m_Animator.Play(_mStr_AnimStyle[_randomStyle], 0,0);
        _m_Text.text = $"{_damage}";

        UIManager.GetInstance().GetWOCanvas(out var _parent);
        this.gameObject.transform.SetParent(_parent.transform);

        this.transform.position = Camera.main.WorldToScreenPoint(_pos);

        Regist();
    }
    public void Regist()
    {
        this.gameObject.SetActive(true);

        _me_PooledType = PooledObject.WO;
        _me_PooledInnerType = PooledObjectInner.WO_DamageTag;
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
