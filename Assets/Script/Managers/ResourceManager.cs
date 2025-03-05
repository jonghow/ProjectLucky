using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum ResourceType
{
    MapData,                // ������ ���� ��Ÿ���� ������ Ÿ��
    MapNaviData,        // ���� �̵��� �Ǵ��ϴ� �׸��� �׺���̼� Ÿ��
    SpawnerData,         // ���� ���� ���� ������ Ÿ��

    Entity,
    NPCEntity,
    FactoryEntity, // ���� Ÿ��

    HandCardData, // ���� ������

    PlayerAnimationController, // �÷��̾� �ִϸ��̼� ��Ʈ�ѷ�
    EnemyAnimationController, // ���� �ִϸ��̼� ��Ʈ�ѷ�
    MealFactoryAnimationController, // �����丮 �ִϸ��̼� ��Ʈ�ѷ�

    HUD,

    Projectile,
    OrderDisplay, // 
    UIWO,
    PortraitAtlas,
    Effect
}

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager Instance;

    public static ResourceManager GetInstance()
    {
        if (Instance == null)
        {
            GameObject obj = new GameObject("ResourceManager");
            Instance = obj.AddComponent<ResourceManager>();
        }

        return Instance;
    }

    private Dictionary<ResourceType, Dictionary<string, UnityEngine.Object>> _dic_Objects = new Dictionary<ResourceType, Dictionary<string, UnityEngine.Object>>();
    private Dictionary<AssetBundleType, AssetBundle> _dic_AssetBundle = new Dictionary<AssetBundleType, AssetBundle>();

    CancellationTokenSource _cancellationToken;

    public void GetResource(ResourceType _resourceType, int _relativeID, bool _isCache, Action<GameObject> _callback)
    {
        string _addressKey = string.Empty;

        switch (_resourceType)
        {
            case ResourceType.MapData:
                UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"GetResource", $"Not Defined Logic ResourceType.MapData Case.");
                break;
            case ResourceType.FactoryEntity:
                ClientUtility.ConvertMealFactoryIDToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.SpawnerData:
                ClientUtility.ConvertSpawnerIDToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.HandCardData:
                ClientUtility.ConvertMealHandCardIDToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.MapNaviData:
                _addressKey = $"stage{_relativeID}";
                break;
            case ResourceType.Entity:
                ClientUtility.ConvertJobIDToAddressableKey(_relativeID, out _addressKey);
                // Converting Entity Table ID To AddressableKey
                break;
            case ResourceType.NPCEntity:
                ClientUtility.ConvertMobIDToAddressableKey(_relativeID, out _addressKey);
                // Converting Entity Table ID To AddressableKey
                break;
            case ResourceType.Projectile:
                ClientUtility.ConvertProjectileIDToAddressableKey(_relativeID, out _addressKey);
                // Converting Entity Table ID To AddressableKey
                break;
            case ResourceType.OrderDisplay:
                ClientUtility.ConvertOrderDisplayIDToAddressableKey(_relativeID, out _addressKey);
                // Converting Entity Table ID To AddressableKey
                break;
            case ResourceType.PlayerAnimationController:
                ClientUtility.ConvertPlayerAnimationControllerToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.EnemyAnimationController:
                ClientUtility.ConvertEnemyAnimationControllerToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.MealFactoryAnimationController:
                ClientUtility.ConvertMealFactoryAnimationControllerToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.HUD:
                ClientUtility.ConvertHUDToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.Effect:
                break;
            case ResourceType.UIWO:
                ClientUtility.ConvertUIWOToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.PortraitAtlas:
                ClientUtility.ConvertAtlasKeyToAddressableKey(_relativeID, out _addressKey);
                break;
            default:
                break;
        }

        if(_dic_Objects.TryGetValue(_resourceType, out var _dict_Inner_Object))
        {
            if(_dict_Inner_Object.TryGetValue(_addressKey, out var _findObject))
            {
                var _castObject = _findObject as GameObject;
                _callback?.Invoke(_castObject);
                return;
            }
        }

        GetResourceAddressable(_resourceType, _addressKey, _isCache, _callback);
    }
    public void GetResource(ResourceType _resourceType, int _relativeID, bool _isCache, Action<UnityEngine.Object> _callback)
    {
        string _addressKey = string.Empty;

        switch (_resourceType)
        {
            case ResourceType.MapData:
                UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"GetResource", $"Not Defined Logic ResourceType.MapData Case.");
                break;
            case ResourceType.FactoryEntity:
                ClientUtility.ConvertMealFactoryIDToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.SpawnerData:
                ClientUtility.ConvertSpawnerIDToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.HandCardData:
                ClientUtility.ConvertMealHandCardIDToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.MapNaviData:
                _addressKey = $"stage{_relativeID}";
                break;
            case ResourceType.Entity:
                ClientUtility.ConvertJobIDToAddressableKey(_relativeID, out _addressKey);
                // Converting Entity Table ID To AddressableKey
                break;
            case ResourceType.NPCEntity:
                ClientUtility.ConvertMobIDToAddressableKey(_relativeID, out _addressKey);
                // Converting Entity Table ID To AddressableKey
                break;
            case ResourceType.Projectile:
                ClientUtility.ConvertProjectileIDToAddressableKey(_relativeID, out _addressKey);
                // Converting Entity Table ID To AddressableKey
                break;
            case ResourceType.OrderDisplay:
                ClientUtility.ConvertOrderDisplayIDToAddressableKey(_relativeID, out _addressKey);
                // Converting Entity Table ID To AddressableKey
                break;
            case ResourceType.PlayerAnimationController:
                ClientUtility.ConvertPlayerAnimationControllerToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.EnemyAnimationController:
                ClientUtility.ConvertEnemyAnimationControllerToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.MealFactoryAnimationController:
                ClientUtility.ConvertMealFactoryAnimationControllerToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.HUD:
                ClientUtility.ConvertHUDToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.Effect:
                break;
            case ResourceType.UIWO:
                ClientUtility.ConvertUIWOToAddressableKey(_relativeID, out _addressKey);
                break;
            case ResourceType.PortraitAtlas:
                ClientUtility.ConvertAtlasKeyToAddressableKey(_relativeID, out _addressKey);
                break;
            default:
                break;
        }

        if (_dic_Objects.TryGetValue(_resourceType, out var _dict_Inner_Object))
        {
            if (_dict_Inner_Object.TryGetValue(_addressKey, out var _findObject))
            {
                _callback?.Invoke(_findObject);
                return;
            }
        }

        GetResourceAddressable(_resourceType, _addressKey, _isCache, _callback);
    }



    // Unitask
    private async UniTask<UnityEngine.Object> UTGetResource(ResourceType _resourceType,string _fileName, bool _isCache, Action<UnityEngine.Object> _callback)
    {
        UnityEngine.Object _ret = null;

        // ���ٸ�,
        string path = Application.dataPath + $"/ResourceData/00.Entity/{_fileName}";
        var resourceRequest = Resources.LoadAsync<UnityEngine.Object>(path);

        await UniTask.WaitUntil(() => resourceRequest.isDone == true);

        _ret = resourceRequest.asset as UnityEngine.Object;

        if(_isCache == true)
        {
            if (_dic_Objects.ContainsKey(_resourceType) == false)
                _dic_Objects.Add(_resourceType, new Dictionary<string, UnityEngine.Object>());

            if (_dic_Objects[_resourceType].ContainsKey(_fileName))
                _dic_Objects[_resourceType][_fileName] = _ret;
        }

        _callback?.Invoke(_ret);
        return _ret;
    }
    private void GetResourceAddressable(ResourceType _resourceType, string _addressKey, bool _isCache, Action<GameObject> _callback)
    {
        Addressables.LoadAssetAsync<GameObject>(_addressKey).Completed += (op) =>
        {
            if(((AsyncOperationHandle<GameObject>)op).Status == AsyncOperationStatus.Succeeded)
            {
                var _loadedObject = op.Result;

                if(_isCache == true)
                {
                    if (_dic_Objects.ContainsKey(_resourceType) == false)
                        _dic_Objects.Add(_resourceType, new Dictionary<string, UnityEngine.Object>());

                    if (_dic_Objects[_resourceType].ContainsKey(_addressKey))
                        _dic_Objects[_resourceType][_addressKey] = _loadedObject;
                }

                _callback?.Invoke(_loadedObject);
            }
        };
    }
    private void GetResourceAddressable(ResourceType _resourceType, string _addressKey, bool _isCache, Action<UnityEngine.Object> _callback)
    {
        Addressables.LoadAssetAsync<UnityEngine.Object>(_addressKey).Completed += (op) =>
        {
            if (((AsyncOperationHandle<UnityEngine.Object>)op).Status == AsyncOperationStatus.Succeeded)
            {
                var _loadedObject = op.Result;

                if (_isCache == true)
                {
                    if (_dic_Objects.ContainsKey(_resourceType) == false)
                        _dic_Objects.Add(_resourceType, new Dictionary<string, UnityEngine.Object>());

                    if (_dic_Objects[_resourceType].ContainsKey(_addressKey))
                        _dic_Objects[_resourceType][_addressKey] = _loadedObject;
                }

                _callback?.Invoke(_loadedObject);
            }
        };
    }
    private void ClearUnitaskToken()
    {
        if (_cancellationToken != null)
        {
            _cancellationToken.Cancel();
            _cancellationToken.Dispose();
            _cancellationToken = null;
        }

        _cancellationToken = new CancellationTokenSource();
    }

    #region Based AssetBundle Loading 

    [Obsolete("���� ������� �ʽ��ϴ�. ���� ���� ��� �ε� ���, ResourceManager.GetResource() �� ������ּ���.")]
    private async UniTask<UnityEngine.Object> UTGetResourceAssetBundle(AssetBundleType _bundleType, ResourceType _resourceType, string _fileName, bool _isCache, Action<UnityEngine.Object> _callback)
    {
        AssetBundle _bundle = null;

        // ĳ�õ� AssetBundle�� �ִ��� Ȯ���ϰ�, ������ �񵿱� �ε�
        if (!_dic_AssetBundle.TryGetValue(_bundleType, out _bundle))
        {
            // AssetBundle �񵿱� �ε� �۾� ����
            var _myLoadedAssetBundle = AssetBundle.LoadFromFileAsync(Path.Combine(Application.streamingAssetsPath, "testassetbundle"));

            // �ε尡 �Ϸ�� ������ ��ٸ�
            await _myLoadedAssetBundle;

            _bundle = _myLoadedAssetBundle.assetBundle;

            // AssetBundle�� null�̸� �ε� ���� ó��
            if (_bundle == null)
            {
                Debug.LogError("Failed to load AssetBundle!");
                return null;
            }

            // �ε�� ���� ������ ĳ�ÿ� �߰�
            _dic_AssetBundle.Add(_bundleType, _bundle);
        }

        // Ư�� �ڻ� �񵿱� �ε�
        var _targetAsset = _bundle.LoadAssetAsync<UnityEngine.Object>("SampleMonster");

        // �ڻ� �ε尡 �Ϸ�� ������ ���
        await _targetAsset;

        var _asset = _targetAsset.asset;

        // �ݹ� ���� (�ݹ��� null�� �ƴ� ���)
        _callback?.Invoke(_asset);

        // �ε�� �ڻ� ��ȯ
        return _asset;
    }

    #endregion
}