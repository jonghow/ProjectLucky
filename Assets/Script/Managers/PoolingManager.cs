using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;


public class PoolingManager
{
    public static PoolingManager Instance;

    public static PoolingManager GetInstance()
    {
        if (Instance == null)
        {
            Instance = new PoolingManager();
        }

        return Instance;
    }

    private Dictionary<PooledObject, Dictionary<PooledObjectInner, List<IPoolBase>>> _m_Dict_PoolObject;
    
    private GameObject _m_ParentBehavior;
    private Dictionary<PooledObject, Dictionary<PooledObjectInner, GameObject>> _m_Dict_HierarchyBehavior;

    public PoolingManager()
    {
        OnInitialize();
        _ = InitLoadDatas();
    }

    public void OnInitialize()
    {
        _m_Dict_PoolObject = new Dictionary<PooledObject, Dictionary<PooledObjectInner, List<IPoolBase>>>();
        _m_Dict_HierarchyBehavior = new Dictionary<PooledObject, Dictionary<PooledObjectInner, GameObject>>();

        _m_Dict_PoolObject.Clear();
    }

    private async UniTask InitLoadDatas()
    {
        bool _isLoaded = false;

        ResourceManager.GetInstance().GetResource(ResourceType.Projectile, 1, true, (_loadedObject) =>
        {
            _isLoaded = true;
        });

        await UniTask.WaitUntil(() => _isLoaded == true);

        _isLoaded = false;

        ResourceManager.GetInstance().GetResource(ResourceType.OrderDisplay, 1, true, (_loadedObject) =>
        {
            _isLoaded = true;
        });

        await UniTask.WaitUntil(() => _isLoaded == true);

        _isLoaded = false;

        ResourceManager.GetInstance().GetResource(ResourceType.OrderDisplay, 2, true, (_loadedObject) =>
        {
            _isLoaded = true;
        });

        await UniTask.WaitUntil(() => _isLoaded == true);

        ResourceManager.GetInstance().GetResource(ResourceType.UIWO, 1, true, (_loadedObject) =>
        {
            _isLoaded = true;
        });// DamageTag

        await UniTask.WaitUntil(() => _isLoaded == true);

        ResourceManager.GetInstance().GetResource(ResourceType.UIWO, 2, true, (_loadedObject) =>
        {
            _isLoaded = true;
        });// CoinCount

        await UniTask.WaitUntil(() => _isLoaded == true);

        ResourceManager.GetInstance().GetResource(ResourceType.UIWO, 3, true, (_loadedObject) =>
        {
            _isLoaded = true;
        });// World HP Bar

        await UniTask.WaitUntil(() => _isLoaded == true);
    }

    public async UniTask InitCollectObjects(GameObject _parent)
    {
        bool _isLoaded = false;
        _m_ParentBehavior = _parent;

        GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_Projectile_Arrow, out GameObject _WOProjectile_HierarchyBehavior);
        _WOProjectile_HierarchyBehavior.transform.SetParent(_m_ParentBehavior.transform);

