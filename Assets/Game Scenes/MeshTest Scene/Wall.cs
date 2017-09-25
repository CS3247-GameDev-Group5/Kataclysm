using UnityEngine;

public class Wall : MonoBehaviour {

    public GameObject WallBlockPrefab;

    GameObject[] wallBlocksList;
    int mLength;
    int mHeight;

    /* Example
     * 
     * GameObject wall = Instantiate(WallPrefab, position, Quaternion.identity);
     * wall.SetSize(8, 4);
     * wall.SetWallMatrix(new int[][]
     * {
     *   new int[] {1, 1, 1, 1, 1, 1, 0, 1 },
     *   new int[] {1, 1, 1, 1, 1, 1, 1, 1 },
     *   new int[] {1, 1, 1, 1, 1, 1, 1, 1 },
     *   new int[] {1, 1, 1, 1, 1, 1, 1, 1 },
     * });
    */

    public void SetSize(int length, int height)
    {
        if(mLength == length && mHeight == height)
        {
            return;
        }

        mLength = length;
        mHeight = height;
        if (null != wallBlocksList)
        {
            foreach(GameObject w in wallBlocksList) {
                Destroy(w);
            }
        }

        wallBlocksList = new GameObject[length * height];

        for(int y = 0; y < height; y++)
        {
            for (int x = 0; x < length; x++)
            {
                wallBlocksList[y*length + x] = Instantiate(WallBlockPrefab, new Vector3(-1 + length - (x*2), 1 + (y*2), 0), Quaternion.identity, this.transform);
            }
        }
    }

    public bool SetWallMatrix(int[][] matrix)
    {
        if(mHeight != matrix.Length || mLength != matrix[0].Length) {
            return false;
        }

        for(int y = 0; y < mHeight; y++)
        {
            for(int x = 0; x < mLength; x++)
            {
                wallBlocksList[y * mLength + x].SetActive(1 == matrix[mHeight - 1 - y][x]);
            }
        }

        return true;
    }
}
