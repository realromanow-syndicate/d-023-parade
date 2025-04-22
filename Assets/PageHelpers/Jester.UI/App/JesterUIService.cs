using PageHelpers.Jester.DI.Binding;
using PageHelpers.Jester.UI.Api;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PageHelpers.Jester.UI.App {
	public class JesterUIService : IJesterUIService {
		private readonly Dictionary<Canvas, List<GameObject>> _interfaceVariants = new();

		public void SetupJesterForm<TArg> (GameObject form, Canvas canvas, TArg item = default) {
			if (!_interfaceVariants.ContainsKey(canvas)) _interfaceVariants.Add(canvas, new List<GameObject>());

			if (_interfaceVariants[canvas].Contains(form)) {
				Debug.LogError($"Instance of {form.name} already exists on {canvas.name}");
				return;
			}

			var instance = Object.Instantiate(form, canvas.transform);
			_interfaceVariants[canvas].Add(instance);

			if (item != null) instance.GetComponentInChildren<JesterItemBinding<TArg>>().SetItem(item, form.name + "(Clone)");
		}

		public void RemoveJesterForm (GameObject form, Canvas canvas) {
			if (!TryGetInterfaceList(canvas, out var variantsList)) return;

			if (variantsList.All(x => x.name != form.name + "(Clone)")) {
				Debug.Log($"Instance of {form.name} not found on {canvas.name}");
				return;
			}

			var instance = _interfaceVariants[canvas].Single(x => x.name == form.name + "(Clone)");
			_interfaceVariants[canvas].Remove(instance);

			Object.Destroy(instance);
		}

		private bool TryGetInterfaceList (Canvas canvas, out List<GameObject> variantsList) {
			return _interfaceVariants.TryGetValue(canvas, out variantsList);
		}
	}
}
