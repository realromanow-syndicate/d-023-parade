using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace PageHelpers.Editor {
	/// <summary>
	/// Unity Editor tool that adds utility code to C# files to create unique binary fingerprints
	/// </summary>
	public class CodeDiversity : EditorWindow {
		#region UI Fields
		private string _targetDirectory = "";
		private bool _processSubdirectories = true;
		private int _minUtilityClasses = 1;  // Уменьшаем минимум до 1
		private int _minUtilityMethods = 3;
		private int _maxUtilityMethods = 8;
		private int _minMethodCallsPerFile = 1; // Начинаем с минимального значения для более натурального кода
		private bool _isProcessing = false;
		private string _statusMessage = "";
		private float _progressValue = 0f;
		#endregion

		#region Internal Classes
		// Class that holds method signature information
		public class MethodSignature {
			public string Name { get; set; }
			public string ReturnType { get; set; }
			public List<ParameterSignature> Parameters { get; set; } = new List<ParameterSignature>();
			
			public string GetParameterList() {
				return string.Join(", ", Parameters.Select(p => p.ToString()));
			}
		}
		
		public class ParameterSignature {
			public string Name { get; set; }
			public string Type { get; set; }
			public string DefaultValue { get; set; }
			
			public override string ToString() {
				return DefaultValue != null ? $"{Type} {Name} = {DefaultValue}" : $"{Type} {Name}";
			}
		}
		
		// Track existing classes in namespaces to avoid duplicates
		private Dictionary<string, HashSet<string>> _existingClassesInNamespace = new Dictionary<string, HashSet<string>>();
		#endregion

		#region Code Generation Data
		// Constants for code generation
		private static readonly string[] DOMAINS = {
			"Data", "String", "Number", "Security", "Performance", "Object", 
			"Config", "Resource", "State", "Network", "Cache", "Memory",
			"Logic", "Hash", "Query", "Text", "Collection", "Validation"
		};
		
		private static readonly string[] OPERATIONS = {
			"Process", "Calculate", "Validate", "Format", "Transform", "Optimize",
			"Analyze", "Handle", "Generate", "Verify", "Track", "Monitor",
			"Initialize", "Prepare", "Compute", "Normalize", "Convert", "Manage"
		};
		
		private static readonly string[] SUBJECTS = {
			"Data", "Value", "String", "Result", "State", "Object", "Request", 
			"Response", "Context", "Buffer", "Content", "Parameters", "Config", 
			"Instance", "Collection", "Sequence", "Element", "Entry"
		};
		
		private static readonly string[] RETURN_TYPES = {
			"int", "bool", "string", "float", "double", "long"
		};
		
		// Storage for generated code tracking
		private Dictionary<string, List<MethodSignature>> _generatedMethods = new Dictionary<string, List<MethodSignature>>();
		private List<string> _generatedClassNames = new List<string>();
		#endregion

		#region Unity Editor Methods
		[MenuItem("Tools/Code Fingerprinter")]
		public static void ShowWindow() {
			GetWindow<CodeDiversity>("Code Fingerprinter");
		}

		private void OnGUI() {
			GUILayout.Label("Code Fingerprinting Tool", EditorStyles.boldLabel);
			
			EditorGUI.BeginDisabledGroup(_isProcessing);
			
			_targetDirectory = EditorGUILayout.TextField("Target Directory:", _targetDirectory);
			
			if (GUILayout.Button("Browse...", GUILayout.Width(100))) {
				string dir = EditorUtility.OpenFolderPanel("Select Target Directory", "", "");
				if (!string.IsNullOrEmpty(dir)) {
					_targetDirectory = dir;
				}
			}
			
			_processSubdirectories = EditorGUILayout.Toggle("Include Subdirectories:", _processSubdirectories);
			
			EditorGUILayout.Space();
			
			_minUtilityClasses = EditorGUILayout.IntSlider("Utility Classes:", _minUtilityClasses, 1, 3);
			
			_minUtilityMethods = EditorGUILayout.IntSlider("Methods Per Class:", _minUtilityMethods, 2, 8);
			_maxUtilityMethods = _minUtilityMethods + 2; // Небольшой разброс
			
			_minMethodCallsPerFile = EditorGUILayout.IntSlider("Method Calls Per File:", _minMethodCallsPerFile, 1, 5);
			
			EditorGUILayout.HelpBox("Lower values produce more natural-looking code.", MessageType.Info);
			
			EditorGUILayout.Space();
			
			if (GUILayout.Button("Process Files", GUILayout.Height(30))) {
				if (string.IsNullOrEmpty(_targetDirectory)) {
					EditorUtility.DisplayDialog("Error", "Please select a target directory", "OK");
				} else {
					StartProcessing();
				}
			}
			
			EditorGUI.EndDisabledGroup();
			
			if (_isProcessing) {
				EditorGUILayout.HelpBox(_statusMessage, MessageType.Info);
				EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(false, 20), _progressValue, $"{_progressValue * 100:F0}%");
			}
		}
		#endregion

		#region Processing Logic
		private void StartProcessing() {
			_isProcessing = true;
			_progressValue = 0f;
			_statusMessage = "Starting...";
			
			EditorApplication.delayCall += ProcessFiles;
		}
		
		private void ProcessFiles() {
			try {
				// Get all CS files
				List<string> csFiles = GetCSharpFiles();
				
				if (csFiles.Count == 0) {
					_statusMessage = "No C# files found in the specified directory.";
					_isProcessing = false;
					return;
				}
				
				int processedCount = 0;
				
				foreach (string filePath in csFiles) {
					_statusMessage = $"Processing {Path.GetFileName(filePath)} ({processedCount + 1}/{csFiles.Count})";
					_progressValue = (float)processedCount / csFiles.Count;
					
					try {
						ProcessSingleFile(filePath);
						processedCount++;
					}
					catch (Exception ex) {
						Debug.LogError($"Error processing file {filePath}: {ex.Message}");
					}
					
					// Update progress bar occasionally
					if (processedCount % 10 == 0) {
						EditorUtility.DisplayProgressBar("Processing Files", _statusMessage, _progressValue);
						Repaint();
					}
				}
				
				_statusMessage = $"Completed. {processedCount} files processed.";
				_progressValue = 1f;
			}
			catch (Exception ex) {
				_statusMessage = $"Error: {ex.Message}";
				Debug.LogError($"Processing error: {ex}");
			}
			finally {
				EditorUtility.ClearProgressBar();
				_isProcessing = false;
				Repaint();
			}
		}
		
		private List<string> GetCSharpFiles() {
			SearchOption option = _processSubdirectories ? 
				SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
			
			return Directory.GetFiles(_targetDirectory, "*.cs", option)
				.Where(file => ShouldProcessFile(file))
				.ToList();
		}
		
		private bool ShouldProcessFile(string filePath) {
			string fileName = Path.GetFileName(filePath);
			
			// Skip Unity system files, editor scripts, and test files
			if (fileName.StartsWith("Unity") || 
				fileName.Contains("Editor") || 
				fileName.EndsWith("Test.cs") || 
				fileName.EndsWith("Tests.cs") ||
				filePath.EndsWith("Unit.cs")) {
				return false;
			}
			
			return true;
		}
		
		private void ProcessSingleFile(string filePath) {
			// Read file
			string content = File.ReadAllText(filePath);
			
			// Skip special files
			if (content.Contains("interface ") || 
				content.Contains("partial class") || 
				IsGeneratedFile(content)) {
				return;
			}
			
			// Reset class tracking for this file
			_generatedClassNames.Clear();
			_generatedMethods.Clear();
			
			// Parse file and add utility code
			string namespaceName = ExtractNamespace(content);
			if (string.IsNullOrEmpty(namespaceName)) {
				return; // Skip if no namespace found
			}
			
			// Find all existing classes in this namespace
			ExtractExistingClasses(content, namespaceName);
			
			// Add required using statements
			//content = AddRequiredUsings(content);
			
			// Generate utility classes
			string utilityCode = GenerateUtilityClasses(namespaceName);
			
			// Add utility method calls to existing methods
			string modifiedContent = AddMethodCalls(content);
			
			// Insert utility classes into namespace
			string finalContent = InsertUtilityCode(modifiedContent, utilityCode);
			
			// Validate and save
			if (finalContent != content && IsValidCode(finalContent)) {
				File.WriteAllText(filePath, finalContent);
			}
		}
		
		private bool IsGeneratedFile(string content) {
			return content.Contains("[System.CodeDom.Compiler.GeneratedCode") ||
				   content.Contains("// <auto-generated>") ||
				   content.Contains("// This code was generated");
		}
		
		private string ExtractNamespace(string content) {
			Match match = Regex.Match(content, @"namespace\s+([^\s{;]+)");
			return match.Success ? match.Groups[1].Value : "";
		}
		
		private string AddRequiredUsings(string content) {
			// List of required namespaces
			string[] requiredNamespaces = {
				"System",
				"System.Collections.Generic"
			};
			
			StringBuilder newUsings = new StringBuilder();
			
			foreach (string ns in requiredNamespaces) {
				if (!content.Contains($"using {ns};")) {
					newUsings.AppendLine($"using {ns};");
				}
			}
			
			if (newUsings.Length == 0) {
				return content;
			}
			
			// Find where to insert the using statements
			Match lastUsing = Regex.Match(content, @"using [^;]+;(\r?\n|\r)(?!using)");
			if (lastUsing.Success) {
				int insertPos = lastUsing.Index + lastUsing.Length;
				return content.Insert(insertPos, newUsings.ToString());
			}
			
			// Insert at the beginning if no usings found
			return newUsings.ToString() + content;
		}
		#endregion
		
		#region Code Generation
		private string GenerateUtilityClasses(string namespaceName) {
			StringBuilder sb = new StringBuilder();
			
			// Determine number of utility classes to generate - теперь меньше
			int numClasses = _minUtilityClasses; // Используем минимальное значение
			
			for (int i = 0; i < numClasses; i++) {
				string className = GenerateUniqueClassName(namespaceName);
				
				sb.AppendLine($"\t/// <summary>");
				sb.AppendLine($"\t/// Utility functions for {className.Replace("Utility", "").Replace("Helper", "")} operations");
				sb.AppendLine($"\t/// </summary>");
				sb.AppendLine($"\tpublic static class {className} {{");
				
				// Add hash calculation helper method
				sb.AppendLine($"\t\tprivate static int CalculateHash(string input, int seed) {{");
				sb.AppendLine($"\t\t\tint hash = {UnityEngine.Random.Range(0, int.MaxValue)};");
				sb.AppendLine($"\t\t\tforeach (char c in input) {{");
				sb.AppendLine($"\t\t\t\thash = hash * {UnityEngine.Random.Range(0, int.MaxValue)} + c;");
				sb.AppendLine($"\t\t\t}}");
				sb.AppendLine($"\t\t\treturn hash + seed;");
				sb.AppendLine($"\t\t}}");
				sb.AppendLine();
				
				// Generate methods - больше методов в меньшем количестве классов
				int methodCount = UnityEngine.Random.Range(_minUtilityMethods, _maxUtilityMethods);
				HashSet<string> usedMethodNames = new HashSet<string>();
				
				for (int j = 0; j < methodCount; j++) {
					AddMethod(sb, usedMethodNames, className);
				}
				
				sb.AppendLine($"\t}}");
				sb.AppendLine();
			}
			
			return sb.ToString();
		}
		
		private string GenerateUniqueClassName(string namespaceName) {
			// Generate candidate names until we find a unique one
			int attempts = 0;
			string className;
			
			do {
				// Add variations to ensure uniqueness
				string domain = DOMAINS[UnityEngine.Random.Range(0, DOMAINS.Length)];
				
				// After some attempts, add more variation
				if (attempts > 5) {
					int randomSuffix = UnityEngine.Random.Range(1, 999);
					className = $"{domain}{(attempts % 2 == 0 ? "Utility" : "Helper")}{randomSuffix}";
				} else {
					className = $"{domain}{(attempts % 2 == 0 ? "Utility" : "Helper")}";
				}
				
				attempts++;
				
				// Prevent infinite loops
				if (attempts > 100) {
					className = $"Custom{UnityEngine.Random.Range(1000, 9999)}Utility";
					break;
				}
			} while (_generatedClassNames.Contains(className) || 
					(_existingClassesInNamespace.ContainsKey(namespaceName) && 
					 _existingClassesInNamespace[namespaceName].Contains(className)));
			
			_generatedClassNames.Add(className);
			
			// Also track it as existing to avoid future duplicates
			if (!_existingClassesInNamespace.ContainsKey(namespaceName)) {
				_existingClassesInNamespace[namespaceName] = new HashSet<string>();
			}
			_existingClassesInNamespace[namespaceName].Add(className);
			
			return className;
		}
		
		private void AddMethod(StringBuilder sb, HashSet<string> usedMethodNames, string className) {
			// Create method signature
			string operation = OPERATIONS[UnityEngine.Random.Range(0, OPERATIONS.Length)];
			string subject = SUBJECTS[UnityEngine.Random.Range(0, SUBJECTS.Length)];
			
			string methodName = $"{operation}{subject}";
			if (usedMethodNames.Contains(methodName)) {
				methodName = $"{operation}{subject}Extended";
			}
			usedMethodNames.Add(methodName);
			
			// Choose return type
			string returnType = RETURN_TYPES[UnityEngine.Random.Range(0, RETURN_TYPES.Length)];
			
			// Create method info
			MethodSignature methodSig = new MethodSignature {
				Name = methodName,
				ReturnType = returnType
			};
			
			// Always add string input parameter
			methodSig.Parameters.Add(new ParameterSignature { 
				Name = "input", 
				Type = "string" 
			});
			
			// Add appropriate second parameter
			if (returnType == "bool") {
				methodSig.Parameters.Add(new ParameterSignature { 
					Name = "defaultValue", 
					Type = "bool", 
					DefaultValue = "false" 
				});
			} else if (returnType == "int") {
				methodSig.Parameters.Add(new ParameterSignature { 
					Name = "modifier", 
					Type = "int", 
					DefaultValue = "1" 
				});
			} else if (returnType == "string") {
				methodSig.Parameters.Add(new ParameterSignature { 
					Name = "prefix", 
					Type = "string", 
					DefaultValue = "null" 
				});
			} else if (returnType == "float" || returnType == "double") {
				methodSig.Parameters.Add(new ParameterSignature { 
					Name = "factor", 
					Type = "double", 
					DefaultValue = "1.0" 
				});
			}
			
			// Generate comment with proper verb form
			string verbForm = GetVerbForm(operation);
			
			sb.AppendLine($"\t\t/// <summary>");
			sb.AppendLine($"\t\t/// {verbForm} the {subject.ToLower()} based on the provided input");
			sb.AppendLine($"\t\t/// </summary>");
			
			// Method signature
			sb.AppendLine($"\t\tpublic static {returnType} {methodName}({methodSig.GetParameterList()}) {{");
			
			// Method body
			sb.AppendLine($"\t\t\tif (string.IsNullOrEmpty(input)) {{");
			
			// Return statement for empty input
			if (returnType == "bool") {
				sb.AppendLine($"\t\t\t\treturn defaultValue;");
				sb.AppendLine($"\t\t\t}}");
				sb.AppendLine();
				sb.AppendLine($"\t\t\tint hash = CalculateHash(input, 0);");
				sb.AppendLine($"\t\t\treturn hash % 2 == 0;");
			} else if (returnType == "int") {
				sb.AppendLine($"\t\t\t\treturn 0;");
				sb.AppendLine($"\t\t\t}}");
				sb.AppendLine();
				sb.AppendLine($"\t\t\treturn CalculateHash(input, modifier);");
			} else if (returnType == "string") {
				sb.AppendLine($"\t\t\t\treturn string.Empty;");
				sb.AppendLine($"\t\t\t}}");
				sb.AppendLine();
				sb.AppendLine($"\t\t\tstring result = input;");
				sb.AppendLine($"\t\t\tif (prefix != null) {{");
				sb.AppendLine($"\t\t\t\tresult = prefix + result;");
				sb.AppendLine($"\t\t\t}}");
				sb.AppendLine($"\t\t\treturn result;");
			} else if (returnType == "float") {
				sb.AppendLine($"\t\t\t\treturn 0f;");
				sb.AppendLine($"\t\t\t}}");
				sb.AppendLine();
				sb.AppendLine($"\t\t\tint hash = CalculateHash(input, 0);");
				sb.AppendLine($"\t\t\treturn (float)(hash % 1000) / (float)factor;");
			} else if (returnType == "double") {
				sb.AppendLine($"\t\t\t\treturn 0.0;");
				sb.AppendLine($"\t\t\t}}");
				sb.AppendLine();
				sb.AppendLine($"\t\t\tint hash = CalculateHash(input, 0);");
				sb.AppendLine($"\t\t\treturn (double)(hash % 1000) / factor;");
			} else {
				sb.AppendLine($"\t\t\t\treturn 0;");
				sb.AppendLine($"\t\t\t}}");
				sb.AppendLine();
				sb.AppendLine($"\t\t\treturn CalculateHash(input, 0);");
			}
			
			sb.AppendLine($"\t\t}}");
			sb.AppendLine();
			
			// Track this method
			if (!_generatedMethods.ContainsKey(className)) {
				_generatedMethods[className] = new List<MethodSignature>();
			}
			_generatedMethods[className].Add(methodSig);
		}
		
		private string GetVerbForm(string verb) {
			// Conjugate verb to third person singular form
			if (verb.EndsWith("y")) {
				return verb.Substring(0, verb.Length - 1) + "ies";
			} else if (verb.EndsWith("s") || verb.EndsWith("x") || verb.EndsWith("z") || 
			           verb.EndsWith("ch") || verb.EndsWith("sh")) {
				return verb + "es";
			} else {
				return verb + "s";
			}
		}
		
		private string AddMethodCalls(string content) {
			if (_generatedClassNames.Count == 0 || _generatedMethods.Count == 0) {
				return content;
			}
			
			// Найдем все методы в коде
			string methodPattern = @"(\s*)((?:public|private|protected|internal)(?:\s+static)?)\s+(\w+)\s+(\w+)\s*\([^{]*\)\s*{";
			MatchCollection methodMatches = Regex.Matches(content, methodPattern);
			
			if (methodMatches.Count == 0) {
				return content; // Нет методов для модификации
			}
			
			// Соберем подходящие методы (фильтруем по имени и другим критериям)
			List<Match> eligibleMethods = new List<Match>();
			foreach (Match match in methodMatches) {
				string methodName = match.Groups[4].Value;
				if (!ShouldSkipMethod(methodName)) {
					eligibleMethods.Add(match);
				}
			}
			
			if (eligibleMethods.Count == 0) {
				return content; // Нет подходящих методов
			}
			
			// Получим доступные классы с методами
			List<string> eligibleClasses = _generatedClassNames
				.Where(c => _generatedMethods.ContainsKey(c) && _generatedMethods[c].Count > 0)
				.ToList();
				
			if (eligibleClasses.Count == 0) {
				return content; // Нет доступных классов
			}
			
			// Создаем копию контента для модификации
			StringBuilder newContent = new StringBuilder(content);
			
			// Определяем, сколько методов модифицировать
			int methodsToModify = Math.Min(eligibleMethods.Count, _minMethodCallsPerFile);
			
			// Отсортируем методы по порядку их появления в коде (по индексу)
			eligibleMethods.Sort((a, b) => a.Index.CompareTo(b.Index));
			
			// Отслеживание смещения для вставок
			int offset = 0;
			
			// Обходим выбранные методы
			for (int i = 0; i < methodsToModify; i++) {
				Match methodMatch = eligibleMethods[i];
				string indent = methodMatch.Groups[1].Value; // Получаем отступ
				
				// Находим индекс открывающей скобки метода с учетом смещения
				int openingBraceIndex = content.IndexOf('{', methodMatch.Index) + offset;
				
				// Если у нас есть еще один символ, то учтем, может там перенос строки
				string lineBreak = "";
				if (openingBraceIndex + 1 < newContent.Length) {
					char nextChar = newContent[openingBraceIndex + 1];
					if (nextChar != '\r' && nextChar != '\n') {
						lineBreak = Environment.NewLine;
					}
				}
				
				// Выбираем случайный класс и метод
				string className = eligibleClasses[UnityEngine.Random.Range(0, eligibleClasses.Count)];
				MethodSignature methodSig = _generatedMethods[className][
					UnityEngine.Random.Range(0, _generatedMethods[className].Count)
				];
				
				// Формируем параметры вызова
				StringBuilder parameters = new StringBuilder();
				parameters.Append($"\"{methodMatch.Groups[4].Value}\""); // Первый параметр - имя метода + суффикс
				
				// Добавляем второй параметр если нужно
				if (methodSig.Parameters.Count > 1) {
					var secondParam = methodSig.Parameters[1];
					
					if (secondParam.Type == "bool") {
						parameters.Append(", false");
					} else if (secondParam.Type == "int") {
						parameters.Append($", {UnityEngine.Random.Range(1, 100)}");
					} else if (secondParam.Type == "string") {
						parameters.Append(", null");
					} else if (secondParam.Type == "double") {
						parameters.Append($", {Math.Round(UnityEngine.Random.Range(1f, 10f), 1)}");
					}
				}
				
				// Генерируем строку вызова с правильным отступом
				string call = $"{lineBreak}{indent}\t{className}.{methodSig.Name}({parameters});{Environment.NewLine}";
				
				// Вставляем вызов после открывающей скобки метода
				newContent.Insert(openingBraceIndex + 1, call);
				
				// Обновляем смещение для следующих вставок
				offset += call.Length;
			}
			
			return newContent.ToString();
		}
		
		private bool ShouldSkipMethod(string methodName) {
			return methodName.StartsWith("get_") || 
				   methodName.StartsWith("set_") || 
				   methodName == "Start" || 
				   methodName == "Update" || 
				   methodName == "Awake" || 
				   methodName == "OnEnable" || 
				   methodName == "OnDisable" || 
				   !char.IsUpper(methodName[0]); // Skip methods not starting with uppercase
		}
		
		private string InsertUtilityCode(string content, string utilityCode) {
			// Find namespace opening
			string pattern = @"(namespace\s+[^\s{;]+\s*{)";
			Match match = Regex.Match(content, pattern);
			
			if (match.Success) {
				int insertPosition = match.Index + match.Length;
				
				// Make sure we add a line break
				string lineBreak = Environment.NewLine;
				return content.Substring(0, insertPosition) + lineBreak + lineBreak + 
					   utilityCode + content.Substring(insertPosition);
			}
			
			return content;
		}
		
		private bool IsValidCode(string content) {
			// Basic validation checks
			int openBraces = content.Count(c => c == '{');
			int closeBraces = content.Count(c => c == '}');
			
			if (openBraces != closeBraces) {
				return false;
			}
			
			// Check for broken namespace declarations
			foreach (Match match in Regex.Matches(content, @"namespace\s+([^{;]+)")) {
				string namespaceName = match.Groups[1].Value.Trim();
				if (namespaceName.EndsWith(".") || namespaceName.Contains("..")) {
					return false;
				}
			}
			
			return true;
		}
		#endregion
		
		private void ExtractExistingClasses(string content, string namespaceName) {
			// Create set for this namespace if it doesn't exist
			if (!_existingClassesInNamespace.ContainsKey(namespaceName)) {
				_existingClassesInNamespace[namespaceName] = new HashSet<string>();
			}
			
			// Extract class names from the file
			string pattern = @"class\s+(\w+)";
			MatchCollection matches = Regex.Matches(content, pattern);
			
			foreach (Match match in matches) {
				string className = match.Groups[1].Value;
				_existingClassesInNamespace[namespaceName].Add(className);
			}
		}
	}
}
