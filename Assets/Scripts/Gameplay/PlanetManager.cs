using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;
using Random = UnityEngine.Random;

public class PlanetManager : NetworkBehaviour
{
    [Header("Prefabs")]
    [SerializeField] Planet planetPrefab;

    [Header("Map Settings")]
    [SerializeField] Vector2 mapHalfSize = new(14, 8);  
    [SerializeField] int neutralCount = 10;

    [Header("Planet Size")]
    [Tooltip("Однаковий масштаб для ВСІХ планет")]
    [SerializeField] float planetSize = 2.0f;           
    [Tooltip("Мінімальний проміжок між краями планет")]
    [SerializeField] float gapBetween = 1.8f;           

    readonly List<Vector2> occupied = new();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        SpawnPlayers();
        SpawnNeutrals();
    }

    void SpawnPlayers()
    {
        int playerN = NetworkManager.Singleton.ConnectedClientsList.Count;
        float radius = Mathf.Min(mapHalfSize.x, mapHalfSize.y) - planetSize; // всередині карти

        for (int i = 0; i < playerN; i++)
        {
            float ang = 2 * Mathf.PI * i / playerN;
            Vector2 pos = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang)) * radius;
            SpawnPlanet(NetworkManager.Singleton.ConnectedClientsList[i].ClientId, pos);
        }
    }

    void SpawnNeutrals()
    {
        for (int n = 0; n < neutralCount; n++)
            SpawnPlanet(9999, FindFreeSpot());
    }

    Vector2 FindFreeSpot()
    {
        const int maxTries = 100;
        for (int i = 0; i < maxTries; i++)
        {
            Vector2 c = new(
                Random.Range(-mapHalfSize.x, mapHalfSize.x),
                Random.Range(-mapHalfSize.y, mapHalfSize.y));

            if (!IsInside(c)) continue;

            bool ok = true;
            foreach (var p in occupied)
            {
                float minDist = planetSize + gapBetween;         // центр-до-центр
                if (Vector2.Distance(c, p) < minDist) { ok = false; break; }
            }
            if (ok) return c;
        }
        Debug.LogWarning("FindFreeSpot fallback");
        return Vector2.zero;
    }

    bool IsInside(Vector2 p) =>
        Mathf.Abs(p.x) <= mapHalfSize.x - planetSize &&
        Mathf.Abs(p.y) <= mapHalfSize.y - planetSize;

    void SpawnPlanet(ulong owner, Vector2 pos)
    {
        var p = Instantiate(planetPrefab, pos, Quaternion.identity);
        p.transform.localScale = Vector3.one * planetSize;

        var circle = p.GetComponent<CircleCollider2D>();
        circle.radius *= planetSize;

        var no = p.GetComponent<NetworkObject>();
        no.Spawn();
        p.OwnerId.Value = owner;

        occupied.Add(pos);  
    }
}
