using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using GlobalGameDataSpace;
using System.Diagnostics;

/// <summary>
/// ĳ���� ������ �ʿ��� ���� ���� �Դϴ�.
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
        // ��ũ�� ��

        int _selectedToolBar = _m_LvDesignEditComData._mi_SelectedLvEditToolbar;
        _m_LvDesignEditComData._mi_SelectedLvEditToolbar = GUILayout.Toolbar(_selectedToolBar, _m_LvDesignEditComData._mStr_LvEditToolBar);

        switch (_selectedToolBar)
        {
            case 0:
                // NOTICE :: ĳ���� ���� �� �������ͽ� ������ �ʿ��� ��

                _m_LvDesignEditComData._mv2_VerticalScrollPos[_selectedToolBar] = EditorGUILayout.BeginScrollView(_m_LvDesignEditComData._mv2_VerticalScrollPos[_selectedToolBar]);

                OnDrawCharacterInstantiateAttribute();
                OnDrawCharacterStatus();
                OnDrawCharacterActiveSkills();
                OnDrawCharacterPassiveSkills();
                OnDrawCharacterPersonality();
                // TODO : ���� �ý����� ���߿�

                // ��ũ�� �� ��
                EditorGUILayout.EndScrollView();

                break;
            case 1:
                // NOTICE :: �� �ε� �� ������ ���� ��
                OnDrawMapInstantiateData();

                break;
            case 2:
                _m_SpanwerUpdator.OnUpdateLevelDesign();

                break;
        }
    }

    /// <summary>
    /// ĳ���� ���� �� �������ͽ� ���ۿ� �ʿ��� ������ ������ִ� GUI Part 
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
            // TODO : JobID�� ������ ������ ���� �� �ֵ��� �۾��� ����, ���� ���̺� ����
        }

        GUILayout.EndHorizontal();

        // NOTICE :: ��������� ���� ����
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
        GUILayout.Label($"Job ID : {_mEntity.JobID} ", options); // ���� ID 

        options = new[] { GUILayout.Width(140), GUILayout.Height(20) };
        string _retString = string.Empty;
        ClientUtility.ConvertJobNameToJobID(_mEntity.JobID, out _retString);
        GUILayout.Label($"Job Name : {_retString}", options); // ���� �̸�

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(160), GUILayout.Height(20) };
        ClientUtility.ConvertChrNameToJobID(_mEntity.JobID, out _retString);
        GUILayout.Label($"Nick Name : {_retString} ", options); // ĳ������ ���� �̸�

        options = new[] { GUILayout.Width(150), GUILayout.Height(20) };
        GUILayout.Label($"Set Map Index : x {99} , y {99}", options);

        GUILayout.EndHorizontal();

        _m_ChrEditData._mb_isStatusFoldOut = EditorGUILayout.Foldout(_m_ChrEditData._mb_isStatusFoldOut, "Status");

        if (_m_ChrEditData._mb_isStatusFoldOut)
        {
            // ����ƿ��� ������ �� ������ ���� _ �������ͽ�
            EditorGUI.indentLevel += 1;
            EditorGUILayout.LabelField($"- HP/MaxHP : {_mEntity.Info.HP}/{_mEntity.Info.MaxHP} ({_mEntity.Info.HPPercent}%)");
            EditorGUILayout.LabelField($"- MP/MaxMP : {_mEntity.Info.MP}/{_mEntity.Info.MaxMP} ({_mEntity.Info.MPPercent}%)");
            // HP, MP

            OnDrawCharacterFirstStat();
            // 1�� ����

            OnDrawCharacterSecondStat();
            // 2�� ����

            EditorGUI.indentLevel -= 1;
        }

        _m_ChrEditData._mb_isActiveSkillFoldOut = EditorGUILayout.Foldout(_m_ChrEditData._mb_isActiveSkillFoldOut, "ActiveSkill");

        if (_m_ChrEditData._mb_isActiveSkillFoldOut)
        {
            // ����ƿ��� ������ �� ������ ���� _ ��Ƽ�� ��ų
            EditorGUILayout.LabelField("This is inside the foldout.");
            EditorGUILayout.IntField("Example Int", 42);
            EditorGUILayout.ColorField("Example Color", Color.red);
        }

        _m_ChrEditData._mb_isPassiveSkillFoldOut = EditorGUILayout.Foldout(_m_ChrEditData._mb_isPassiveSkillFoldOut, "PassiveSkill");

        if (_m_ChrEditData._mb_isPassiveSkillFoldOut)
        {
            // ����ƿ��� ������ �� ������ ���� _ �нú� ��ų
            EditorGUILayout.LabelField("This is inside the foldout.");
            EditorGUILayout.IntField("Example Int", 42);
            EditorGUILayout.ColorField("Example Color", Color.red);
        }

    }
    private void OnDrawCharacterFirstStat()
    {
        GUILayoutOption[] options = new[] { GUILayout.Width(150), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"[ 1�� ���� (First Stat) ]", options);

        GUIStyle centeredStyle = new GUIStyle(EditorStyles.numberField);
        centeredStyle.alignment = TextAnchor.MiddleCenter; // �ؽ�Ʈ ��� ����

        EditorGUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(85), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- Strength : ", options);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        EditorGUILayout.IntField(_mEntity.Info.Status.STR, centeredStyle, options);
        // STR _ 1�� ����

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(85), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- Dexterity : ", options);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        EditorGUILayout.IntField(_mEntity.Info.Status.DEX, centeredStyle, options);
        // DEX _ 1�� ����

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(85), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- Wisdom : ", options);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        EditorGUILayout.IntField(_mEntity.Info.Status.WIS, centeredStyle, options);
        // WIS _ 1�� ����

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(85), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- Guts : ", options);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        EditorGUILayout.IntField(_mEntity.Info.Status.GUT, centeredStyle, options);
        // GUT _ 1�� ����

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();

        options = new[] { GUILayout.Width(85), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- Mentality : ", options);

        options = new[] { GUILayout.Width(50), GUILayout.Height(20) };
        EditorGUILayout.IntField(_mEntity.Info.Status.MET, centeredStyle, options);
        // MEN _ 1�� ����

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
        EditorGUILayout.LabelField($"[ 2�� ���� (Second Stat) ]", options);

        GUIStyle centeredStyle = new GUIStyle(EditorStyles.numberField);
        centeredStyle.alignment = TextAnchor.MiddleCenter; // �ؽ�Ʈ ��� ����

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ���ݷ� :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ���� :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ġ��Ÿ Ȯ�� :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ġ��Ÿ ȸ���� :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ġ��Ÿ ������ ������ :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ġ��Ÿ ������ ������ :  {0} ", options);

        // Physical Part

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ���ݷ� :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ���� :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ġ��Ÿ Ȯ�� :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ġ��Ÿ ȸ���� :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ġ��Ÿ ������ ������ :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ġ��Ÿ ������ ������ :  {0} ", options);

        // Magical Part

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���߷� :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ȸ���� :  {0} ", options);

        // Normal Part

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ȿ�� :  {0} ", options);

        options = new[] { GUILayout.Width(450), GUILayout.Height(20) };
        EditorGUILayout.LabelField($"- ���� ���� :  {0} ", options);

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

        // ���� �ڸ����� �츮���� ������Ʈ ����
        //var spawner = new UserEntityFactory();
        //spawner.CreateEntity(1);
    }

    /// <summary>
    /// �� ���� �� �������ͽ� ���ۿ� �ʿ��� ������ ������ִ� GUI Part 
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
            // �ε��� �� ���� ����
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
            // �ε��� �� ���� ����
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
                UnityLogger.GetInstance().Log($"QuickBatch ���� �����Դϴ�. �����͸� �־��ּ���.");
            }
            else if (_m_NvEditData._mStr_QuickBatch.Contains('/') == false)
            {
                UnityLogger.GetInstance().Log($"QuickBatch�� �䱸�ϴ� ������ �������� ���߽��ϴ�. ���̵带 �а� �����͸� �Է����ּ���.");
            }
            else
            {
                bool _userRet = EditorUtility.DisplayDialog($"NOTICE!!", $"������ MapData ��� ������ �� \n���Ӱ� Quick Batch �մϴ�.\n�����ұ��? ( ���� �����ʹ� ���� X ) ", $"OK", $"Cancle");

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
            bool _userRet = EditorUtility.DisplayDialog($"NOTICE!!", $"���� �۾����� Scene�� MapData�� ��� �����մϴ�. \n�����ұ��? ", $"OK", $"Cancle");

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

            string _ret = EditorUtility.SaveFilePanel($"����", _path, $"", $"xml");
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
    /// �׺���̼� �׸����� ��ü �����͸� �����ش�.
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

                    GUILayout.Space(EditorGUI.indentLevel * 18); // << IndentLevel�� ���� ���� �߰�

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
    /// ���õ� �׸����� �Ӽ� ���� �����ش�.
    /// </summary>
    public void OnDrawSelectedNavElementData()
    {
        EditorGUILayout.LabelField("Selected Nav Section", EditorStyles.boldLabel);





    }

    /// <summary>
    /// �׺���̼� �׸��带 �����մϴ�.
    /// </summary>
    public void InstantiateNavMap()
    {
        UnityEngine.Debug.Log($"[LevelDesigneEditWindow] Command QuickBatch Start!");

        Vector2Int _batchIndex = new Vector2Int(_m_NvEditData.QuickBatchX, _m_NvEditData.QuickBatchY);
        MapManager.GetInstance().QuickBatchNavigation(_batchIndex);
    }
    /// <summary>
    /// �ۼ����� �׺���̼� �׸��带 ��� �����մϴ�.
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
        EditorUtility.DisplayDialog("NOTICE!!", "������ �Ϸ�Ǿ����ϴ�.", "Ȯ��");
    }
}
