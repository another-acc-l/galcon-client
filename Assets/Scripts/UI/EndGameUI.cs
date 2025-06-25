using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class EndGameUI : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Text resultLabel;
    [SerializeField] Button btnMainMenu;

    void Start()
    {
        resultLabel.text = GameManager.LocalPlayerWon ? "You Win!" : "You Lose.";

        btnMainMenu.onClick.AddListener(MoveToMainMenu);
        //btnMainMenu.onClick.AddListener(() =>
        //{
        //    ConnectionManager.Instance?.Shutdown();          // відʼєднати NGO
        //    SceneManager.LoadScene("MainMenuScene");
        //});
    }

    void MoveToMainMenu()
    {
        ConnectionManager.Instance?.Shutdown();        
        SceneManager.LoadScene("MainMenuScene");
    }
}
