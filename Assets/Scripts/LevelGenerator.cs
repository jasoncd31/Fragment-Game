/*
 * Author: Kieran Ahn
 * 
 * This script contains all the logic which governs procedural level generation
 * in Fragment, from terrain generation all the way to enemy placement. 
 * 
 * Currently a work in progress!
 *
 * STATUS:
 * Terrain Generation: GOOD ENOUGH
 * Tile placement: DONE
 * Player spawn placement: GOOD ENOUGH
 * Enemy placement: GOOD ENOUGH
 * 
 */

//TODO: figure out a way to use tilesets
//TODO: figure out how to create lists of enemies to pick from
    //TODO ABOUT THAT TODO: balance enemy encounters (or maybe just have people do it themselves)
//TODO: figure out more todos
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections.Specialized;

public class LevelGenerator : MonoBehaviour
{
    int [,] map;

    public int width;
    public int height;

    public string seed;
    private System.Random rand;

    public int terrainCoveragePercentage;
    public int noiseLevel;

    public int enemyDensity;

    public GameObject playerPrefab;
    public Camera playerCamera;
    public GameObject terrainTile;
    public List<GameObject> enemies = new List<GameObject>();


    private List<int[]> walkable = new List<int[]>();

    private int BORDER_SIZE = 1;
    private int TILE_OFFSET = 100;
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
        PlaceEnemySpawns();
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
                GameObject player = Instantiate(playerPrefab, new Vector3(playerSpawn[0] * TILE_OFFSET, 0, playerSpawn[1] * TILE_OFFSET), Quaternion.identity);

                PlayerController playerController = player.GetComponent<PlayerController>();
                Camera playerCamera = Instantiate(this.playerCamera, this.playerCamera.transform.position, this.playerCamera.transform.rotation);

                FollowObject cameraTransform = playerCamera.GetComponent<FollowObject>();
                cameraTransform.playerObject = player.transform;

                playerController.mainCam = playerCamera;
                
                break;
            }
            possibleSpawnPoints.RemoveAt(spawnPoint);
        }
        if(!spawnFound)
        {
            throw new NotSupportedException("No valid player spawn has been found, but Kieran hasn't implemented regenerating the terrain yet.");
        }
    }

    void PlaceEnemySpawns()
    {
        if(enemies.Count == 0)
        {
            throw new Exception("Cannot place enemies with empty enemies list!");
        }
        List<int[]> possibleSpawnPoints = walkable.GetRange(0, walkable.Count);
        foreach(int[] neighbor in GetNeighbors(playerSpawn[0], playerSpawn[1]))
        {
            possibleSpawnPoints.Remove(neighbor);
            possibleSpawnPoints.RemoveAll(potentialSpawn => GetNeighbors(neighbor[0], neighbor[1]).Contains(potentialSpawn));
        }

        int possibleSpawnPointCount = possibleSpawnPoints.Count;
        for(int i = 0; (decimal)i < (decimal)possibleSpawnPointCount * ((decimal)enemyDensity/100m); i++)
        {
            int[] nextSpawnPoint = possibleSpawnPoints[rand.Next(0, possibleSpawnPoints.Count)];
            GameObject chosenEnemy = enemies[rand.Next(0, enemies.Count)];
            Debug.Log(String.Format("Spawning {0} at ({1}, {2})", chosenEnemy, nextSpawnPoint[0], nextSpawnPoint[1]));
            Instantiate(chosenEnemy, new Vector3(nextSpawnPoint[0]*TILE_OFFSET, (float)0.1, nextSpawnPoint[1]*TILE_OFFSET), Quaternion.identity);
            possibleSpawnPoints.Remove(nextSpawnPoint);
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
