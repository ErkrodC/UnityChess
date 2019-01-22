using System.Collections.Generic;
using UnityChess;
using UnityEngine;
using static UnityChess.SquareUtil;

public class PieceBehaviour : MonoBehaviour {
	public Side PieceColor;
	public Square CurrentSquare => StringToSquare(transform.parent.name);
	
	private const float SquareCollisionRadius = 9f;
	
	[SerializeField] private GameEvent PieceMovedEvent = null;
	private Camera boardCamera;
	private Vector3 piecePositionSS;
	private Vector2 mouseToPieceSS;
	private SphereCollider pieceBoundingSphere;
	private List<GameObject> potentialLandingSquares;
	private Transform thisTransform;

	private void Start() {
		potentialLandingSquares = new List<GameObject>();
		thisTransform = transform;
		boardCamera = Camera.main;
	}

	public void OnMouseDown() {
		if (enabled) {
			piecePositionSS = boardCamera.WorldToScreenPoint(transform.position);
			mouseToPieceSS = new Vector2(Input.mousePosition.x - piecePositionSS.x, Input.mousePosition.y - piecePositionSS.y);
		}
	}

	private void OnMouseDrag() {
		if (enabled) {
			Vector3 nextPiecePositionSS = new Vector3(Input.mousePosition.x - mouseToPieceSS.x, Input.mousePosition.y - mouseToPieceSS.y, piecePositionSS.z);
			Vector3 nextPiecePositionWS = boardCamera.ScreenToWorldPoint(nextPiecePositionSS);

			thisTransform.position = new Vector3(nextPiecePositionWS.x, thisTransform.position.y, nextPiecePositionWS.z);
		}
	}

	public void OnMouseUp() {
		if (enabled) {
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
	}

	private void TryExecuteMove(Transform closestSquareTransform) {
		Square closestSquare = StringToSquare(closestSquareTransform.name);
		
		Movement baseMove = new Movement(CurrentSquare, closestSquare);
		if (GameManager.Instance.MoveIsLegal(baseMove, out Movement foundValidMove)) {
			if (GameManager.Instance.CurrentBoard[closestSquare] != null) BoardManager.Instance.DestroyPieceAtPosition(closestSquare);
			thisTransform.parent = closestSquareTransform;
			thisTransform.position = closestSquareTransform.position;
			GameManager.Instance.MoveQueue.Enqueue(foundValidMove);
			PieceMovedEvent.Raise();
		} else {
			thisTransform.position = thisTransform.parent.position;
#if DEBUG_VIEW
			UnityChessDebug.ShowLegalMovesInLog(GameManager.Instance.CurrentBoard[CurrentSquare]);
#endif
		}
	}
}