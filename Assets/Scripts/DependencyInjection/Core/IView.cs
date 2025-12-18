namespace UnityChess.DependencyInjection {
	public interface IView<in TVm> where TVm : class, IViewModel {
		public void Initialize(TVm vm);
	}
}