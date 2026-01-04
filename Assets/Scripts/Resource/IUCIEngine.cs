using System.Threading.Tasks;
using UnityChess.Core;

namespace UnityChess.Resource {
	public interface IUCIEngine {
		ElectedPiece promotionElection { get; }
		Task StartNewGameAsync();
		Task<(Square start, Square end)> GetBestMove(string fen, int timeoutMS = -1);
	}
}