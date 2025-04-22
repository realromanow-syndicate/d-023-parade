using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PageHelpers.Jester.GetRequest.Api;
using PageHelpers.Jester.LayerLauncher.Api;
using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace PageHelpers.Jester.LayerLauncher.App {
	public class JesterApiService : IJesterApiService {
		private readonly Preferences _preferences;
		private readonly IJesterGetRequestsService _jesterGetRequestsService;

		public JesterApiService (
			Preferences preferences,
			IJesterGetRequestsService jesterGetRequestsService) {
			_preferences = preferences;
			_jesterGetRequestsService = jesterGetRequestsService;
		}

		public UniTask<Dictionary<string, object>> GetJesterConfigAsync (Dictionary<string, object> senderData, CancellationToken cancellationToken) {
			return _jesterGetRequestsService.SendUrlRequest(_preferences.requestAddress, senderData, cancellationToken)
				.ContinueWith(JsonConvert.DeserializeObject<Dictionary<string, object>>);
		}

		[Serializable]
		public class Preferences {
			[SerializeField]
			private string _requestAddress;

			public string requestAddress => _requestAddress;
		}
	}
}
