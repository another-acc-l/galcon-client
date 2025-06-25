// Assets/_Project/Scripts/UI/LegendItem.cs
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LegendItem : MonoBehaviour
{
    public Image icon;
    public TMP_Text label;

    void Awake()
    {
        if (!icon) icon = GetComponentInChildren<Image>();
        if (!label) label = GetComponentInChildren<TMP_Text>();
    }
}
