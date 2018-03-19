using System.Linq;
using UnityChess;
using UnityEngine;

public class PieceManager : MonoBehaviour {

	private Vector3 distance;
	private float posX;
	private float posY;

	public Piece Piece;
	public string Type;

	private void OnMouseDown() {
	
		distance = Camera.main.WorldToScreenPoint(transform.position);
		posX = Input.mousePosition.x - distance.x;
		posY = Input.mousePosition.y - distance.y;
	}

	private void OnMouseDrag() {
		
		Vector3 curPos = new Vector3(Input.mousePosition.x - posX, Input.mousePosition.y - posY, distance.z);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(curPos);

		transform.position = new Vector3(worldPos.x, worldPos.y, transform.position.z);
	}

	private void OnMouseUp() {

		//Find nearest square and set piece's parent to it. also reset position to 0,0,0 relative to square.
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

		transform.parent = closestSquare;
		// ReSharper disable once PossibleNullReferenceException
		transform.position = closestSquare.position;
		Destroy(GetComponent<SphereCollider>());
	}
}