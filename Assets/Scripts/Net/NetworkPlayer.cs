using Unity.Netcode;
using Unity.Collections;
using UnityEngine;

public class NetworkPlayer : NetworkBehaviour
{
    public readonly NetworkVariable<FixedString32Bytes> PlayerName =
        new(writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            string nick = PlayerPrefs.GetString("username", $"Player{OwnerClientId}");
            PlayerName.Value = new FixedString32Bytes(nick);
        }
    }
}