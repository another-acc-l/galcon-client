using Unity.Netcode;
using UnityEngine;
using TMPro;
using System.Collections;
using Random = UnityEngine.Random;

public class Planet : NetworkBehaviour
{
    [Header("Sprites")]
    [SerializeField] Sprite neutralSprite;
    [SerializeField] Sprite[] playerSprites = new Sprite[5];

    [Header("Initial & Production")]
    [SerializeField] Vector2Int initialShipRange = new(10, 51);
    [SerializeField] int flatProduction = 1;
    [SerializeField] float percentProduction = 0.05f;
    [SerializeField] int threshold = 50;
    [SerializeField] float productionPeriod = 1f;

    [Header("Dynamic Scale")]
    [SerializeField] float baseScale = 1.0f;  // при 1 кораблі
    [SerializeField] float scalePerSqrtShip = 0.15f; // множник для √Ships
    [SerializeField] float maxScale = 3.0f;
    [Header("UI Refs")]
    [SerializeField] TMP_Text shipLabel;
    [SerializeField] GameObject highlightRing;

    public readonly NetworkVariable<ulong> OwnerId = new();
    public readonly NetworkVariable<int> Ships = new();

    SpriteRenderer sr;
    Coroutine prodRoutine;
    Coroutine highlightRoutine;
    static readonly Color neutralColor = Color.gray;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (!shipLabel) shipLabel = GetComponentInChildren<TMP_Text>();
        if (!highlightRing) highlightRing = transform.Find("HighlightRing")?.gameObject;
        if (highlightRing) highlightRing.SetActive(false);
    }

    public override void OnNetworkSpawn()
    {
        if (IsServer && Ships.Value == 0)
            Ships.Value = Random.Range(initialShipRange.x, initialShipRange.y + 1);

        OwnerId.OnValueChanged += (_, _) => { UpdateColor(); UpdateSprite(); CheckProd(); };
        Ships.OnValueChanged += (_, v) => { UpdateLabel(v); UpdateScale(v); };

        UpdateColor(); UpdateSprite(); UpdateLabel(Ships.Value); UpdateScale(Ships.Value);
        CheckProd();
    }

    void OnMouseDown() => GameManager.Instance.SelectOrAttack(this);

    void CheckProd()
    {
        if (!IsServer) return;
        bool owned = OwnerId.Value != 9999;

        if (owned && prodRoutine == null)
            prodRoutine = StartCoroutine(ProduceShips());
        else if (!owned && prodRoutine != null)
        {
            StopCoroutine(prodRoutine);
            prodRoutine = null;
        }
    }

    IEnumerator ProduceShips()
    {
        var wait = new WaitForSeconds(productionPeriod);
        while (true)
        {
            yield return wait;

            int add = flatProduction;
            if (Ships.Value >= threshold)
                add += Mathf.CeilToInt(Ships.Value * percentProduction);

            Ships.Value += add;              
        }
    }

    void UpdateColor() => sr.color = GetColorForOwner(OwnerId.Value);

    void UpdateSprite()
    {
        Sprite s = neutralSprite;
        if (OwnerId.Value != 9999)
        {
            int idx = (int)(OwnerId.Value % (ulong)playerSprites.Length);
            if (idx < playerSprites.Length && playerSprites[idx])
                s = playerSprites[idx];
        }
        sr.sprite = s;
    }

    void UpdateLabel(int v) => shipLabel.text = v.ToString();

    void UpdateScale(int v)
    {
        float target = baseScale + Mathf.Sqrt(v) * scalePerSqrtShip;
        target = Mathf.Min(target, maxScale);         // ◄── обрізаємо
        transform.localScale = Vector3.one * target;
    }

    static Color GetColorForOwner(ulong id)
    {
        if (id == 9999) return neutralColor;
        float h = (id * 0.61803398875f) % 1f;
        return Color.HSVToRGB(h, 0.7f, 0.9f);
    }

    public void SetHighlight(bool on, Color c = default, float autoOff = 0.4f)
    {
        if (!highlightRing)
            highlightRing = transform.Find("HighlightRing")?.gameObject;
        if (!highlightRing) return;

        if (!on)
        {
            if (highlightRoutine != null) { StopCoroutine(highlightRoutine); highlightRoutine = null; }
            highlightRing.SetActive(false);
            return;
        }

        var ringSr = highlightRing.GetComponent<SpriteRenderer>();
        ringSr.color = c == default ? Color.white : c;
        highlightRing.SetActive(true);

        if (highlightRoutine != null) StopCoroutine(highlightRoutine);
        highlightRoutine = StartCoroutine(AutoOff(autoOff));
    }

    IEnumerator AutoOff(float t)
    {
        yield return new WaitForSeconds(t);
        highlightRing.SetActive(false);
        highlightRoutine = null;
    }
}
