using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Text;
using GlobalGameDataSpace;
using System.IO;

public class JHTools : MonoBehaviour
{
    [MenuItem("JHTools/Load_StartScene")]
    private static void OpenStartScene()
    {
        string sceneName = "StartScene.unity";

        StringBuilder _sb = new StringBuilder();
        _sb.Append(Application.dataPath);
        _sb.Append($"/Scenes");
        _sb.Append($"/{sceneName}");

        string path = _sb.ToString();
        EditorSceneManager.OpenScene(path);
    }

    [MenuItem("JHTools/Start_StartScene")]
    private static void StartStartScene()
    {
        string sceneName = "StartScene.unity";

        StringBuilder _sb = new StringBuilder();
        _sb.Append(Application.dataPath);
        _sb.Append($"/Scenes");
        _sb.Append($"/{sceneName}");

        string path = _sb.ToString();
        // �� �ε�
        if (EditorSceneManager.OpenScene(path) != null)
        {
            // �÷��� ��� ����
            EditorApplication.isPlaying = true;
        }
        else
        {
            Debug.LogError("Scene could not be loaded. Check the path and ensure the scene is included in the build settings.");
        }
    }

    [MenuItem("JHTools/Load_StageScene")]
    private static void OpenStageScene()
    {
        string sceneName = "StageScene.unity";

        StringBuilder _sb = new StringBuilder();
        _sb.Append(Application.dataPath);
        _sb.Append($"/Scenes");
        _sb.Append($"/{sceneName}");

        string path = _sb.ToString();
        EditorSceneManager.OpenScene(path);
    }

    [MenuItem("JHTools/Start_StageScene")]
    private static void StartStageScene()
    {
        string sceneName = "StageScene.unity";

        StringBuilder _sb = new StringBuilder();
        _sb.Append(Application.dataPath);
        _sb.Append($"/Scenes");
        _sb.Append($"/{sceneName}");

        string path = _sb.ToString();
        // �� �ε�
        if (EditorSceneManager.OpenScene(path) != null)
        {
            // �÷��� ��� ����
            EditorApplication.isPlaying = true;
        }
        else
        {
            Debug.LogError("Scene could not be loaded. Check the path and ensure the scene is included in the build settings.");
        }
    }

    [MenuItem("JHTools/Tools/LevelDesign/Open_LevelDesignTool")]
    private static void OpenLevelDesignTool()
    {
        string sceneName = "LevelDesignToolScene.unity";

        StringBuilder _sb = new StringBuilder();
        _sb.Append(Application.dataPath);
        _sb.Append($"/Scenes/ToolScene");
        _sb.Append($"/{sceneName}");

        string path = _sb.ToString();
        EditorSceneManager.OpenScene(path);
    }
        
    [MenuItem("JHTools/Tools/LevelDesign/Open_LevelDesignEditWindow")]
    private static void OpenLevelDesignEditWindow()
    {
        EditorWindow.GetWindow(typeof(LevelDesignEditWindow));
    }



public class TileSheetExporter : MonoBehaviour
{
    [MenuItem("JHTools/Export Tiles from SpriteSheet")]
    static void ExportTiles()
    {
        // ��������Ʈ ��Ʈ ����
        Texture2D spriteSheet = Selection.activeObject as Texture2D;
        if (spriteSheet == null)
        {
            Debug.LogError("��������Ʈ ��Ʈ�� �������ּ���!");
            return;
        }

        int tileSize = 32; // Ÿ�� ũ��
        int sheetWidth = spriteSheet.width;
        int sheetHeight = spriteSheet.height;

        // ���� ���� ����
        string folderPath = Application.dataPath + "/TileExports";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // �߶� ���Ϸ� ����
        int tileIndex = 0;
        for (int y = 0; y < sheetHeight; y += tileSize)
        {
            for (int x = 0; x < sheetWidth; x += tileSize)
            {
                Texture2D tileTexture = new Texture2D(tileSize, tileSize);
                tileTexture.SetPixels(spriteSheet.GetPixels(x, y, tileSize, tileSize));
                tileTexture.Apply();

                byte[] bytes = tileTexture.EncodeToPNG();
                string fileName = $"{folderPath}/tile_{tileIndex:D3}.png";
                File.WriteAllBytes(fileName, bytes);
                tileIndex++;
            }
        }

        Debug.Log($"�� {tileIndex}�� Ÿ���� {folderPath}�� ����Ǿ����ϴ�.");
        AssetDatabase.Refresh();
    }
}
}
