using Cysharp.Threading.Tasks;
using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;
using PageHelpers.Jester.UnityConfig.App;
using UnityEngine;

namespace PageHelpers.Jester.UnityConfig.Units {
	[CreateAssetMenu(menuName = "Units/UnityConfig", fileName = "UnityConfig")]
	public class UnityConfigUnit : JesterAppUnit {
		[SerializeField]
		private UnityConfigListenerService.Preferences _preferences;
		
		public override void SetupUnit (JesterComponentsRegistry componentsRegistry) {
			base.SetupUnit(componentsRegistry);
			
			componentsRegistry
				.Instantiate<UnityConfigInitializerService>()
				.Launch()
				.Forget(Debug.LogException);

			componentsRegistry.Instantiate<UnityConfigListenerService>(_preferences);
		}
	}
}
