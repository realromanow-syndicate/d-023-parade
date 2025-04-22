using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;
using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace PageHelpers.Jester.DI.Setups {
	[CreateAssetMenu(menuName = "AppSetup", fileName = "AppSetup")]
	public class JesterAppSetup : ScriptableObject {
		public JesterComponentsRegistry JesterComponentsRegistry { get; private set; }

		[FormerlySerializedAs("units")]
		[SerializeField]
		private JesterAppUnit[] _units;
		
		public static JesterAppSetup liveInstance { get; private set; }
		
		public void RegisterSetup () {
			JesterComponentsRegistry = new JesterComponentsRegistry();
			liveInstance = this;
			
			foreach (var appUnit in _units) {
				appUnit.SetupUnit(JesterComponentsRegistry);
			}
		}

		public void RegisterDestroy () {
			foreach (var item in JesterComponentsRegistry.items) {
				if (item is IDisposable disposable) {
					disposable.Dispose();
				}
			}
			
			foreach (var appUnit in _units) {
				appUnit.DestroyUnit(JesterComponentsRegistry);
			}
		}
	}
}
