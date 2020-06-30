using System.Linq;
using UnityEngine;

public class UserInput : MonoBehaviour
{
    public CommandManager commandManager;

    public GameObject slot;
    private CardSetting cardSetting;
    private float timer;
    private float doubleClickTime = 0.3f;
    private int clickCount;

    void Start()
    {
        cardSetting = FindObjectOfType<CardSetting>();
        slot = this.gameObject;
    }

    void Update()
    {
        if (clickCount == 1)
        {
            timer += Time.deltaTime;
        }

        if (clickCount == 3)
        {
            timer = 0;
            clickCount = 1;
        }

        if (timer > doubleClickTime)
        {
            timer = 0;
            clickCount = 0;
        }

        GetMouseClick();
    }

    void GetMouseClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            clickCount++;
            //Debug.Log(clickCount);


            Vector3 mousePos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y - 10));
            RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit)
            {
                if (hit.collider.CompareTag("Deck"))
                {
                    Debug.Log("Deck");
                    Deck();
                }
                else if (hit.collider.CompareTag("Card"))
                {
                    Debug.Log("Card");
                    Card(hit.collider.gameObject);
                }
                else if (hit.collider.CompareTag("Suite"))
                {
                    Debug.Log("Top");
                    Suite(hit.collider.gameObject);//組札
                }
                else if (hit.collider.CompareTag("Bottom"))
                {
                    Debug.Log("Bottom");
                    Bottom(hit.collider.gameObject);
                }
            }
        }
    }

    /// <summary>
    /// デッキの場合
    /// </summary>
    void Deck()
    {
        cardSetting.DealFromDeck();
        slot = this.gameObject;
    }

    /// <summary>
    /// カードの場合
    /// </summary>
    /// <param name="selected">クリックされたカード</param>
    void Card(GameObject selected)
    {
        //めくられていないカードの判定
        if (!selected.GetComponent<CanSelect>().turn)
        {
            if (!Blocked(selected))
            {
                commandManager.ExecuteCommand(new CardFlipCommand(selected.GetComponent<CanSelect>()));
                //selected.GetComponent<CanSelect>().turn = true;
                slot = this.gameObject;
            }
        }
        //デッキからめくったカードの判定
        else if (selected.GetComponent<CanSelect>().inDeckPile)
        {
            if (!Blocked(selected))
            {
                if (slot == selected)
                {
                    //選択したカードがダブルクリックされて、組札に置けるなら自動で移動
                    if (DoubleClick()) AutoStack(selected);
                }
                else
                {
                    slot = selected;
                }
            }
        }
        //それ以外
        else
        {
            if (slot == this.gameObject)
            {
                slot = selected;
            }
            else if (slot != selected)
            {
                if (Stackable(selected)) Stack(selected);
                else
                {
                    slot = selected;
                }
            }
            else if (slot == selected)
            {
                if (DoubleClick()) AutoStack(selected);
            }
        }
    }

    /// <summary>
    /// ダブルクリックの判定
    /// </summary>
    /// <returns>可か不可</returns>
    bool DoubleClick()
    {
        if (timer < doubleClickTime && clickCount == 2) return true;
        else return false;
    }

    /// <summary>
    /// 組札の場合
    /// </summary>
    /// <param name="selected">選択された組札</param>
    void Suite(GameObject selected)
    {
        if (slot.CompareTag("Card"))
        {
            //組札にカードが無いときのみ処理が入る（カードがあるときは、マウスをクリックするとカードにヒットするから）
            if (slot.GetComponent<CanSelect>().value == 1)
            {
                Stack(selected);
            }
        }
    }

    /// <summary>
    /// 番札（カードが置かれていないフィールド）の場合
    /// </summary>
    /// <param name="selected"></param>
    void Bottom(GameObject selected)
    {
        if (slot.CompareTag("Card"))
        {
            if (slot.GetComponent<CanSelect>().value == 13)
            {
                Stack(selected);
            }
        }
    }

    /// <summary>
    /// 対象のカードが条件を満たしているならトリガーを解除する
    /// </summary>
    /// <param name="selected">選択されたカード</param>
    /// <returns>true   めくれない</returns>
    /// <returns>false  めくれる</returns>
    bool Blocked(GameObject selected)
    {
        CanSelect cs2 = selected.GetComponent<CanSelect>();
        if (cs2.inDeckPile == true)
        {
            //山札からめくったカードが最初のであれば選択可能な状態にする
            //それ以外は選択不可
            if (cs2.name == cardSetting.flipCardsOnDisplay.First())
            {
                return false;
            }
        }
        else
        {
            //番札にあるめくられていないカードが一番上にあるならめくれるようにする
            //それ以外は判定不可
            if (cs2.name == cardSetting.bottoms[cs2.row].Last())
            {
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// カードを次の場所に移動できるかどうかの判定
    /// </summary>
    /// <param name="selected"></param>
    /// <returns></returns>
    bool Stackable(GameObject selected)
    {
        CanSelect cs1 = slot.GetComponent<CanSelect>();
        CanSelect cs2 = selected.GetComponent<CanSelect>();

        //山札にあるカードかどうか
        if (!cs2.inDeckPile)
        {
            //先に選択されたカードが後に選択された組札に置けるかの判定
            if (cs2.suite)
            {
                if (cs1.mark == cs2.mark || (cs1.value == 1 && cs2.mark == null))
                {
                    if (cs1.value == cs2.value + 1) return true;
                }
                else
                {
                    return false;
                }
            }
            //番札の一番下に階段式に置けるか
            else
            {
                if (cs1.value == cs2.value - 1)
                {
                    //カードのマークが違う色ならtrue、同じならfalseを返す
                    if (cs1.cardRed != cs2.cardRed) return true;
                    else return false;
                }
            }
        }
        //山札のカードを選択していた場合はfalseを返す
        return false;
    }

    //カードの移動
    void Stack(GameObject selected)
    {
        commandManager.ExecuteCommand(new CardMoveCommand(slot,selected,cardSetting));
        
        slot = this.gameObject;
        
    }

    /// <summary>
    /// ダブルクリックしたときに自動で組札に移動
    /// </summary>
    /// <param name="selected"></param>
    void AutoStack(GameObject selected)
    {
        for (int i = 0; i < cardSetting.suitePos.Length; i++)
        {
            //ダブルクリックをしたカードが１の時
            CanSelect stack = cardSetting.suitePos[i].GetComponent<CanSelect>();
            if (selected.GetComponent<CanSelect>().value == 1)
            {
                if (cardSetting.suitePos[i].GetComponent<CanSelect>().value == 0)
                {
                    slot = selected;
                    Stack(stack.gameObject);
                    break;
                }
            }
            //それ以外
            else
            {
                if ((cardSetting.suitePos[i].GetComponent<CanSelect>().mark == slot.GetComponent<CanSelect>().mark)
                    && (cardSetting.suitePos[i].GetComponent<CanSelect>().value == slot.GetComponent<CanSelect>().value - 1))
                {
                    if (HasNoChildren(slot))
                    {
                        slot = selected;
                        string lastCardname = stack.mark + stack.value.ToString();

                        GameObject lastCard = GameObject.Find(lastCardname);
                        Stack(lastCard);
                        break;
                    }
                }
            }
        }
    }

    /// <summary>
    /// 番札にあるカードに子があるか判定
    /// 無い→true
    /// ある→false
    /// </summary>
    bool HasNoChildren(GameObject card)
    {
        int i = 0;
        foreach (Transform chile in card.transform)
        {
            i++;
        }

        if (i == 0) return true;
        else return false;
    }
}
