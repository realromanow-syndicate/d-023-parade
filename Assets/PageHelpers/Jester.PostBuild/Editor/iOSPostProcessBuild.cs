using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace PageHelpers.Jester.PostBuild.Editor {
	public class iOSPostProcessBuild {
		[PostProcessBuild(998)]
		public static void OnPostprocessBuild (BuildTarget buildTarget, string pathToBuiltProject) {
			if (buildTarget != BuildTarget.iOS)
				return;

			var projectPath = Path.Combine(pathToBuiltProject, "Unity-iPhone.xcodeproj/project.pbxproj");
			var plistPath = Path.Combine(pathToBuiltProject, "Info.plist");

			// Найдем существующий entitlements файл или создадим новый путь
			var entitlementsPath = FindExistingEntitlements(pathToBuiltProject);
			if (string.IsNullOrEmpty(entitlementsPath)) entitlementsPath = Path.Combine(pathToBuiltProject, "Unity-iPhone/Unity-iPhone.entitlements");

			Debug.Log("Using entitlements file: " + entitlementsPath);

			// Load project
			var project = new PBXProject();
			project.ReadFromFile(projectPath);

			// Get the main target GUID
			var targetGUID = project.GetUnityMainTargetGuid();

			// Add UserNotifications framework
			project.AddFrameworkToProject(targetGUID, "UserNotifications.framework", false);
			project.WriteToFile(projectPath);

			// Add Push Notifications and Background Modes capabilities
			AddPushNotificationsCapability(projectPath, entitlementsPath);

			// Add Remote Notifications to Background Modes in Info.plist
			AddRemoteNotificationsBackgroundMode(plistPath);
		}

		// Метод для поиска существующих entitlements файлов
		private static string FindExistingEntitlements (string pathToBuiltProject) {
			var entitlementFiles = Directory.GetFiles(pathToBuiltProject, "*.entitlements", SearchOption.AllDirectories);

			foreach (var entitlementFile in entitlementFiles) {
				Debug.Log(entitlementFile);
			}
			
			if (entitlementFiles.Length > 0) {
				Debug.Log("Found existing entitlements file: " + entitlementFiles[0]);
				return entitlementFiles[0];
			}

			return null;
		}

		private static void AddPushNotificationsCapability (string projectPath, string entitlementsPath) {
			// Проверяем, существует ли уже файл
			var entitlementsExist = File.Exists(entitlementsPath);

			// Создаем объект только если файл не существует, или если нужно обновить существующий
			var capabilityManager = new ProjectCapabilityManager(
				projectPath, entitlementsPath, "Unity-iPhone");

			// Add Push Notifications capability
			capabilityManager.AddPushNotifications(true); // true for development mode

			// Try different methods to add the background modes capability
			try {
				capabilityManager.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
			}
			catch (System.Exception ex) {
				Debug.LogWarning("Could not add Background Modes capability via string array method: " + ex.Message);

				// Альтернативный способ добавления background modes
				try {
					capabilityManager.AddBackgroundModes(BackgroundModesOptions.RemoteNotifications);
				}
				catch (System.Exception e) {
					Debug.LogError("Failed to add background modes: " + e.Message);
				}
			}

			// Apply the changes
			capabilityManager.WriteToFile();

			Debug.Log("Successfully updated entitlements at: " + entitlementsPath);
		}

		private static void AddRemoteNotificationsBackgroundMode (string plistPath) {
			// Load Info.plist
			var plist = new PlistDocument();
			plist.ReadFromFile(plistPath);

			// Check if UIBackgroundModes exists, if not create it
			PlistElementArray backgroundModes;
			if (plist.root.values.ContainsKey("UIBackgroundModes"))
				backgroundModes = plist.root.values["UIBackgroundModes"].AsArray();
			else
				backgroundModes = plist.root.CreateArray("UIBackgroundModes");

			// Add remote-notification if it doesn't exist
			var hasRemoteNotification = false;
			foreach (var mode in backgroundModes.values) {
				if (mode.AsString() == "remote-notification") {
					hasRemoteNotification = true;
					break;
				}
			}

			if (!hasRemoteNotification) backgroundModes.AddString("remote-notification");

			// Save Info.plist
			plist.WriteToFile(plistPath);
		}
	}
}
