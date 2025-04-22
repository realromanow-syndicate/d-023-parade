using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;

namespace PageHelpers.Jester.StartupProvider.Units {
	public class JesterStartupProviderUnit : JesterAppUnit {
		public override void SetupUnit (JesterComponentsRegistry componentRegistry) {
			base.SetupUnit(componentRegistry);
			
			componentRegistry.Instantiate<App.JesterStartupProvider>();
		}
	}
}
