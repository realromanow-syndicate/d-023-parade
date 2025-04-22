using PageHelpers.Jester.DI.App;
using UnityEngine;

namespace PageHelpers.Jester.DI.Units {
	
	public class JesterAppUnit : ScriptableObject {
		public virtual void SetupUnit (JesterComponentsRegistry componentsRegistry) {}
		
		public virtual void DestroyUnit (JesterComponentsRegistry componentsRegistry) {}
	}
}
