using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [Header("Input Fields")]
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField joinCodeField;

    [Header("Buttons")]
    [SerializeField] private Button btnCreateHost;
    [SerializeField] private Button btnJoinClient;

    private void Start()
    {
        usernameField.text = PlayerPrefs.GetString("username", "");
        btnCreateHost.onClick.AddListener(CreateHost);
        btnJoinClient.onClick.AddListener(JoinClient);
    }

    public void CreateHost()
    {
        if (usernameField == null)
        {
            Debug.LogError("UsernameField is not assigned!");
            return;
        }

        if (ConnectionManager.Instance == null)
        {
            Debug.LogError("ConnectionManager.Instance is null!");
            return;
        }

        SaveName();
        Debug.Log("CreateHost with username: " + usernameField.text);
        ConnectionManager.Instance.StartHost();
        SceneManager.LoadScene("RoomScene");
    }


    public void JoinClient()
    {
        SaveName();
        Debug.Log($"Joining with code: {joinCodeField.text} {usernameField}");
        ConnectionManager.Instance.JoinClient(joinCodeField.text);
        SceneManager.LoadScene("RoomScene");
    }

    private void SaveName()
    {
        PlayerPrefs.SetString("username", usernameField.text);
    }
}
