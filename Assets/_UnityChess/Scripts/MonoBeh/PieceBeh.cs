using System.Linq;
using UnityChess;
using UnityEngine;
using static FileConverter;

public class PieceBeh : MonoBehaviour {

	public Piece Piece;
	public MoveHistory MoveHistory;
	public PieceType PieceType;
	public Side Side;
	public GameEvent PieceMovedEvent;
	[HideInInspector] public string Type;

	private Square currentSquare;
	private Vector3 distance;
	private float posX;
	private float posY;
	
	private void Start() {
		string parentSquareName = transform.parent.name;
		
		int file = FileToInt(parentSquareName.Substring(0, 1));
		int rank = int.Parse(parentSquareName.Substring(1, 1));

		currentSquare = new Square(file, rank);

		Piece = GameManager.Instance.Game.BoardList.Last.Value.GetPiece(currentSquare);
	}

	private void OnMouseDown() {
		CalculateMousePositionOnBoard();
	}

	private void OnMouseDrag() {
		MovePieceWithMouse();
	}

	private void OnMouseUp() {
		PlacePieceBasedOnLegality();
	}

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
		//Find nearest square
		gameObject.AddComponent<SphereCollider>();
		GetComponent<SphereCollider>().isTrigger = true;
		GetComponent<SphereCollider>().radius = 9;

		Transform closestSquare = null;
		float closestDistanceSqr = Mathf.Infinity;
		Vector3 currentPosition = transform.position;

		foreach (GameObject square in GameObject.FindGameObjectsWithTag("Square").ToList().FindAll(square => GetComponent<SphereCollider>()
		                                                                                                     .bounds
		                                                                                                     .Contains(square.transform.position))) {
			Vector3 directionToTarget = square.transform.position - currentPosition;
			float dSqrToTarget = directionToTarget.sqrMagnitude;
			if (dSqrToTarget < closestDistanceSqr) {
				closestDistanceSqr = dSqrToTarget;
				closestSquare = square.transform;
			}
		}

		if (closestSquare == null) {
			transform.position = transform.parent.position;
			return;
		}
		
		Movement potentialMove = GenerateMove(closestSquare);
		if (!potentialMove.IsLegal(GameManager.Instance.Game.CurrentTurn))
		{
			transform.position = transform.parent.position;
		} else {
			transform.parent = closestSquare;
			// ReSharper disable once PossibleNullReferenceException
			transform.position = closestSquare.position;
			MoveHistory.Push(potentialMove);
			PieceMovedEvent.Raise();
		}
		
		//probably should rid the need for this
		Destroy(GetComponent<SphereCollider>());
	}
	
	private Movement GenerateMove(Transform toSquareTransform) {
		string toSquareString = toSquareTransform.name;
		int file = FileToInt(toSquareString.Substring(0, 1));
		int rank = int.Parse(toSquareString.Substring(1, 1));
		
		Square toSquare = new Square(file, rank);
		
		return new Movement(toSquare, Piece);
	}
}