using Microsoft.FxCop.Sdk;
using System;

namespace Gilmond.Helpers.CodeGate.FxCop
{
	abstract class FxCopRuleBase : BaseIntrospectionRule
	{
		static Type LocalType = typeof(FxCopRuleBase);

		protected FxCopRuleBase(string ruleName)
			: base(ruleName, $"{LocalType.Namespace}.RuleMetadata.xml", LocalType.Assembly)
		{ }
	}
}
