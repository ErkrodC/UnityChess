using System.Linq;
using UnityEngine;

public class HoldingPiece : StateMachineBehaviour {
	private Vector3 distance;

	private GameManager gameManager;
	private float posX, posY;

	public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		gameManager = GameObject.FindWithTag("GameManager").GetComponent<GameManager>();

		distance = Camera.main.WorldToScreenPoint(gameManager.PieceInHand.transform.position);
		posX = Input.mousePosition.x - distance.x;
		posY = Input.mousePosition.y - distance.y;
	}

	public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		Vector3 curPos = new Vector3(Input.mousePosition.x - posX, Input.mousePosition.y - posY, distance.z);
		Vector3 worldPos = Camera.main.ScreenToWorldPoint(curPos);

		gameManager.PieceInHand.transform.position = new Vector3(worldPos.x, worldPos.y, gameManager.PieceInHand.transform.position.z);
	}

	public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		//Find nearest square and set piece's parent to it. also reset position to 0,0,0 relative to square.
		gameManager.PieceInHand.AddComponent<SphereCollider>();
		gameManager.PieceInHand.GetComponent<SphereCollider>().isTrigger = true;
		gameManager.PieceInHand.GetComponent<SphereCollider>().radius = 9;

		Transform closestSquare = null;
		float closestDistanceSqr = Mathf.Infinity;
		Vector3 currentPosition = gameManager.PieceInHand.transform.position;

		foreach (GameObject square in GameObject.FindGameObjectsWithTag("Square").ToList().FindAll(square => gameManager.PieceInHand.GetComponent<SphereCollider>().bounds.Contains(square.transform.position))) {
			Vector3 directionToTarget = square.transform.position - currentPosition;
			float dSqrToTarget = directionToTarget.sqrMagnitude;
			if (dSqrToTarget < closestDistanceSqr) {
				closestDistanceSqr = dSqrToTarget;
				closestSquare = square.transform;
			}
		}

		gameManager.PieceInHand.transform.parent = closestSquare;
		// ReSharper disable once PossibleNullReferenceException
		gameManager.PieceInHand.transform.position = closestSquare.position;
		Destroy(gameManager.PieceInHand.GetComponent<SphereCollider>());
		gameManager.PieceInHand = null;
	}
}