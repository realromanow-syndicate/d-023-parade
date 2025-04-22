using PageHelpers.Jester.CloudMessages.UI.ViewModels;
using PageHelpers.Jester.DI.Binding;

namespace PageHelpers.Jester.CloudMessages.UI.Binding {
	public class JesterMessagingPermissionScreenViewModelBinding : JesterItemBinding<JesterMessagesPermissionScreenViewModel> {
		public void SubmitPermission () {
			item.Submit();
		}
		
		public void RejectPermission () {
			item.Reject();
		}
	}
}
