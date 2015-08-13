using Microsoft.FxCop.Sdk;
using System;
using System.Reflection;

namespace Gilmond.Helpers.CodeGate.FxCop
{
	static class ReflectionHelpers
	{
		public static MethodInfo ToMethodInfo(this Method method)
		{
			if (method.NodeType == NodeType.InstanceInitializer) return null;
			var declaringType = Type.GetType(method.DeclaringType.FullName, false, false);
			return declaringType?.GetMethod(method.Name.Name, method.GetBindingFlags());
		}

		public static bool IsVoid(this TypeNode typeNode)
		{
			return typeNode.FullName == typeof(void).FullName;
		}

		public static BindingFlags GetBindingFlags(this Method method)
		{
			var result = method.IsPublic ? BindingFlags.Public : BindingFlags.NonPublic;
			result = result | (method.IsStatic ? BindingFlags.Static : BindingFlags.Instance);
			return result | BindingFlags.DeclaredOnly;
		}
	}
}
