using Cysharp.Threading.Tasks;
using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;
using PageHelpers.Jestery.InnerMessages.App;
using UnityEngine;

namespace PageHelpers.Jestery.InnerMessages.Units {
	[CreateAssetMenu(menuName = "Jester/InnerMessages/Units/JesterInnerMessagesUnit", fileName = "JesterInnerMessagesUnit")]
	public class JesterInnerMessagesUnit : JesterAppUnit {
		public override void SetupUnit (JesterComponentsRegistry componentsRegistry) {
			base.SetupUnit(componentsRegistry);

			componentsRegistry.Instantiate<JesteryInnerMessagesService>().Init().Forget(Debug.LogException);
		}
	}
}
