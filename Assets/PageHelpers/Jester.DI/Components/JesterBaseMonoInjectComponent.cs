using PageHelpers.Jester.DI.Setups;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace PageHelpers.Jester.DI.Components {
	public abstract class JesterBaseMonoInjectComponent : MonoBehaviour {
		private void Start () {
			Inject(JesterAppSetup.liveInstance.JesterComponentsRegistry.items);
		}

		private void Inject (params object[] args) {
			var mainType = this.GetType();
			var properties = mainType.GetFields(BindingFlags.Instance | BindingFlags.NonPublic);

			foreach (var property in properties) {
				var targetType = property.FieldType;
				var targetArg = args.FirstOrDefault(x => targetType.IsInstanceOfType(x));
				
				if (targetArg != null) property.SetValue(this, targetArg);
			}
		}
	}
}
