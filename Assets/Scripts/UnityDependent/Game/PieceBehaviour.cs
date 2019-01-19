using System.Collections.Generic;
using UnityChess;
using UnityEngine;
using static UnityChess.SquareUtil;

public class PieceBehaviour : MonoBehaviour {
	public GameEvent PieceMovedEvent;

	private const float SquareCollisionRadius = 9f;
	private Square CurrentSquare => StringToSquare(transform.parent.name);
	private Vector3 distance;
	private float posX;
	private float posY;
	private SphereCollider pieceBoundingSphere;
	private List<GameObject> potentialLandingSquares;
	private Transform thisTransform;

	private void Start() {
		potentialLandingSquares = new List<GameObject>();
		thisTransform = transform;
	}

	private void OnMouseDown() {
		distance = Camera.main.WorldToScreenPoint(transform.position);
		posX = Input.mousePosition.x - distance.x;
		posY = Input.mousePosition.y - distance.y;
	}

	private void OnMouseDrag() {
		Vector3 curPos = new Vector3(Input.mousePosition.x - posX, Input.mousePosition.y - posY, distance.z);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(curPos);

		thisTransform.position = new Vector3(worldPos.x, worldPos.y, thisTransform.position.z);
	}

	private void OnMouseUp() {
		potentialLandingSquares.Clear();
		BoardManager.Instance.GetSquareGOsWithinRadius(potentialLandingSquares, thisTransform.position, SquareCollisionRadius);

		if (potentialLandingSquares.Count == 0) { // piece moved off board
			thisTransform.position = thisTransform.parent.position;
			return;
		}
	
		// determine closest square out of potential landing squares.
		Transform closestSquareTransform = potentialLandingSquares[0].transform;
		float shortestDistanceFromPieceSquared = (closestSquareTransform.transform.position - thisTransform.position).sqrMagnitude;
		for (int i = 1; i < potentialLandingSquares.Count; i++) {
			GameObject potentialLandingSquare = potentialLandingSquares[i];
			float distanceFromPieceSquared = (potentialLandingSquare.transform.position - thisTransform.position).sqrMagnitude;

			if (distanceFromPieceSquared < shortestDistanceFromPieceSquared) {
				shortestDistanceFromPieceSquared = distanceFromPieceSquared;
				closestSquareTransform = potentialLandingSquare.transform;
			}
		}

		TryExecuteMove(closestSquareTransform);
	}

	private void TryExecuteMove(Transform closestSquareTransform) {
		Square closestSquare = StringToSquare(closestSquareTransform.name);
		
		Movement baseMove = new Movement(CurrentSquare, closestSquare);
		if (GameManager.Instance.Game.MoveIsLegal(baseMove, out Movement foundValidMove)) {
			BoardManager.Instance.DestroyPieceAtPosition(closestSquare);
			thisTransform.parent = closestSquareTransform;
			thisTransform.position = closestSquareTransform.position;
			GameManager.Instance.MoveQueue.Enqueue(foundValidMove);
			PieceMovedEvent.Raise();
		} else {
			thisTransform.position = thisTransform.parent.position;
#if DEBUG_VIEW
			UnityChessDebug.ShowLegalMovesInLog(GameManager.Instance.CurrentBoard.GetPiece(CurrentSquare));
#endif
		}
	}
}