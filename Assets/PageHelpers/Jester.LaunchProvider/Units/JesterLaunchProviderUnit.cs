using Cysharp.Threading.Tasks;
using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;
using PageHelpers.Jester.LaunchProvider.UI.App;
using System.Threading;
using UnityEngine;

namespace PageHelpers.Jester.LaunchProvider.Units {
	public class JesterLaunchProviderUnit : JesterAppUnit {
		[SerializeField]
		private JesterStartupProviderUIService.Preferences _preferences;
		
		private readonly CancellationTokenSource _cancellationTokenSource = new();
		
		public override void SetupUnit (JesterComponentsRegistry componentRegistry) {
			base.SetupUnit(componentRegistry);

			componentRegistry.Instantiate<JesterStartupProviderUIService>(_preferences);
			
			componentRegistry
				.Instantiate<App.JesterLaunchProvider>()
				.LaunchJesterMetaLayer(_cancellationTokenSource.Token)
				.Forget(Debug.LogException);
		}

		public override void DestroyUnit (JesterComponentsRegistry componentsRegistry) {
			_cancellationTokenSource.Cancel();
			_cancellationTokenSource.Dispose();
			
			base.DestroyUnit(componentsRegistry);
		}
	}
}
