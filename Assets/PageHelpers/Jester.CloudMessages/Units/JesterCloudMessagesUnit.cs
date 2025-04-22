using PageHelpers.Jester.CloudMessages.App;
using PageHelpers.Jester.CloudMessages.UI.App;
using PageHelpers.Jester.DI.App;
using PageHelpers.Jester.DI.Units;
using UnityEngine;

namespace PageHelpers.Jester.CloudMessages.Units {
	[CreateAssetMenu(menuName = "Jester/CloudMessages/Units/JesterCloudMessagesUnit", fileName = "JesterCloudMessagesUnit")]
	public class JesterCloudMessagesUnit : JesterAppUnit {
		[SerializeField]
		private JesterCloudMessagesUIScreenService.Preferences uiPreferences;

		public override void SetupUnit (JesterComponentsRegistry componentRegistry) {
			base.SetupUnit(componentRegistry);

			componentRegistry.Instantiate<JesterCloudMessagesUIScreenService>(uiPreferences);
			componentRegistry.Instantiate<JesterCloudMessagesService>();
			componentRegistry.Instantiate<JesterCloudMessagesListener>();
		}
	}
}
