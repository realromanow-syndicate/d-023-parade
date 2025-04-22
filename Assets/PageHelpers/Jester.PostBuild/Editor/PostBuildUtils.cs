using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
#if UNITY_IOS
#endif

namespace PageHelpers.Jester.PostBuild.Editor {
	public class PostBuildUtils {
		private const string K_TRACKING_DESCRIPTION = "Please allow us to collect crash and termination reports (crash data) so that we can fix critical bugs and release updates in a timely and efficient manner";

		[PostProcessBuild(0)]
		public static void OnPostProcessBuild (BuildTarget buildTarget, string pathToXcode) {
			if (buildTarget == BuildTarget.iOS) AddPListValues(pathToXcode);
		}

		private static void AddPListValues (string pathToXcode) {
			var plistPath = CreatePath(pathToXcode, out var plistObj);

			CreateProperties(plistObj, plistPath);
			SaveProperties(plistPath, plistObj);
		}

		private static string CreatePath (string pathToXcode, out PlistDocument plistObj) {
			var plistPath = pathToXcode + "/Info.plist";
			plistObj = new PlistDocument();
			return plistPath;
		}

		private static void CreateProperties (PlistDocument plistObj, string plistPath) {
			plistObj.ReadFromString(File.ReadAllText(plistPath));
			var plistRoot = plistObj.root;
			plistRoot.SetString("NSUserTrackingUsageDescription", K_TRACKING_DESCRIPTION);
		}

		private static void SaveProperties (string plistPath, PlistDocument plistObj) {
			File.WriteAllText(plistPath, plistObj.WriteToString());
		}
	}
}
