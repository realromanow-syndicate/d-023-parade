//using Apple.GameKit;
using TMPro;
using UnityEngine;

namespace PageHelpers.Jestery.InnerMessages.Utils {
	public class PlayerNameUtil : MonoBehaviour {
		[SerializeField]
		private TMP_InputField _inputField;

		private void Start () {
			//_inputField.text = GKLocalPlayer.Local.DisplayName;
		}
	}
}
