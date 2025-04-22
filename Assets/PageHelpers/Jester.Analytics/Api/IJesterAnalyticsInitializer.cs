using PageHelpers.Jester.DI.App;

namespace PageHelpers.Jester.Analytics.Api {
	public interface IJesterAnalyticsInitializer {
		void StartAnalytics (JesterComponentsRegistry componentRegistry);
	}
}
