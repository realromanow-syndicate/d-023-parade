using UniRx;

namespace PageHelpers.Jester.IternalVision.Api {
	public interface IJesterIternalVisionService {
		IReadOnlyReactiveProperty<int> visionGeneration { get; }

		ReactiveCommand jesterVisionLoaded { get; }

		void LoadJesterVision (string url);

		void LaunchJesterVision ();
	}
}
