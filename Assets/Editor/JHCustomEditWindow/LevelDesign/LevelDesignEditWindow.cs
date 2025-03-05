using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using GlobalGameDataSpace;
using System.Diagnostics;

/// <summary>
/// 캐릭터 생성에 필요한 생성 변수 입니다.
/// </summary>
public struct CharacterEditData
{
    public int _mi_JobID;
    public bool _mb_isStatusFoldOut;
    public bool _mb_isActiveSkillFoldOut;
    public bool _mb_isPassiveSkillFoldOut;

    public void InitData()
    {
        _mi_JobID = 1;
        _mb_isStatusFoldOut = false;
        _mb_isActiveSkillFoldOut = false;
        _mb_isPassiveSkillFoldOut = false;
    }
}

public struct NavigationEditData
{
    public int _mi_mapID;
    public int _mi_NavID;

    public string _mStr_QuickBatch;
    public int QuickBatchX
    {
        get
        {
            if (_mStr_QuickBatch == null || _mStr_QuickBatch.Length == 0)
                return 0;

            return int.Parse(_mStr_QuickBatch.Split('/')[0]);
        }
    }
    public int QuickBatchY
    {
        get
        {
            if (_mStr_QuickBatch == null || _mStr_QuickBatch.Length == 0)
                return 0;

            return int.Parse(_mStr_QuickBatch.Split('/')[1]);
        }
    }

    public bool _mb_isGridNavFoldOut;

    public Vector2 _mv2_GridNavScrollPos;
    public void InitData()
    {
        _mi_mapID = -1;
        _mi_NavID = -1;
        _mb_isGridNavFoldOut = false;
        _mv2_GridNavScrollPos = new Vector2(0f, 0f);
    }
}

public struct LevelDesignEditCommonData
{
    public string[] _mStr_LvEditToolBar;
    public Vector2[] _mv2_VerticalScrollPos;

    public int _mi_SelectedLvEditToolbar;

    public void InitData()
    {
        _mStr_LvEditToolBar = new string[3] { $"Character", $"Navigation", $"Spawner" };
        _mv2_VerticalScrollPos = new Vector2[3] { Vector2.zero, Vector2.zero, Vector2.zero };
        _mi_SelectedLvEditToolbar = 0;
    }
}

public class LevelDesignEditWindow : EditorWindow
{
    Entity _mEntity;
    NavigationElement _mNavElement;

    CharacterEditData _m_ChrEditData;
    NavigationEditData _m_NvEditData;
    LevelDesignEditCommonData _m_LvDesignEditComData;

    LevelDesignContentsSpawner _m_SpanwerUpdator;

    private void OnEnable()
    {
        _m_ChrEditData = new CharacterEditData();
        _m_ChrEditData.InitData();

        _m_NvEditData = new NavigationEditData();
        _m_NvEditData.InitData();

        _m_LvDesignEditComData = new LevelDesignEditCommonData();
        _m_LvDesignEditComData.InitData();

        _m_SpanwerUpdator = new LevelDesignContentsSpawner();

        RegistSelectedEvent();
    }

    private void OnDisable()
    {
        ReleaseSelectedEvent();
    }

    private void RegistSelectedEvent()
    {
        Selection.selectionChanged += OnChangeSelectObject;
    }

    private void ReleaseSelectedEvent()
    {
        Selection.selectionChanged -= OnChangeSelectObject;
    }

    private void OnGUI()
    {
        OnUpdateLevelDesign();
    }

    public void OnChangeSelectObject()
    {
        if (Selection.activeObject == null) return;
        if (Selection.activeGameObject == null) return;

        bool _isEntity = Selection.activeGameObject.GetComponent<Entity>() != null;

        if (_isEntity)
        {
            var _cEntity = Selection.activeGameObject.GetComponent<Entity>();
            _mEntity = _cEntity;
            Repaint();
            //UniDebug("HI");
        }
        else
        {
            _mEntity = null;
            Repaint();
        }
    }

