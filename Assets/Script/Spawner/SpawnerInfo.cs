using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using System.Xml.Serialization;
using Unity.VisualScripting;

public interface ISpawnerBase
{
    public void Initialize();
    public ISpawnerBase Clone();
}

public class SpawnerBase
{
    public int _mi_IndexID;
    public Vector3 _mv3_Pos;

    public string _mStr_SpawnIdx; // ���� �ε���
    public int _mi_SpawnCount; // ���� ��ǥ ����

    [XmlIgnore] // XML ȭ ����
    public int[] _miAr_SpawnIdx; // Property�� GetValue �� �������� ����. 

    public float _mf_InitSpawnDelay;
    public float _mf_SpawnDelay;

    public bool _isMySpwner; // �� �����ʸ� Probe ������ �ٲ����
}

/// <summary>
/// EditTool ��� ���� Spanwer �Դϴ�.
/// </summary>
[Serializable]
public class SpawnerUseEdit : SpawnerBase, ISpawnerBase
{
    [XmlIgnore]
    public int _mi_CreateCount; // ������ ����

    [XmlIgnore]
    public float _mf_AccInitDelayTime; // �ʱ� ��� �ð�

    [XmlIgnore]
    public float _mf_AccEntityCreateDelayTime; // ��ƼƼ �� ����  ��� �ð�

    [XmlIgnore]
    private bool _mb_IsSpawning; // ���� ���� ��������?

    public SpawnerUseEdit()
    {
        _mi_IndexID = 0;
        _mv3_Pos = Vector3.zero;

        _mStr_SpawnIdx = string.Empty;
        _mi_SpawnCount = 0;

        _mf_InitSpawnDelay = 0f;
        _mf_SpawnDelay = 0f;

        _mb_IsSpawning = false;
    }

    public ISpawnerBase Clone()
    {
        return new SpawnerUseEdit()
        {
            _mi_IndexID = this._mi_IndexID,
            _mv3_Pos = new Vector3(this._mv3_Pos.x, this._mv3_Pos.y, this._mv3_Pos.z),

            _mStr_SpawnIdx = this._mStr_SpawnIdx,
            _mi_SpawnCount = this._mi_SpawnCount,

            _mf_InitSpawnDelay = this._mf_InitSpawnDelay,
            _mf_SpawnDelay = this._mf_SpawnDelay
        };
    }

    public void Initialize()
    {
        _CancellationToken = new CancellationTokenSource();
    }

    public void StopSpawn()
    {
        _CancellationToken.Cancel();
        _CancellationToken.Dispose();
    }

    public void ClearSpawnCount()
    {
        _mi_SpawnCount = 0;
        _mb_IsSpawning = false;
    }

    public void StartSpawn()
    {
        _mb_IsSpawning = true;
        _ = Spawn();
    }

    public bool IsSpawning() => _mb_IsSpawning;

    public async UniTask Spawn()
    {
        await WaitInitDelay(); // �ʱ� �ð� ���

        _mf_AccEntityCreateDelayTime = 0f;
        var _factory = new RivalEntityFactory();

        while (_mi_CreateCount < _mi_SpawnCount)
        {
            if (_CancellationToken.IsCancellationRequested)
                break;

            if (_mf_AccEntityCreateDelayTime >= _mf_SpawnDelay)
            {
                // ����
                _mf_AccEntityCreateDelayTime = 0f;
                ++_mi_CreateCount;

                float _randomX = 0f;// UnityEngine.Random.Range(-0.5f, 0.5f);
                float _randomY = 0f;// UnityEngine.Random.Range(-0.5f, 0.5f);

                int _spawnEnitityID = int.Parse(_mStr_SpawnIdx);

                Vector3 _newSpawnPos = new Vector3(_mv3_Pos.x,_mv3_Pos.y,_mv3_Pos.z);

                _newSpawnPos.x += _randomX;
                _newSpawnPos.y += _randomY;

                _ = _factory.CreateEntity(_spawnEnitityID, _newSpawnPos,_isMySpwner, (_createEntity) => 
                {
                    _createEntity.Controller._onCB_DiedProcess -= () => { _createEntity.Controller.OnDieEvent(_createEntity); };
                    _createEntity.Controller._onCB_DiedProcess += () => { _createEntity.Controller.OnDieEvent(_createEntity); };
                } );
            }

            _mf_AccEntityCreateDelayTime += Time.deltaTime;

            await UniTask.Yield(cancellationToken: _CancellationToken.Token);
        }

        _mb_IsSpawning = false;
    }

    public async UniTask WaitInitDelay()
    {
        _mf_AccInitDelayTime = 0f;

        while (_mf_AccInitDelayTime <= _mf_InitSpawnDelay)
        {
            if (_CancellationToken.IsCancellationRequested)
                break;

            _mf_AccInitDelayTime += Time.deltaTime;
            await UniTask.Yield(_CancellationToken.Token);
        }
    }

    CancellationTokenSource _CancellationToken;
}

[Serializable]
[XmlInclude(typeof(SpawnerUseEdit))]
public class SpawnerWrap
{
    public List<SpawnerBase> List = new List<SpawnerBase>();
}

