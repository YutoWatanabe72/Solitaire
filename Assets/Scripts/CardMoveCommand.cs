using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardMoveCommand : IAction
{
    private GameObject slot;
    private GameObject selected;
    private CardSetting cardSetting;

    private CanSelect cs1;//slotのCanSelect
    private CanSelect cs2;//selectedのCanSelect
    private GameObject parented;//親
    private Vector3 removedPos;//移動前の位置
    public List<string> memoryDeck;//デッキ
    private List<string> flipCards;//めくられたカード
    private int suiterowNum;//組札の列情報
    private string suitemark;//組札のマーク情報
    private bool inDeck;//デッキにあるか
    private int cs1rowNum;//cs1の列
    private bool _suite;//組札にあるか

    //番札
    private List<string>[] bottomsData;
    private List<string> bottom0 = new List<string>();
    private List<string> bottom1 = new List<string>();
    private List<string> bottom2 = new List<string>();
    private List<string> bottom3 = new List<string>();
    private List<string> bottom4 = new List<string>();
    private List<string> bottom5 = new List<string>();
    private List<string> bottom6 = new List<string>();


    public CardMoveCommand(GameObject _solt, GameObject _selected, CardSetting _cardSetting)
    {
        slot = _solt;
        selected = _selected;
        cardSetting = _cardSetting;
    }

    public void ExecuteCommand()
    {
        //記録するデータの保存----------------------------------------------------------

        //番札
        bottom0 = new List<string>(cardSetting.number0);
        bottom1 = new List<string>(cardSetting.number1);
        bottom2 = new List<string>(cardSetting.number2);
        bottom3 = new List<string>(cardSetting.number3);
        bottom4 = new List<string>(cardSetting.number4);
        bottom5 = new List<string>(cardSetting.number5);
        bottom6 = new List<string>(cardSetting.number6);
        bottomsData = new List<string>[] { bottom0, bottom1, bottom2, bottom3, bottom4, bottom5, bottom6 };

        //CanSelect
        cs1 = slot.GetComponent<CanSelect>();
        cs2 = selected.GetComponent<CanSelect>();
        //ポジション
        removedPos = slot.transform.position;
        //親子関係
        parented = slot.transform.parent.gameObject;
        //デッキ
        memoryDeck = new List<string>(cardSetting.deck);
        //めくられたカードの情報
        flipCards = new List<string>(cardSetting.flipCardsOnDisplay);
        //組札にあるか
        _suite = cs1.suite;
        //組札の情報
        if (cs1.suite)
        {
            suiterowNum = cardSetting.suitePos[cs1.row].GetComponent<CanSelect>().value;
            suitemark = cardSetting.suitePos[cs1.row].GetComponent<CanSelect>().mark;
        }
        //デッキにあるかどうか
        inDeck = cs1.inDeckPile;
        //列
        cs1rowNum = cs1.row;
        //---------------------------------------------------------------------------------


        float OffsetY;

        //後に選択されたのが組札、もしくは先に選択されたカードが13の場合はカードをずらすＹ座標を０。
        //それ以外は少し下げる
        if (cs2.suite || (!cs2.suite && cs1.value == 13)) OffsetY = 0;
        else OffsetY = 0.5f;

        //先に選択されたカードが後に選択されたカードの子になる
        slot.transform.position = new Vector3(selected.transform.position.x, selected.transform.position.y - OffsetY, selected.transform.position.z - 0.01f);
        slot.transform.parent = selected.transform;


        //山札の場合は移動したカードを山札の配列から取り除く
        if (cs1.inDeckPile)
        {
            //int t = cardSetting.flipCardsOnDisplay.IndexOf(slot.name);
            //Debug.Log(t);
            cardSetting.flipCardsOnDisplay.Remove(slot.name);
            cardSetting.deck.Remove(slot.name);
        }
        //組札間での移動（1のみ）
        else if (cs1.suite && cs2.suite && cs1.value == 1)//後に選択された組札に置けるかはStackable関数で判定済みなのでcs2にはnullが入っていることになっている
        {
            cardSetting.suitePos[cs1.row].GetComponent<CanSelect>().value = 0;
            cardSetting.suitePos[cs1.row].GetComponent<CanSelect>().mark = null;
        }
        //先に選択されたカードが組札から取り除かれるとき
        else if (cs1.suite)
        {
            cardSetting.suitePos[cs1.row].GetComponent<CanSelect>().value = cs1.value - 1;
        }
        //移動したカードを元の配列から取り除く
        else
        {
            cardSetting.bottoms[cs1.row].Remove(slot.name);
        }

        //山札から取り除かれた以降、山札にカードを意図的に追加できないようにする
        cs1.inDeckPile = false;
        cs1.row = cs2.row;

        //組札への移動の時
        if (cs2.suite)
        {
            cardSetting.suitePos[cs1.row].GetComponent<CanSelect>().value = cs1.value;
            cardSetting.suitePos[cs1.row].GetComponent<CanSelect>().mark = cs1.mark;
            cs1.suite = true;
        }
        //それ以外
        else
        {
            cs1.suite = false;
        }
    }

    public void UndoCommand()
    {
        //戻した時点でデッキボタンについている子オブジェクトを削除
        foreach (Transform child in cardSetting.deckButton.transform)
        {
            if (child.CompareTag("Card"))
            {
                if (!cardSetting.flipCardsOnDisplay.Contains(child.gameObject.name))
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }

        //ExecuteCommand関数が実行される前の状態に戻すための値を再代入----------------------------------------
        
        //ポジション
        slot.transform.position = removedPos;
        //親子関係
        slot.transform.parent = parented.transform;
        //デッキ
        cardSetting.deck = new List<string>(memoryDeck);
        //めくられたカード
        cardSetting.flipCardsOnDisplay = new List<string>(flipCards);
        
        //組札の情報更新
        if (cs1.suite || cs2.suite)
        {
            cardSetting.suitePos[cs1.row].GetComponent<CanSelect>().value = suiterowNum;
            cardSetting.suitePos[cs1.row].GetComponent<CanSelect>().mark = suitemark;
        }
        cs1.suite = _suite;

        //番札の情報更新
        int i = 0;
        foreach (var b in bottomsData)
        {
            cardSetting.bottoms[i].Clear();
            foreach (var b2 in bottomsData[i])
            {
                cardSetting.bottoms[i].Add(b2);
            }
            i++;
        }

        //山札
        if (inDeck)
        {
            cs1.inDeckPile = inDeck;
        }
        //列
        cs1.row = cs1rowNum;
    }
}
