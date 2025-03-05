using Cysharp.Threading.Tasks;
using DTR_Extension;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class PooledObjectOrderDisplay : PooledBase, IPoolBase
{
    [SerializeField] float _mf_DisplayViewTime; // 보여줄 시간
    [SerializeField] float _mf_DisplayAccTime; // 보여주고 있는 시간

    [SerializeField] float _mf_AnimSpeed;

    [SerializeField] Animator _m_Animator;
    [SerializeField] string _mStr_PingAnim;

    [SerializeField] AnimatorStateInfo _m_CachedStateInfo;

    public void SetData(Vector3 _pos)
    {
        this.transform.position = _pos;
        _m_Animator.Play(_mStr_PingAnim);
        

        Regist();
    }
    public void Regist()
    {
        this.gameObject.SetActive(true);

        _me_PooledType = PooledObject.WO;
        _me_PooledInnerType = PooledObjectInner.WO_OrderDisplay_Move;
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
        _m_CachedStateInfo = _m_Animator.GetCurrentAnimatorStateInfo(0);
        if (_m_CachedStateInfo.normalizedTime >= 0.99f)
        {
            Release();
        }
    }
    public void ClearData()
    {
        _mf_DisplayAccTime = 0f;
    }
}
