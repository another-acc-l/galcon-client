using NUnit.Framework;
using UnityEngine;

public class RoomStateScriptTests
{
    [Test]
    public void SetRoomState_ChangesCurrentState()
    {
        // Arrange
        var go = new GameObject();
        var roomStateScript = go.AddComponent<RoomStateScript>();

        // Act
        roomStateScript.SetRoomState(RoomStateScript.RoomState.InGame);

        // Assert
        Assert.AreEqual(RoomStateScript.RoomState.InGame, roomStateScript.currentState);
    }
}
