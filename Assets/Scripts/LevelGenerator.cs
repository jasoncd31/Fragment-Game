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
 * Boss room placement: GOOD ENOUGH
 * Terrain regeneration: DONE
 * Obstacle placement: IN PROGRESS
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
    public GameObject bossRoom;
    public GameObject terrainTile;
    public List<GameObject> enemies = new List<GameObject>();


    private List<int[]> walkable = new List<int[]>();

    private int BORDER_SIZE = 1;
    private int TILE_OFFSET = 100;
    private int BASIC_TILE = 1;
    private int BOSS_ROOM_RADIUS = 4;

    private int[] playerSpawn;
    private int[] bossLocation;

    // Start is called before the first frame update
    void Start()
    {
        bool generationComplete = false;
        int generationAttempts = 1;
        while (!generationComplete)
        {
            Debug.Log(String.Format("Generating terrain: attempt {0}", generationAttempts));
            generationComplete = GenerateLevel();
        }
    }

    bool GenerateLevel()
    {
        seed = string.IsNullOrEmpty(seed) ? DateTime.Now.ToString() : seed;
        rand = new System.Random(seed.GetHashCode());

        Debug.Log(String.Format("Generating terrain with seed {0}", seed));

        CreateTerrain();

        bool bossRoomPlaced = PlaceBossRoom();
        if (!bossRoomPlaced)
        {
            return false;
        }

        bool playerSpawnPlaced = PlacePlayerSpawn();
        if(!playerSpawnPlaced)
        {
            return false;
        }

        PlaceEnemySpawns();
        return true;
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

    bool PlacePlayerSpawn()
    {
        List<int[]> possibleSpawnPoints = walkable.GetRange(0, walkable.Count);
        bool spawnFound = false;
        while(possibleSpawnPoints.Count > 0)
        {
            int spawnPoint = rand.Next(0, possibleSpawnPoints.Count);
            int[] potentialPlayerSpawn = possibleSpawnPoints[spawnPoint];
            List<int[]> potentialSpawnNeighbors = GetTilesInRadius(potentialPlayerSpawn[0], potentialPlayerSpawn[1], 1);
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
            return false;
        }
        return true;
    }

    void PlaceEnemySpawns()
    {
        if(enemies.Count == 0)
        {
            throw new Exception("Cannot place enemies with empty enemies list!");
        }
        List<int[]> possibleSpawnPoints = walkable.GetRange(0, walkable.Count);
        foreach(int[] neighbor in GetTilesInRadius(playerSpawn[0], playerSpawn[1], 1))
        {
            possibleSpawnPoints.Remove(neighbor);
        }

        int possibleSpawnPointCount = possibleSpawnPoints.Count;
        for(int i = 0; (decimal)i < (decimal)possibleSpawnPointCount * ((decimal)enemyDensity/100m); i++)
        {
            int[] nextSpawnPoint = possibleSpawnPoints[rand.Next(0, possibleSpawnPoints.Count)];
            GameObject chosenEnemy = enemies[rand.Next(0, enemies.Count)];
            Debug.Log(String.Format("Spawning {0} at ({1}, {2})", chosenEnemy, nextSpawnPoint[0], nextSpawnPoint[1]));
            Instantiate(chosenEnemy, new Vector3(nextSpawnPoint[0]*TILE_OFFSET, 0, nextSpawnPoint[1]*TILE_OFFSET), Quaternion.identity);
            possibleSpawnPoints.Remove(nextSpawnPoint);
        }
    }

    bool PlaceBossRoom()
    {
        List<int[]> possibleSpawnPoints = walkable.GetRange(0, walkable.Count);
        possibleSpawnPoints.RemoveAll(potentialSpawn => (Math.Abs(potentialSpawn[0] - width) < BOSS_ROOM_RADIUS) || (Math.Abs(potentialSpawn[1] - height) < BOSS_ROOM_RADIUS) || potentialSpawn[0] < BOSS_ROOM_RADIUS || potentialSpawn[1] < BOSS_ROOM_RADIUS);
        
        bool locationFound = false;
        while(possibleSpawnPoints.Count > 0)
        {
            int[] potentialSpawnPoint = possibleSpawnPoints[rand.Next(0, possibleSpawnPoints.Count)];
            Debug.Log(String.Format("Checking ({0}, {1})", potentialSpawnPoint[0], potentialSpawnPoint[1]));
            List<int[]> tilesAroundBossRoom = GetTilesInRadius(potentialSpawnPoint[0], potentialSpawnPoint[1], BOSS_ROOM_RADIUS);
            if (tilesAroundBossRoom.TrueForAll(tile => map[tile[0], tile[1]] == BASIC_TILE))
            {
                locationFound = true;
                bossLocation = potentialSpawnPoint;
                Instantiate(bossRoom, new Vector3(bossLocation[0] * TILE_OFFSET, 30f, bossLocation[1] * TILE_OFFSET), Quaternion.identity);
                walkable.RemoveAll(tile => tilesAroundBossRoom.Contains(tile));
                break;
            }
            possibleSpawnPoints.Remove(potentialSpawnPoint);
        }
        if(!locationFound)
        {
            return false;
        }
        return true;
    }

    // --- HELPER FUNCTIONS ---
    /**
     * Gets every tile within the radius centered on the coordinate. 
     * Breaks the Pythagorean theorem.
     */
    List<int[]> GetTilesInRadius(int x, int y, int radius)
    {
        List<int[]> tilesInRadius = new List<int[]>();
        for(int i = radius; i > 0; i--)
        {
            for(int j = i; j > 0; j--)
            {
                tilesInRadius.Add(new int[] { x+i, y+j });
                tilesInRadius.Add(new int[] { x+i, y-j });
                tilesInRadius.Add(new int[] { x-i, y+j });
                tilesInRadius.Add(new int[] { x-i, y-j });
            }
            tilesInRadius.Add(new int[] { x + i, y });
            tilesInRadius.Add(new int[] { x - i, y });
            tilesInRadius.Add(new int[] { x, y + i });
            tilesInRadius.Add(new int[] { x, y - i });
        }
        return tilesInRadius;
    }

    /**
     * Gets the four tiles to the north, south, east, and west of the coordinate
     */
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
