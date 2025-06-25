using Unity.Netcode;
using Unity.Collections;

public class RoomState : NetworkBehaviour
{
    public NetworkList<FixedString32Bytes> PlayerNames;

    void Awake()
    {
        PlayerNames = new NetworkList<FixedString32Bytes>();
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer)
            foreach (var kvp in NetworkManager.Singleton.ConnectedClients)
                PlayerNames.Add(GetNick(kvp.Key));
    }

    string GetNick(ulong id) =>
        NetworkManager.Singleton.ConnectedClients[id]
            .PlayerObject.GetComponent<NetworkPlayer>()
            .PlayerName.Value.ToString();
}
