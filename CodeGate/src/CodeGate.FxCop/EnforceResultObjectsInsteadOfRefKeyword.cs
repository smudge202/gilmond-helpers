using Microsoft.FxCop.Sdk;
using System.Linq;

namespace Gilmond.Helpers.CodeGate.FxCop
{
	sealed class EnforceResultObjectsInsteadOfRefKeyword : FxCopRuleBase
	{
		public EnforceResultObjectsInsteadOfRefKeyword()
			: base(nameof(EnforceResultObjectsInsteadOfRefKeyword))
		{ }

		public override ProblemCollection Check(Member member)
		{
			var method = member as Method;
			if (method == null) return Problems;
			var methodInfo = method.ToMethodInfo();
			if (methodInfo == null) return Problems;

			var parameters = methodInfo.GetParameters();
			foreach (var parameter in parameters.Where(x => x.ParameterType.IsByRef))
				if (method.ReturnType.IsVoid())
					AddProblem("Introduce", parameter.Name);
				else
					AddProblem("Extend", parameter.Name);

			return Problems;
		}

		private void AddProblem(string resolutionName, string parameterName)
		{
			Problems.Add(new Problem(GetNamedResolution(resolutionName, parameterName)));
		}
	}
}
