using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;

namespace PageHelpers.Jester.Storage.Units {
	public class JesterStorageUnit : JesterAppUnit {
		public override void SetupUnit (JesterComponentsRegistry componentRegistry) {
			base.SetupUnit(componentRegistry);

			componentRegistry
				.Instantiate<App.JesterAppStorage>()
				.LoadAppData();
		}
	}
}
