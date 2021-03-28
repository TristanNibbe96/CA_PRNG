using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
	public TextMeshProUGUI genTex;
    public Text startingGenerationHamDisTex;
    public Text previousGenerationHamDisTex;
    public TextMeshProUGUI randomNumText;
    public BoardController board;
    public FileController fileController;

    public void UpdateStartingGenHamTex()
    {
        double percent = ((double) board.compareHammingDistanceOfToStartingGen());
       	percent = percent / (board.boardX * board.boardY) * 100;
        
        startingGenerationHamDisTex.text = "Start HD: " + percent.ToString("F2") + "%";
    }

    public void writeRandomNumToText()
    {
        randomNumText.text = fileController.getRandomlyGeneratedNumber().ToString();
    }

    public void UpdatePreviousGenHamTex()
    {
    	double percent = ((double) board.compareHammingDistanceToPreviousGen());
       	percent = percent / (board.boardX * board.boardY) * 100;

        previousGenerationHamDisTex.text = "Gen-1 HD: " + percent.ToString("F2") + "%";
    }

    public void closeTextWriters()
    {
        fileController.closeTextWriters();
    }

    public void upDateGenText(int genNumber)
    {
        genTex.text = "Gens: " + genNumber;
    }

    public void updateUI(int genNumber){
    	upDateGenText(genNumber);
        UpdateStartingGenHamTex();
        UpdatePreviousGenHamTex();
        writeRandomNumToText();
        fileController.WriteRandomlyGeneratedNumberToFile();
    }

}
