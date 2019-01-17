using System.Linq;
using UnityChess;
using UnityEngine;
using static UnityChess.SquareUtil;

public class PieceBeh : MonoBehaviour {
	public Piece Piece => GameManager.Instance.CurrentBoard.GetPiece(currentSquare);
	public Side Side;
	public GameEvent PieceMovedEvent;

	private Square currentSquare => StringToSquare(transform.parent.name);
	private Vector3 distance;
	private float posX;
	private float posY;
	private SphereCollider pieceBoundingSphere;

	private void OnMouseDown() => CalculateMousePositionOnBoard();

	private void OnMouseDrag() => MovePieceWithMouse();

	private void OnMouseUp() => PlacePieceBasedOnLegality();

	private void CalculateMousePositionOnBoard() {
		distance = Camera.main.WorldToScreenPoint(transform.position);
		posX = Input.mousePosition.x - distance.x;
		posY = Input.mousePosition.y - distance.y;
	}
	
	private void MovePieceWithMouse() {
		Vector3 curPos = new Vector3(Input.mousePosition.x - posX, Input.mousePosition.y - posY, distance.z);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(curPos);

		transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
	}

	private void PlacePieceBasedOnLegality() {
		// find closest square
		GameObject[] collidingSquares = BoardManager.Instance.AllSquares.Where(square => (square.transform.position - transform.position).magnitude < 9f).ToArray();
		Transform closestSquareTransform = null;
		float closestDistanceSqr = float.PositiveInfinity;
		foreach (GameObject collidingSquare in collidingSquares) {
			float dSqrToTarget = (collidingSquare.transform.position - transform.position).sqrMagnitude;
			if (dSqrToTarget < closestDistanceSqr) {
				closestDistanceSqr = dSqrToTarget;
				closestSquareTransform = collidingSquare.transform;
			}
		}

		// return if no square found
		if (closestSquareTransform == null) {
			transform.position = transform.parent.position;
			return;
		}

		// try potential move using closest square
		Movement potentialMove = new Movement(Piece, StringToSquare(closestSquareTransform.name));
		if (potentialMove.IsLegal(GameManager.Instance.Game.CurrentTurn, Piece)) {
			transform.parent = closestSquareTransform;
			transform.position = closestSquareTransform.position;
			GameManager.Instance.MoveHistory.Push(potentialMove);
			PieceMovedEvent.Raise();
		} else {
			transform.position = transform.parent.position;
#if DEBUG_VIEW
			UnityChessDebug.ShowLegalMovesInLog(Piece);
#endif
		}
	}
}