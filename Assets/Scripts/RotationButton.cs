using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationButton : MonoBehaviour
{
    public BoardController bc;
    void OnMouseUp()
    {
        bc.RotateActiveShip();
    }
}
