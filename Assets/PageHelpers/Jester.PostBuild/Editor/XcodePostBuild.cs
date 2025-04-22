using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using UnityEngine;

namespace PageHelpers.Jester.PostBuild.Editor
{
	public class XcodeEntitlementsPostprocessor
	{
		// Этот метод вызывается после завершения сборки проекта
		[PostProcessBuild(999)] // Высокий приоритет, чтобы выполнить после других постпроцессоров
		public static void OnPostProcessBuild(BuildTarget buildTarget, string pathToBuiltProject)
		{
			// Проверяем, что сборка для iOS
			if (buildTarget != BuildTarget.iOS)
				return;
            
			Debug.Log("Запуск XcodeEntitlementsPostprocessor...");
        
			// Ищем все .entitlements файлы в директории Xcode проекта
			string[] entitlementsFiles = Directory.GetFiles(pathToBuiltProject, "*.entitlements", SearchOption.AllDirectories);
        
			if (entitlementsFiles.Length == 0)
			{
				Debug.LogWarning("В проекте не найдены .entitlements файлы. CODE_SIGN_ENTITLEMENTS не будет установлен.");
				return;
			}
        
			// Используем первый найденный .entitlements файл
			string entitlementsFilePath = entitlementsFiles[0];
        
			// Если найдено несколько .entitlements файлов, выводим предупреждение
			if (entitlementsFiles.Length > 1)
			{
				Debug.LogWarning("Найдено несколько .entitlements файлов. Используется: " + entitlementsFilePath);
				Debug.LogWarning("Другие найденные файлы:");
				for (int i = 1; i < entitlementsFiles.Length; i++)
				{
					Debug.LogWarning("- " + entitlementsFiles[i]);
				}
			}
        
			// Получаем имя файла с расширением относительно корня Xcode проекта
			string relativePath = entitlementsFilePath.Replace(pathToBuiltProject + "/", "");
			Debug.Log($"Найден .entitlements файл: {relativePath}");
        
			// Загружаем Xcode проект
			string pbxProjPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
			PBXProject pbxProject = new PBXProject();
			pbxProject.ReadFromFile(pbxProjPath);
        
			// Получаем GUID основного таргета и таргета UnityFramework
			string targetGuid = pbxProject.GetUnityMainTargetGuid();
			string unityFrameworkTargetGuid = pbxProject.GetUnityFrameworkTargetGuid();
        
			// Устанавливаем CODE_SIGN_ENTITLEMENTS для основного таргета
			pbxProject.SetBuildProperty(targetGuid, "CODE_SIGN_ENTITLEMENTS", relativePath);
			Debug.Log($"CODE_SIGN_ENTITLEMENTS установлен на {relativePath} для основного таргета");
        
			// Устанавливаем CODE_SIGN_ENTITLEMENTS для UnityFramework таргета, если это необходимо
			// Раскомментируйте следующую строку, если нужно установить entitlements и для UnityFramework
			// pbxProject.SetBuildProperty(unityFrameworkTargetGuid, "CODE_SIGN_ENTITLEMENTS", relativePath);
        
			// Сохраняем изменения в проект
			pbxProject.WriteToFile(pbxProjPath);
			Debug.Log("Изменения в Xcode проект успешно сохранены");
		}
    
		// Добавляем пункт меню для поиска entitlements файлов в последнем собранном проекте
		[MenuItem("Tools/iOS/Check Entitlements in Last Build")]
		public static void CheckEntitlementsInLastBuild()
		{
			string lastBuildPath = EditorPrefs.GetString("LastIOSBuildPath", "");
			if (string.IsNullOrEmpty(lastBuildPath) || !Directory.Exists(lastBuildPath))
			{
				EditorUtility.DisplayDialog("Ошибка", "Путь к последней сборке iOS не найден. Сначала соберите iOS проект.", "OK");
				return;
			}
        
			string[] entitlementsFiles = Directory.GetFiles(lastBuildPath, "*.entitlements", SearchOption.AllDirectories);
        
			if (entitlementsFiles.Length == 0)
			{
				EditorUtility.DisplayDialog("Информация", "В проекте не найдены .entitlements файлы.", "OK");
				return;
			}
        
			string message = $"Найдено {entitlementsFiles.Length} .entitlements файлов:\n\n";
			foreach (string file in entitlementsFiles)
			{
				message += $"• {file.Replace(lastBuildPath + "/", "")}\n";
			}
        
			EditorUtility.DisplayDialog("Найденные .entitlements файлы", message, "OK");
		}
    
		// Сохраняем путь к последней сборке
		[PostProcessBuild(1000)]
		public static void SaveLastBuildPath(BuildTarget buildTarget, string pathToBuiltProject)
		{
			if (buildTarget == BuildTarget.iOS)
			{
				EditorPrefs.SetString("LastIOSBuildPath", pathToBuiltProject);
			}
		}
	}
}
