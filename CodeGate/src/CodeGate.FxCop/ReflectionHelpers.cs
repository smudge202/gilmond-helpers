using Microsoft.FxCop.Sdk;
using System;
using System.Linq;
using System.Reflection;

namespace Gilmond.Helpers.CodeGate.FxCop
{
	static class ReflectionHelpers
	{
		public static MethodInfo ToMethodInfo(this Method method)
		{
			if (method.NodeType == NodeType.InstanceInitializer) return null;
			var declaringType = method.DeclaringType.GetSystemType();
			return declaringType?.GetMethod(method.Name.Name, method.GetBindingFlags(), Type.DefaultBinder, method.GetParameterTypes(), null);
		}

		public static bool IsVoid(this TypeNode typeNode)
		{
			return typeNode.FullName == typeof(void).FullName;
		}

		private static BindingFlags GetBindingFlags(this Method method)
		{
			var result = method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
			result = result | (method.IsStatic ? BindingFlags.Static : BindingFlags.Instance);
			return result | BindingFlags.DeclaredOnly;
		}

		private static Type[] GetParameterTypes(this Method method)
		{
			return method.Parameters.Select(x => x.Type.GetSystemType()).ToArray();
		}

		private static Type GetSystemType(this TypeNode typeNode)
		{
			return Type.GetType(typeNode.FullName, ResolveAssembly, ResolveType, true, false);
		}

		private static Assembly ResolveAssembly(AssemblyName assemblyName)
		{
			return null;
		}

		private static Type ResolveType(Assembly assembly, string typeName, bool ignoreCase)
		{
			var isByRef = typeName.EndsWith("@");
			typeName = typeName.Replace("@", "");
			var result = Type.GetType(typeName, false, ignoreCase);
			if (result == null)
				if (assembly != null)
					result = assembly.GetType(typeName);
				else
					result = ScanAssembliesForType(typeName, ignoreCase);
			if (isByRef)
				return result.MakeByRefType();
			return result;
		}

		private static Type ScanAssembliesForType(string typeName, bool ignoreCase)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
				if (assembly.GetType(typeName, false, ignoreCase) != null)
					return assembly.GetType(typeName, true, ignoreCase);
			throw new TypeLoadException($"{typeName} could not be found.");
		}
	}
}
