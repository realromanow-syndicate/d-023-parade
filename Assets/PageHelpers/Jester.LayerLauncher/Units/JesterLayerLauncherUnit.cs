using Cysharp.Threading.Tasks;
using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;
using PageHelpers.Jester.LayerLauncher.App;
using System.Threading;
using UnityEngine;

namespace PageHelpers.Jester.LayerLauncher.Units {
	public class JesterLayerLauncherUnit : JesterAppUnit {
		[SerializeField]
		private JesterApiService.Preferences apiPreferences;
		
		[SerializeField]
		private JesterCollectorService.Preferences paramsPreferences;

		private readonly CancellationTokenSource _cancellationTokenSource = new();

		public override void SetupUnit (JesterComponentsRegistry componentRegistry) {
			base.SetupUnit(componentRegistry);

			componentRegistry.Instantiate<JesterCollectorService>(paramsPreferences);
			componentRegistry.Instantiate<JesterApiService>(apiPreferences);

			componentRegistry
				.Instantiate<JesterLauncherService>()
				.LaunchJesterMetaAsync(_cancellationTokenSource.Token)
				.Forget(Debug.LogException);
		}
	}
}
