using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
    int [,] map;

    public int width;
    public int height;

    public string seed;
    private System.Random rand;

    public int terrainCoveragePercentage;

    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        seed = (seed == "0") ? Time.time.ToString() : seed;
        rand = new System.Random(seed.GetHashCode());

        CreateTerrain();
        
        for (int i = 0; i < 2; i++)
        {
            SmoothTerrain();
            RemoveDeadTiles();
        }
        
    }

    void CreateTerrain() 
    {
        map = new int[width, height];
        
        for(int x = 0; x < width; x++) 
        {
            for(int y = 0; y < height; y++)
            {
                map[x, y] = (rand.Next(0, 100) < terrainCoveragePercentage) ? 1 : 0;
            }
        }
    }

    void SmoothTerrain()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++) 
            {
                map[x, y] = (GetNeighborStates(x, y, map) > 2) ? 1 : map[x, y];
            }
        }
    }

    void RemoveDeadTiles()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++) 
            {
                map[x, y] = (GetNeighborStates(x, y, map) == 0) ? 0 : map[x, y];
            }
        }
    }

    int GetNeighborStates(int x, int y, int[,] map)
    {
        return map[x-1, y] + map[x, y+1] + map[x+1, y] + map[x, y-1];
    }

    void OnDrawGizmos()
    {
        if (map != null)
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.green : Color.black;
                    Vector3 pos = new Vector3(x + 1, 0, y + 1);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}
