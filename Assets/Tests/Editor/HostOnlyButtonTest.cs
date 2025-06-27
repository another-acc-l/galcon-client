using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;

public class HostOnlyButtonTests
{
    [Test]
    public void HostButton_IsDisabled_WhenNotHost()
    {
        // Arrange
        var go = new GameObject();
        var button = go.AddComponent<Button>();
        var hostOnly = go.AddComponent<HostOnlyButton>();
        hostOnly.targetButton = button;

        // Act
        hostOnly.SetInteractable(false);

        // Assert
        Assert.IsFalse(button.interactable);
    }
}
