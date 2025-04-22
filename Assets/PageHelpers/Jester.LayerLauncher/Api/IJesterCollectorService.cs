using System.Collections.Generic;

namespace PageHelpers.Jester.LayerLauncher.Api {
	public interface IJesterCollectorService {
		Dictionary<string, object> GetJesterConfigurationParameters ();
	}
}
