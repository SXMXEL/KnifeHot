using NUnit.Framework;
using Altom.AltUnityDriver;
using Items;
using UnityEngine;

public class PlayButtonTest
{
    public AltUnityDriver AltUnityDriver;

    //Before any test it connects with the socket
    [OneTimeSetUp]
    public void SetUp()
    {
        AltUnityDriver = new AltUnityDriver();
    }

    //At the end of the test closes the connection with the socket
    [OneTimeTearDown]
    public void TearDown()
    {
        AltUnityDriver.Stop();
    }

    [Test]
    public void Test()
    {
        //Here you can write the test
        AltUnityDriver
            .FindObject (By.NAME, "AltUnityRunnerPrefab")
            .SetComponentProperty("AltUnityRunner", "ShowInputs", "true");
        AltUnityDriver.FindObject(By.NAME, "PlayButton").Tap();

        var gamePage = AltUnityDriver.WaitForObject(By.NAME, "GamePage");
        Assert.IsTrue(gamePage.enabled);
        AltUnityDriver.WaitForObject(By.NAME, "KnifeFireButton").Tap();
    }
}