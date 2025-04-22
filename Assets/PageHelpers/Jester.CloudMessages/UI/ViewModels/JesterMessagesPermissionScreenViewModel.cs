using System;
using UniRx;

namespace PageHelpers.Jester.CloudMessages.UI.ViewModels {
	public class JesterMessagesPermissionScreenViewModel : IDisposable {
		public ReactiveCommand<bool> permissionUpdate { get; } = new();
		public bool isHasPermitAnswer { get; private set; }

		public void Submit () {
			permissionUpdate.Execute(true);
			isHasPermitAnswer = true;
		}

		public void Reject () {
			permissionUpdate.Execute(false);
			isHasPermitAnswer = true;
		}

		public void Dispose() {
			permissionUpdate?.Dispose();
		}
	}
}
