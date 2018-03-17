using System;
using UnityChess;
using UnityEngine;

[Serializable]
public class PieceManager : MonoBehaviour {

	private Vector3 distance;
	private float posX;
	private float posY;

	public Piece Piece;
	public string Type;

	private void OnMouseDown() {
		GameObject.FindWithTag("GameManager").GetComponent<GameManager>().GrabPiece(gameObject);
	}

	private void OnMouseUp() {
		GameObject.FindWithTag("GameManager").GetComponent<GameManager>().DropPieceInHand();
	}
}