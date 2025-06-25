using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class HostOnlyButton : MonoBehaviour
{
    [SerializeField] bool hideForClients = true;     

    void Start()
    {
        if (NetworkManager.Singleton == null) return; 

        bool isHost = NetworkManager.Singleton.IsHost;

        if (hideForClients && !isHost)
            gameObject.SetActive(false);            
        else
            GetComponent<Button>().interactable = isHost;
    }
}
