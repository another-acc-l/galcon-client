using Unity.Netcode;
using UnityEngine;

public class Fleet : NetworkBehaviour
{
    [SerializeField] float speed = 4f;

    public NetworkObjectReference TargetRef;                     
    public NetworkVariable<ulong> OwnerClientId = new(writePerm: NetworkVariableWritePermission.Server);

    Transform tgt;
    SpriteRenderer sr;

    void Awake() => sr = GetComponent<SpriteRenderer>();

    public override void OnNetworkSpawn()
    {
        TryAssignTarget();                                       
        sr.color = GetColorForOwner(OwnerClientId.Value);

        OwnerClientId.OnValueChanged += (_, now) =>
            sr.color = GetColorForOwner(now);                    
    }

    void TryAssignTarget()
    {
        if (!tgt && TargetRef.TryGet(out var netObj))
            tgt = netObj.transform;
    }

    void Update()
    {
        if (!IsSpawned) return;

        if (!tgt) { TryAssignTarget(); return; }                 

        if (IsServer)
        {
            transform.position = Vector3.MoveTowards(
                transform.position, tgt.position, speed * Time.deltaTime);

            if ((transform.position - tgt.position).sqrMagnitude < 0.02f)
                NetworkObject.Despawn();
        }
    }

    /* ── утиліта ── */
    static Color GetColorForOwner(ulong id)
    {
        if (id == 9999) return Color.gray;
        float h = (id * 0.61803398875f) % 1f;
        return Color.HSVToRGB(h, 0.7f, 0.9f);
    }
}
