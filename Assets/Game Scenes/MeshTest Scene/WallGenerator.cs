using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallGenerator : MonoBehaviour {
    public GameObject WallPrefab;

    // [x][y][z]
    public int[][] GenerateWallMatrix(List<int[][][]> katMatrices)
    {
        if(null == katMatrices || 0 == katMatrices.Count)
        {
            throw new System.Exception("No kat matrices provided");
        }
        List<int[][]> katSilhouettes = new List<int[][]>();

        // stack kats random first then merge in z
        foreach(int[][][] katMatrix in katMatrices)
        {
            if(null == katMatrix || 0 == katMatrix.Length || 0 == katMatrix[0].Length || 0 == katMatrix[0][0].Length)
            {
                throw new System.Exception("katMatrix invalid");
            }

            int maxX = katMatrix.Length;
            int maxY = katMatrix[0].Length;
            int maxZ = katMatrix[0][0].Length;
            int[][] newSilhouette = new int[maxX][];

            for(int x = 0; x < maxX; x++)
            {
                newSilhouette[x] = new int[maxY];

                for(int y = 0; y < maxY; y++)
                {
                    for (int z = 0; z < maxZ; z++)
                    {
                        if(katMatrix[x][y][z] != 0)
                        {
                            newSilhouette[x][y] = 1;
                            break;
                        }
                    }
                }   
            }
            katSilhouettes.Add(newSilhouette);
        }

        return new int[1][];
    }
}
