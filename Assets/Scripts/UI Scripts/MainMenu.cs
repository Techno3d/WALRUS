using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start() {
        AudioManager.instance?.ChangeClip(TypeMusic.UIMusic);
    }

    public void GoToScene(string sceneName)
    {
        Enemy.TotalNumEnemies = 0;
        Enemy.NumEnemies = 0;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitApp()
    {
        Application.Quit();
        Debug.Log("we quit");
    }
}
