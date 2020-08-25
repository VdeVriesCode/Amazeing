using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class Grid : MonoBehaviour
{
    public Transform tilePrefab;
    //public Transform firePrefab;
    public Transform obstaclePrefab;
    public Vector2 mapSize;
    public int seed = 10;
    [HideInInspector]
    public bool[,] obstacleMap;
    
    [Range(0, 1)]
    public float outlinePercent;
    //[Range(0, 1)]
    // public float firePercent;
    [Range(0, 1)]
    public float obstaclePercent;

    List<Coord> allTileCoords;
    Queue<Coord> shuffledTileCoords;
    Coord playerSpawnPosition = new Coord(0, 0);
    Coord mapCentre;
    private void Start()
    {
        GenerateGrid();
    }
    public void GenerateGrid()
    {

        allTileCoords = new List<Coord>();
        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x,y));
            }
        }
        
        shuffledTileCoords = new Queue<Coord>(Shuffle.ShuffleArray(allTileCoords.ToArray(), seed));
        mapCentre = new Coord ((int)mapSize.x / 2, (int)mapSize.y / 2);

        string holderName = "Generated Grid";
        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform gridHolder = new GameObject(holderName).transform;
        gridHolder.parent = transform;

        for (int x = 0; x < mapSize.x; x++)
        {
            for (int y = 0; y < mapSize.y; y++)
            {
                Coord tilePos = new Coord(x, y);
                Vector3 tilePosition = CoordToPosition(tilePos);
                Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;

                newTile.localScale = Vector3.one * (1 - outlinePercent);
                newTile.parent = gridHolder;

            }
        }

        obstacleMap = new bool[(int) mapSize.x, (int) mapSize.y];

        int obstacleCount = (int)(mapSize.x * mapSize.y * obstaclePercent);
        int currentObstacleCount = 0;
        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            if (randomCoord != mapCentre && MapFullyAccesible(obstacleMap, currentObstacleCount) && randomCoord != playerSpawnPosition ) 
            {
                Transform newObstacle = Instantiate(obstaclePrefab, CoordToPosition(randomCoord) , Quaternion.identity) as Transform;
                newObstacle.localScale = Vector3.one * (1 - outlinePercent);
                newObstacle.position = newObstacle.position + new Vector3(0, newObstacle.localScale.y / 2, 0);
                newObstacle.parent = gridHolder;
            }
            else {
                obstacleMap[randomCoord.x,randomCoord.y] = false;
                currentObstacleCount --;
            }
        }
    }

     bool MapFullyAccesible(bool[,] obstacleMap, int currentObstacleCount)
    {
        bool[,] mapFlags = new bool[obstacleMap.GetLength(0),obstacleMap.GetLength(1)];
        Queue<Coord> queue = new Queue<Coord> ();
        queue.Enqueue (mapCentre);
        mapFlags [mapCentre.x, mapCentre.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0) {
            Coord tile = queue.Dequeue();

            for (int x = -1; x <= 1; x ++) {
                for (int y = -1; y <= 1; y ++) {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0) {
                        if (neighbourX >= 0 && neighbourX < obstacleMap.GetLength(0) && neighbourY >= 0 && neighbourY < obstacleMap.GetLength(1)) {
                            if (!mapFlags[neighbourX,neighbourY] && !obstacleMap[neighbourX,neighbourY]) {
                                mapFlags[neighbourX,neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX,neighbourY));
                                accessibleTileCount ++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(mapSize.x * mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

     public Vector3 CoordToPosition(Coord c)
     {
         return new Vector3(-mapSize.x / 2 + 0.5f + c.x, 0, -mapSize.y / 2 + 0.5f + c.y);
     }
    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }
        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1 ==c2);
        }

        public static Coord operator +(Coord c1, Coord c2)
        {
            Coord sum;
            sum.x = c1.x + c2.x;
            sum.y = c1.y + c2.y;
            return sum;
        }
    }
}

