using System.Collections.Generic;
using UnityEngine;

public class UpdateSprite : MonoBehaviour
{
    public Sprite cardFace;
    public Sprite cardBack;
    private SpriteRenderer spriteRenderer;
    private CanSelect canSelect;
    private CardSetting cardSetting;
    private UserInput userInput;

    void Start()
    {
        List<string> deck = CardSetting.DeckCreate();
        cardSetting = FindObjectOfType<CardSetting>();
        userInput = FindObjectOfType<UserInput>();

        int i = 0;
        foreach(string card in deck)
        {
            if(this.name == card)
            {
                cardFace = cardSetting.faceSprites[i];
                break;
            }
            i++;
        }
        spriteRenderer = GetComponent<SpriteRenderer>();
        canSelect = GetComponent<CanSelect>();
    }

    /// <summary>
    /// スプライトデータの更新
    /// 選択されたら黄色に表示
    /// </summary>
    void Update()
    {
        if (canSelect.turn)
        {
            spriteRenderer.sprite = cardFace;
        }
        else
        {
            spriteRenderer.sprite = cardBack;
        }

        if (userInput.slot)
        {
            if(name == userInput.slot.name)
            {
                spriteRenderer.color = Color.yellow;
            }
            else
            {
                spriteRenderer.color = Color.white;
            }
        }
    }
}
