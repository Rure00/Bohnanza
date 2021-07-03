using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DropZoneLogic : MonoBehaviour
{
    GameManager gameManagerLogic;
    public PlayerLogic playerLogic;

    public GameObject[] plantedBeans;

    public int beanIndex = -1;
    public int beanNumb = 0;
    public int ZoneIndex;
    public int farmIndex;

    public int score;

    public Text scoreText;

    public bool isDropable;
    public bool isCursorOn;
    public bool isBought;

    void Awake()
    {
        gameManagerLogic = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();
        getMyPlayerLogic();

        plantedBeans = new GameObject[20];

        scoreText.text = "0(0)";

        if (farmIndex == 2)
            isBought = false;
        else
            isBought = true;
    }


    public void getMyPlayerLogic()
    {
        playerLogic = gameManagerLogic.playerLogics[ZoneIndex].GetComponent<PlayerLogic>();
    }

    public void Scoring(int BeanIndex)
    {
        switch (BeanIndex)
        {
            case -1:
                score = 0;
                break;
            case 0:
                if (beanNumb >= 3)
                    score = 3;
                else if(beanNumb >= 2)
                    score = 2;
                else if (beanNumb >= 0)
                    score = 0;
                break;
            case 1:
                if (beanNumb >= 5)
                    score = 4;
                else if (beanNumb >= 4)
                    score = 3;
                else if (beanNumb >= 3)
                    score = 2;
                else if (beanNumb >= 2)
                    score = 1;
                else if (beanNumb >= 0)
                    score = 0;
                break;
            case 2:
                if (beanNumb >= 6)
                    score = 4;
                else if (beanNumb >= 5)
                    score = 3;
                else if (beanNumb >= 4)
                    score = 2;
                else if (beanNumb >= 2)
                    score = 1;
                else if (beanNumb >= 0)
                    score = 0;
                break;
            case 3:
                if (beanNumb >= 7)
                    score = 4;
                else if (beanNumb >= 6)
                    score = 3;
                else if (beanNumb >= 4)
                    score = 2;
                else if (beanNumb >= 2)
                    score = 1;
                else if (beanNumb >= 0)
                    score = 0;
                break;
            case 4:
                if (beanNumb >= 7)
                    score = 4;
                else if (beanNumb >= 6)
                    score = 3;
                else if (beanNumb >= 5)
                    score = 2;
                else if (beanNumb >= 3)
                    score = 1;
                else if (beanNumb >= 0)
                    score = 0;
                break;
            case 5:
                if (beanNumb >= 8)
                    score = 4;
                else if (beanNumb >= 7)
                    score = 3;
                else if (beanNumb >= 5)
                    score = 2;
                else if (beanNumb >= 3)
                    score = 1;
                else if (beanNumb >= 0)
                    score = 0;
                break;
            case 6:
                if (beanNumb >= 9)
                    score = 4;
                else if (beanNumb >= 8)
                    score = 3;
                else if (beanNumb >= 6)
                    score = 2;
                else if (beanNumb >= 3)
                    score = 1;
                else if (beanNumb >= 0)
                    score = 0;
                break;
            case 7:
                if (beanNumb >= 10)
                    score = 4;
                else if (beanNumb >= 8)
                    score = 3;
                else if (beanNumb >= 6)
                    score = 2;
                else if (beanNumb >= 4)
                    score = 1;
                else if (beanNumb >= 0)
                    score = 0;
                break;

        }

        scoringText();
    }

    void scoringText()
    {
        if(beanNumb == 0)
        {
            scoreText.text = score.ToString();
        }
        else
        {
            scoreText.text = score.ToString() + "(" + beanNumb.ToString() + ")";
        }
    }

    void OnMouseEnter()
    {
        playerLogic.farmYouChoose = gameObject;
    }
    void OnMouseExit()
    {
        playerLogic.farmYouChoose = null;
    }
}