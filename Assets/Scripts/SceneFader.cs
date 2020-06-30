using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneFader : MonoBehaviour
{
    public Image image;
    public AnimationCurve curve;

    void Start()
    {
        StartCoroutine(FadeIn());
    }

    /// <summary>
    /// シーン遷移
    /// </summary>
    /// <param name="scene"></param>
    public void FadeTo(string scene)
    {
        StartCoroutine(FadeOut(scene));
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    /// <returns></returns>
    IEnumerator FadeIn()
    {
        float t = 1f;
        while(t > 0f)
        {
            t -= Time.deltaTime;
            float a = curve.Evaluate(t);
            image.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    /// <param name="scene">遷移するシーン名</param>
    /// <returns></returns>
    IEnumerator FadeOut(string scene)
    {
        float t = 0f;

        while(t < 1f)
        {
            t += Time.deltaTime;
            float a = curve.Evaluate(t);
            image.color = new Color(0f, 0f, 0f, a);
            yield return 0;
        }

        SceneManager.LoadScene(scene);
    }
}
