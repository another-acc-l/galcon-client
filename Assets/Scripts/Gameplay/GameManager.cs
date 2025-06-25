using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;
    void Awake() => Instance = this;

    Planet selected;                       

    static readonly Color srcColor = Color.green;
    static readonly Color trgColor = Color.red;
    const float blinkSecs = 0.8f;

    [Header("Fleet visuals")]
    [SerializeField] Fleet fleetPrefab;        
    [SerializeField] int shipsPerSprite = 5; 
    [SerializeField] int maxFleetSprites = 20;
    public static bool LocalPlayerWon;

    public void SelectOrAttack(Planet clicked)
    {
        ulong me = NetworkManager.Singleton.LocalClientId;

        if (selected == null)
        {
            if (clicked.OwnerId.Value != me) return;
            selected = clicked;
            selected.SetHighlight(true, srcColor, blinkSecs);
            return;
        }

        if (clicked == selected)
        {
            selected.SetHighlight(false);
            selected = null;
            return;
        }

        clicked.SetHighlight(true, trgColor, blinkSecs);
        SendShipsServerRpc(clicked.NetworkObjectId, selected.NetworkObjectId);
        selected = null;                                  
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && selected)
        {
            selected.SetHighlight(false);
            selected = null;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SendShipsServerRpc(ulong trgId, ulong srcId, ServerRpcParams rpcParams = default)
    {
        var from = NetworkManager.SpawnManager.SpawnedObjects[srcId].GetComponent<Planet>();
        var to = NetworkManager.SpawnManager.SpawnedObjects[trgId].GetComponent<Planet>();

        
        if (from.OwnerId.Value != rpcParams.Receive.SenderClientId) return;

        int send = from.Ships.Value / 2;
        if (send == 0) return;

        from.Ships.Value -= send;

        
        if (to.OwnerId.Value == from.OwnerId.Value)
        {
            to.Ships.Value += send;                       
        }
        else
        {
            to.Ships.Value -= send;                       
            if (to.Ships.Value < 0)
            {
                to.OwnerId.Value = from.OwnerId.Value;    
                to.Ships.Value = -to.Ships.Value;
            }
        }

        SpawnFleetSprites(from, to, send);                
        CheckWinCondition();                              
    }

    void SpawnFleetSprites(Planet from, Planet to, int shipsSent)
    {
        if (!fleetPrefab) { Debug.LogError("Fleet prefab not assigned!"); return; }

        int count = Mathf.Clamp(Mathf.CeilToInt(shipsSent / (float)shipsPerSprite), 1, maxFleetSprites);

        for (int i = 0; i < count; i++)
        {
            Vector3 offset = Random.insideUnitCircle * 0.3f;

            var fleet = Instantiate(fleetPrefab, from.transform.position + offset, Quaternion.identity);
            fleet.TargetRef = to.GetComponent<NetworkObject>();   // ціль
            fleet.OwnerClientId.Value = from.OwnerId.Value;                
            fleet.GetComponent<NetworkObject>().Spawn();                   
        }
    }

    void CheckWinCondition()
    {
        var owners = new HashSet<ulong>();

        foreach (var obj in NetworkManager.Singleton.SpawnManager.SpawnedObjectsList)
            if (obj.TryGetComponent<Planet>(out var p) && p.OwnerId.Value != 9999)
                owners.Add(p.OwnerId.Value);

        if (owners.Count > 1) return;                    

        ulong winner = owners.Count == 1 ? owners.First() : 9999;
        DeclareWinnerClientRpc(winner);
    }

    [ClientRpc]
    void DeclareWinnerClientRpc(ulong winnerId)
    {
        LocalPlayerWon = winnerId == NetworkManager.Singleton.LocalClientId;

        if (IsServer)
            NetworkManager.Singleton.SceneManager.LoadScene(
                "EndScene", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }
}
