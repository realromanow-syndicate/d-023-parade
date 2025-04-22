using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PageHelpers.Jester.DI.App {
	public class JesterComponentsRegistry {
		private readonly List<object> _items = new();
		
		public object[] items => _items.ToArray();

		public T Instantiate<T> (params object[] args) where T : class {
			var constructorParams = GetParametersInfoArray<T>();
			var requiredTypes = GetRequiredTypesArray<T>(constructorParams);
			var types = requiredTypes as Type[] ?? requiredTypes.ToArray();
			var requiredInstances = GetRequiredInstancesList<T>(types);

			args = ConcatArgs(args, requiredInstances);

			var sortedArgsArray = GetSortedArgsArray<T>(args, types);
			var instance = GetInstance<T>(args, sortedArgsArray);

			AddItem(instance);

			return GetInstanceAsGeneric<T>(instance);
		}

		private static T GetInstanceAsGeneric<T> (object instance) where T : class {
			return instance as T;
		}

		private T AddItem<T> (T instance) where T : class {
			_items.Add(instance);

			return instance;
		}

		private static T GetInstance<T> (object[] args, object[] sortedArgsArray) where T : class {
			return args.Any()
				? Activator.CreateInstance(typeof(T), sortedArgsArray) as T
				: Activator.CreateInstance(typeof(T)) as T;
		}

		private static object[] GetSortedArgsArray<T> (object[] args, IEnumerable<Type> requiredTypes) where T : class {
			return requiredTypes
				.Select(type => args.First(type.IsInstanceOfType))
				.ToArray();
		}

		private static object[] ConcatArgs (IEnumerable<object> args, List<object> requiredInstances) {
			return args.Concat(requiredInstances).ToArray();
		}

		private List<object> GetRequiredInstancesList<T> (IEnumerable<Type> requiredTypes) where T : class {
			return _items
				.Where(x => requiredTypes.Any(type => type.IsInstanceOfType(x)))
				.ToList();
		}

		private static IEnumerable<Type> GetRequiredTypesArray<T> (ParameterInfo[] constructorParams) where T : class {
			return constructorParams
				.Select(x => x.ParameterType);
		}

		private static ParameterInfo[] GetParametersInfoArray<T> () where T : class {
			return typeof(T)
				.GetConstructors()
				.First()
				.GetParameters();
		}

		public T Return<T> () where T : class {
			return _items
				.OfType<T>()
				.SingleOrDefault();
		}

		public T[] ReturnAllOfType<T> () where T : class {
			return _items
				.OfType<T>()
				.ToArray();
		}

		public void Registry<T> (T item) where T : class {
			_items.Add(item);
		}
	}
}
