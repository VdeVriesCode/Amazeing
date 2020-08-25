using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    Grid grid;
    Grid.Coord playerCoord, oldPlayerCoord;
    void Awake()
    {
        grid = GameObject.Find("Grid").GetComponent<Grid>();
        playerCoord = new Grid.Coord(0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }
    
    private void Movement()
    {
        oldPlayerCoord = playerCoord;
        if (Input.GetKeyDown(KeyCode.W))
        {
            if (playerCoord.y < grid.mapSize.y - 1)
                playerCoord.y++;
        }
        if (Input.GetKeyDown(KeyCode.A))
        {
            if (playerCoord.x > 0 )
                playerCoord.x--;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            if (playerCoord.y > 0)
                playerCoord.y--;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            if (playerCoord.x < grid.mapSize.x - 1)
                playerCoord.x++;
        }
        if(!grid.obstacleMap[playerCoord.x,playerCoord.y])
            transform.position = grid.CoordToPosition(playerCoord);
        else
            playerCoord = oldPlayerCoord;
        

    }
}