        ResourceManager.GetInstance().GetResource(ResourceType.Projectile, 1, true, (_loadedObject) =>
        {
            int count = 30;

            for(int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_Projectile_Arrow))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_Projectile_Arrow, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_Projectile_Arrow].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_Projectile_Arrow, out var _hierarchyBehavior);
                _newInstance.transform.SetParent(_hierarchyBehavior.transform);
            }

            _isLoaded = true;
        }); // 화살

        await UniTask.WaitUntil(() => _isLoaded == true);

        GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_Projectile_FireBall, out GameObject _WO_ProjectileFireball_HierarchyBehavior);
        _WO_ProjectileFireball_HierarchyBehavior.transform.SetParent(_m_ParentBehavior.transform);

        ResourceManager.GetInstance().GetResource(ResourceType.Projectile, 2, true, (_loadedObject) =>
        {
            int count = 30;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_Projectile_FireBall))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_Projectile_FireBall, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_Projectile_FireBall].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_Projectile_FireBall, out var _hierarchyBehavior);
                _newInstance.transform.SetParent(_hierarchyBehavior.transform);
            }

            _isLoaded = true;
        }); // 화염구

        await UniTask.WaitUntil(() => _isLoaded == true);

        GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_OrderDisplay_Move, out GameObject _WO_OrderDisplayMove_HierarchyBehavior);
        _WO_OrderDisplayMove_HierarchyBehavior.transform.SetParent(_m_ParentBehavior.transform);

        ResourceManager.GetInstance().GetResource(ResourceType.OrderDisplay, 1, true, (_loadedObject) =>
        {
            int count = 30;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_OrderDisplay_Move))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_OrderDisplay_Move, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_OrderDisplay_Move].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_OrderDisplay_Move, out GameObject _WO_OrderDisplayMove_HierarchyBehavior);
                _newInstance.transform.SetParent(_WO_OrderDisplayMove_HierarchyBehavior.transform);
            }

            _isLoaded = true;
        }); // 이동 포인트

        await UniTask.WaitUntil(() => _isLoaded == true);

        GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_OrderDisplay_Attack, out GameObject _WO_OrderDisplayAttack_HierarchyBehavior);
        _WO_OrderDisplayAttack_HierarchyBehavior.transform.SetParent(_m_ParentBehavior.transform);

        ResourceManager.GetInstance().GetResource(ResourceType.OrderDisplay, 2, true, (_loadedObject) =>
        {
            int count = 30;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_OrderDisplay_Attack))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_OrderDisplay_Attack, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_OrderDisplay_Attack].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_OrderDisplay_Attack, out GameObject _WO_OrderDisplayAttack_HierarchyBehavior);
                _newInstance.transform.SetParent(_WO_OrderDisplayAttack_HierarchyBehavior.transform);
            }

            _isLoaded = true;
        }); // 이동 포인트

        await UniTask.WaitUntil(() => _isLoaded == true);

        GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_DamageTag, out GameObject _WO_UIDamageTab_HierarchyBehavior);
        _WO_UIDamageTab_HierarchyBehavior.transform.SetParent(_m_ParentBehavior.transform);

        ResourceManager.GetInstance().GetResource(ResourceType.UIWO, 1, true, (_loadedObject) =>
        {
            int count = 30;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_DamageTag))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_DamageTag, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_DamageTag].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_DamageTag, out var _hierarchyBehavior);
                _newInstance.transform.SetParent(_hierarchyBehavior.transform);
            }

            _isLoaded = true;
        }); // 데미지 태그

        await UniTask.WaitUntil(() => _isLoaded == true);

        GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_CoinCountTag, out GameObject _WO_UICoinCountTagTab_HierarchyBehavior);
        _WO_UICoinCountTagTab_HierarchyBehavior.transform.SetParent(_m_ParentBehavior.transform);

        ResourceManager.GetInstance().GetResource(ResourceType.UIWO, 2, true, (_loadedObject) =>
        {
            int count = 30;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_CoinCountTag))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_CoinCountTag, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_CoinCountTag].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_CoinCountTag, out var _hierarchyBehavior);
                _newInstance.transform.SetParent(_hierarchyBehavior.transform);
            }

            _isLoaded = true;
        }); // Coin Count 태그

        await UniTask.WaitUntil(() => _isLoaded == true);

        GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_WorldHealBarTag, out GameObject _WO_UIWorldHealBarTagTab_HierarchyBehavior);
        _WO_UIWorldHealBarTagTab_HierarchyBehavior.transform.SetParent(_m_ParentBehavior.transform);

        ResourceManager.GetInstance().GetResource(ResourceType.UIWO, 3, true, (_loadedObject) =>
        {
            int count = 30;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_WorldHealBarTag))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_WorldHealBarTag, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_WorldHealBarTag].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_WorldHealBarTag, out var _hierarchyBehavior);
                _newInstance.transform.SetParent(_hierarchyBehavior.transform);
            }

            _isLoaded = true;
        }); // Coin Count 태그

        await UniTask.WaitUntil(() => _isLoaded == true);
    }

    public void GetPooledObject(PooledObject _pooledCategory, PooledObjectInner _pooledInnerCategory, out IPoolBase _ret)
    {
        _ret = null;

        if (!EnoughObjectCount(_pooledCategory, _pooledInnerCategory))
            return;

        GetObjectFirst(_pooledCategory, _pooledInnerCategory, out _ret);

        if(!IsCheckEnoughCount(_pooledCategory, _pooledInnerCategory, 10))
        {
            ProduceObject(_pooledCategory, _pooledInnerCategory, 3);
        }
    }

    private bool EnoughObjectCount(PooledObject _pooledCategory, PooledObjectInner _pooledInnerCategory)
    {
        bool _ret = false;

        if (!_m_Dict_PoolObject.ContainsKey(_pooledCategory))
            _m_Dict_PoolObject.Add(_pooledCategory, new Dictionary<PooledObjectInner, List<IPoolBase>>());

        if (!_m_Dict_PoolObject[_pooledCategory].ContainsKey(_pooledInnerCategory))
            _m_Dict_PoolObject[_pooledCategory].Add(_pooledInnerCategory, new List<IPoolBase>());

        _ret = _m_Dict_PoolObject[_pooledCategory][_pooledInnerCategory].Count > 0;
        return _ret;
    }

    private void GetObjectFirst(PooledObject _pooledCategory, PooledObjectInner _pooledInnerCategory, out IPoolBase _ret)
    {
        _ret = _m_Dict_PoolObject[_pooledCategory][_pooledInnerCategory][0];
        _m_Dict_PoolObject[_pooledCategory][_pooledInnerCategory].RemoveAt(0);
    }

    private bool IsCheckEnoughCount(PooledObject _pooledCategory, PooledObjectInner _pooledInnerCategory, int _lowerCount)
    {
        bool _ret = false;

        if (!_m_Dict_PoolObject.ContainsKey(_pooledCategory))
            _m_Dict_PoolObject.Add(_pooledCategory, new Dictionary<PooledObjectInner, List<IPoolBase>>());

        if (!_m_Dict_PoolObject[_pooledCategory].ContainsKey(_pooledInnerCategory))
            _m_Dict_PoolObject[_pooledCategory].Add(_pooledInnerCategory, new List<IPoolBase>());

        _ret = _m_Dict_PoolObject[_pooledCategory][_pooledInnerCategory].Count >= _lowerCount;
        return _ret;
    }

    private void ProduceObject(PooledObject _pooledCategory, PooledObjectInner _pooledInnerCategory, int _createCount)
    {
        switch (_pooledCategory)
        {
            case PooledObject.WO:
                ProduceObject_WO(_pooledInnerCategory, _createCount);
                break;
            case PooledObject.NameTag:
                break;
            case PooledObject.Effect:
                break;
            default:
                break;
        }
    }

    private void ProduceObject_WO(PooledObjectInner _pooledInnerCategory, int _createCount)
    {
        switch (_pooledInnerCategory)
        {
            case PooledObjectInner.WO_Projectile_Arrow:
                ProduceObject_WO_Projectile_Arrow(_createCount);
                break;
            case PooledObjectInner.WO_Projectile_FireBall:
                ProduceObject_WO_Projectile_Fireball(_createCount);
                break;
            case PooledObjectInner.WO_OrderDisplay_Move:
                ProduceObject_WO_OrderDisplayMove(_createCount);
                break;
            case PooledObjectInner.WO_OrderDisplay_Attack:
                ProduceObject_WO_OrderDisplayAttack(_createCount);
                break;
            case PooledObjectInner.WO_DamageTag:
                ProduceObject_WO_DamageTag(_createCount);
                break;
            case PooledObjectInner.WO_CoinCountTag:
                ProduceObject_WO_CoinCountTag(_createCount);
                break;
            case PooledObjectInner.WO_WorldHealBarTag:
                ProduceObject_WO_WorldHealBarTag(_createCount);
                break;
            default:
                break;
        }
    }

    private void ProduceObject_WO_Projectile_Arrow(int _createCount)
    {
        ResourceManager.GetInstance().GetResource(ResourceType.Projectile, 1, true, (_loadedObject) =>
        {
            int count = _createCount;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_Projectile_Arrow))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_Projectile_Arrow, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_Projectile_Arrow].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_Projectile_Arrow, out GameObject _HierarchyBehavior);
                _newInstance.transform.SetParent(_HierarchyBehavior.transform);
            }
        }); // 화살
    }

    private void ProduceObject_WO_Projectile_Fireball(int _createCount)
    {
        ResourceManager.GetInstance().GetResource(ResourceType.Projectile, 1, true, (_loadedObject) =>
        {
            int count = _createCount;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_Projectile_FireBall))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_Projectile_FireBall, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_Projectile_FireBall].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_Projectile_FireBall, out GameObject _HierarchyBehavior);
                _newInstance.transform.SetParent(_HierarchyBehavior.transform);
            }
        }); // 화살
    }

    private void ProduceObject_WO_OrderDisplayMove(int _createCount)
    {
        ResourceManager.GetInstance().GetResource(ResourceType.OrderDisplay, 1, true, (_loadedObject) =>
        {
            int count = _createCount;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_OrderDisplay_Move))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_OrderDisplay_Move, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_OrderDisplay_Move].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_OrderDisplay_Move, out GameObject _HierarchyBehavior);
                _newInstance.transform.SetParent(_m_ParentBehavior.transform);
            }
        }); // 이동 포인트
    }
    private void ProduceObject_WO_OrderDisplayAttack(int _createCount)
    {
        ResourceManager.GetInstance().GetResource(ResourceType.OrderDisplay, 2, true, (_loadedObject) =>
        {
            int count = _createCount;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_OrderDisplay_Attack))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_OrderDisplay_Attack, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_OrderDisplay_Attack].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_OrderDisplay_Attack, out GameObject _HierarchyBehavior);
                _newInstance.transform.SetParent(_HierarchyBehavior.transform);
            }
        }); // 공격 포인트
    }

    private void ProduceObject_WO_DamageTag(int _createCount)
    {
        ResourceManager.GetInstance().GetResource(ResourceType.UIWO, 1, true, (_loadedObject) =>
        {
            int count = _createCount;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_DamageTag))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_DamageTag, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_DamageTag].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_DamageTag, out GameObject _HierarchyBehavior);
                _newInstance.transform.SetParent(_HierarchyBehavior.transform);
            }
        }); // 데미지 태그
    }

    private void ProduceObject_WO_CoinCountTag(int _createCount)
    {
        ResourceManager.GetInstance().GetResource(ResourceType.UIWO, 2, true, (_loadedObject) =>
        {
            int count = _createCount;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_CoinCountTag))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_CoinCountTag, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_CoinCountTag].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_CoinCountTag, out GameObject _HierarchyBehavior);
                _newInstance.transform.SetParent(_HierarchyBehavior.transform);
            }
        }); // 데미지 태그
    }

    private void ProduceObject_WO_WorldHealBarTag(int _createCount)
    {
        ResourceManager.GetInstance().GetResource(ResourceType.UIWO, 3, true, (_loadedObject) =>
        {
            int count = _createCount;

            for (int i = 0; i < count; i++)
            {
                var _newInstance = GameObject.Instantiate(_loadedObject as GameObject);
                var _iPooled = _newInstance.GetComponent<IPoolBase>();

                _newInstance.SetActive(false);

                if (!_m_Dict_PoolObject.ContainsKey(PooledObject.WO))
                    _m_Dict_PoolObject.Add(PooledObject.WO, new Dictionary<PooledObjectInner, List<IPoolBase>>());

                if (!_m_Dict_PoolObject[PooledObject.WO].ContainsKey(PooledObjectInner.WO_WorldHealBarTag))
                    _m_Dict_PoolObject[PooledObject.WO].Add(PooledObjectInner.WO_WorldHealBarTag, new List<IPoolBase>());

                _m_Dict_PoolObject[PooledObject.WO][PooledObjectInner.WO_WorldHealBarTag].Add(_iPooled);

                GetHierarchyParent(PooledObject.WO, PooledObjectInner.WO_WorldHealBarTag, out GameObject _HierarchyBehavior);
                _newInstance.transform.SetParent(_HierarchyBehavior.transform);
            }
        }); // 데미지 태그
    }

    


    public void CollectObject(PooledObject _pooledCategory, PooledObjectInner _pooledInnerCategory, ref IPoolBase _collectedObject)
    {
        var _collectedGameObject = (_collectedObject as PooledBase).gameObject;
        _collectedGameObject.SetActive(false);

        if(IsOverLimitCount(_pooledCategory, _pooledInnerCategory,30))
        {
            GameObject.Destroy(_collectedGameObject);
        }
        else
        {
            if (!_m_Dict_PoolObject.ContainsKey(_pooledCategory))
                _m_Dict_PoolObject.Add(_pooledCategory, new Dictionary<PooledObjectInner, List<IPoolBase>>());

            if (!_m_Dict_PoolObject[_pooledCategory].ContainsKey(_pooledInnerCategory))
                _m_Dict_PoolObject[_pooledCategory].Add(_pooledInnerCategory, new List<IPoolBase>());

            _m_Dict_PoolObject[_pooledCategory][_pooledInnerCategory].Add(_collectedObject);

            CollectSetHierarchyParent(_pooledCategory, _pooledInnerCategory, ref _collectedObject);
        }
    }

    public void CollectSetHierarchyParent(PooledObject _pooledCategory, PooledObjectInner _pooledInnerCategory, ref IPoolBase _collectedObject)
    {
        GetHierarchyParent(_pooledCategory, _pooledInnerCategory, out GameObject _HierarchyBehavior);

        PooledBase _pooled = _collectedObject as PooledBase;
        _pooled.transform.SetParent(_HierarchyBehavior.transform);
    }

    private bool IsOverLimitCount(PooledObject _pooledCategory, PooledObjectInner _pooledInnerCategory, int _upperCount)
    {
        bool _ret = false;

        if (!_m_Dict_PoolObject.ContainsKey(_pooledCategory))
            _m_Dict_PoolObject.Add(_pooledCategory, new Dictionary<PooledObjectInner, List<IPoolBase>>());

        if (!_m_Dict_PoolObject[_pooledCategory].ContainsKey(_pooledInnerCategory))
            _m_Dict_PoolObject[_pooledCategory].Add(_pooledInnerCategory, new List<IPoolBase>());

        return _ret = _m_Dict_PoolObject[_pooledCategory][_pooledInnerCategory].Count >= _upperCount;
    }

    #region Used_Hierarchy

    private void GetHierarchyParent(PooledObject _pooledCategory, PooledObjectInner _pooledInnerCategory, out GameObject _ret )
    {
        _ret = null;

        if (!_m_Dict_HierarchyBehavior.ContainsKey(_pooledCategory))
            _m_Dict_HierarchyBehavior.Add(_pooledCategory, new Dictionary<PooledObjectInner, GameObject>());

        if (!_m_Dict_HierarchyBehavior[_pooledCategory].ContainsKey(_pooledInnerCategory))
        {
            var _parentHierarchy = new GameObject($"{_pooledCategory.ToString()}_{_pooledInnerCategory.ToString()}");
            _m_Dict_HierarchyBehavior[_pooledCategory].Add(_pooledInnerCategory, _parentHierarchy);
        }

        _ret = _m_Dict_HierarchyBehavior[_pooledCategory][_pooledInnerCategory];
    }

    #endregion
}
