using UnityEngine;

public class ButtonsAction : MonoBehaviour
{
    public SceneFader sceneFader;
    public string scene = "TitleScene";
    public GameJudge gameJudge;

    //盤面をリセット
    public void ResetScene()
    {
        UpdateSprite[] cards = FindObjectsOfType<UpdateSprite>();
        foreach(UpdateSprite card in cards)
        {
            Destroy(card.gameObject);
        }
        foreach(var e in gameJudge.winEffect)
        {
            e.SetActive(false);
        }
        ClearSuiteValues();
        FindObjectOfType<CardSetting>().Initialization();
    }

    //タイトルにシーン遷移
    public void ToTitle()
    {
        sceneFader.FadeTo(scene);
    }

    //組札にあるデータを削除
    void ClearSuiteValues()
    {
        CanSelect[] canSelects = FindObjectsOfType<CanSelect>();
        foreach(CanSelect canSelect in canSelects)
        {
            if (canSelect.CompareTag("Suite"))
            {
                canSelect.mark = null;
                canSelect.value = 0;
            }
        }

        if (GetComponent<GameJudge>().gameWin.activeSelf)
        {
            GetComponent<GameJudge>().gameWin.SetActive(false);
        }
    }
}