    public void OnUpdateLevelDesign()
    {
        // 스크롤 뷰

        int _selectedToolBar = _m_LvDesignEditComData._mi_SelectedLvEditToolbar;
        _m_LvDesignEditComData._mi_SelectedLvEditToolbar = GUILayout.Toolbar(_selectedToolBar, _m_LvDesignEditComData._mStr_LvEditToolBar);

        switch (_selectedToolBar)
        {
            case 0:
                // NOTICE :: 캐릭터 생성 및 스테이터스 관리에 필요한 툴

                _m_LvDesignEditComData._mv2_VerticalScrollPos[_selectedToolBar] = EditorGUILayout.BeginScrollView(_m_LvDesignEditComData._mv2_VerticalScrollPos[_selectedToolBar]);

                OnDrawCharacterInstantiateAttribute();
                OnDrawCharacterStatus();
                OnDrawCharacterActiveSkills();
                OnDrawCharacterPassiveSkills();
                OnDrawCharacterPersonality();
                // TODO : 성격 시스템은 나중에

                // 스크롤 뷰 끝
                EditorGUILayout.EndScrollView();

                break;
            case 1:
                // NOTICE :: 맵 로드 및 생성에 관한 툴
                OnDrawMapInstantiateData();

                break;
            case 2:
                _m_SpanwerUpdator.OnUpdateLevelDesign();

                break;
        }
    }

    /// <summary>
    /// 캐릭터 생성 및 스테이터스 조작에 필요한 내용을 출력해주는 GUI Part 
    /// </summary>
    private void OnDrawCharacterInstantiateAttribute()
    {
        GUILayout.Label("Create Section", EditorStyles.boldLabel);

        GUILayout.BeginHorizontal();

        GUILayoutOption[] options = new[] { GUILayout.Width(40), GUILayout.Height(20) };
        GUILayout.Label($"Job ID", options);

        options = new[] { GUILayout.Width(60), GUILayout.Height(20) };
        _m_ChrEditData._mi_JobID = EditorGUILayout.IntField("", _m_ChrEditData._mi_JobID, options);

        GUILayout.Space(10);

        options = new[] { GUILayout.Width(150), GUILayout.Height(20) };
        if (GUILayout.Button("Create Selected JobID", options))
        {
            CreatePC();
        }

        options = new[] { GUILayout.Width(80), GUILayout.Height(20) };
        if (GUILayout.Button("Job List!", options))
        {
            // TODO : JobID에 관련한 정보를 나올 수 있도록 작업할 예정, 아직 테이블도 없음
        }

        GUILayout.EndHorizontal();

        // NOTICE :: 여기까지가 생성 섹션
    }
    private void OnDrawCharacterStatus()
    {
        EditorGUILayout.Space();

        if (_mEntity == null)
        {
            GUILayout.Label("No Selected Entity", EditorStyles.boldLabel);
            return;
        }

        GUILayout.Label("Selected Entity Section", EditorStyles.boldLabel);

        GUILayoutOption[] options = new[] { GUILayout.Width(200), GUILayout.Height(20) };
        GUILayout.Label($"UID : {_mEntity.UID}", options);  // UID

        GUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(65), GUILayout.Height(20) };
        GUILayout.Label($"Job ID : {_mEntity.JobID} ", options); // 직업 ID 

