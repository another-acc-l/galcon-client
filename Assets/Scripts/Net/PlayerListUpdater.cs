// Assets/_Project/Scripts/Net/PlayerListUpdater.cs
using Unity.Netcode;
using UnityEngine;
using TMPro;
using Unity.Collections;
using System.Collections;
using System.Collections.Generic;

public class PlayerListUpdater : MonoBehaviour
{
    [SerializeField] RectTransform listRoot;  
    [SerializeField] TMP_Text textPrefab;

    readonly Dictionary<ulong, TMP_Text> nodes = new();
    readonly Dictionary<ulong, NetworkPlayer> players = new();

    /*────────────────────────────  старт  ────────────────────────────*/
    void Start() => StartCoroutine(InitWhenReady());

    IEnumerator InitWhenReady()
    {
        yield return new WaitUntil(() =>
            NetworkManager.Singleton.LocalClient.PlayerObject != null);

        foreach (var cl in NetworkManager.Singleton.ConnectedClientsList)
            HookPlayer(cl.ClientId);

        var nm = NetworkManager.Singleton;
        nm.OnClientConnectedCallback += HookPlayer;
        nm.OnClientDisconnectCallback += UnhookPlayer;
    }

    void OnDestroy()
    {
        if (!NetworkManager.Singleton) return;
        var nm = NetworkManager.Singleton;
        nm.OnClientConnectedCallback -= HookPlayer;
        nm.OnClientDisconnectCallback -= UnhookPlayer;

        foreach (var p in players.Values)
            p.PlayerName.OnValueChanged -= Dummy; 
    }

    void HookPlayer(ulong id)
    {
        var obj = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id);
        if (obj == null) { StartCoroutine(HookDelayed(id)); return; }

        var np = obj.GetComponent<NetworkPlayer>();
        players[id] = np;

        np.PlayerName.OnValueChanged += (prev, curr) => OnNameChanged(id, curr);

        AddOrUpdate(id, np.PlayerName.Value);
    }
    IEnumerator HookDelayed(ulong id) { yield return null; HookPlayer(id); }

    void UnhookPlayer(ulong id)
    {
        if (players.TryGetValue(id, out var np))
            np.PlayerName.OnValueChanged -= Dummy;    

        players.Remove(id);

        if (nodes.TryGetValue(id, out var txt))
            Destroy(txt.gameObject);
        nodes.Remove(id);
    }

    /*──────────────────────────  UI  ──────────────────────────*/
    void OnNameChanged(ulong id, FixedString32Bytes nickFs) => AddOrUpdate(id, nickFs);
    void Dummy(FixedString32Bytes _, FixedString32Bytes __) { } 

    void AddOrUpdate(ulong id, FixedString32Bytes nickFs)
    {
        string nick = nickFs.ToString();

        if (nodes.TryGetValue(id, out var txt))
        {
            txt.text = nick;
            return;
        }

        var t = Instantiate(textPrefab, listRoot);
        t.text = nick;
        nodes[id] = t;
    }
}
