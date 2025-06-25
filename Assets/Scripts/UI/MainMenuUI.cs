using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public Button btnLobby;
    public Button btnExit;

    private void Start()
    {
        btnLobby.onClick.AddListener(OpenLobby);
        btnExit.onClick.AddListener(ExitGame);
    }

    private void OpenLobby()
    {
        SceneManager.LoadScene("LobbyScene");
    }

    private void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
