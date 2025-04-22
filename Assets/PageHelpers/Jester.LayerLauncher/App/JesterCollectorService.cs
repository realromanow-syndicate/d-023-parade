using AppsFlyerSDK;
using PageHelpers.Jester.CloudMessages.Api;
using PageHelpers.Jester.LayerLauncher.Api;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PageHelpers.Jester.LayerLauncher.App {
	public class JesterCollectorService : IJesterCollectorService {
		private readonly Preferences _preferences;
		private readonly IJesterCloudMessagesListener _quickCloudMessagesListener;

		public JesterCollectorService (
			Preferences preferences,
			IJesterCloudMessagesListener quickCloudMessagesListener) {
			_preferences = preferences;
			_quickCloudMessagesListener = quickCloudMessagesListener;
		}

		public Dictionary<string, object> GetJesterConfigurationParameters () {
			var list = PrepareParamsCollection();

			var os = GetAppOsId();
			var storeId = GetAppStoreId();

			OutIosInList(list, os);
			PutStoreIdInList(list, storeId);
			PutPushTokenInList(list);

			return list;
		}

		private void PutPushTokenInList (IDictionary<string, object> parameters) {
			parameters.Add("push_token", _quickCloudMessagesListener.lastValidToken.Value);
		}

		private static void PutStoreIdInList (IDictionary<string, object> parameters, string storeId) {
			parameters.Add("store_id", storeId);
		}

		private static void OutIosInList (IDictionary<string, object> parameters, string os) {
			parameters.Add("os", os);
		}

		private string GetAppStoreId () {
			return Application.platform switch {
				RuntimePlatform.Android => Application.identifier,
				RuntimePlatform.IPhonePlayer => _preferences.appleIos,
				_ => string.Empty,
			};
		}

		private static string GetAppOsId () {
			return Application.platform switch {
				RuntimePlatform.Android => "Android",
				RuntimePlatform.IPhonePlayer => "iOS",
				_ => string.Empty,
			};
		}

		private Dictionary<string, object> PrepareParamsCollection () {
			return new Dictionary<string, object> {
				{ "af_id", AppsFlyer.getAppsFlyerId() },
				{ "bundle_id", Application.identifier },
				{ "locale", Application.systemLanguage.ToString("G") },
				{ "firebase_project_id", _preferences.googleFirebaseProject },
			};
		}

		[Serializable]
		public class Preferences {
			[SerializeField]
			private string _googleFirebaseProject;

			[SerializeField]
			private string _appleIos;

			public string googleFirebaseProject => _googleFirebaseProject;
			public string appleIos => _appleIos;
		}
	}
}
