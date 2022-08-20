using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class MapData : MonoBehaviour
{

    public int width = 64;
    public int height = 16;

    public TextAsset textAsset;

    public List<string> GetTextFiles(TextAsset tAsset)
    {
        List<string> lines = new List<string>();

        if(tAsset != null)
        {
            string textData = tAsset.text;
            string[] delimiters = { "\r\n" };
            lines.AddRange(textData.Split(delimiters, System.StringSplitOptions.None));
            lines.Reverse();
        }
        else
        {
            Debug.Log("Invalid Text file");
        }

        return lines;

    }

    public List<string> GetTextFromFile()
    {
        return GetTextFiles(textAsset);
    }

    public void SetDimensions(List<string> textLines) // Incase one of the lines is missing a number this function will complete it by adding 0s to the width
    {
            height = textLines.Count;
        foreach(string line in textLines)
        {
            if(line.Length > width)
            {
                width = line.Length;
            }
        }

    }
    public int[,] MakeMap()
    {
        List <string> lines = new List<string>();
        lines = GetTextFromFile();
        SetDimensions(lines);
        int[,] map = new int[width, height];

        for(int y = 0; y < height; y++)
        {

            for(int x = 0; x < width; x++)
            {
                if (lines[y].Length > x)
                {
                    map[x, y] = (int)Char.GetNumericValue(lines[y][x]);// This is to convert the text charachters into integers thats why i used using System above
                }
            }
        }

      /*  map[1, 0] = 1;
        map[1, 1] = 1;
        map[1, 2] = 1;
        map[3, 2] = 1;
        map[3, 3] = 1;
        map[3, 4] = 1;
        map[4, 2] = 1;
        map[5, 1] = 1;
        map[5, 2] = 1;
        map[6, 2] = 1;
        map[6, 3] = 1;
        map[8, 0] = 1;
        map[8, 1] = 1;
        map[8, 2] = 1;
        map[8, 4] = 1;
        */


        return map;
    }
    
}
