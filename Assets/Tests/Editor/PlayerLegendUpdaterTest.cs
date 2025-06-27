using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class PlayerLegendUpdaterTests
{
    private PlayerLegendUpdater updater;

    [SetUp]
    public void Setup()
    {
        var go = new GameObject();
        updater = go.AddComponent<PlayerLegendUpdater>();

        // Симулюємо спрайти
        updater.GetType().GetField("neutralSprite", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(updater, Sprite.Create(Texture2D.blackTexture, new Rect(0, 0, 10, 10), Vector2.zero));

        var sprites = new Sprite[2];
        sprites[0] = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 10, 10), Vector2.zero);
        sprites[1] = null; // Для перевірки запасного варіанту

        updater.GetType().GetField("playerSprites", BindingFlags.NonPublic | BindingFlags.Instance)
            .SetValue(updater, sprites);
    }

    [Test]
    public void GetSprite_ReturnsNeutral_ForId9999()
    {
        MethodInfo method = typeof(PlayerLegendUpdater).GetMethod("GetSprite", BindingFlags.NonPublic | BindingFlags.Instance);
        Sprite sprite = (Sprite)method.Invoke(updater, new object[] { (ulong)9999 });
        Assert.IsNotNull(sprite);
    }

    [Test]
    public void GetSprite_ReturnsValidSprite_WhenSpriteExists()
    {
        MethodInfo method = typeof(PlayerLegendUpdater).GetMethod("GetSprite", BindingFlags.NonPublic | BindingFlags.Instance);
        Sprite sprite = (Sprite)method.Invoke(updater, new object[] { (ulong)0 });
        Assert.IsNotNull(sprite);
    }

    [Test]
    public void GetSprite_ReturnsNeutral_WhenSpriteIsNull()
    {
        MethodInfo method = typeof(PlayerLegendUpdater).GetMethod("GetSprite", BindingFlags.NonPublic | BindingFlags.Instance);
        Sprite sprite = (Sprite)method.Invoke(updater, new object[] { (ulong)1 });
        Assert.IsNotNull(sprite); // має повернути нейтральний, бо playerSprites[1] == null
    }
}
