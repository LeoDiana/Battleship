using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Ship
{
    public int size;
    public bool isVertical;
    public Coords coords; //місце де стоїть корабель на дошці. 
                          //рахується або знайлівішого, або з нижнього "шматочка"  (хооо)

    public Ship(int _size, bool _isVertical, Coords _coords)
    {
        size = _size;
        isVertical = _isVertical;
        coords = _coords;
    }

}

public class ShipController : MonoBehaviour
{
    public Ship ship;
    public BoardController bc;

    private Vector2 mousePosition;
    private Vector2 lastPosition;
    private float dX, dY;


    void OnMouseDown()
    {
        Debug.Log(gameObject);
        lastPosition = transform.position;
        dX = Camera.main.ScreenToWorldPoint(Input.mousePosition).x - transform.position.x;
        dY = Camera.main.ScreenToWorldPoint(Input.mousePosition).y - transform.position.y;

        if (bc.Check(ship, bc.WorldToBoardCoords(transform.position)) == 0)
        {
            bc.ByeShip(ship, bc.WorldToBoardCoords(transform.position));
        }

        bc.ChangeActiveShip(gameObject);
    }

    void OnMouseDrag()
    {
        mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = new Vector2(Convert.ToInt32(mousePosition.x - dX), Convert.ToInt32(mousePosition.y - dY));
    }

    void OnMouseUp()
    {
        switch (bc.Check(ship, bc.WorldToBoardCoords(transform.position)))
        {
            case 0: { //повертаємо назад
                        transform.position = lastPosition;
                        bc.HelloShip(ship, bc.WorldToBoardCoords(transform.position));
                        break;
                    }
            case 1: { //ок, ставимо на дошку
                        bc.HelloShip(ship, bc.WorldToBoardCoords(transform.position));
                        break;
                    };
        }
    }
}
