using UnityEditor;
using UnityEditor.SceneManagement;

public class SceneOpener {

    [MenuItem("Scenes/OpenMainMenu")]
    private static void OpenMainMenu() {
        EditorSceneManager.OpenScene("Assets/_Scenes/MainMenu.unity");
    }

    [MenuItem("Scenes/OpenMap")]
    private static void OpenMap() {
        EditorSceneManager.OpenScene("Assets/_Scenes/BattleScene.unity");
        EditorSceneManager.OpenScene("Assets/_Scenes/DialogueScene.unity", OpenSceneMode.Additive);
    }

    [MenuItem("Scenes/OpenBase")]
    private static void OpenBase() {
        EditorSceneManager.OpenScene("Assets/_Scenes/BaseScene.unity");
    }

    [MenuItem("Scenes/LoadingScreen")]
    private static void OpenLoadingScreen() {
        EditorSceneManager.OpenScene("Assets/_Scenes/LoadingScreen.unity");
    }

    [MenuItem("Scenes/OpenSaveMenu")]
    private static void OpenSaveMenu() {
        EditorSceneManager.OpenScene("Assets/_Scenes/SaveScene.unity");
    }
}