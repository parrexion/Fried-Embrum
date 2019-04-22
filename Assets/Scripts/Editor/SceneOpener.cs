using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneOpener {

    [MenuItem("Scenes/Main Menu")]
    private static void OpenMainMenu() {
        EditorSceneManager.OpenScene("Assets/_Scenes/MainMenu.unity");
    }

    [MenuItem("Scenes/Battle Map")]
    private static void OpenMap() {
        EditorSceneManager.OpenScene("Assets/_Scenes/BattleScene.unity");
        EditorSceneManager.OpenScene("Assets/_Scenes/DialogueScene.unity", OpenSceneMode.Additive);
    }

    [MenuItem("Scenes/Base Menu")]
    private static void OpenBase() {
        EditorSceneManager.OpenScene("Assets/_Scenes/BaseScene.unity");
    }

    [MenuItem("Scenes/Loading Screen")]
    private static void OpenLoadingScreen() {
        EditorSceneManager.OpenScene("Assets/_Scenes/LoadingScreen.unity");
    }

    [MenuItem("Scenes/Dialogue Scene")]
    private static void OpenDialogueScene() {
        EditorSceneManager.OpenScene("Assets/_Scenes/DialogueScene.unity");
    }

    [MenuItem("Scenes/Save Menu")]
    private static void OpenSaveMenu() {
        EditorSceneManager.OpenScene("Assets/_Scenes/SaveScene.unity");
    }

    [MenuItem("Scenes/Startup Scene")]
    private static void OpenStartupMenu() {
        EditorSceneManager.OpenScene("Assets/_Scenes/StartupScene.unity");
    }
}