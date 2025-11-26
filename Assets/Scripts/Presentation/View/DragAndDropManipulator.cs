using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class DragAndDropManipulator : PointerManipulator {
		private Vector2 _targetStartPosition;
		private Vector3 _pointerStartPosition;
		private bool _isDragging;
		private VisualElement _root;

		public DragAndDropManipulator(VisualElement target, VisualElement root) {
			this.target = target;
			_root = root;
		}

		protected override void RegisterCallbacksOnTarget() {
			target.RegisterCallback<PointerDownEvent>(PointerDownHandler);
			target.RegisterCallback<PointerMoveEvent>(PointerMoveHandler);
			target.RegisterCallback<PointerUpEvent>(PointerUpHandler);
			target.RegisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
		}

		protected override void UnregisterCallbacksFromTarget() {
			target.UnregisterCallback<PointerDownEvent>(PointerDownHandler);
			target.UnregisterCallback<PointerMoveEvent>(PointerMoveHandler);
			target.UnregisterCallback<PointerUpEvent>(PointerUpHandler);
			target.UnregisterCallback<PointerCaptureOutEvent>(PointerCaptureOutHandler);
		}

		private void PointerDownHandler(PointerDownEvent evt) {
			_targetStartPosition = target.transform.position;
			_pointerStartPosition = evt.position;
			target.CapturePointer(evt.pointerId);
			_isDragging = true;
		}

		private void PointerMoveHandler(PointerMoveEvent evt) {
			if (!_isDragging || !target.HasPointerCapture(evt.pointerId)) { return; }

			Vector3 pointerDelta = evt.position - _pointerStartPosition;

			Rect worldBound = target.panel.visualTree.worldBound;
			target.transform.position = new Vector2(
				Mathf.Clamp(_targetStartPosition.x + pointerDelta.x, 0, worldBound.width),
				Mathf.Clamp(_targetStartPosition.y + pointerDelta.y, 0, worldBound.height)
			);
		}

		private void PointerUpHandler(PointerUpEvent evt) {
			if (!_isDragging || !target.HasPointerCapture(evt.pointerId)) { return; }

			target.ReleasePointer(evt.pointerId);
		}

		private void PointerCaptureOutHandler(PointerCaptureOutEvent evt) {
			if (!_isDragging) { return; }

			VisualElement closestSquare = FindClosestSquare();
			Vector3 closestPos = Vector3.zero;
			if (closestSquare != null) {
				closestPos = GetPosInRootSpace(closestSquare);
				closestPos = new Vector2(closestPos.x - 5, closestPos.y - 5); // ER TODO ??? what's "- 5"
			}

			target.transform.position = closestSquare != null
				? closestPos
				: _targetStartPosition;

			_isDragging = false;
		}

		private VisualElement FindClosestSquare() {
			VisualElement board = _root.Q<VisualElement>("board");
			UQueryBuilder<VisualElement> allSquares = board.Query<VisualElement>(className: "board-square");
			UQueryBuilder<VisualElement> overlappingSquares = allSquares
				.Where((square) => target.worldBound.Overlaps(square.worldBound));
			List<VisualElement> squaresList = overlappingSquares.ToList();

			float minSqrDist = float.MaxValue;
			VisualElement result = null;
			foreach (VisualElement square in squaresList) {
				Vector3 targetToSquare = GetPosInRootSpace(square) - target.transform.position;
				float sqrDist = targetToSquare.sqrMagnitude;
				if (sqrDist < minSqrDist) {
					minSqrDist = sqrDist;
					result = square;
				}
			}

			return result;
		}

		private Vector3 GetPosInRootSpace(VisualElement square) {
			return _root.WorldToLocal(square.parent.LocalToWorld(square.layout.position));
		}
	}
}