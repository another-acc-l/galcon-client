using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerLegendUpdater : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] RectTransform listRoot;
    [SerializeField] LegendItem itemPrefab;

    [Header("Sprites")]
    [SerializeField] Sprite neutralSprite;
    [SerializeField] Sprite[] playerSprites;

    readonly Dictionary<ulong, LegendItem> rows = new();

    void Start()
    {
        var nm = NetworkManager.Singleton;
        nm.OnClientConnectedCallback += OnClientConnect;
        nm.OnClientDisconnectCallback += OnClientDisconnect;

        foreach (var cl in nm.ConnectedClientsList)
            OnClientConnect(cl.ClientId);
    }

    void OnDestroy()
    {
        if (!NetworkManager.Singleton) return;
        var nm = NetworkManager.Singleton;
        nm.OnClientConnectedCallback -= OnClientConnect;
        nm.OnClientDisconnectCallback -= OnClientDisconnect;
    }

    void OnClientConnect(ulong id)
    {
        if (rows.ContainsKey(id)) return;

        var row = Instantiate(itemPrefab, listRoot);
        row.icon.sprite = GetSprite(id);
        rows[id] = row;

        NetworkManager.Singleton.StartCoroutine(AttachNameListener(id, row));
    }

    void OnClientDisconnect(ulong id)
    {
        if (rows.TryGetValue(id, out var r)) Destroy(r.gameObject);
        rows.Remove(id);
    }

    IEnumerator AttachNameListener(ulong id, LegendItem row)
    {
        SuperPlayer pl = null;
        while (pl == null)
        {
            var obj = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(id);
            if (obj) obj.TryGetComponent(out pl);
            yield return null;
        }

        pl.NickName.OnValueChanged += (_, name) => UpdateRow(row, name.ToString());
        UpdateRow(row, pl.NickName.Value.ToString());  
    }


    void UpdateRow(LegendItem row, string nick)
    {
        row.label.text = $"{row.icon.sprite.name} – планета гравця  {nick}";
    }

    Sprite GetSprite(ulong id)
    {
        if (id == 9999) return neutralSprite;
        int idx = (int)(id % (ulong)playerSprites.Length);
        return playerSprites[idx] ? playerSprites[idx] : neutralSprite;
    }

}
