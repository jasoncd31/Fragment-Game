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
    private int [,] map;
    private (int, bool) [,] heightMap;

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
    public GameObject islandUndersideBlock;
    public List<GameObject> enemies = new List<GameObject>();


    private List<int[]> walkable = new List<int[]>();

    private int BORDER_SIZE = 1;
    private int TILE_OFFSET = 100;
    private int BASIC_TILE = 1;
    private int BOSS_ROOM_RADIUS = 3;

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
            generationAttempts++;
            if(generationAttempts > 5)
            {
                throw new Exception("Something has gone horribly wrong.");
            }
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
            Debug.Log("Boss room unable to be placed.");
            return false;
        }

        bool playerSpawnPlaced = PlacePlayerSpawn();
        if(!playerSpawnPlaced)
        {
            Debug.Log("Player spawn unable to be placed.");
            return false;
        }

        PlaceEnemySpawns();
        MakeCliffs();
        return true;
    }

    void CreateTerrain()
    {
        (map, heightMap) = (new int[width + BORDER_SIZE, height + BORDER_SIZE], new (int, bool)[width + BORDER_SIZE, height + BORDER_SIZE]);

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
            heightMap[nextTile[0], nextTile[1]] = (1, false);
            Instantiate(terrainTile, new Vector3(nextTile[0]*TILE_OFFSET, 0, nextTile[1]*TILE_OFFSET), Quaternion.identity);
            walkable.Add(nextTile);

            List<int[]> neighbors = new List<int[]>(4);
            neighbors.Add(new int[] { nextTile[0] + 1, nextTile[1] });
            neighbors.Add(new int[] { nextTile[0] - 1, nextTile[1] });
            neighbors.Add(new int[] { nextTile[0], nextTile[1] + 1 });
            neighbors.Add(new int[] { nextTile[0], nextTile[1] - 1 });
            foreach(int[] tile in neighbors)
            {
                if (tile[0] >= width || tile[1] >= height || tile[0] <= 0 || tile[1] <= 0)
                {
                    continue;
                }

                if (frontier.Count == 0 || (!frontier.Exists(node => node[0] == tile[0] && node[1] == tile[1]) && map[tile[0], tile[1]] == 0 && rand.Next(0, 100) > noiseLevel))
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
        possibleSpawnPoints.RemoveAll(tile => tile[0] == playerSpawn[0] && tile[1] == playerSpawn[1]);
        foreach(int[] neighbor in GetTilesInRadius(playerSpawn[0], playerSpawn[1], 1))
        {
            possibleSpawnPoints.RemoveAll(tile => tile[0] == neighbor[0] && tile[1] == neighbor[1]);
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
            if (tilesAroundBossRoom.Count == (Math.Pow(BOSS_ROOM_RADIUS, 2) + BOSS_ROOM_RADIUS) * 4)
            {
                locationFound = true;
                bossLocation = potentialSpawnPoint;
                Instantiate(bossRoom, new Vector3(bossLocation[0] * TILE_OFFSET, 30f, bossLocation[1] * TILE_OFFSET), Quaternion.identity);
                walkable.Remove(potentialSpawnPoint);
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

    void MakeCliffs()
    {
        int startX = rand.Next(1, width);
        int startY = rand.Next(1, height);

        while(!walkable.Exists(tile => tile[0] == startX & tile[1] == startY)) 
        {
            startX = rand.Next(1, width);
            startY = rand.Next(1, height);
        }

        UpdateHeightMap(startX, startY);

    }

    void UpdateHeightMap(int x, int y)
    {
        int east = map[x + 1, y] == 0 ? -1 : map[x + 1, y];
        int west = map[x - 1, y] == 0 ? -1 : map[x - 1, y];
        int north = map[x, y + 1] == 0 ? -1 : map[x, y + 1];
        int south = map[x, y - 1] == 0 ? -1 : map[x, y - 1];

        heightMap[x, y] = (Math.Min(Math.Max(east + west + north + south, 1), 20), true);

        GameObject cliff = Instantiate(islandUndersideBlock, new Vector3(x * TILE_OFFSET, -(heightMap[x, y].Item1 * TILE_OFFSET / 2) - 0.01f, y * TILE_OFFSET), Quaternion.identity);
        cliff.transform.localScale = new Vector3(TILE_OFFSET/10, heightMap[x, y].Item1 * TILE_OFFSET/10, TILE_OFFSET/10);

        if (map[x + 1, y] != 0 && !heightMap[x + 1, y].Item2)
        {
            UpdateHeightMap(x + 1, y);
        }
        if (map[x - 1, y] != 0 && !heightMap[x - 1, y].Item2)
        {
            UpdateHeightMap(x - 1, y);
        }
        if (map[x, y + 1] != 0 && !heightMap[x, y + 1].Item2)
        {
            UpdateHeightMap(x, y + 1);
        }
        if (map[x, y-1] != 0 && !heightMap[x, y - 1].Item2)
        {
            UpdateHeightMap(x, y-1);
        }
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
            for(int j = radius; j > 0; j--)
            {
                (int,int)[] diagonals = { (x + i, y + j), (x + i, y - j), (x - i, y + j), (x - i, y - j) };
                foreach (var direction in diagonals)
                {
                    var tile = walkable.Find(position => position[0] == direction.Item1 && position[1] == direction.Item2);
                    if (tile != null)
                    {
                        tilesInRadius.Add(tile);
                    }
                }

            }
            (int,int)[] cardinals = { (x + i, y), (x - i, y), (x, y + i), (x, y - i) };
            foreach (var direction in cardinals)
            {
                var tile = walkable.Find(position => position[0] == direction.Item1 && position[1] == direction.Item2);
                if (tile != null)
                {
                    tilesInRadius.Add(tile);
                }
            }
        }
        return tilesInRadius;
    }

    /**
     * Gets the four tiles to the north, south, east, and west of the coordinate
     */
    List<int[]> GetNeighbors(int x, int y)
    {
        List<int[]> neighbors = new List<int[]>();
        (int,int)[] directions = { (x + 1, y), (x - 1, y), (x, y - 1), (x, y + 1) };
        foreach (var direction in directions)
        {
            var tile = walkable.Find(position => position[0] == direction.Item1 && position[1] == direction.Item2);
            if(tile != null)
            {
                neighbors.Add(tile);
            }
                
        }

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
