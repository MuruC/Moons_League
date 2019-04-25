using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableUnit : MonoBehaviour
{
    public bool thisUnitHasBeenClicked = false;
    public Vector2 destination;
    public int destinationXIndex;
    public int destinationYIndex;
    public float speed;
    public bool bMoving = false;
    public List<GameObject> greenTiles;
    // Start is called before the first frame update
    void Start()
    {
        destination = transform.position;
        greenTiles = new List<GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.x == destination.x && transform.position.y == destination.y)
        {
            if (bMoving)
            {
                bMoving = false;
                whenUnitFirstArriveDestination();
            }
        }
        else
        {
            bMoving = true;
        }


        Vector2 dir = new Vector2(destination.x - transform.position.x, destination.y - transform.position.y);
        Vector2 velocity = dir.normalized * speed * Time.deltaTime;

        velocity = Vector2.ClampMagnitude(velocity, dir.magnitude);

        transform.Translate(velocity);
    }

    void whenUnitFirstArriveDestination() {
        if (thisUnitHasBeenClicked == false) {
            for (int i = 0; i < greenTiles.Count; i++)
            {
                greenTiles[i].GetComponent<SpriteRenderer>().color = Color.white;
            }
            greenTiles.Clear();
        }
    }
}
