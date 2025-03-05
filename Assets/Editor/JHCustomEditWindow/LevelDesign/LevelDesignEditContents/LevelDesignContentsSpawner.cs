using System.Collections.Generic;
using UnityEngine;
using GlobalGameDataSpace;
using UnityEditor;
using System.Text;
using System.Xml;
using Unity.VisualScripting;
using System.IO;
using System.Text.RegularExpressions;

public class LevelDesignContentsSpawner : ILevelDegineEditContents
{
    bool _mb_IsEditFile = false;
    int _mi_SpawnerKey = 0;

    bool _mb_isSpawnerInfoFoldOut = false;
    Vector3 _mv3_ScrollPos = Vector3.zero;

    public void OnUpdateLevelDesign()
    {
        if (EditorApplication.isPlaying)
            _mb_IsEditFile = true;

        if (_mb_IsEditFile)
        {
            OnUpdateSpawnerToolButton();
            OnUpdateSpawnerInfo();
        }
        else
        {
            OnUpdateNewOrLoadButton();
        }
    }
    private void OnUpdateNewOrLoadButton()
    {
        EditorGUILayout.LabelField("Select Edit Mode", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        GUILayoutOption[] options = new[] { GUILayout.Width(110), GUILayout.Height(20) };

        GUILayout.Space(2);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        if (GUILayout.Button($"New", options))
        {
            // New Map File
            _mb_IsEditFile = true;
        }

        GUILayout.Space(2);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        if (GUILayout.Button($"Load", options))
        {
            string _path = string.Empty;
            PathManager.GetInstance().GetGameDataPath(out _path);
            _path += "/SpawnerData";
            string _ret = EditorUtility.OpenFilePanel($"파일 로드", _path, $"xml");
            
            if (!string.IsNullOrEmpty(_ret))
            {
                if (!File.Exists(_ret)) return;

                string _fileName = Path.GetFileName(_ret);
                int _relativeID = int.Parse(Regex.Replace(_fileName, "[^0-9]", ""));

                ResourceManager.GetInstance().GetResource(ResourceType.SpawnerData, _relativeID, true, (_gObj_SpawnerXMLData) =>
                {
                    UnityEngine.Object _xmlObject = _gObj_SpawnerXMLData as UnityEngine.Object;
                    var _mapText = _xmlObject as TextAsset;

                    if (string.IsNullOrEmpty(_mapText.text))
                    {
                        UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"LoadStage", $" Loaded Stage Idx : {_relativeID} Spawner File Empty");
                        return;
                    }

                    SpawnerWrap _loadedSpawnerData = XMLUtility.Deserialize<SpawnerWrap>(_mapText.text);

                    for (int i = 0; i < _loadedSpawnerData.List.Count; ++i)
                    {
                        var _element = _loadedSpawnerData.List[i];
                        int _index = _element._mi_IndexID;
                        var _ISpawner = _element as ISpawnerBase;

                        SpawnerManager.GetInstance().GetSpawnerDatas(out var spawnerDatas);

                        if (spawnerDatas.ContainsKey(_index) == true)
                        {
                            UnityLogger.GetInstance().LogFuncFailed(this.GetType().Name, $"LoadStage", $"stage ID {_relativeID}, 동일한 스포너 ID가 있습니다. 게임 종료하겠습니다. 확인해주세요!");
                            Application.Quit();
                        }
                        SpawnerManager.GetInstance().AddSpawner(_index, ref _ISpawner);
                    }

                    _mb_IsEditFile = true; // 로딩 성공 시,
                });
                
            }
            // Load Map File
        }

        EditorGUILayout.EndHorizontal();
    }

    private void OnUpdateSpawnerToolButton()
    {
        EditorGUILayout.LabelField("Spawner EditWindow", EditorStyles.boldLabel);
        GUILayoutOption[] options;

        EditorGUILayout.BeginHorizontal();

        GUILayout.Space(2);
        options = new[] { GUILayout.Width(105), GUILayout.Height(20) };
        if (GUILayout.Button($"+ Add Spawner", options))
        {
            // New Map File
            int _spawnerKey = SpawnerManager.GetInstance().GetSpawnerKey();
            SpawnerUseEdit _spawner = new SpawnerUseEdit();
            _spawner._mi_IndexID = _spawnerKey;
            ISpawnerBase _spawnerBase = _spawner as ISpawnerBase;

            SpawnerManager.GetInstance().AddSpawner(_spawnerKey, ref _spawnerBase);
        }

        GUILayout.Space(2);
        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        if (GUILayout.Button($"Save", options))
        {
            if (!SpawnerManager.GetInstance().IsContainAnyData())
            {
                EditorUtility.DisplayDialog("알림", "저장할 데이터가 없습니다.", "확인");
                return;
            }
            else
            {
                string _path = string.Empty;
                PathManager.GetInstance().GetGameDataPath(out _path);
                _path += "/SpawnerData";

                string _ret = EditorUtility.SaveFilePanel($"저장", _path, $"", $"xml");

                if (!string.IsNullOrEmpty(_ret))
                {
                    SpawnerManager.GetInstance().GetSpawnerDatas(out var _dict_Spawner);

                    SpawnerWrap _data = new SpawnerWrap();

                    foreach(var spawner in _dict_Spawner)
                    {
                        SpawnerBase _spawnerBase = spawner.Value as SpawnerBase;
                        _data.List.Add(_spawnerBase);
                    }

                    XMLUtility.Serialize<SpawnerWrap>(_ret, _data);
                }
            }
        }

        GUILayout.Space(2);
        options = new[] { GUILayout.Width(95), GUILayout.Height(20) };
        if (GUILayout.Button($"Clear && Load", options))
        {
            if (SpawnerManager.GetInstance().IsContainAnyData())
            {
                bool _userRet = EditorUtility.DisplayDialog($"NOTICE!!", $"현재 작업중인 Spawner Data를 모두 삭제하고 새롭게 로드합니다. \n OK를 누르시면 로드할 파일을 선택할 수 있습니다.", $"OK", $"Cancle");

                if (_userRet)
                {
                    string _path = string.Empty;
                    PathManager.GetInstance().GetGameDataPath(out _path);
                    _path += "/SpawnerData";

                    if (!string.IsNullOrEmpty(EditorUtility.OpenFilePanel($"파일 로드", _path, $"xml")))
                    {
                        _mb_IsEditFile = true; // 로딩 성공 시,
                        SpawnerManager.GetInstance().ClearSpawner();
                        // 스포너 삭제하고 새롭게 로드가 필요
                    }
                }
            }
        }

        GUILayout.Space(2);
        options = new[] { GUILayout.Width(80), GUILayout.Height(20) };
        if (GUILayout.Button($"Quit Edit", options))
        {
            bool _userRet = EditorUtility.DisplayDialog($"NOTICE!!", $"현재 작업중인 Spawner Data를 모두 삭제합니다. \n실행할까요? ", $"OK", $"Cancle");

            // 종료 전에 팝업
            if (_userRet && SpawnerManager.GetInstance().IsContainAnyData())
            {
                SpawnerManager.GetInstance().ClearSpawner();
            }
            _mb_IsEditFile = false;
        }

        EditorGUILayout.EndHorizontal();

    }

