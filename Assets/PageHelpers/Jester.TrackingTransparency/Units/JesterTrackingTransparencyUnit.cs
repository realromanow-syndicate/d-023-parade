using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;
using PageHelpers.Jester.TrackingTransparency.App;
using PageHelpers.Jester.TrackingTransparency.UI.App;
using UnityEngine;

namespace PageHelpers.Jester.TrackingTransparency.Units {
	public class JesterTrackingTransparencyUnit : JesterAppUnit {
		[SerializeField]
		private TrackingTransparencyUIService.Preferences _uiPreferences;
		
		public override void SetupUnit (JesterComponentsRegistry componentRegistry) {
			base.SetupUnit(componentRegistry);

			componentRegistry.Instantiate<TrackingTransparencyUIService>(_uiPreferences);
			componentRegistry.Instantiate<JesterTrackingTransparencyService>();
		}
	}
}
