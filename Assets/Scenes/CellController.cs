using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CellController : MonoBehaviour
{
    public bool alive;
    public bool nextState;
    public int neighborsToResurect;
    public int neighborsToSurvive;
    public TextMeshProUGUI text; 
    public Image box;

    private int positionX;
    private int positionY;

    // Start is called before the first frame update
    void Awake()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void printToConsole()
    {
        if (alive)
        {
            print("1");
        }
        else
        {
            print("0");
        }
    }

    public void highlightBox(){
        box.color = Color.white;
    }

    public void CalculateNextStage(int count)
    {
        if (alive)
        {
            if(count == neighborsToResurect || count == neighborsToSurvive)
            {
                nextState = true;
            }
            else
            {
                nextState = false;
            }
        }
        else
        {
            if(count == neighborsToResurect)
            {
                nextState = true;
            }
            else
            {
                nextState = false;
            }
        }
    }

    public void SetInitialState(int state, int x , int y)
    {
        positionX = x;
        positionY = y;
        if (state == 0)
        {
            ChangeState(false);
        }
        else if (state == 1)
        {
            ChangeState(true);
        }
    }


    public void ChangeState(bool state)
    {
        this.alive = state;
        box.color = Color.black;

        if (state)
        {
            text.text = "1";
            text.color = Color.white;
        }
        else
        {
            text.text = "0";
            text.color = Color.black;
        }
    }

    public void Advance()
    {
        ChangeState(nextState);
    }
}
