using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

namespace dlobo.Seek
{
	public static class SeekWindow_Reflection
	{
		private static Dictionary<Type, IEnumerable<FieldInfo>> cacheForFieldsOfType = new Dictionary<Type, IEnumerable<FieldInfo>>();
		private static Dictionary<Type, IEnumerable<PropertyInfo>> cacheForPropertiesOfType = new Dictionary<Type, IEnumerable<PropertyInfo>>();

		public static void ChangeEditorWindowIcon(EditorWindow editorWindow, Texture icon)
		{
			// only makes sense before Unity 5.3. Afterwards you can use titleContent directly
			BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetField;
			PropertyInfo titleInfo = typeof(EditorWindow).GetProperty("cachedTitleContent", flags);
			if (titleInfo != null) {
				var titleContent = (GUIContent) titleInfo.GetValue(editorWindow, null);
				if (titleContent != null) {
					titleContent.image = icon;
				}
			}
		}

		public static List<Result> DoIntrospectionSearchOnPrefabs(List<Result> results, SearchConfig config)
		{
			Regex componentNameRegex = null;
			if (config.componentName.MatchingType == MatchingType.Regex) {
				componentNameRegex = new Regex(config.componentName.String, config.componentName.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
			}
			Regex variableNameRegex = null;
			if (config.variableName.MatchingType == MatchingType.Regex) {
				variableNameRegex = new Regex(config.variableName.String, config.variableName.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
			}
			Regex variableValueRegex = null;
			if (config.variableValue.MatchingType == MatchingType.Regex) {
				variableValueRegex = new Regex(config.variableValue.String, config.variableValue.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
			}

			const string nullString = "null";

			BindingFlags flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static;

			var newResults = new List<Result>();

			// float time = Time.realtimeSinceStartup;

			foreach (Result result in results)
			{
				GameObject asset = (GameObject) AssetDatabase.LoadAssetAtPath(result.FullPath, typeof(GameObject));
				Component[] components;
				if (config.doSearchChildren) {
					components = asset.GetComponentsInChildren<Component>(true);
				} else {
					components = asset.GetComponents<Component>();
				}
				bool resultMatches = false;

				foreach (Component component in components)
				{
					if (component == null) {
						continue;
					}

					Type componentType = component.GetType();
					string componentTypeString = componentType.ToString();

					bool isMatch = false;

					switch (config.componentName.MatchingType)
					{
						case MatchingType.Exact:
							isMatch = componentTypeString.IsMatch_Exact(config.componentName.String, !config.componentName.IsCaseSensitive);
							break;
						case MatchingType.Greedy:
							isMatch = componentTypeString.IsMatch_Greedy(config.componentName.String, !config.componentName.IsCaseSensitive);
							break;
						case MatchingType.Regex:
							isMatch = componentNameRegex.Match(componentTypeString).Success;
							break;
					}

					if (isMatch)
					{
						IEnumerable<FieldInfo> fields;
						if (!cacheForFieldsOfType.TryGetValue(componentType, out fields))
						{
							var fieldsList = new List<FieldInfo>();

							foreach (var field in componentType.GetFields(flags))
							{
								if (!isSearchable(field.FieldType)) {
									continue;
								}

								fieldsList.Add(field);
							}

							fields = cacheForFieldsOfType[componentType] = fieldsList;
						}

						foreach (var field in fields)
						{
							switch (config.variableName.MatchingType)
							{
								case MatchingType.Exact:
									isMatch = field.Name.IsMatch_Exact(config.variableName.String, !config.variableName.IsCaseSensitive);
									break;
								case MatchingType.Greedy:
									isMatch = field.Name.IsMatch_Greedy(config.variableName.String, !config.variableName.IsCaseSensitive);
									break;
								case MatchingType.Regex:
									isMatch = variableNameRegex.Match(field.Name).Success;
									break;
							}

							if (!isMatch) {
								continue;
							}

							string fieldValue;
							object val = field.GetValue(component);
							fieldValue = val != null ? val.ToString() : nullString;

							switch (config.variableValue.MatchingType)
							{
								case MatchingType.Exact:
									isMatch = fieldValue.IsMatch_Exact(config.variableValue.String, !config.variableValue.IsCaseSensitive);
									break;
								case MatchingType.Greedy:
									isMatch = fieldValue.IsMatch_Greedy(config.variableValue.String, !config.variableValue.IsCaseSensitive);
									break;
								case MatchingType.Regex:
									isMatch = variableValueRegex.Match(fieldValue).Success;
									break;
							}

							if (isMatch) {
								newResults.Add(result);
								resultMatches = true;
								break;
							}
						}

						if (resultMatches) {
							break;
						}

						IEnumerable<PropertyInfo> properties;
						if (!cacheForPropertiesOfType.TryGetValue(componentType, out properties))
						{
							var propertiesList = new List<PropertyInfo>();

							foreach (var field in componentType.GetProperties(flags))
							{
								if (!isSearchable(field.PropertyType)) {
									continue;
								}

								// skip properties that generate console logs on access
								if (isUglyLoggerProperty(componentType, field)) {
									continue;
								}

								propertiesList.Add(field);
							}

							properties = cacheForPropertiesOfType[componentType] = propertiesList;
						}

						foreach (var field in properties)
						{
							// if (component is Animator) {
							// 	Debug.Log(asset.name + " " + component + " " + field.Name);
							// }

							switch (config.variableName.MatchingType)
							{
								case MatchingType.Exact:
									isMatch = field.Name.IsMatch_Exact(config.variableName.String, !config.variableName.IsCaseSensitive);
									break;
								case MatchingType.Greedy:
									isMatch = field.Name.IsMatch_Greedy(config.variableName.String, !config.variableName.IsCaseSensitive);
									break;
								case MatchingType.Regex:
									isMatch = variableNameRegex.Match(field.Name).Success;
									break;
							}

							if (!isMatch) {
								continue;
							}

							string fieldValue;
							object val = field.GetValue(component, null);
							fieldValue = val != null ? val.ToString() : nullString;

							switch (config.variableValue.MatchingType)
							{
								case MatchingType.Exact:
									isMatch = fieldValue.IsMatch_Exact(config.variableValue.String, !config.variableValue.IsCaseSensitive);
									break;
								case MatchingType.Greedy:
									isMatch = fieldValue.IsMatch_Greedy(config.variableValue.String, !config.variableValue.IsCaseSensitive);
									break;
								case MatchingType.Regex:
									isMatch = variableValueRegex.Match(fieldValue).Success;
									break;
							}

							if (isMatch) {
								newResults.Add(result);
								resultMatches = true;
								break;
							}
						}
					}

					if (resultMatches) {
						break;
					}
				}
			}

			// Debug.Log(Time.realtimeSinceStartup - time);

			return newResults;
		}

		private static bool isSearchable(Type type)
		{
			return (
				   type == typeof(float)
				|| type == typeof(int)
				|| type == typeof(uint)
				|| type == typeof(bool)
				|| type == typeof(long)
				|| type == typeof(ulong)
				|| type == typeof(string)
				|| type == typeof(byte)
				|| type == typeof(sbyte)
				|| type == typeof(short)
				|| type == typeof(ushort)
				|| type == typeof(double)
				|| type == typeof(decimal)
				|| type == typeof(Vector2)
				|| type == typeof(Vector3)
			);
		}

		private static bool isUglyLoggerProperty(Type componentType, PropertyInfo property)
		{
			return false
				|| (componentType == typeof(UnityEngine.Animator)        && property.Name == "playbackTime")
				|| (componentType == typeof(UnityEngine.Animator)        && property.Name == "bodyPosition")
				|| (componentType == typeof(UnityEngine.AudioSource)     && property.Name == "maxVolume")
				|| (componentType == typeof(UnityEngine.AudioSource)     && property.Name == "minVolume")
				|| (componentType == typeof(UnityEngine.AudioSource)     && property.Name == "rolloffFactor")
				|| (componentType == typeof(UnityEngine.Rigidbody)       && property.Name == "sleepAngularVelocity")
				|| (componentType == typeof(UnityEngine.Rigidbody)       && property.Name == "sleepVelocity")
				|| (componentType == typeof(UnityEngine.Rigidbody)       && property.Name == "useConeFriction")
				#if UNITY_5_5_OR_NEWER
				|| (componentType == typeof(UnityEngine.AI.NavMeshAgent) && property.Name == "remainingDistance")
				|| (componentType == typeof(UnityEngine.AI.NavMeshAgent) && property.Name == "isStopped")
				#else
				|| (componentType == typeof(UnityEngine.NavMeshAgent)    && property.Name == "remainingDistance")
				|| (componentType == typeof(UnityEngine.NavMeshAgent)    && property.Name == "pathEndPosition")
				#endif
				#if UNITY_5_3_OR_NEWER
				|| (componentType == typeof(UnityEngine.Animator)        && property.Name == "layerCount")
				|| (componentType == typeof(UnityEngine.Animator)        && property.Name == "parameterCount")
				#endif
			;
		}
	}
}
