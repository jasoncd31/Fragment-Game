/*
 * Author: Kieran Ahn
 * 
 * This script contains all the logic which governs procedural level generation
 * in Fragment, from terrain generation all the way to enemy placement. 
 * 
 * Currently a work in progress!
 *
 * STATUS:
 * Terrain Generation: DONE
 * Tile placement: IN PROGRESS
 * 
 */


//TODO: to check if wall tile, just start from the edges and increment your way inwards until the next tile in line is a 1, not a 0. Should be O(w*h) time (linear scaling with width and height)
//TODO: figure out a way to use tilesets
//TODO: figure out how to create lists of enemies to pick from
    //TODO ABOUT THAT TODO: balance enemy encounters (or maybe just have people do it themselves)
//TODO: figure out more todos
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

    //This is currently unnecessary imo, but I'll leave it in in case we want to do extra smoothing later. -Kieran
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

    // Adapted from tutorial by pavcreations at https://pavcreations.com/procedural-generation-of-2d-maps-in-unity/3/
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
