using System.Threading.Tasks;

namespace UnityChess.Engine {
	public interface IUCIEngine {
		void Start();
		
		void ShutDown();
		
		Task SetupNewGame(Game game);
		
		Task<Movement> GetBestMove(int timeoutMS);
	}
}