using System.Collections.Generic;
using UniRx;

namespace PageHelpers.Jester.UnityConfig.Api {
	public interface IUnityConfigInitializerService {
		IReadOnlyReactiveProperty<IDictionary<string, object>> cachedRawData { get; }
		IReadOnlyReactiveProperty<bool> isCacheInit { get; }
	}
}
