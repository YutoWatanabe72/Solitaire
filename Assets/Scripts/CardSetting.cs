using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardSetting : MonoBehaviour
{
    public CommandManager commandManager;

    [Header("ToSetOnDisplay")]
    public GameObject cardPrefab;
    public Sprite[] faceSprites;
    public GameObject deckButton;
    public GameObject[] numberplatePos;
    public GameObject[] suitePos;
    public GameObject difficultySelection;

    public static string[] mark = new string[] { "C", "D", "H", "S" };

    public List<string> deck;
    public List<string> discardPile = new List<string>();

    [HideInInspector]
    public List<string>[] bottoms;
    [HideInInspector]
    public List<string> number0 = new List<string>();
    [HideInInspector]
    public List<string> number1 = new List<string>();
    [HideInInspector]
    public List<string> number2 = new List<string>();
    [HideInInspector]
    public List<string> number3 = new List<string>();
    [HideInInspector]
    public List<string> number4 = new List<string>();
    [HideInInspector]
    public List<string> number5 = new List<string>();
    [HideInInspector]
    public List<string> number6 = new List<string>();

    [HideInInspector]
    public int flipNum = 1;
    [HideInInspector]
    public int flipCards;
    private int flipCardsRemainder;
    public List<List<string>> deckFlipCards = new List<List<string>>();
    public int deckLocation;
    public List<string> flipCardsOnDisplay = new List<string>();

    private float timer;
    private bool start;
    void Start()
    {
        bottoms = new List<string>[] { number0, number1, number2, number3, number4, number5, number6 };
        timer = 0f;
        start = false;
    }

    void Update()
    {
        if (start) return;

        timer += Time.deltaTime;
        if(timer >= 0.7f)
        {
            difficultySelection.SetActive(true);
            start = true;
        }
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialization()
    {
        foreach (List<string> list in bottoms)
        {
            list.Clear();
        }

        deck = DeckCreate();
        TurnShuffle(deck);

        Deploy();
        StartCoroutine(Generate());
        FlipTheDeck();

    }

    /// <summary>
    /// 山札の生成
    /// </summary>
    /// <returns></returns>
    public static List<string> DeckCreate()
    {
        List<string> newDeck = new List<string>();
        foreach (string str in mark)
        {
            for (int i = 1; i <= 13; i++)
            {
                newDeck.Add(str + i.ToString());
            }
        }

        return newDeck;
    }

    /// <summary>
    /// 山札の順番をシャッフル
    /// </summary>
    void TurnShuffle<T>(List<T> list)
    {
        int num = list.Count;
        while (num > 1)
        {
            int rnd = Random.Range(0, num);
            num--;

            T temp = list[rnd];
            list[rnd] = list[num];
            list[num] = temp;
        }

    }

    /// <summary>
    /// カードを生成
    /// カードの構成要素をCardオブジェクトに追加
    /// </summary>
    IEnumerator Generate()
    {
        for (int i = 0; i < 7; i++)
        {
            float OffsetY = 0;
            float OffsetZ = 0.03f;
            foreach (string card in bottoms[i])
            {
                yield return new WaitForSeconds(0.05f);
                GameObject newCard = Instantiate(cardPrefab,
                    new Vector3(numberplatePos[i].transform.position.x, numberplatePos[i].transform.position.y - OffsetY, numberplatePos[i].transform.position.z - OffsetZ),
                    Quaternion.identity, numberplatePos[i].transform);
                newCard.name = card;
                newCard.GetComponent<CanSelect>().row = i;
                if (card == bottoms[i][bottoms[i].Count - 1])
                {
                    newCard.GetComponent<CanSelect>().turn = true;
                }

                OffsetY += 0.25f;
                OffsetZ += 0.03f;

                discardPile.Add(card);
            }
        }

        foreach (string card in discardPile)
        {
            if (deck.Contains(card))
            {
                deck.Remove(card);
            }
        }

        discardPile.Clear();
    }

    /// <summary>
    /// カードの配置
    /// </summary>
    /// <param name="_tranpCard">配置するカード</param>
    /// <param name="_cardStatus">配置するカードのCardStatusスクリプト</param>
    void Deploy()
    {
        for (int i = 0; i < 7; i++)
        {
            for (int j = i; j < 7; j++)
            {
                bottoms[j].Add(deck.Last<string>());
                deck.RemoveAt(deck.Count - 1);
            }
        }

        //確認用
        for (int i = 0; i < bottoms.Length; i++)
        {
            foreach (var l in bottoms[i])
            {
                UnityEngine.Debug.Log(l + i);
            }
        }
    }

    /// <summary>
    /// 山札をめくる
    /// </summary>
    void FlipTheDeck()
    {
        flipCards = deck.Count / flipNum;
        flipCardsRemainder = deck.Count % flipNum;
        deckFlipCards.Clear();

        int range = 0;
        for (int i = 0; i < flipCards; i++)
        {
            List<string> myFlipCards = new List<string>();
            for (int j = 0; j < flipNum; j++)
            {
                myFlipCards.Add(deck[j + range]);
            }
            deckFlipCards.Add(myFlipCards);
            range += flipNum;
        }
        if (flipCardsRemainder != 0)
        {
            List<string> myCardsRemainders = new List<string>();
            range = 0;
            for (int k = 0; k < flipCardsRemainder; k++)
            {
                myCardsRemainders.Add(deck[deck.Count - flipCardsRemainder + range]);
                range++;
            }
            deckFlipCards.Add(myCardsRemainders);
            flipCards++;
        }
        deckLocation = 0;
    }

    /// <summary>
    /// デッキをクリックしたときの処理
    /// </summary>
    public void DealFromDeck()
    {
        commandManager.ExecuteCommand(new DeckCommand(GetComponent<CardSetting>()));
    }

    /// <summary>
    /// デッキがなくなった時にめくったカードをデッキに戻す
    /// </summary>
    public void RestackToDeck()
    {
        deck.Clear();
        foreach (string card in discardPile)
        {
            deck.Add(card);
        }
        discardPile.Clear();
        FlipTheDeck();

    }
}
