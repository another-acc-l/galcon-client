using NUnit.Framework;
using UnityEngine;

public class SuperPlayerTests
{
    [Test]
    public void SetPlayerName_AssignsNameCorrectly()
    {
        // Arrange
        var go = new GameObject();
        var superPlayer = go.AddComponent<SuperPlayer>();

        // Act
        string expectedName = "Player42";
        superPlayer.SetPlayerName(expectedName);

        // Assert
        Assert.AreEqual(expectedName, superPlayer.playerName);
    }
}