    private void OnUpdateSpawnerInfo()
    {
        // 스포너 정보, 
        // Spanwer Index, Spanwer Pos, Target Spawn Index, Spawn Count, Init Delay, Spawn Delay

        EditorGUILayout.LabelField("Grid Nav Element Data", EditorStyles.boldLabel);

        _mb_isSpawnerInfoFoldOut = EditorGUILayout.Foldout(_mb_isSpawnerInfoFoldOut, "Spawner Info");

        if (_mb_isSpawnerInfoFoldOut)
        {
            SpawnerManager.GetInstance().GetSpawnerDatas(out var _dict_Spawner);

            if (!SpawnerManager.GetInstance().IsContainAnyData())
            {
                // No Data
                EditorGUILayout.LabelField("No Datas");
            }
            else
            {
                _mv3_ScrollPos = EditorGUILayout.BeginScrollView(_mv3_ScrollPos, GUILayout.ExpandHeight(false));

                EditorGUILayout.BeginVertical();
                string _index;

                EditorGUI.indentLevel += 1;

                foreach (var _spawnerPair in _dict_Spawner)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(EditorGUI.indentLevel * 18); // << IndentLevel에 따라 공간 추가

                    SpawnerUseEdit _editSpawner = _spawnerPair.Value as SpawnerUseEdit;

                    // Index
                    _index = $"Idx : {_spawnerPair.Key}";
                    GUILayout.Label(_index, GUILayout.Width(65));

                    // Position X
                    GUILayoutOption[] options = new[] { GUILayout.Width(50), GUILayout.Height(20) };

                    GUILayout.Label($"Pos X", GUILayout.Width(35));
                    GUILayout.Space(-8);
                    _editSpawner._mv3_Pos.x = EditorGUILayout.FloatField(_editSpawner._mv3_Pos.x, options);

                    // Position Y
                    GUILayout.Label($"Pos Y", GUILayout.Width(35));
                    GUILayout.Space(-8);
                    _editSpawner._mv3_Pos.y = EditorGUILayout.FloatField(_editSpawner._mv3_Pos.y, options);

                    GUILayout.Space(1);

                    // TargetSpawn Index
                    GUILayout.Label($"Target ID", GUILayout.Width(54));
                    GUILayout.Space(-8);
                    _editSpawner._mStr_SpawnIdx = EditorGUILayout.TextField(_editSpawner._mStr_SpawnIdx, new[] { GUILayout.Width(55), GUILayout.Height(20) });

                    GUILayout.Space(1);

                    // TargetSpawn Count
                    GUILayout.Label($"Spawn Count", GUILayout.Width(75));
                    GUILayout.Space(-8);
                    _editSpawner._mi_SpawnCount = EditorGUILayout.IntField(_editSpawner._mi_SpawnCount, options);

                    GUILayout.Space(1);

                    // Init Spawn Delay
                    GUILayout.Label($"Init Spawn Delay", GUILayout.Width(100));
                    GUILayout.Space(-8);
                    _editSpawner._mf_InitSpawnDelay = EditorGUILayout.FloatField(_editSpawner._mf_InitSpawnDelay, options);

                    // Spawn Delay
                    GUILayout.Label($"Spawn Delay", GUILayout.Width(80));
                    GUILayout.Space(-8);
                    _editSpawner._mf_SpawnDelay = EditorGUILayout.FloatField(_editSpawner._mf_SpawnDelay, options);

                    // Selected Object Emphasize
                    if (GUILayout.Button("Select", GUILayout.Width(65)))
                    {
                        UnityLogger.GetInstance().Log($"Button clicked at index: [{_spawnerPair.Key}]");
                        SpawnerManager.GetInstance().SelectedSpawnerInfo = _spawnerPair.Value;
                        SceneView.RepaintAll();
                    }

                    GUILayout.EndHorizontal();
                }

                EditorGUI.indentLevel -= 1;

                EditorGUILayout.EndVertical();
                EditorGUILayout.EndScrollView();
            }
































        }
    }
}

