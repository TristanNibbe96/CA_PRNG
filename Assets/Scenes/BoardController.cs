using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public int boardX;
    public int boardY;
    public CellController[,] board;
    public CellController Cell;
    public UIController uiController;
    public FileController fileController;
    public int generationNum = 0;
    public int generationsToAdvance;
    public float timeToWaitBetweenGens;

    private bool[,] startingGen;
    private bool[,] currGenMinusOne;


    // Start is called before the first frame update
    void Start()
    {

        board = new CellController[boardX, boardY];
        startingGen = new bool[boardX, boardY];
        currGenMinusOne = new bool[boardX, boardY];
        for (int i = 0; i < boardX; i++)
        {
            for (int j = 0; j < boardY; j++)
            {
                Vector3 pos = new Vector3(this.transform.position.x + i, this.transform.position.y - j, this.transform.position.z);
                GameObject obj = Instantiate(Cell.gameObject, pos, this.transform.rotation, this.transform);
                board[i, j] = obj.GetComponent<CellController>();
                board[i, j].SetInitialState(Random.Range(0, 2),i,j);
                startingGen[i, j] = board[i, j].alive;
            }
        }
        fileController.advance();
        uiController.updateUI(generationNum);

        StartCoroutine("startAdvancing");
    }

    IEnumerator startAdvancing(){
        for(int i = 0; i < generationsToAdvance; i++){
            CalculateNextGeneration();
            yield return new WaitForSeconds(timeToWaitBetweenGens);
        }
    }

    public void OutputStateToFile()
    {
        fileController.WriteGenerationToFile();
    }


    public void CalculateNextGeneration()
    {
        SetPreviousGenBoolArray();
        for (int i = 0; i < boardX; i++)
        {
            for (int j = 0; j < boardY; j++)
            {
                board[i, j].CalculateNextStage(GetAliveNeighborCount(i, j));
            }
        }
        AdvanceGeneration();
    }

    public int GetAliveNeighborCount(int x, int y)
    {
        int count = 0;

        for (int i = x - 1; i <= x + 1; i++)
        {
            for (int j = y - 1; j <= y + 1; j++)
            {
                int neighborX = i;
                int neighborY = j;

                if (i >= board.GetLength(0))
                {
                    neighborX = 0;
                }
                else if (i < 0)
                {
                    neighborX = board.GetLength(0) - 1;

                }
                if (j >= board.GetLength(1))
                {
                    neighborY = 0;

                }
                else if (j < 0)
                {
                    neighborY = board.GetLength(1) - 1;

                }
                if (board[neighborX, neighborY].alive)
                {
                    count++;
                }
            }
        }
        if (board[x, y].alive)
        {
            count--;
        }
        return count;
    }

    public void AdvanceGeneration()
    {
        for (int i = 0; i < boardX; i++)
        {
            for (int j = 0; j < boardY; j++)
            {
                board[i, j].Advance();
            }
        }

        generationNum++;
        fileController.advance();
        uiController.updateUI(generationNum);
    }

    public int compareHammingDistanceOfToStartingGen()
    {
        int count = 0;
        for (int i = 0; i < boardX; i++)
        {
            for (int j = 0; j < boardY; j++)
            {
                if(startingGen[i, j] != board[i, j].alive)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public int compareHammingDistanceToPreviousGen()
    {
        int count = 0;
        for (int i = 0; i < boardX; i++)
        {
            for (int j = 0; j < boardY; j++)
            {
                if (currGenMinusOne[i, j] != board[i, j].alive)
                {
                    count++;
                }
            }
        }
        fileController.writeHammingDisToFile(count);   
        return count;
    }



    public void SetPreviousGenBoolArray()
    {
        for (int i = 0; i < boardX; i++)
        {
            for (int j = 0; j < boardY; j++)
            {
                currGenMinusOne[i,j] = board[i, j].alive;
            }
        }

    }
}

