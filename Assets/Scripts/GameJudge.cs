using UnityEngine;

public class GameJudge : MonoBehaviour
{
    public CanSelect[] suiteStacks;//組札
    public GameObject gameWin;//ゲームクリアオブジェクト（UI）
    public GameObject[] winEffect;//ゲームクリア時にOnになるエフェクト
    public CardSetting cardSetting;
    public GameObject difficultSelection;


    void Start()
    {
        gameWin.SetActive(false);
        foreach(var e in winEffect)
        {
            e.SetActive(false);
        }
    }

    void Update()
    {
        if (HasWon())
        {
            Win();
        }
    }

    /// <summary>
    /// 各組札のvalueの値の合計が52以上かどうか
    /// 超えていればゲームクリア
    /// 超えていればゲームクリア
    /// </summary>
    /// <returns></returns>
    public bool HasWon()
    {
        int i = 0;
        foreach(CanSelect suiteStack in suiteStacks)
        {
            i += suiteStack.value;
        }

        if(i >= 52)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// ゲームクリア時にUIオン
    /// </summary>
    void Win()
    {
        gameWin.SetActive(true);
        foreach(var e in winEffect)
        {
            e.SetActive(true);
        }
    }

    /// <summary>
    /// 難易度選択
    /// </summary>
    /// <param name="n">めくるカードの枚数</param>
    public void DifficultySelection(int n)
    {
        if(n == 1)
        {
            cardSetting.flipNum = 1;
        }
        else if(n == 3)
        {
            cardSetting.flipNum = 3;
        }
        difficultSelection.SetActive(false);
        cardSetting.Initialization();
    }
}
