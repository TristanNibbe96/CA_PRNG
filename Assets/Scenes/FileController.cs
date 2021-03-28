using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEditor;
using System.Linq;

public class FileController : MonoBehaviour
{

    public int outputPRGNBitSize;
    public BoardController board;
    public int RNGstepSize;

    private string currRandomNumberBin;
    private int currRandomNumberDec;
    private StreamWriter HDWriter;
    private StreamWriter numWriter;

    void Start(){
    	string hDPath = "Assets/GenerationOutput/HD.txt";
        string numbersPath = "Assets/Output/PRNG.txt";

        FileUtil.DeleteFileOrDirectory(hDPath);
        HDWriter = new StreamWriter(hDPath, true);

        FileUtil.DeleteFileOrDirectory(numbersPath);
        numWriter = new StreamWriter(numbersPath, true);
    } 

    public void writeHammingDisToFile(int count){
        HDWriter.WriteLine(count);
    }

    public void WriteGenerationToFile()
    {
        string path = "Assets/GenerationOutput/Gen" + board.generationNum + ".txt";
        FileUtil.DeleteFileOrDirectory(path);
        StreamWriter writer = new StreamWriter(path, true);
        for (int j = 0; j < board.board.GetLength(1); j++)
        {
            string line = "";
            for (int i = 0; i < board.board.GetLength(0); i++)
            {

                if (board.board[i, j].alive)
                {
                    line += " 1";
                }
                else
                {
                    line += " 0";
                }
            }
            writer.WriteLine(line);
        }
        writer.Close();
    }

    public void closeTextWriters(){
    	    HDWriter.Close();
    	    numWriter.Close();
    }

    public int getRandomlyGeneratedNumber()
    {
        return currRandomNumberDec;
    }
    
    public string convertDecToBin(int dec)
    {
        int decToConvert = dec;
        string reverseBin = "";

        while (decToConvert != 0)
        {
            reverseBin += decToConvert % 2;
            decToConvert = decToConvert / 2;
        }

        print(reverseBin);
        string bin = "";

        for(int i = reverseBin.Length-1; i >= 0; i--)
        {
            bin += reverseBin[i];
        }
        return bin;
    }

    public void advance()
    {
        int[,] boardAsBinary = ConvertBoardToBinaryArray();
        int xoredBoardAsDecimal = PerformXOROnBoard(boardAsBinary);

        currRandomNumberBin = convertDecToBin(xoredBoardAsDecimal);
        currRandomNumberDec = xoredBoardAsDecimal;
    }

    public void WriteRandomlyGeneratedNumberToFile()
    {
        numWriter.WriteLine(currRandomNumberDec + " " + currRandomNumberBin);
    }

    public string convertIntArrayToString(int[,] intArr)
    {
        string arrString = "";

        for (int j = 0; j < outputPRGNBitSize; j++)
        {
            for (int i = 0; i < outputPRGNBitSize; i++)
            {
                arrString += intArr[i, j];
            }
        }

        return arrString;
    }

    public int PerformXOROnBoard(int[,] RGN){
        int sliceStartLocation = 16 - 3 - (2 * (outputPRGNBitSize-2));
        int[,] topRightSlicedSection = sliceSectionOfLargeBoard(sliceStartLocation,0,2);
        int[,] bottomLeftSlicedSection = sliceSectionOfLargeBoard(0,sliceStartLocation,2);

        int RGNasDEC = ConvertBinaryArrayToDecimal(RGN);
        int topRightAsDecimal = ConvertBinaryArrayToDecimal(topRightSlicedSection);
        int bottomLeftAsDecimal = ConvertBinaryArrayToDecimal(bottomLeftSlicedSection);

        print("ORG Bin: " + convertIntArrayToString(RGN) + " ORG Dec: " + RGNasDEC);
        print("TR Bin: " + convertIntArrayToString(topRightSlicedSection) + " TR Dec: " + topRightAsDecimal);
        print("BL Bin: " + convertIntArrayToString(bottomLeftSlicedSection) + " BL Dec: " + bottomLeftAsDecimal);

        int RGNXORedwithSlice = RGNasDEC ^ topRightAsDecimal;
        print(convertIntArrayToString(RGN) + " XOR " + convertIntArrayToString(topRightSlicedSection) + " = " + RGNXORedwithSlice);
        RGNXORedwithSlice = RGNXORedwithSlice ^ bottomLeftAsDecimal;


        return RGNXORedwithSlice;
    }

    public int[,] sliceSectionOfLargeBoard(int horOffset,int vertOffset,int stepSize){
        int[,] slicedSection = new int[outputPRGNBitSize,outputPRGNBitSize];

        for (int j = 0; j < outputPRGNBitSize; j++)
        {
            for (int i = 0; i < outputPRGNBitSize; i++)
            {
                board.board[(i * stepSize) + horOffset, (j * stepSize) + vertOffset].highlightBox();
                if(board.board[(i * stepSize) + horOffset, (j * stepSize) + vertOffset].alive){
                    slicedSection[i,j] = 1;
                }else{
                    slicedSection[i,j] = 0;
                }
            }
        }

        return slicedSection;
    }

    public int ConvertBinaryArrayToDecimal(int[,] binary)
    {
        int dec = 0;
        for (int j = 0; j < outputPRGNBitSize; j++)
        {
            for (int i = 0; i < outputPRGNBitSize; i++)
            {
                dec = 2 * dec + binary[i, j];
            }
        }

        return dec;
    }


    public int[,] ConvertBoardToBinaryArray()
    {
        int[,] binaryBoard = new int[outputPRGNBitSize, outputPRGNBitSize];

        for (int j = 0; j < outputPRGNBitSize; j++)
        {
            for(int i = 0; i < outputPRGNBitSize; i++)
            {
                int boardX = (board.generationNum + (RNGstepSize * i)) % board.boardX;
                int boardY = (board.generationNum + (RNGstepSize * j)) % board.boardY;
            	board.board[boardX,boardY].highlightBox();
                if (board.board[boardX, boardY].alive)
                {
                   binaryBoard[i, j] = 1;
                }
                else
                {
                    binaryBoard[i, j] = 0;
                }
            }
        }


        return binaryBoard;
    }

    public void Encode()
    {
        string inName = "Assets/Input/in.txt";
        string outName = "Assets/Output/out.txt";
        FileUtil.DeleteFileOrDirectory(outName);
        StreamReader input = new StreamReader(inName);
        StreamWriter output = new StreamWriter(outName, true);
        

        for (int j = 0; j < board.boardY; j++)
        {
            string line = "";
            for(int i = 0; i < board.boardX; i++)
            {
                int temp = input.Read();
                while (temp != 48 && temp != 49)
                {
                    temp = input.Read();
                }

                bool tempBool = decodeInt(temp);
                if (tempBool == board.board[i,j].alive)
                {
                   line += "1";
                }
                else
                {
                    line += "0";
                }
            }
            output.WriteLine(line);
        }

        input.Close();
        output.Close();

    }

    public bool decodeInt(int i)
    {
        if(i == 49){
            return true;
        }
        else
        {
            return false;
        }
    }
}
