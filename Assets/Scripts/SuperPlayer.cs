using Unity.Netcode;
using Unity.Collections;
using UnityEngine;

public class SuperPlayer : NetworkBehaviour
{
    public readonly NetworkVariable<FixedString32Bytes> NickName =
        new(writePerm: NetworkVariableWritePermission.Server);

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            string nick = PlayerPrefs.GetString("username", $"Player{OwnerClientId}");
            CmdSetNameServerRpc(nick);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void CmdSetNameServerRpc(string nick, ServerRpcParams rpc = default)
    {
        NickName.Value = nick.Substring(0, Mathf.Min(nick.Length, 32));
    }
}
