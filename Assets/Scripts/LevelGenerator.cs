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
 * Tile placement: DONE
 * Player spawn placement: IN PROGRESS
 * 
 */


//TODO: to check if wall tile, just start from the edges and increment your way inwards until the next tile in line is a 1, not a 0. Should be O(w*h) time (linear scaling with width and height)
    //TODO ABOUT THAT TODO: terrible idea and doesn't work. Fix it.
//TODO: figure out a way to use tilesets
//TODO: figure out how to create lists of enemies to pick from
    //TODO ABOUT THAT TODO: balance enemy encounters (or maybe just have people do it themselves
//TODO: figure out more todos
using System.Collections;
using System.Collections.Generic;
using System;
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

    public GameObject terrainTile;

    private List<int[]> walkable = new List<int[]>();

    private int BORDER_SIZE = 1;
    private int TILE_OFFSET = 10;
    private int BASIC_TILE = 1;

    private int[] playerSpawn;

    // Start is called before the first frame update
    void Start()
    {
        GenerateLevel();
    }

    void GenerateLevel()
    {
        seed = string.IsNullOrEmpty(seed) ? DateTime.Now.ToString() : seed;
        rand = new System.Random(seed.GetHashCode());

        CreateTerrain();
        PlacePlayerSpawn();
    }

    void CreateTerrain() 
    {
        map = new int[width + BORDER_SIZE, height + BORDER_SIZE];
        
        int startX = rand.Next(1, width);
        int startY = rand.Next(1, height);
        List<int[]> frontier = new List<int[]>();
        frontier.Add(new int[] {startX, startY});

        int[] nextTile;
        int chosenTile;
        for (int i = 0; (decimal)i < ((decimal)width * (decimal)height) * ((decimal)terrainCoveragePercentage/100m); i++)
        {
            if(frontier.Count == 0)
            {
                break;
            }

            chosenTile = rand.Next(0, frontier.Count);
            nextTile = frontier[chosenTile];
            frontier.RemoveAt(chosenTile);

            map[nextTile[0], nextTile[1]] = BASIC_TILE;
            Instantiate(terrainTile, new Vector3(nextTile[0]*TILE_OFFSET, 0, nextTile[1]*TILE_OFFSET), Quaternion.identity);
            walkable.Add(nextTile);

            foreach(int[] tile in GetNeighbors(nextTile[0], nextTile[1]))
            {
                if (tile[0] >= width || tile[1] >= height || tile[0] <= 0 || tile[1] <= 0)
                {
                    continue;
                }

                if (frontier.Count == 0 || (!frontier.Contains(tile) && map[tile[0], tile[1]] == 0 && rand.Next(0, 100) > noiseLevel))
                {
                    frontier.Add(tile);
                }
            }
        }
    }

    void PlacePlayerSpawn()
    {
        List<int[]> possibleSpawnPoints = walkable.GetRange(0, walkable.Count);
        bool spawnFound = false;
        while(possibleSpawnPoints.Count > 0)
        {
            int spawnPoint = rand.Next(0, possibleSpawnPoints.Count);
            int[] potentialPlayerSpawn = possibleSpawnPoints[spawnPoint];
            List<int[]> potentialSpawnNeighbors = GetNeighbors(potentialPlayerSpawn[0], potentialPlayerSpawn[1]);
            if(potentialSpawnNeighbors.TrueForAll(neighbor => map[neighbor[0], neighbor[1]] == BASIC_TILE))
            {
                playerSpawn = potentialPlayerSpawn;
                spawnFound = true;
                Debug.Log(String.Format("Player spawn set at ({0}, {1}).", playerSpawn[0], playerSpawn[1]));
                break;
            }
            possibleSpawnPoints.RemoveAt(spawnPoint);
        }
        if(!spawnFound)
        {
            throw new NotSupportedException("No valid player spawn has been found, but Kieran hasn't implemented regenerating the terrain yet.");
        }
    }

    //This is currently unnecessary imo, but I'll leave it in in case we want to do extra smoothing later. -Kieran
    void SmoothTerrain()
    {
        for (int x = 1; x < width; x++)
        {
            for (int y = 1; y < height; y++) 
            {
                map[x, y] = (GetNeighborStates(x, y, map) > 2) ? 1 : map[x, y];
            }
        }
    }


    // --- HELPER FUNCTIONS ---
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
            for (int x = 0; x < map.GetLength(0); x++)
            {
                for (int y = 0; y < map.GetLength(1); y++)
                {
                    Gizmos.color = (map[x, y] == 1) ? Color.green : Color.black;
                    Vector3 pos = new Vector3(x + 1, 0, y + 1);
                    Gizmos.DrawCube(pos, Vector3.one);
                }
            }
        }
    }
}
