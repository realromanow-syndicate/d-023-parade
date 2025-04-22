using AppsFlyerSDK;
using PageHelpers.Jester.Analytics.Api;
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace PageHelpers.Jester.Analytics.App {
	public class JesterAnalyticsListener : MonoBehaviour, IJesterAnalyticsListener, IAppsFlyerConversionData, IDisposable {
		public ReactiveCommand<Dictionary<string, object>> dataSuccessReceived { get; } = new();
		public ReactiveCommand dataFailReceived { get; } = new();

		public void onConversionDataSuccess (string conversionData) {
			var data = AppsFlyer.CallbackStringToDictionary(conversionData);

			dataSuccessReceived.Execute(data);
		}

		public void onConversionDataFail (string error) {
			dataFailReceived.Execute();
		}

		public void onAppOpenAttribution (string attributionData) {}

		public void onAppOpenAttributionFailure (string error) {}

		public void Dispose () {
			dataSuccessReceived?.Dispose();
			dataFailReceived?.Dispose();
		}
	}
}
