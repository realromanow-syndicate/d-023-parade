using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;
using PageHelpers.Jester.UI.App;

namespace PageHelpers.Jester.UI.Units {
	public class JesterUIUnit : JesterAppUnit {
		public override void SetupUnit (JesterComponentsRegistry componentsRegistry) {
			componentsRegistry.Instantiate<JesterUIService>();
		}
	}
}
