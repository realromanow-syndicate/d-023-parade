using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PageHelpers.Editor {
	public class KeywordReplacementTool : EditorWindow {
		private string rootDirectory = "";
		private string oldKeyword = "";
		private string newKeyword = "";
		private bool includeDirectories = true;
		private bool includeFileNames = true;
		private bool includeFileContents = true;
		private Vector2 scrollPosition;

		private List<ReplacementInfo> replacements = new();
		private bool analyzed = false;

		[MenuItem("Tools/Keyword Replacement Tool")]
		public static void ShowWindow () {
			GetWindow<KeywordReplacementTool>("Keyword Replacement Tool");
		}

		private void OnGUI () {
			GUILayout.Label("Keyword Replacement Tool", EditorStyles.boldLabel);
			EditorGUILayout.Space();

			rootDirectory = EditorGUILayout.TextField("Root Directory", rootDirectory);
			if (GUILayout.Button("Browse...")) {
				var selectedPath = EditorUtility.OpenFolderPanel("Select Root Directory", "", "");
				if (!string.IsNullOrEmpty(selectedPath)) {
					rootDirectory = selectedPath;
					Repaint();
				}
			}

			EditorGUILayout.Space();
			oldKeyword = EditorGUILayout.TextField("Old Keyword", oldKeyword);
			newKeyword = EditorGUILayout.TextField("New Keyword", newKeyword);

			EditorGUILayout.Space();
			GUILayout.Label("Search Options", EditorStyles.boldLabel);
			includeDirectories = EditorGUILayout.Toggle("Include Directories", includeDirectories);
			includeFileNames = EditorGUILayout.Toggle("Include File Names", includeFileNames);
			includeFileContents = EditorGUILayout.Toggle("Include File Contents", includeFileContents);

			EditorGUILayout.Space();

			GUI.enabled = !string.IsNullOrEmpty(rootDirectory) && !string.IsNullOrEmpty(oldKeyword) && !string.IsNullOrEmpty(newKeyword);
			if (GUILayout.Button("Analyze Replacements")) {
				replacements.Clear();
				AnalyzeReplacements();
				analyzed = true;
			}

			GUI.enabled = analyzed && replacements.Count > 0;
			if (GUILayout.Button("Apply Replacements")) {
				ApplyReplacements();
				EditorUtility.DisplayDialog("Replacements Complete", $"{replacements.Count} replacements have been applied successfully.", "OK");
				analyzed = false;
				replacements.Clear();
			}

			GUI.enabled = true;

			EditorGUILayout.Space();

			if (analyzed) {
				GUILayout.Label($"Found {replacements.Count} potential replacements:", EditorStyles.boldLabel);

				scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

				if (replacements.Count == 0)
					EditorGUILayout.HelpBox("No replacements found.", MessageType.Info);
				else
					DisplayReplacements();

				EditorGUILayout.EndScrollView();
			}
		}

		private void AnalyzeReplacements () {
			if (!Directory.Exists(rootDirectory)) {
				EditorUtility.DisplayDialog("Error", "The specified directory does not exist.", "OK");
				return;
			}

			try {
				if (includeDirectories) AnalyzeDirectoryNames(rootDirectory);

				if (includeFileNames || includeFileContents) AnalyzeFiles(rootDirectory);

				replacements = replacements.OrderBy(r => r.Path).ToList();
			}
			catch (Exception ex) {
				EditorUtility.DisplayDialog("Error", $"An error occurred during analysis: {ex.Message}", "OK");
			}
		}

		private void AnalyzeDirectoryNames (string directory) {
			// Get all subdirectories
			var subdirectories = Directory.GetDirectories(directory, "*", SearchOption.AllDirectories);

			// Check each directory name
			foreach (var subdirectory in subdirectories) {
				var dirName = Path.GetFileName(subdirectory);

				if (ContainsKeywordCaseInsensitive(dirName)) {
					var newDirName = ReplaceKeywordPreservingCase(dirName);
					var newPath = Path.Combine(Path.GetDirectoryName(subdirectory), newDirName);

					replacements.Add(new ReplacementInfo {
						Type = ReplacementType.Directory,
						Path = subdirectory,
						OriginalName = dirName,
						NewName = newDirName,
						NewPath = newPath,
					});
				}
			}
		}

		private void AnalyzeFiles (string directory) {
			// Get all files with supported extensions
			var extensions = new[] { "*.cs", "*.asmdef", "*.asset" };
			var files = new List<string>();

			foreach (var extension in extensions) {
				files.AddRange(Directory.GetFiles(directory, extension, SearchOption.AllDirectories));
			}

			// Check each file
			foreach (var file in files) {
				var fileName = Path.GetFileName(file);

				// Check file name
				if (includeFileNames && ContainsKeywordCaseInsensitive(fileName)) {
					var newFileName = ReplaceKeywordPreservingCase(fileName);
					var newPath = Path.Combine(Path.GetDirectoryName(file), newFileName);

					replacements.Add(new ReplacementInfo {
						Type = ReplacementType.FileName,
						Path = file,
						OriginalName = fileName,
						NewName = newFileName,
						NewPath = newPath,
					});
				}

				// Check file contents
				if (includeFileContents) AnalyzeFileContents(file);
			}
		}

		private void AnalyzeFileContents (string filePath) {
			try {
				var extension = Path.GetExtension(filePath).ToLower();

				// Special handling for .asmdef files
				if (extension == ".asmdef") {
					AnalyzeAsmdefContents(filePath);
					return;
				}

				var lines = File.ReadAllLines(filePath);
				var contentReplacements = new List<ContentReplacementInfo>();

				for (var i = 0; i < lines.Length; i++) {
					var line = lines[i];
					var matches = Regex.Matches(line, Regex.Escape(oldKeyword), RegexOptions.IgnoreCase);

					foreach (Match match in matches) {
						var originalText = match.Value;
						var replacementText = ReplaceKeywordPreservingCase(originalText);

						// Create context with a few lines before and after
						var startLineIndex = Math.Max(0, i - 2);
						var endLineIndex = Math.Min(lines.Length - 1, i + 2);
						var contextBuilder = new StringBuilder();

						for (var j = startLineIndex; j <= endLineIndex; j++) {
							var contextLine = lines[j];
							var linePrefix = $"Line {j + 1}: ";

							if (j == i) {
								// For the line with the match, highlight the match
								var matchStart = match.Index;
								var matchEnd = match.Index + match.Length;

								var beforeMatch = contextLine.Substring(0, matchStart);
								var matchText = contextLine.Substring(matchStart, match.Length);
								var afterMatch = contextLine.Substring(matchEnd);

								contextBuilder.AppendLine($"{linePrefix}→ {beforeMatch}<color=#FF4000><b>{matchText}</b></color>{afterMatch}");
							}
							else {
								contextBuilder.AppendLine($"{linePrefix}  {contextLine}");
							}
						}

						contentReplacements.Add(new ContentReplacementInfo {
							LineNumber = i + 1,
							OriginalText = originalText,
							ReplacementText = replacementText,
							Context = contextBuilder.ToString(),
						});
					}
				}

				if (contentReplacements.Count > 0)
					replacements.Add(new ReplacementInfo {
						Type = ReplacementType.FileContent,
						Path = filePath,
						ContentReplacements = contentReplacements,
					});
			}
			catch (Exception ex) {
				Debug.LogError($"Error analyzing file contents of {filePath}: {ex.Message}");
			}
		}

		private void AnalyzeAsmdefContents (string filePath) {
			try {
				var json = File.ReadAllText(filePath);

				// Use regex to find the "name" field value
				var nameMatch = Regex.Match(json, "\"name\"\\s*:\\s*\"([^\"]+)\"");
				if (nameMatch.Success && nameMatch.Groups.Count > 1) {
					var nameValue = nameMatch.Groups[1].Value;

					if (ContainsKeywordCaseInsensitive(nameValue)) {
						var replacedNameValue = ReplaceKeywordPreservingCase(nameValue);

						// Create context for the replacement
						var lines = json.Split('\n');
						var contextBuilder = new StringBuilder();

						var lineNumber = -1;
						for (var i = 0; i < lines.Length; i++) {
							if (lines[i].Contains("\"name\"") && lines[i].Contains(nameValue)) {
								lineNumber = i;
								break;
							}
						}

						if (lineNumber >= 0) {
							var startLineIndex = Math.Max(0, lineNumber - 2);
							var endLineIndex = Math.Min(lines.Length - 1, lineNumber + 2);

							for (var j = startLineIndex; j <= endLineIndex; j++) {
								var contextLine = lines[j];
								var linePrefix = $"Line {j + 1}: ";

								if (j == lineNumber) {
									// Highlight the name value
									var nameIndex = contextLine.IndexOf(nameValue);
									if (nameIndex >= 0) {
										var beforeName = contextLine.Substring(0, nameIndex);
										var afterName = contextLine.Substring(nameIndex + nameValue.Length);

										contextBuilder.AppendLine($"{linePrefix}→ {beforeName}<color=#FF4000><b>{nameValue}</b></color>{afterName}");
									}
									else {
										contextBuilder.AppendLine($"{linePrefix}→ {contextLine}");
									}
								}
								else {
									contextBuilder.AppendLine($"{linePrefix}  {contextLine}");
								}
							}

							replacements.Add(new ReplacementInfo {
								Type = ReplacementType.AsmdefNameField,
								Path = filePath,
								OriginalName = nameValue,
								NewName = replacedNameValue,
								ContentReplacements = new List<ContentReplacementInfo> {
									new() {
										LineNumber = lineNumber + 1,
										OriginalText = nameValue,
										ReplacementText = replacedNameValue,
										Context = contextBuilder.ToString(),
									},
								},
							});
						}
					}
				}
			}
			catch (Exception ex) {
				Debug.LogError($"Error analyzing .asmdef file {filePath}: {ex.Message}");
			}
		}

		private bool ContainsKeywordCaseInsensitive (string text) {
			return Regex.IsMatch(text, Regex.Escape(oldKeyword), RegexOptions.IgnoreCase);
		}

		private string ReplaceKeywordPreservingCase (string text) {
			return Regex.Replace(
				text,
				Regex.Escape(oldKeyword),
				match => {
					var matchValue = match.Value;
					var result = newKeyword;

					// Try to preserve the case pattern of the matched text
					if (matchValue.ToUpper() == matchValue)
						// All uppercase
						result = newKeyword.ToUpper();
					else if (matchValue.ToLower() == matchValue)
						// All lowercase
						result = newKeyword.ToLower();
					else if (char.IsUpper(matchValue[0]))
						// First letter uppercase
						result = char.ToUpper(newKeyword[0]) + newKeyword.Substring(1);

					return result;
				},
				RegexOptions.IgnoreCase
			);
		}

		private void DisplayReplacements () {
			EditorGUILayout.BeginVertical(EditorStyles.helpBox);

			foreach (var replacement in replacements) {
				EditorGUILayout.BeginVertical(GUI.skin.box);

				var relativePath = replacement.Path;
				if (replacement.Path.StartsWith(Application.dataPath)) relativePath = "Assets" + replacement.Path.Substring(Application.dataPath.Length);

				GUILayout.Label($"Path: {relativePath}", EditorStyles.boldLabel);

				switch (replacement.Type) {
					case ReplacementType.Directory:
						EditorGUILayout.LabelField("Type: Directory");
						EditorGUILayout.LabelField($"Original: {replacement.OriginalName}");
						EditorGUILayout.LabelField($"New: {replacement.NewName}");
						break;

					case ReplacementType.FileName:
						EditorGUILayout.LabelField("Type: File Name");
						EditorGUILayout.LabelField($"Original: {replacement.OriginalName}");
						EditorGUILayout.LabelField($"New: {replacement.NewName}");
						break;

					case ReplacementType.FileContent:
						EditorGUILayout.LabelField($"Type: File Content ({replacement.ContentReplacements.Count} replacements)");

						foreach (var contentReplacement in replacement.ContentReplacements) {
							EditorGUILayout.BeginVertical(EditorStyles.helpBox);

							GUILayout.Label($"Line {contentReplacement.LineNumber}:", EditorStyles.boldLabel);
							EditorGUILayout.LabelField("Context:");

							var richTextStyle = new GUIStyle(EditorStyles.textArea);
							richTextStyle.richText = true;
							richTextStyle.wordWrap = true;

							EditorGUILayout.TextArea(contentReplacement.Context, richTextStyle);

							EditorGUILayout.LabelField($"Original: {contentReplacement.OriginalText}");
							EditorGUILayout.LabelField($"New: {contentReplacement.ReplacementText}");

							EditorGUILayout.EndVertical();
							EditorGUILayout.Space();
						}
						break;

					case ReplacementType.AsmdefNameField:
						EditorGUILayout.LabelField("Type: Assembly Definition Name Field");
						EditorGUILayout.LabelField($"Original: {replacement.OriginalName}");
						EditorGUILayout.LabelField($"New: {replacement.NewName}");

						if (replacement.ContentReplacements.Count > 0) {
							var contentReplacement = replacement.ContentReplacements[0];
							EditorGUILayout.BeginVertical(EditorStyles.helpBox);

							GUILayout.Label($"Line {contentReplacement.LineNumber}:", EditorStyles.boldLabel);
							EditorGUILayout.LabelField("Context:");

							var richTextStyle = new GUIStyle(EditorStyles.textArea);
							richTextStyle.richText = true;
							richTextStyle.wordWrap = true;

							EditorGUILayout.TextArea(contentReplacement.Context, richTextStyle);

							EditorGUILayout.EndVertical();
						}
						break;
				}

				EditorGUILayout.EndVertical();
				EditorGUILayout.Space();
			}

			EditorGUILayout.EndVertical();
		}

		private void ApplyReplacements () {
			// First handle .asmdef replacements
			foreach (var replacement in replacements.Where(r => r.Type == ReplacementType.AsmdefNameField)) {
				try {
					var content = File.ReadAllText(replacement.Path);

					// Replace only the value of the "name" field while preserving the field itself
					var newContent = Regex.Replace(
						content,
						"(\"name\"\\s*:\\s*\")(" + Regex.Escape(replacement.OriginalName) + ")(\")",
						m => m.Groups[1].Value + replacement.NewName + m.Groups[3].Value
					);

					File.WriteAllText(replacement.Path, newContent);

					// Update corresponding .meta file if necessary
					var metaPath = replacement.Path + ".meta";
					if (File.Exists(metaPath)) {
						var metaContent = File.ReadAllText(metaPath);
						if (metaContent.Contains(replacement.OriginalName)) {
							var newMetaContent = Regex.Replace(
								metaContent,
								Regex.Escape(replacement.OriginalName),
								replacement.NewName
							);
							File.WriteAllText(metaPath, newMetaContent);
						}
					}
				}
				catch (Exception ex) {
					Debug.LogError($"Error updating .asmdef file {replacement.Path}: {ex.Message}");
				}
			}

			// Next handle regular content replacements (before we rename any files or directories)
			foreach (var replacement in replacements.Where(r => r.Type == ReplacementType.FileContent)) {
				try {
					var content = File.ReadAllText(replacement.Path);

					// Replace all occurrences preserving case
					var newContent = Regex.Replace(
						content,
						Regex.Escape(oldKeyword),
						match => {
							var matchValue = match.Value;
							var result = newKeyword;

							// Try to preserve the case pattern of the matched text
							if (matchValue.ToUpper() == matchValue)
								// All uppercase
								result = newKeyword.ToUpper();
							else if (matchValue.ToLower() == matchValue)
								// All lowercase
								result = newKeyword.ToLower();
							else if (char.IsUpper(matchValue[0]))
								// First letter uppercase
								result = char.ToUpper(newKeyword[0]) + newKeyword.Substring(1);

							return result;
						},
						RegexOptions.IgnoreCase
					);

					File.WriteAllText(replacement.Path, newContent);
				}
				catch (Exception ex) {
					Debug.LogError($"Error updating content in {replacement.Path}: {ex.Message}");
				}
			}

			// Now handle file name replacements
			foreach (var replacement in replacements.Where(r => r.Type == ReplacementType.FileName)) {
				try {
					if (File.Exists(replacement.Path)) {
						// Make sure the directory exists
						var directory = Path.GetDirectoryName(replacement.NewPath);
						if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);

						// Handle .meta file before moving the main file
						var metaPath = replacement.Path + ".meta";
						var newMetaPath = replacement.NewPath + ".meta";

						if (File.Exists(metaPath)) {
							// Read and update content if needed
							var metaContent = File.ReadAllText(metaPath);
							//bool metaContentChanged = false;

							if (ContainsKeywordCaseInsensitive(metaContent)) {
								var newMetaContent = Regex.Replace(
									metaContent,
									Regex.Escape(oldKeyword),
									match => ReplaceKeywordPreservingCase(match.Value),
									RegexOptions.IgnoreCase
								);

								if (newMetaContent != metaContent) File.WriteAllText(metaPath, newMetaContent);
								//metaContentChanged = true;
							}

							// Move the .meta file to match the main file
							if (File.Exists(metaPath)) File.Move(metaPath, newMetaPath);
						}

						// Move the main file
						File.Move(replacement.Path, replacement.NewPath);
					}
				}
				catch (Exception ex) {
					Debug.LogError($"Error renaming file {replacement.Path}: {ex.Message}");
				}
			}

			// Finally handle directory name replacements (bottom-up to avoid path issues)
			foreach (var replacement in replacements.Where(r => r.Type == ReplacementType.Directory).OrderByDescending(r => r.Path.Length)) {
				try {
					if (Directory.Exists(replacement.Path)) {
						// Handle .meta file for the directory
						var metaPath = replacement.Path + ".meta";
						var newMetaPath = replacement.NewPath + ".meta";

						if (File.Exists(metaPath)) {
							// Read and update content if needed
							var metaContent = File.ReadAllText(metaPath);
							//bool metaContentChanged = false;

							if (ContainsKeywordCaseInsensitive(metaContent)) {
								var newMetaContent = Regex.Replace(
									metaContent,
									Regex.Escape(oldKeyword),
									match => ReplaceKeywordPreservingCase(match.Value),
									RegexOptions.IgnoreCase
								);

								if (newMetaContent != metaContent) File.WriteAllText(metaPath, newMetaContent);
								//metaContentChanged = true;
							}
						}

						// Make sure the parent directory exists
						var parentDirectory = Path.GetDirectoryName(replacement.NewPath);
						if (!Directory.Exists(parentDirectory)) Directory.CreateDirectory(parentDirectory);

						// Move the directory
						Directory.Move(replacement.Path, replacement.NewPath);

						// Move the .meta file to match the directory
						if (File.Exists(metaPath)) File.Move(metaPath, newMetaPath);
					}
				}
				catch (Exception ex) {
					Debug.LogError($"Error renaming directory {replacement.Path}: {ex.Message}");
				}
			}

			AssetDatabase.Refresh();
		}

		private enum ReplacementType {
			Directory,
			FileName,
			FileContent,
			AsmdefNameField,
		}

		private class ReplacementInfo {
			public ReplacementType Type { get; set; }
			public string Path { get; set; }
			public string OriginalName { get; set; }
			public string NewName { get; set; }
			public string NewPath { get; set; }
			public List<ContentReplacementInfo> ContentReplacements { get; set; } = new();
		}

		private class ContentReplacementInfo {
			public int LineNumber { get; set; }
			public string OriginalText { get; set; }
			public string ReplacementText { get; set; }
			public string Context { get; set; }
		}
	}
}
