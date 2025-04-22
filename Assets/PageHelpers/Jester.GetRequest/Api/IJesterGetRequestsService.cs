using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;

namespace PageHelpers.Jester.GetRequest.Api {
	public interface IJesterGetRequestsService {
		UniTask<string> SendUrlRequest (string url, Dictionary<string, object> forms, CancellationToken cancellationToken);
	}
}
