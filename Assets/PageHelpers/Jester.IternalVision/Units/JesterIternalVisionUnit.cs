using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;
using PageHelpers.Jester.IternalVision.App;

namespace PageHelpers.Jester.IternalVision.Units {
	public class JesterIternalVisionUnit : JesterAppUnit {
		public override void SetupUnit (JesterComponentsRegistry componentRegistry) {
			base.SetupUnit(componentRegistry);

			componentRegistry
				.Instantiate<JesterIternalVisionInitializerService>()
				.SetupJesterOverVision(componentRegistry);

			componentRegistry
				.Instantiate<JesterIternalVisionService>();
		}
	}
}
