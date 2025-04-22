using PageHelpers.Jester.UnityConfig.Api;
using System;
using UniRx;
using UnityEngine;

namespace PageHelpers.Jester.UnityConfig.App {
	public class UnityConfigListenerService : IUnityConfigListenerService, IDisposable {
		public ReactiveCommand<bool> isRestrictionCompleted { get; } = new();

		private readonly IUnityConfigInitializerService _initializerService;
		private readonly Preferences _preferences;
		private readonly CompositeDisposable _compositeDisposable = new();

		public UnityConfigListenerService (
			IUnityConfigInitializerService initializerService,
			Preferences preferences) {
			_initializerService = initializerService;
			_preferences = preferences;
		}

		public void StartParseConfig () {
			_initializerService.cachedRawData
				.Where(raw => raw != null)
				.Take(1)
				.Subscribe(raw => {
					if (raw.TryGetValue(_preferences.key, out var value)) {
						if ((string)value == _preferences.value) {
							isRestrictionCompleted.Execute(true);
							return;
						}
					}

					isRestrictionCompleted.Execute(false);
				})
				.AddTo(_compositeDisposable);
		}

		public void Dispose () {
			_compositeDisposable.Dispose();
			isRestrictionCompleted.Dispose();
		}
		
		[Serializable]
		public class Preferences {
			[SerializeField]
			private string _key;

			[SerializeField]
			private string _value;

			public string key => _key;
			public string value => _value;
		}
	}
}
