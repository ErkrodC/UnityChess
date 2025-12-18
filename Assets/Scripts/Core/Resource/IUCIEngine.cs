using System.Threading.Tasks;

namespace UnityChess.Core.Resource {
	public interface IUCIEngine {
		void StartAsync();

		void ShutDown();

		Task SetupNewGame(Game game);

		Task<Movement> GetBestMove(int timeoutMS);
	}
}