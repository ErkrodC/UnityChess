using System.Collections.Generic;
using UnityChess;
using UnityEngine;
using static UnityChess.SquareUtil;

public class BoardManager : MonoBehaviourSingleton<BoardManager> {
	[HideInInspector] public GameObject[] AllSquares = new GameObject[64];
	private static Dictionary<Square, GameObject> positionMap;

	private void Start() {
		positionMap = new Dictionary<Square, GameObject>(64);
		Transform boardTransform = transform;
		Vector3 boardPosition = boardTransform.position;
		
		for (int file = 1; file <= 8; file++) {
			for (int rank = 1; rank <= 8; rank++) {
				GameObject squareGO = new GameObject(FileRankToSquareString(file, rank), typeof(BoxCollider2D));
				BoxCollider2D squareCollider = squareGO.GetComponent<BoxCollider2D>();
				squareCollider.isTrigger = false;
				squareCollider.size = new Vector2(9, 9);
				squareGO.transform.position = new Vector3(boardPosition.x - 31.5f + (file - 1) * 9, boardPosition.y - 31.5f + (rank - 1) * 9, boardPosition.z - 7.2f);
				squareGO.transform.parent = boardTransform;
				squareGO.tag = "Square";
				
				positionMap.Add(new Square(file, rank), squareGO);
				AllSquares[(file - 1) * 8 + (rank - 1)] = squareGO;
			}
		}
	}

	public void PopulateStartBoard() {
		foreach (Piece piece in GameManager.Instance.CurrentPieces) {
			string modelName = $"{piece.PieceOwner} {piece.GetType().Name}";
			Instantiate(Resources.Load("PieceSets/Marble/" + modelName) as GameObject, positionMap[piece.Position].transform);
		}
	}
}