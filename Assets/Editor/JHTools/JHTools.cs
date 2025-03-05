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
        // 씬 로드
        if (EditorSceneManager.OpenScene(path) != null)
        {
            // 플레이 모드 시작
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
        // 씬 로드
        if (EditorSceneManager.OpenScene(path) != null)
        {
            // 플레이 모드 시작
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
        // 스프라이트 시트 선택
        Texture2D spriteSheet = Selection.activeObject as Texture2D;
        if (spriteSheet == null)
        {
            Debug.LogError("스프라이트 시트를 선택해주세요!");
            return;
        }

        int tileSize = 32; // 타일 크기
        int sheetWidth = spriteSheet.width;
        int sheetHeight = spriteSheet.height;

        // 저장 폴더 생성
        string folderPath = Application.dataPath + "/TileExports";
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 잘라서 파일로 저장
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

        Debug.Log($"총 {tileIndex}개 타일이 {folderPath}에 저장되었습니다.");
        AssetDatabase.Refresh();
    }
}
}
