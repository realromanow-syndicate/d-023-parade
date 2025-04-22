using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using PageHelpers.Jester.GetRequest.Api;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using UnityEngine.Networking;

namespace PageHelpers.Jester.GetRequest.App {
	public class JesterGetRequestsService : IJesterGetRequestsService {
		public async UniTask<string> SendUrlRequest (string url, Dictionary<string, object> forms, CancellationToken cancellationToken) {
			using var request = new UnityWebRequest(url, "POST");

			SetupHeaders(forms, request);

			request.SendWebRequest();
			
			await WaitUntilDone(request, cancellationToken);

			var response = request.downloadHandler.text;

			return response;
		}

		private static async UniTask WaitUntilDone (UnityWebRequest request, CancellationToken cancellationToken) {
			while (!request.isDone) {
				await UniTask.Yield(cancellationToken: cancellationToken);
			}
		}

		private static void SetupHeaders (Dictionary<string, object> body, UnityWebRequest request) {
			var bodyRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(body));

			request.uploadHandler = new UploadHandlerRaw(bodyRaw);
			request.downloadHandler = new DownloadHandlerBuffer();

			request.SetRequestHeader("Content-Type", "application/json");
		}
	}
}
