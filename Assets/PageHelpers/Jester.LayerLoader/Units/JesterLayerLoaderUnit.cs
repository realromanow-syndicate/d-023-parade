using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;
using PageHelpers.Jester.LayerLoader.UI.App;
using UnityEngine;

namespace PageHelpers.Jester.LayerLoader.Units {
	public class JesterLayerLoaderUnit : JesterAppUnit {
		[SerializeField]
		private JesterLayerLoaderUIScreenService.Preferences uiPreferences;

		public override void SetupUnit (JesterComponentsRegistry componentRegistry) {
			base.SetupUnit(componentRegistry);

			componentRegistry.Instantiate<JesterLayerLoaderUIScreenService>(uiPreferences);
		}
	}
}
