using PageHelpers.Jester.Analytics.App;
using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;
using UnityEngine;
using UnityEngine.Serialization;

namespace PageHelpers.Jester.Analytics.Units {
	public class JesterAnalyticsUnit : JesterAppUnit {
		[FormerlySerializedAs("preferences")]
		[SerializeField]
		private JesterAnalyticsInitializer.Preferences _preferences;

		public override void SetupUnit (JesterComponentsRegistry componentRegistry) {
			base.SetupUnit(componentRegistry);

			componentRegistry
				.Instantiate<JesterAnalyticsInitializer>(_preferences)
				.StartAnalytics(componentRegistry);
		}
	}
}
