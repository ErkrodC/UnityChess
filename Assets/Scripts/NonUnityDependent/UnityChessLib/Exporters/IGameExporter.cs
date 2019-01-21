using System.Collections.Generic;

namespace UnityChess {
	public interface IGameExporter {
		string Export(Board currentBoard, List<HalfMove> turns, GameConditions endingConditions);
	}
}