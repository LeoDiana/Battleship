using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattleController : MonoBehaviour
{
    public BattleBoard board_p1;
    public BattleBoard board_p2;
    public Text text;

    void Start()
    {
        board_p1.isUnderAtack = false;
        board_p2.isUnderAtack = true;
    }

    public void EndTurn(int forPlayer)
    {
        text.text = $"Player {forPlayer} turn";
        if(forPlayer == 1)
        {
            board_p1.isUnderAtack = false;
            board_p2.isUnderAtack = true;
        }
        if (forPlayer == 2)
        {
            board_p2.isUnderAtack = false;
            board_p1.isUnderAtack = true;
        }
    }

    public void Win(int forPlayer)
    {
        board_p1.isUnderAtack = false;
        board_p2.isUnderAtack = false;

        text.text = $"Player {(forPlayer % 2) +1} win!!!";
    }
}
