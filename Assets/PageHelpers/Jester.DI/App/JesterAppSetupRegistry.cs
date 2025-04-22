using PageHelpers.Jester.DI.Setups;
using UnityEngine;

namespace PageHelpers.Jester.DI.App {
	public class JesterAppSetupRegistry : MonoBehaviour {
		[SerializeField]
		private JesterAppSetup _setup;

		private void Awake () {
			_setup.RegisterSetup();
		}

		private void OnDestroy () {
			_setup.RegisterDestroy();
		}
	}
}
