using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScene : MonoBehaviour
{
    public string loadScene = "MainScene";
    public SceneFader sceneFader;

    public void GameStart()
    {
        sceneFader.FadeTo(loadScene);
    }

    public void End()
    {
        Application.Quit();
    }
}
