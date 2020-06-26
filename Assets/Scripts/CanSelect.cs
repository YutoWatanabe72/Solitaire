using System;
using UnityEngine;

public class CanSelect : MonoBehaviour
{
    public bool suite = false;//組札
    public string mark;//マーク
    public bool cardRed;//赤（♡♦）かどうか
    public int value;//値
    public int row;//現在いる列
    public bool turn = false;//めくられているか
    public bool inDeckPile = false;//デッキにあるか

    private string valueString;//オブジェクトの名前を一時保存

    void Start()
    {
        if (CompareTag("Card"))
        {
            mark = transform.name[0].ToString();
            cardRed = CardColorCheck();
            valueString = transform.name.Substring(1);

            value = Int32.Parse(valueString);
            Debug.Log(mark + value);
        }
    }

    /// <summary>
    /// 色判定
    /// </summary>
    /// <returns>true チェック入れる</returns>
    /// <returns>false チェック入れない</returns>
    bool CardColorCheck()
    {
        if (mark == "H" || mark == "D") return true;
        else return false;
    }
}
