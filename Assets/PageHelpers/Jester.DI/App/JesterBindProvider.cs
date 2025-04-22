using PageHelpers.Jester.DI.Binding;
using System;
using System.Linq;

namespace PageHelpers.Jester.DI.App {
	public class JesterBindProvider {
		public static void BindObjectById<T> (T obj, string bindId) {
			var targetBindings = UnityEngine.Object.FindObjectsOfType<JesterItemBinding<T>>();
			var target = targetBindings.SingleOrDefault(bind => bind.bindId == bindId);

			if (target == null) throw new Exception($"Object {bindId} not found");
			
			target.SetItem(obj, bindId);
		}
	}
}
