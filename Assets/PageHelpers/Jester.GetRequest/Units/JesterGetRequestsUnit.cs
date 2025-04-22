using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;
using PageHelpers.Jester.GetRequest.App;

namespace PageHelpers.Jester.GetRequest.Units {
	public class JesterGetRequestsUnit : JesterAppUnit {
		public override void SetupUnit (JesterComponentsRegistry componentRegistry) {
			base.SetupUnit(componentRegistry);

			componentRegistry.Instantiate<JesterGetRequestsService>();
		}
	}
}
