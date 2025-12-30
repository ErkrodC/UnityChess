using System.Threading.Tasks;
using UnityChess.Core;
using UnityChess.Core.Util;

namespace UnityChess.Application.Service {
	public interface IPlayerService {
		Task<(Square start, Square end)> GetMoveAsync(string fen);
		void ReportMoveValidity(bool isValid);
		Task<ElectedPiece> ElectPieceAsync(Side side);
	}
}