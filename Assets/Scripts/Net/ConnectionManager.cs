using Unity.Netcode;
using Unity.Netcode.Transports.UTP;   
using UnityEngine;

[RequireComponent(typeof(NetworkManager))]
[DisallowMultipleComponent]
public class ConnectionManager : MonoBehaviour
{
    public static ConnectionManager Instance { get; private set; }

    [SerializeField] private ushort port = 7777;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        var transport = GetComponent<UnityTransport>();
        if (!transport)
            transport = gameObject.AddComponent<UnityTransport>();

        transport.ConnectionData.Port = port;

        var netMan = GetComponent<NetworkManager>();
        netMan.NetworkConfig.NetworkTransport = transport;
        netMan.LogLevel = LogLevel.Developer;    

        if (!netMan.NetworkConfig.PlayerPrefab)
            Debug.LogWarning("[ConnectionManager] Player Prefab is not assigned!");
    }

    public void StartHost()
    {
        var nm = NetworkManager.Singleton;
        if (!nm.StartHost())
        {
            Debug.LogError("[ConnectionManager] StartHost FAILED!");
            return;
        }
        SpawnLocalPlayer();
    }

    public void JoinClient(string ip = "127.0.0.1")
    {
        ((UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport)
            .ConnectionData.Address = string.IsNullOrWhiteSpace(ip) ? "127.0.0.1" : ip;

        if (!NetworkManager.Singleton.StartClient())
            Debug.LogError("[ConnectionManager] StartClient FAILED!");
    }

    public void Shutdown()
    {
        if (!NetworkManager.Singleton) return;
        NetworkManager.Singleton.Shutdown();
        Instance = null;
        Destroy(gameObject);
    }

    private void SpawnLocalPlayer()
    {
        var prefab = NetworkManager.Singleton.NetworkConfig.PlayerPrefab;
        if (!prefab)
        {
            Debug.LogError("[ConnectionManager] Player Prefab not assigned!");
            return;
        }

        Instantiate(prefab).GetComponent<NetworkObject>().Spawn(true);
    }
}
