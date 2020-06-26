using System.Collections.Generic;
using UnityEngine;

public class DeckCommand : IAction
{
    private CardSetting cardSetting;

    private List<GameObject> opendCard = new List<GameObject>();
    private List<Vector3> preMovePos = new List<Vector3>();
    private List<GameObject> createdNewCard = new List<GameObject>();
    private int locationNum;
    private List<string> flipCards;
    private List<string> memorydeck = new List<string>();
    private List<string> memorydiscardPile = new List<string>();

    private GameObject flipedCards;


    public DeckCommand(CardSetting _cardSetting)
    {
        cardSetting = _cardSetting;
    }

    public void ExecuteCommand()
    {
        flipedCards = GameObject.Find("FlipedCards");

        memorydeck = new List<string>(cardSetting.deck);
        
        memorydiscardPile = new List<string>(cardSetting.discardPile);

        foreach (Transform child in cardSetting.deckButton.transform)
        {
            if (child.CompareTag("Card"))
            {
                cardSetting.deck.Remove(child.name);
                cardSetting.discardPile.Add(child.name);
                preMovePos.Add(child.gameObject.transform.position);
                child.gameObject.transform.position = flipedCards.transform.position;
                child.gameObject.SetActive(false);
                opendCard.Add(child.gameObject);
            }
        }

        while(cardSetting.deckButton.transform.childCount > 0)
        {
            cardSetting.deckButton.transform.GetChild(0).parent = flipedCards.transform;

        }

        if (cardSetting.deckLocation < cardSetting.flipCards)
        {
            flipCards = new List<string>(cardSetting.flipCardsOnDisplay);
            cardSetting.flipCardsOnDisplay.Clear();
            float OffsetX = 3.5f;
            float OffsetZ = 0.2f;

            foreach (string card in cardSetting.deckFlipCards[cardSetting.deckLocation])
            {
                GameObject newTopCard = GameObject.Instantiate(cardSetting.cardPrefab,
                    new Vector3(cardSetting.deckButton.transform.position.x + OffsetX, cardSetting.deckButton.transform.position.y, cardSetting.deckButton.transform.position.z + OffsetZ),
                    Quaternion.identity);

                newTopCard.transform.parent = cardSetting.deckButton.transform;

                newTopCard.transform.localScale = new Vector3(0.95f, 0.95f, 1);


                OffsetX -= 0.5f;
                OffsetZ += 0.2f;
                newTopCard.name = card;
                cardSetting.flipCardsOnDisplay.Add(card);
                newTopCard.GetComponent<CanSelect>().turn = true;
                newTopCard.GetComponent<CanSelect>().inDeckPile = true;
                createdNewCard.Add(newTopCard);
            }
            locationNum = cardSetting.deckLocation;
            cardSetting.deckLocation++;
        }
        else
        {
            cardSetting.RestackToDeck();
        }
    }

    public void UndoCommand()
    {
        if (opendCard != null)
        {
            int i = 0;
            foreach (var o in opendCard)
            {
                o.transform.position = preMovePos[i];
                o.transform.parent = cardSetting.deckButton.transform;
                o.SetActive(true);
                i++;
            }
        }
        for(int i= 0; i < createdNewCard.Count; i++)
        {
            GameObject.Destroy(createdNewCard[i]);
        }
        cardSetting.deckLocation = locationNum;
        cardSetting.flipCardsOnDisplay.Clear();
        if (flipCards != null)
        {
            foreach (var f in flipCards)
            {
                cardSetting.flipCardsOnDisplay.Add(f);
            }
        }

        cardSetting.deck = new List<string>(memorydeck);
        
        cardSetting.discardPile = new List<string>(memorydiscardPile);
    }
}
