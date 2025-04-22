using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace PageHelpers.Jester.Storage.App {
	public class JesterAppStorage {
		private const string PLAYER_CHARGE_SYMBOL = "user_prefs_charge";

		private Dictionary<string, JesterStorageDataEntity> _chargeEntities = new();

		public void LoadAppData () {
			if (!JesterStorageStatusIsNotEmpty()) return;

			InitializeJesterStorage();
		}

		private void InitializeJesterStorage () {
			var json = PlayerPrefs.GetString(PLAYER_CHARGE_SYMBOL);
			_chargeEntities = JsonConvert.DeserializeObject<Dictionary<string, JesterStorageDataEntity>>(json);
		}

		private static bool JesterStorageStatusIsNotEmpty () {
			return PlayerPrefs.HasKey(PLAYER_CHARGE_SYMBOL);
		}

		public void AddJesterStorageEntity (string key, object entity, int expirationTimeStamp) {
			var dateTime = GetJesterStorageExpireTimeFromTimeStamp(expirationTimeStamp);

			var entityObject = new JesterStorageDataEntity() {
				entityJester = entity,
				entityJesterExpirationDate = dateTime,
			};

			if (_chargeEntities.TryGetValue(key, out var oldValue))
				oldValue.entityJester = entityObject;
			else
				_chargeEntities.Add(key, entityObject);

			SaveJesterStorageEntities();
		}

		private static DateTime GetJesterStorageExpireTimeFromTimeStamp (int expirationTimeStamp) {
			return new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc)
				.AddSeconds(expirationTimeStamp);
		}

		public bool TryExtractJesterStorageEntity (string key, out object value) {
			if (_chargeEntities.TryGetValue(key, out var valuePack)) {
				value = valuePack.entityJester;

				if (valuePack.entityJesterExpirationDate > DateTime.UtcNow) return true;

				return false;
			}

			value = null;
			return false;
		}

		private void SaveJesterStorageEntities () {
			var json = JsonConvert.SerializeObject(_chargeEntities);
			PlayerPrefs.SetString(PLAYER_CHARGE_SYMBOL, json);
		}
		

		private class JesterStorageDataEntity {
			public object entityJester;
			public DateTime entityJesterExpirationDate;
		}
	}
}
