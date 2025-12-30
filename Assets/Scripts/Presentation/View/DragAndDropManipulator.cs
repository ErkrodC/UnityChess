using System.Collections.Generic;
using UnityChess.Presentation.ViewModel;
using UnityEngine;
using UnityEngine.UIElements;

namespace UnityChess.Presentation.View {
	public class DragAndDropManipulator : PointerManipulator {
		private readonly VisualElement _root;
		private Vector2 _pointerStartDS;
		private Vector2 _pointerOffset;
		private bool _isDragging;
		private readonly VisualElement _dragLayer;
		private readonly Label _dragLabel;
		private readonly BoardVM _vm;

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
			_dragLabel.text = (target as Label)?.text;

			target.style.visibility = Visibility.Hidden;
			_dragLabel.style.visibility = Visibility.Visible;

			_pointerStartDS = WSToDS(evt.position);
			Vector2 labelOriginDS = WSToDS(LSToWS(target));
			_pointerOffset = _pointerStartDS - labelOriginDS;

			_dragLabel.style.left = Mathf.Clamp(_pointerStartDS.x - _pointerOffset.x, 0, _dragLayer.layout.width);
			_dragLabel.style.top = Mathf.Clamp(_pointerStartDS.y - _pointerOffset.y, 0, _dragLayer.layout.height);

			target.CapturePointer(evt.pointerId);
			_isDragging = true;
		}

		private void PointerMoveHandler(PointerMoveEvent evt) {
			if (!_isDragging || !target.HasPointerCapture(evt.pointerId)) { return; }

			Vector2 pointerDS = WSToDS(evt.position);
			Vector2 pointerDelta = pointerDS - _pointerStartDS;

			_dragLabel.style.left = Mathf.Clamp(
				_pointerStartDS.x + pointerDelta.x - _pointerOffset.x,
				0,
				_dragLayer.layout.width
			);
			_dragLabel.style.top = Mathf.Clamp(
				_pointerStartDS.y + pointerDelta.y - _pointerOffset.y,
				0,
				_dragLayer.layout.height
			);
		}

		private void PointerUpHandler(PointerUpEvent evt) {
			if (!_isDragging || !target.HasPointerCapture(evt.pointerId)) { return; }

			target.ReleasePointer(evt.pointerId);
		}

		private async void PointerCaptureOutHandler(PointerCaptureOutEvent evt) {
			if (!_isDragging) { return; }

			string fromSquare = target.parent.name;
			VisualElement closestSquare = FindClosestSquare();

			bool wasMoveValid = !string.IsNullOrEmpty(fromSquare)
			                    && closestSquare != null
			                    && await _vm.onPieceDropped(fromSquare, closestSquare.name);

			_dragLabel.style.visibility = Visibility.Hidden;
			target.style.visibility = Visibility.Visible;

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
				Vector2 dragLabelToSquare = LSToWS(overlappingSquare) - LSToWS(_dragLabel);
				float sqrDist = dragLabelToSquare.sqrMagnitude;
				if (sqrDist < minSqrDist) {
					minSqrDist = sqrDist;
					result = overlappingSquare;
				}
			}

			return result;
		}

		// ER NOTE
		// LS: Local Space, relative to the parent
		// DS: Drag Space,  relative to the drag layer
		// WS: World Space, relative to the root
		private Vector2 LSToWS(VisualElement element) {
			return element.worldBound.position;
		}

		private Vector2 WSToDS(Vector2 position) {
			return _dragLayer.WorldToLocal(position);
		}
	}
}