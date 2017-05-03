using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;
using Assets.Script;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{
    public static BoardManager Instance { set; get; }
    private bool[,] allowedMoves { set; get; }
    public Chessman[,] Chessmans { set; get; }
    public Chessman selectedChessman;

    private const float TILE_SIZE = 1.0f;
    private const float TILE_OFFSET = -4.0f;
    public List<GameObject> chessPrefabs;
    private List<GameObject> activeChessMan;

    private Quaternion orientation = Quaternion.Euler(0, 180, 0);

    public Button showTeamButton;
    public bool isWhiteTurn = true;

    private int selectionX = -1;
    private int selectionY = -1;

    private void Start()
    {
        Instance = this;
        SpawnAllChessmans();
    }
    private void Update()
    {
        UpdateSelection();
        DrawChessboard();

        if (Input.GetMouseButtonDown(0))
        {
            if (selectionX >= 0 && selectionY >= 0)
            {
                if (selectedChessman == null)
                {
                    //select the chessman
                    SelectChessman(selectionX, selectionY);
                }
                else
                {
                    //move the chessman
                    MoveChessman(selectionX, selectionY);
                    if(isWhiteTurn)
                    {
                        showTeamButton.GetComponentInChildren<Text>().text = "White's turn";
                    } else
                    {
                        showTeamButton.GetComponentInChildren<Text>().text = "Black's turn";
                    }
                }
            }
        }
    }


    private void SelectChessman(int x, int y)
    {
        if (Chessmans[x, y] == null)
        {
            return; // return
        }

        if (Chessmans[x, y].isWhite != isWhiteTurn)
        {
            return; //trying pick piece is black piec
        }

        bool hasAtleastOneMove = false;
        allowedMoves = Chessmans[x, y].PossibleMove();
        for (int i = 0; i < 8; i++)
            for (int j = 0; j < 8; j++)
                if (allowedMoves[i, j])
                    hasAtleastOneMove = true;

        if (!hasAtleastOneMove)
            return;

        selectedChessman = Chessmans[x, y];
        BoardHighlights.Instance.HighLightAllowedMoves(allowedMoves);
    }

    private void MoveChessman(int x, int y)
    {
        if (allowedMoves[x, y])
        {

            Chessman c = Chessmans[x, y];
            if (c != null && c.isWhite != isWhiteTurn)
            {

                // Capture a piece

                // If it is the king end the game
                if (c.GetType() == typeof(King))
                {
                    EndGame();
                    return;
                }
                activeChessMan.Remove(c.gameObject);
                Destroy(c.gameObject);

            }

            Chessmans[selectedChessman.CurrentX, selectedChessman.CurrentY] = null; //we move
            selectedChessman.transform.position = GetTileCenter(x, y + 1);
            selectedChessman.SetPosition(x, y);
            Chessmans[x, y] = selectedChessman;
            isWhiteTurn = !isWhiteTurn; // swap to other value, only 
        }
        BoardHighlights.Instance.Hidehighlights();
        selectedChessman = null; // unselect if click on place doesnt make sense
    }

    private void UpdateSelection()
    {
        if (!Camera.main)
        {
            return;
        }
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 25.0f, LayerMask.GetMask("ChessPlane")))
        {
            Debug.Log((int)hit.point.z + 3);
            selectionX = (int)Math.Ceiling(hit.point.x) + 3;
            selectionY = (int)Math.Ceiling(hit.point.z) + 3;
        }

        else
        {
            selectionX = -1;
            selectionY = -1;
        }



    }

    private Vector3 GetTileCenter(int x, int y)
    {
        Vector3 origin = Vector3.zero;
        origin.x += (TILE_SIZE * x) + TILE_OFFSET;
        origin.z += (TILE_SIZE * y) + TILE_OFFSET;

        return origin;
    }

    private void SpawnAllChessmans()
    {
        activeChessMan = new List<GameObject>();
        Chessmans = new Chessman[8, 8];
        // Spawn the white team

        // King
        SpawChessMan(0, 3, 0);

        //Qn
        SpawChessMan(1, 4, 0);

        // Rooks
        SpawChessMan(2, 0, 0);
        SpawChessMan(2, 7, 0);

        //Bishops
        SpawChessMan(3, 2, 0);
        SpawChessMan(3, 5, 0);

        //Knights
        SpawChessMan(4, 1, 0);
        SpawChessMan(4, 6, 0);


        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawChessMan(5, i, 1);
        }




        // Spawn the Black team

        // King
        SpawChessMan(6, 4, 7);

        //Qn
        SpawChessMan(7, 3, 7);

        // Rooks
        SpawChessMan(8, 0, 7);
        SpawChessMan(8, 7, 7);

        //Bishops
        SpawChessMan(9, 2, 7);
        SpawChessMan(9, 5, 7);

        //Knights
        SpawChessMan(10, 1, 7);
        SpawChessMan(10, 6, 7);


        // Pawns
        for (int i = 0; i < 8; i++)
        {
            SpawChessMan(11, i, 6);
        }

    }
    private void DrawChessboard()
    {

        Vector3 widthLine = Vector3.right * 8;
        Vector3 heightLine = Vector3.forward * 8;

        for (int i = 0; i <= 8; i++)
        {
            Vector3 start = Vector3.forward * i;
            Debug.DrawLine(start, start + widthLine);

            for (int j = 0; j <= 8; j++)
            {
                start = Vector3.right * j;
                Debug.DrawLine(start, start + heightLine);

            }

        }

        // Draw the selection
        if (selectionX >= 0 && selectionY >= 0)
        {
            Debug.DrawLine(
                Vector3.forward * selectionY + Vector3.right * selectionX,
                Vector3.forward * (selectionY + 1) + Vector3.right * (selectionX + 1));


            Debug.DrawLine(
              Vector3.forward * (selectionY + 1) + Vector3.right * selectionX,
              Vector3.forward * (selectionY) + Vector3.right * (selectionX + 1));

        }

    }

    private void SpawChessMan(int index, int x, int y)
    {
        GameObject go = Instantiate(chessPrefabs[index], GetTileCenter(x, y + 1), orientation) as GameObject;
        go.transform.SetParent(transform);

        Chessmans[x, y] = go.GetComponent<Chessman>(); // Index out of range exception

        Chessmans[x, y].SetPosition(x, y);
        activeChessMan.Add(go);
    }

    private void EndGame()
    {
        if (isWhiteTurn)
            Debug.Log("White team wins");
        else
            Debug.Log("Black team wins");

        foreach (GameObject go in activeChessMan)
            Destroy(go);

        isWhiteTurn = true;
        BoardHighlights.Instance.Hidehighlights();
        SpawnAllChessmans();
    }
}
