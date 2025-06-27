using NUnit.Framework;
using UnityEngine;
using TMPro;

public class LegendItemTests
{
    [Test]
    public void LabelText_AssignsCorrectly()
    {
        // Arrange
        var go = new GameObject();
        var legendItem = go.AddComponent<LegendItem>();
        legendItem.label = go.AddComponent<TextMeshProUGUI>();

        // Act
        string testName = "TestName";
        legendItem.label.text = testName;

        // Assert
        Assert.AreEqual("TestName", legendItem.label.text);
    }
}
