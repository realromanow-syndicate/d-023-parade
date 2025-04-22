using PageHelpers.Jester.IternalVision.Api;
using System;
using UniRx;

namespace PageHelpers.Jester.IternalVision.App {
	public class JesterIternalVisionService : IJesterIternalVisionService, IDisposable {
		public IReadOnlyReactiveProperty<int> visionGeneration => _visionGeneration;
		public ReactiveCommand jesterVisionLoaded { get; } = new();

		private readonly UniWebView _uniWView;
		private readonly ReactiveProperty<int> _visionGeneration = new();
		
		private UniWebViewSafeBrowsing _safeBrowser;
		private int _triesCount;

		public JesterIternalVisionService (UniWebView uniWView) {
			_uniWView = uniWView;

			SubscribeToEvents();
		}

		private void SubscribeToEvents () {
			_uniWView.OnPageFinished += OnPageLoaded;
			_uniWView.OnLoadingErrorReceived += OnLoadingErrorReceived;
			_uniWView.OnWebContentProcessTerminated += OnPageTerminated;
		}

		public void LoadJesterVision (string url) {
			_uniWView.Load(url);
		}

		public void LaunchJesterVision () {
			_uniWView.Show();
		}

		private void OnPageLoaded (UniWebView supervisor, int statusCode, string url) {
			_visionGeneration.Value++;

			jesterVisionLoaded.Execute();
		}

		private static void OnLoadingErrorReceived (UniWebView supervisor, int code, string message, UniWebViewNativeResultPayload payload) {
			supervisor.Load((string)payload.Extra["failingURL"]);
		}

		private static void OnPageTerminated (UniWebView supervisor) {
			supervisor.Reload();
		}

		public void Dispose() {
			_visionGeneration.Dispose();
			jesterVisionLoaded.Dispose();
		}
	}
}
