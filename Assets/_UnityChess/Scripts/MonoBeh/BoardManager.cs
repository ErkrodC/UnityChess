﻿using System.Collections.Generic;
using System.Linq;
using UnityChess;
using UnityEngine;
using static FileConverter;

public class BoardManager : MonoBehaviour {

	private static Dictionary<Square, GameObject> positionMap;

	private void Start() {
		positionMap = new Dictionary<Square, GameObject>(64);

		for (int file = 1; file <= 8; file++) {
			for (int rank = 1; rank <= 8; rank++) {
				GameObject go = new GameObject(IntToFile(file) + rank, typeof(BoxCollider2D));
				go.GetComponent<BoxCollider2D>().isTrigger = false;
				go.GetComponent<BoxCollider2D>().size = new Vector2(9, 9);
				go.transform.position = new Vector3(transform.position.x - 31.5f + (file - 1) * 9, transform.position.y - 31.5f + (rank - 1) * 9, transform.position.z - 7.2f);
				go.transform.parent = transform;
				go.tag = "Square";
				positionMap.Add(new Square(file, rank), go);
			}
		}
	}

	public void PopulateStartBoard() {
		foreach (Piece piece in GameManager.Instance.Game.BoardList.Last.Value.BasePieceList.OfType<Piece>()) {
			string modelName = "";
			modelName += piece.Side + " ";
			modelName += piece.GetType().Name;

			GameObject pieceObject = Instantiate(Resources.Load("PieceSets/Marble/" + modelName) as GameObject, positionMap[piece.Position].transform);
			pieceObject.GetComponent<PieceBeh>().Piece = piece;
			pieceObject.GetComponent<PieceBeh>().Type = modelName;
		}
	}
}