using System.Collections.Generic;
using UniRx;

namespace PageHelpers.Jester.Analytics.Api {
	public interface IJesterAnalyticsListener {
		ReactiveCommand<Dictionary<string, object>> dataSuccessReceived { get; }
		ReactiveCommand dataFailReceived { get; }
	}
}
