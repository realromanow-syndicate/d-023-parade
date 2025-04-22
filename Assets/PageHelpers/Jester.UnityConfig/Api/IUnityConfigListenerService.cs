using UniRx;

namespace PageHelpers.Jester.UnityConfig.Api {
	public interface IUnityConfigListenerService {
		ReactiveCommand<bool> isRestrictionCompleted { get; }

		void StartParseConfig ();
	}
}
