using AppsFlyerSDK;
using PageHelpers.Jester.Analytics.Api;
using PageHelpers.Jester.DI.App;
using System;
using UnityEngine;

namespace PageHelpers.Jester.Analytics.App {
	public class JesterAnalyticsInitializer : IJesterAnalyticsInitializer {
		private readonly Preferences _preferences;

		public JesterAnalyticsInitializer (Preferences preferences) {
			_preferences = preferences;
		}

		public void StartAnalytics (JesterComponentsRegistry componentRegistry) {
			var listener = CreateAndRegisterListener(componentRegistry);

			StartupAnalyticsSdk(listener);
		}

		private void StartupAnalyticsSdk (JesterAnalyticsListener analyticsListener) {
			AppsFlyer.initSDK(_preferences.analyticsDevKey, _preferences.analyticsStoreId, analyticsListener);
			AppsFlyer.startSDK();
		}

		private JesterAnalyticsListener CreateAndRegisterListener (JesterComponentsRegistry componentRegistry) {
			var listener = UnityEngine.Object
				.Instantiate(_preferences.analyticsListenerPrefabReference);

			componentRegistry.Registry(listener);
			return listener;
		}

		[Serializable]
		public class Preferences {
			[SerializeField]
			private string _analyticsDevKey;

			[SerializeField]
			private string _analyticsStoreId;

			[SerializeField]
			private JesterAnalyticsListener _analyticsListenerPrefabReference;

			public string analyticsDevKey => _analyticsDevKey;
			public string analyticsStoreId => _analyticsStoreId;
			public JesterAnalyticsListener analyticsListenerPrefabReference => _analyticsListenerPrefabReference;
		}
	}
}
