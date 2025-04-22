using PageHelpers.Jester.CoreLauncher.App;
using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;

namespace PageHelpers.Jester.CoreLauncher.Units {
	public class JesterCoreLauncherUnit : JesterAppUnit {
		public override void SetupUnit (JesterComponentsRegistry componentRegistry) {
			base.SetupUnit(componentRegistry);

			componentRegistry.Instantiate<JesterCoreLauncherService>();
		}
	}
}
