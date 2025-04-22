using Cysharp.Threading.Tasks;
using System.Threading;

namespace PageHelpers.Jester.TrackingTransparency.Api {
	public interface IJesterTrackingTransparencyService {
		bool hasTrackingTransparencyPermission { get; }
		
		UniTask RequestTrackingTransparencyPermission (CancellationToken cancellationToken);
	}
}
