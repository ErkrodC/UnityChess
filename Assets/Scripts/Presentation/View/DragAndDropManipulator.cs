using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityChess.Presentation.ViewModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class DragAndDropManipulator : PointerManipulator {
		private readonly VisualElement _root;
		private Vector3 _pointerStartWS;
		private Vector3 _pointerStartDS;
		private bool _isDragging;
		private readonly VisualElement _dragLayer;
		private readonly Label _dragLabel;
		private readonly BoardVM _vm;
		private Func<string, string, Task<bool>> DropHandler => _vm.onPieceDropped;

		public DragAndDropManipulator(VisualElement target, VisualElement root, BoardVM vm) {
			this.target = target;
			_root = root;
			_dragLayer = root.Q<VisualElement>("drag-layer");
			_dragLabel = _dragLayer.Q<Label>();
			_vm = vm;
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
			_pointerStartWS = evt.position;
			_dragLabel.text = (target as Label)?.text;

			target.style.visibility = Visibility.Hidden;
			_dragLabel.style.visibility = Visibility.Visible;

			_pointerStartDS = GetPosInDragLayerSpace(_pointerStartWS);
			_dragLabel.style.left = _pointerStartDS.x;
			_dragLabel.style.top = _pointerStartDS.y;

			target.CapturePointer(evt.pointerId);
			_isDragging = true;
		}

		private void PointerMoveHandler(PointerMoveEvent evt) {
			if (!_isDragging || !target.HasPointerCapture(evt.pointerId)) { return; }

			Vector3 pointerDelta = evt.position - _pointerStartWS;

			_dragLabel.style.left = _pointerStartDS.x + pointerDelta.x;
			_dragLabel.style.top = _pointerStartDS.y + pointerDelta.y;
		}

		private void PointerUpHandler(PointerUpEvent evt) {
			if (!_isDragging || !target.HasPointerCapture(evt.pointerId)) { return; }

			target.ReleasePointer(evt.pointerId);
		}

		private void PointerCaptureOutHandler(PointerCaptureOutEvent evt) {
			if (!_isDragging) { return; }

			_dragLabel.style.visibility = Visibility.Hidden;
			target.style.visibility = Visibility.Visible;

			VisualElement closestSquare = FindClosestSquare();
			// ER TODO could read the async return value to update immediately, but might not be necessary?
			if (closestSquare != null) { DropHandler?.Invoke(target.parent.name, closestSquare.name); }

			_isDragging = false;
		}

		private VisualElement FindClosestSquare() {
			VisualElement board = _root.Q<VisualElement>("board");
			List<VisualElement> overlappingSquares = board
				.Query<VisualElement>(className: "board-square")
				.Where((square) => _dragLabel.worldBound.Overlaps(square.worldBound))
				.ToList();

			float minSqrDist = float.MaxValue;
			VisualElement result = null;
			foreach (VisualElement overlappingSquare in overlappingSquares) {
				Vector2 dragLabelToSquare = GetPosInWorldSpace(overlappingSquare) - GetPosInWorldSpace(_dragLabel);
				float sqrDist = dragLabelToSquare.sqrMagnitude;
				if (sqrDist < minSqrDist) {
					minSqrDist = sqrDist;
					result = overlappingSquare;
				}
			}

			return result;
		}

		private Vector2 GetPosInWorldSpace(VisualElement element) {
			return element.parent.LocalToWorld(element.layout.position);
		}

		private Vector2 GetPosInDragLayerSpace(Vector2 position) {
			return _dragLayer.WorldToLocal(position);
		}
	}
}