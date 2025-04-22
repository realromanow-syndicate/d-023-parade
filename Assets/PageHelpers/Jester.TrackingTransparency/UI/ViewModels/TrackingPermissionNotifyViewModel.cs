using System;
using UniRx;

namespace PageHelpers.Jester.TrackingTransparency.UI.ViewModels {
	public class TrackingPermissionNotifyViewModel : IDisposable {
		public ReactiveCommand read { get; } = new();

		public void Submit () {
			read.Execute();
		}

		public void Dispose () {
			read?.Dispose();
		}
	}
}