        options = new[] { GUILayout.Width(140), GUILayout.Height(20) };
        string _retString = string.Empty;
        ClientUtility.ConvertJobNameToJobID(_mEntity.JobID, out _retString);
        GUILayout.Label($"Job Name : {_retString}", options); // 직업 이름

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(160), GUILayout.Height(20) };
        ClientUtility.ConvertChrNameToJobID(_mEntity.JobID, out _retString);
        GUILayout.Label($"Nick Name : {_retString} ", options); // 캐릭터의 실제 이름

        options = new[] { GUILayout.Width(150), GUILayout.Height(20) };
        GUILayout.Label($"Set Map Index : x {99} , y {99}", options);

        GUILayout.EndHorizontal();

        _m_ChrEditData._mb_isStatusFoldOut = EditorGUILayout.Foldout(_m_ChrEditData._mb_isStatusFoldOut, "Status");

        if (_m_ChrEditData._mb_isStatusFoldOut)
        {
            // 폴드아웃이 열렸을 때 보여줄 내용 _ 스테이터스
            EditorGUI.indentLevel += 1;
            EditorGUILayout.LabelField($"- HP/MaxHP : {_mEntity.Info.HP}/{_mEntity.Info.MaxHP} ({_mEntity.Info.HPPercent}%)");
            EditorGUILayout.LabelField($"- MP/MaxMP : {_mEntity.Info.MP}/{_mEntity.Info.MaxMP} ({_mEntity.Info.MPPercent}%)");
            // HP, MP

            OnDrawCharacterFirstStat();
            // 1차 스탯

            OnDrawCharacterSecondStat();
            // 2차 스탯

            EditorGUI.indentLevel -= 1;
        }

        _m_ChrEditData._mb_isActiveSkillFoldOut = EditorGUILayout.Foldout(_m_ChrEditData._mb_isActiveSkillFoldOut, "ActiveSkill");

        if (_m_ChrEditData._mb_isActiveSkillFoldOut)
        {
            // 폴드아웃이 열렸을 때 보여줄 내용 _ 액티브 스킬
            EditorGUILayout.LabelField("This is inside the foldout.");
            EditorGUILayout.IntField("Example Int", 42);
            EditorGUILayout.ColorField("Example Color", Color.red);
        }

        _m_ChrEditData._mb_isPassiveSkillFoldOut = EditorGUILayout.Foldout(_m_ChrEditData._mb_isPassiveSkillFoldOut, "PassiveSkill");

        if (_m_ChrEditData._mb_isPassiveSkillFoldOut)
        {
            // 폴드아웃이 열렸을 때 보여줄 내용 _ 패시브 스킬
            EditorGUILayout.LabelField("This is inside the foldout.");
            EditorGUILayout.IntField("Example Int", 42);
            EditorGUILayout.ColorField("Example Color", Color.red);
        }

    }
    private void OnDrawCharacterFirstStat()
    {
        GUILayoutOption[] options = new[] { GUILayout.Width(150), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"[ 1차 스탯 (First Stat) ]", options);

        GUIStyle centeredStyle = new GUIStyle(EditorStyles.numberField);
        centeredStyle.alignment = TextAnchor.MiddleCenter; // 텍스트 가운데 정렬

        EditorGUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(85), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- Strength : ", options);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        EditorGUILayout.IntField(_mEntity.Info.Status.STR, centeredStyle, options);
        // STR _ 1차 스탯

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(85), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- Dexterity : ", options);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        EditorGUILayout.IntField(_mEntity.Info.Status.DEX, centeredStyle, options);
        // DEX _ 1차 스탯

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(85), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- Wisdom : ", options);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        EditorGUILayout.IntField(_mEntity.Info.Status.WIS, centeredStyle, options);
        // WIS _ 1차 스탯

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(85), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- Guts : ", options);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        EditorGUILayout.IntField(_mEntity.Info.Status.GUT, centeredStyle, options);
        // GUT _ 1차 스탯

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(85), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- Mentality : ", options);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        EditorGUILayout.IntField(_mEntity.Info.Status.MET, centeredStyle, options);
        // MEN _ 1차 스탯

        GUILayout.Space(20);

        options = new[] { GUILayout.Width(120), GUILayout.Height(20) };
        if (GUILayout.Button("Calculate ALL!", options))
        {
            _mEntity.Info.Status.CalculateStatus();
        }

        EditorGUILayout.EndHorizontal();
    }
    private void OnDrawCharacterSecondStat()
    {
        GUILayoutOption[] options = new[] { GUILayout.Width(180), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"[ 2차 스탯 (Second Stat) ]", options);

        GUIStyle centeredStyle = new GUIStyle(EditorStyles.numberField);
        centeredStyle.alignment = TextAnchor.MiddleCenter; // 텍스트 가운데 정렬

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 물리 공격력 :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 물리 방어력 :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 물리 치명타 확률 :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 물리 치명타 회피율 :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 물리 치명타 데미지 증가율 :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 물리 치명타 데미지 감소율 :  {0} ", options);

        // Physical Part

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 마법 공격력 :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 마법 방어력 :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 마법 치명타 확률 :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 마법 치명타 회피율 :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 마법 치명타 데미지 증가율 :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 마법 치명타 데미지 감소율 :  {0} ", options);

        // Magical Part

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 적중률 :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 회피율 :  {0} ", options);

        // Normal Part

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 버프 효율 :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- 버프 저항 :  {0} ", options);

        // Buff Part

    }
    private void OnDrawCharacterActiveSkills()
    {

    }
    private void OnDrawCharacterPassiveSkills()
    {

    }
    private void OnDrawCharacterPersonality()
    {

    }
    public void CreatePC()
    {
        UnityLogger.GetInstance().Log($"CreatePC !! Job ID : {_m_ChrEditData._mi_JobID}");

        // 현재 자리에서 우리팀의 오브젝트 생성
        //var spawner = new UserEntityFactory();
        //spawner.CreateEntity(1);
    }

    /// <summary>
    /// 맵 생성 및 스테이터스 조작에 필요한 내용을 출력해주는 GUI Part 
    /// </summary>
    private void OnDrawMapInstantiateData()
    {
        EditorGUILayout.LabelField("Navigation EditWindow", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();

        EditorGUI.indentLevel += 1;

        GUILayoutOption[] options = new[] { GUILayout.Width(110), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- Map ID : {_m_NvEditData._mi_mapID}", options);

        GUILayout.Space(2);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        if (GUILayout.Button($"New", options))
        {
            // New Map File
            _m_NvEditData._mi_mapID = 99999999;
        }

        GUILayout.Space(2);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        if (GUILayout.Button($"Load", options))
        {
            // Load Map File
        }
        GUILayout.Space(2);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        if (GUILayout.Button($"Reset", options))
        {
            // 로딩된 맵 파일 삭제
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(110), GUILayout.Height(20) };
        EditorGUILayout.LabelField(($"- Nav ID : {_m_NvEditData._mi_NavID}"), options);

        GUILayout.Space(2);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        if (GUILayout.Button($"New", options))
        {
            // Load Map File
            _m_NvEditData._mi_NavID = 99999999;
        }

        GUILayout.Space(2);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        if (GUILayout.Button($"Load", options))
        {
            // Load Map File
        }
        GUILayout.Space(2);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        if (GUILayout.Button($"Reset", options))
        {
            // 로딩된 맵 파일 삭제
        }

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(130), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- QuickBatch Index : ", options);

        options = new[] { GUILayout.Width(80), GUILayout.Height(20) };
        _m_NvEditData._mStr_QuickBatch = EditorGUILayout.TextField(_m_NvEditData._mStr_QuickBatch, options);

        GUILayout.Space(2);

        options = new[] { GUILayout.Width(85), GUILayout.Height(20) };
        if (GUILayout.Button($"Quick Batch", options))
        {
            if (string.IsNullOrEmpty(_m_NvEditData._mStr_QuickBatch) == true)
            {
                UnityLogger.GetInstance().Log($"QuickBatch 란이 공백입니다. 데이터를 넣어주세요.");
            }
            else if (_m_NvEditData._mStr_QuickBatch.Contains('/') == false)
            {
                UnityLogger.GetInstance().Log($"QuickBatch가 요구하는 형식을 충족하지 못했습니다. 가이드를 읽고 데이터를 입력해주세요.");
            }
            else
            {
                bool _userRet = EditorUtility.DisplayDialog($"NOTICE!!", $"설정한 MapData 모두 삭제한 뒤 \n새롭게 Quick Batch 합니다.\n실행할까요? ( 파일 데이터는 영향 X ) ", $"OK", $"Cancle");

                if (_userRet)
                {
                    InstantiateNavMap();
                }
                else
                {
                    UnityLogger.GetInstance().Log($"Cancled Nav QuickBatch Function");
                }
            }
        }

        options = new[] { GUILayout.Width(85), GUILayout.Height(20) };
        if (GUILayout.Button($"Clear Data", options))
        {
            bool _userRet = EditorUtility.DisplayDialog($"NOTICE!!", $"현재 작업중인 Scene의 MapData를 모두 삭제합니다. \n실행할까요? ", $"OK", $"Cancle");

            if (_userRet)
            {
                ClearNavMap();
            }
            else
            {
                UnityLogger.GetInstance().Log($"Cancled Nav Remove All Function");
            }
        }

        options = new[] { GUILayout.Width(65), GUILayout.Height(20) };
        if (GUILayout.Button($"Save", options))
        {
            string _path = string.Empty;
            PathManager.GetInstance().GetGameDataPath(out _path);
            _path += "/NavData";

            string _ret = EditorUtility.SaveFilePanel($"저장", _path, $"", $"xml");
            UnityLogger.GetInstance().Log($"{_ret}");

            if(!string.IsNullOrEmpty(_ret))
            {
                // Save
                SaveNavMap(_ret);
            }
        }

        EditorGUILayout.EndHorizontal();

        EditorGUI.indentLevel -= 1;

        OnDrawGridNavElementData();
        OnDrawSelectedNavElementData();
    }

    /// <summary>
    /// 네비게이션 그리드의 전체 데이터를 보여준다.
    /// </summary>
    public void OnDrawGridNavElementData()
    {
        EditorGUILayout.LabelField("Grid Nav Element Data", EditorStyles.boldLabel);

        _m_NvEditData._mb_isGridNavFoldOut = EditorGUILayout.Foldout(_m_NvEditData._mb_isGridNavFoldOut, "Grid Nav Element");

        if (_m_NvEditData._mb_isGridNavFoldOut)
        {
            MapManager.GetInstance().GetNavigationElements(out var _navigation);
            var _sortLt_Nav = new SortedList<Vector2Int, NavigationElement>();

            if(_navigation == null)
            {
                // No Data
                EditorGUILayout.LabelField("No Datas");
            }
            else
            {
                _m_NvEditData._mv2_GridNavScrollPos = EditorGUILayout.BeginScrollView(_m_NvEditData._mv2_GridNavScrollPos, GUILayout.ExpandHeight(false));
                EditorGUILayout.BeginVertical();
                string _index;

                EditorGUI.indentLevel += 1;

                foreach (var _navPair in _navigation)
                {
                    GUILayout.BeginHorizontal();

                    GUILayout.Space(EditorGUI.indentLevel * 18); // << IndentLevel에 따라 공간 추가

                    // Index
                    _index = $"Idx : {_navPair.Key}";
                    GUILayout.Label(_index, GUILayout.Width(65));

                    //// Position X
                    GUILayoutOption[] options = new[] { GUILayout.Width(60), GUILayout.Height(20) };

                    GUILayout.Label($"Pos X", GUILayout.Width(40));
                    GUILayout.Space(-8);
                    _navPair.Value._mv3_Pos.x = EditorGUILayout.FloatField(_navPair.Value._mv3_Pos.x, options);

                    //// Position Y
                    GUILayout.Label($"Pos Y", GUILayout.Width(40));
                    GUILayout.Space(-8);
                    _navPair.Value._mv3_Pos.y = EditorGUILayout.FloatField(_navPair.Value._mv3_Pos.y, options);

                    GUILayout.Space(1);

                    // Enable Block Attribute
                    GUILayout.Label("IsEnable", GUILayout.Width(53));
                    _navPair.Value._mb_IsEnable = GUILayout.Toggle(_navPair.Value._mb_IsEnable, "", GUILayout.Width(12));
                    GUILayout.Space(7);

                    // Selected Object Emphasize
                    if (GUILayout.Button("Select", GUILayout.Width(65)))
                    {
                        UnityLogger.GetInstance().Log($"Button clicked at index: [{_navPair.Key}]");
                        MapManager.GetInstance().SelectedElement = _navPair.Value;
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
    /// <summary>
    /// 선택된 그리드의 속성 값을 보여준다.
    /// </summary>
    public void OnDrawSelectedNavElementData()
    {
        EditorGUILayout.LabelField("Selected Nav Section", EditorStyles.boldLabel);





    }

    /// <summary>
    /// 네비게이션 그리드를 세팅합니다.
    /// </summary>
    public void InstantiateNavMap()
    {
        UnityEngine.Debug.Log($"[LevelDesigneEditWindow] Command QuickBatch Start!");

        Vector2Int _batchIndex = new Vector2Int(_m_NvEditData.QuickBatchX, _m_NvEditData.QuickBatchY);
        MapManager.GetInstance().QuickBatchNavigation(_batchIndex);
    }
    /// <summary>
    /// 작성중인 네비게이션 그리드를 모두 제거합니다.
    /// </summary>
    public void ClearNavMap()
    {
        UnityEngine.Debug.Log($"[LevelDesigneEditWindow] ClearNavMap Start!");
        MapManager.GetInstance().ClearMapNavigation();
        UnityEngine.Debug.Log($"[LevelDesigneEditWindow] ClearNavMap Complete!");
    }

    public void SaveNavMap(string _filePath)
    {
        XMLUtility.SaveNavMap(_filePath, _m_NvEditData.QuickBatchX, _m_NvEditData.QuickBatchY);
        EditorUtility.DisplayDialog("NOTICE!!", "저장이 완료되었습니다.", "확인");
    }
}
