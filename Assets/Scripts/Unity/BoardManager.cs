using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityChess;

public class BoardManager : MonoBehaviour {

    private Board Board;
    private static Dictionary<Square, Vector3> PositionMap;

	// Use this for initialization
	void Start () {
        Board = new Board();
        PositionMap = new Dictionary<Square, Vector3>(64);

        for (int file=1; file<=8; file++)
        {
            for (int rank = 1; rank <= 8; rank++)
            {
                PositionMap.Add(new Square(file, rank), new Vector3(this.transform.position.x - 31.5f + (file - 1) * 9, this.transform.position.y - 31.5f + (rank - 1) * 9, this.transform.position.z - 7f));
            }
        }
	}
	
	public void PopulateStartBoard()
    {
        string ModelName;

        foreach (Piece piece in Board.BasePieceList.OfType<Piece>())
        {
            ModelName = "";
            ModelName += piece.Side.ToString() + " ";
            ModelName += piece.GetType().Name;

            Instantiate(Resources.Load("Models/Arcane Cyber/Chess/Prefabs/Marble/" + ModelName) as GameObject, PositionMap[piece.Position], new Quaternion(0,0,0,0), GameObject.FindWithTag("Board").transform);
        }

    }
}
