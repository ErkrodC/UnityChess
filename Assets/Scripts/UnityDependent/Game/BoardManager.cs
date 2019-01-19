using System;
using System.Collections.Generic;
using UnityChess;
using UnityEngine;
using static UnityChess.SquareUtil;

public class BoardManager : MonoBehaviourSingleton<BoardManager> {
	[HideInInspector] public GameObject[] AllSquaresGO = new GameObject[64];
	private Dictionary<Square, GameObject> positionMap;
	private const float BoardPlaneSideLength = 14f; // measured from corner square center to corner square center, on same side.
	private const float BoardPlaneSideHalfLength = BoardPlaneSideLength * 0.5f;
	private const float BoardHeight = 1.6f;

	private void Start() {
		positionMap = new Dictionary<Square, GameObject>(64);
		Transform boardTransform = transform;
		Vector3 boardPosition = boardTransform.position;
		
		for (int file = 1; file <= 8; file++) {
			for (int rank = 1; rank <= 8; rank++) {
				GameObject squareGO = new GameObject(FileRankToSquareString(file, rank), typeof(BoxCollider2D));
				squareGO.transform.position = new Vector3(boardPosition.x + FileOrRankToSidePosition(file), boardPosition.y + BoardHeight, boardPosition.z + FileOrRankToSidePosition(rank));
				squareGO.transform.parent = boardTransform;
				squareGO.tag = "Square";
				
				positionMap.Add(new Square(file, rank), squareGO);
				AllSquaresGO[(file - 1) * 8 + (rank - 1)] = squareGO;
			}
		}
	}

	public void PopulateStartBoard() {
		foreach (Piece piece in GameManager.Instance.CurrentPieces)
			CreateAndPlacePieceGO(piece);
	}

	public void CastleRook(Square rookPosition) {
		GameObject rookGO = GetPieceGOAtPosition(rookPosition);

		GameObject landingSquare = null;
		switch (SquareToString(rookPosition)) {
			case "a1":
				landingSquare = GetSquareGOByPosition(new Square(4, 1));
				break;
			case "h1":
				landingSquare = GetSquareGOByPosition(new Square(6, 1));
				break;
			case "a8":
				landingSquare = GetSquareGOByPosition(new Square(4, 8));
				break;
			case "h8":
				landingSquare = GetSquareGOByPosition(new Square(6, 8));
				break;
		}

		
		rookGO.transform.parent = landingSquare.transform;
		rookGO.transform.position = landingSquare.transform.position;
	}

	public void CreateAndPlacePieceGO(Piece piece) {
		string modelName = $"{piece.PieceOwner} {piece.GetType().Name}";
		Instantiate(Resources.Load("PieceSets/Marble/" + modelName) as GameObject, positionMap[piece.Position].transform);
	}

	public void GetSquareGOsWithinRadius(List<GameObject> squareGOs, Vector3 positionWS, float radius) {
		float radiusSqr = radius * radius;
		foreach (GameObject squareGO in AllSquaresGO) {
			if ((squareGO.transform.position - positionWS).sqrMagnitude < radiusSqr)
				squareGOs.Add(squareGO);
		}
	}

	public void DisableAllPieces() {
		foreach (GameObject squareGO in AllSquaresGO) {
			PieceBehaviour pieceOnSquare = squareGO.GetComponentInChildren<PieceBehaviour>();
			if (pieceOnSquare != null) pieceOnSquare.enabled = false;
		}
	}

	public void EnableAllPieces() {
		foreach (GameObject squareGO in AllSquaresGO) {
			PieceBehaviour pieceOnSquare = squareGO.GetComponentInChildren<PieceBehaviour>(true);
			if (pieceOnSquare != null) pieceOnSquare.enabled = true;
		}
	}

	public void DestroyPieceAtPosition(Square position) {
		PieceBehaviour pieceBehaviour = positionMap[position].GetComponentInChildren<PieceBehaviour>();
		if (pieceBehaviour == null) return;
		Destroy(pieceBehaviour.gameObject);
	}
	
	private static float FileOrRankToSidePosition(int index) {
		float t = (index - 1) / 7f;
		return Mathf.Lerp(-BoardPlaneSideHalfLength, BoardPlaneSideHalfLength, t);
	}
	
	private GameObject GetPieceGOAtPosition(Square position) {
		GameObject square = GetSquareGOByPosition(position);
		return square.transform.childCount == 0 ? null : square.transform.GetChild(0).gameObject;
	}

	private GameObject GetSquareGOByPosition(Square position) => Array.Find(AllSquaresGO, go => go.name == SquareToString(position));
}