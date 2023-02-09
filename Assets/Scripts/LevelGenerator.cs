//TODO: document all this, Kieran
//TODO: when writing your check to connect an island, make sure you check to see if the offset plus the position of the tile is outside the bounds of the map (I KNOW YOU'LL FORGET)
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
    public int noiseLevel;

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
        
        int startX = rand.Next(1, width-1);
        int startY = rand.Next(1, height-1);
        List<int[]> frontier = new List<int[]>();
        frontier.Add(new int[] {startX, startY});

        for (int i = 0; (decimal)i < ((decimal)width * (decimal)height) * ((decimal)terrainCoveragePercentage/100m); i++)
        {
            if(frontier.Count == 0)
            {
                break;
            }

            int[] nextTile;
            int chosenTile = rand.Next(0, frontier.Count);
            nextTile = frontier[chosenTile];
            frontier.RemoveAt(chosenTile);

            map[nextTile[0], nextTile[1]] = 1;
            foreach(int[] tile in GetNeighbors(nextTile[0], nextTile[1]))
            {
                if (tile[0] >= width || tile[1] >= height || tile[0] < 0 || tile[1] < 0)
                {
                    continue;
                }

                if (!frontier.Contains(tile) && map[tile[0], tile[1]] == 0 && rand.Next(0, 100) > noiseLevel)
                {
                    frontier.Add(tile);
                }
            }
        }
    }

    void SmoothTerrain()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++) 
            {
                // map[x, y] = (GetNeighborStates(x, y, map) > 2) ? 1 : map[x, y];
            }
        }
    }

    void RemoveDeadTiles()
    {
        for (int x = 1; x < width - 1; x++)
        {
            for (int y = 1; y < height - 1; y++) 
            {
                // map[x, y] = (GetNeighborStates(x, y, map) == 0) ? 0 : map[x, y];
            }
        }
    }

    int GetNeighborStates(int x, int y, int[,] map)
    {
        return map[x-1, y] + map[x, y+1] + map[x+1, y] + map[x, y-1];
    }

    List<int[]> GetNeighbors(int x, int y)
    {
        List<int[]> neighbors = new List<int[]>();
        neighbors.Add(new int[] {x-1,y});
        neighbors.Add(new int[] {x+1,y});
        neighbors.Add(new int[] {x,y-1});
        neighbors.Add(new int[] {x,y+1});

        return neighbors;
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
