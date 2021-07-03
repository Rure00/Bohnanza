using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardNumberLogic : MonoBehaviour
{
    GameManager gameManagerLogic;

    void Awake()
    {
        gameManagerLogic = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        gameObject.GetComponentInChildren<Text>().text = gameManagerLogic.DeckNumb.ToString();
    }
}
