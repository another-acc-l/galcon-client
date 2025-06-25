using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Unity.Netcode;

public class RoomUI : MonoBehaviour
{
    public Button btnStartGame;
    public Button btnLeaveRoom;
    [SerializeField] string gameSceneName = "GameScene";

    private void Start()
    {
        bool isHost = NetworkManager.Singleton.IsHost;
        btnStartGame.gameObject.SetActive(isHost);
        btnStartGame.interactable = isHost;

        if (isHost)
        {
            btnStartGame.onClick.AddListener(StartGame);
        }

        //btnStartGame.onClick.AddListener(StartGame);
        btnLeaveRoom.onClick.AddListener(LeaveRoom);
    }

    private void StartGame()
    {
        if (!NetworkManager.Singleton.IsHost) return;

        //SceneManager.LoadScene("GameScene");
        NetworkManager.Singleton.SceneManager.LoadScene(
            gameSceneName,
            LoadSceneMode.Single);
    }

    private void LeaveRoom()
    {
        ConnectionManager.Instance.Shutdown();
        SceneManager.LoadScene("MainMenuScene");
    }
}
