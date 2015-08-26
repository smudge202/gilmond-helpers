using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using TestAttributes;
using TestHelper;

namespace CodeGate.Analyzer.Test
{
	public class NoRefsTests : CodeFixVerifier
	{
		[Unit]
		public void WhenNoCodeProvidedThenDoesNotGenerateDiagnostics()
		{
			var test = @"";
			VerifyCSharpDiagnostic(test);
		}


		[Unit]
		public void WhenMethodsDoNotContainRefParametersThenDoesNotGenerateDiagnostics()
		{
			var test = @"
	namespace CodeGate.Analyzer.Test
	{
		sealed class TypeName
		{
			void MethodName(string argument) { }
		}
	}";
			VerifyCSharpDiagnostic(test);
		}

		[Unit]
		public void WhenMethodContainsRefParameterThenGeneratesDiagnosticMessage()
		{
			var test = @"
	namespace CodeGate.Analyzer.Test
	{
		sealed class TypeName
		{   
			void MethodName(ref string argument) { }
		}
	}";
			var expected = new DiagnosticResult
			{
				Id = DiagnosticIds.NoRefs,
				Message = string.Format("The ref keyword on parameter '{0}' is likely a lazy way of extending the result set of the method.", "argument"),
				Severity = DiagnosticSeverity.Warning,
				Locations = new[]
				{
					new DiagnosticResultLocation("Test0.cs", 6, 13)
				}
			};

			VerifyCSharpDiagnostic(test, expected);
		}

		[Unit]
		public void WhenRefParameterIsNotUsedByInnerNorOuterThenRemovesParameter()
		{
			var test = @"
	namespace CodeGate.Analyzer.Test
	{
		sealed class TypeName
		{   
			void MethodName(ref string argument) { }
		}
	}";

			var fixtest = @"
	namespace CodeGate.Analyzer.Test
	{
		sealed class TypeName
		{   
			void MethodName() { }
		}
	}";
			VerifyCSharpFix(test, fixtest);
		}

		[Unit]
		public void WhenRefParameterIsReadByInnerThenOnlyRemovesRefModifier()
		{
			var test = @"
	using System;
	namespace CodeGate.Analyzer.Test
	{
		sealed class TypeName
		{   
			void MethodName(ref string argument) 
			{ 
				Console.WriteLine(argument);
			}
		}
	}";

			var fixtest = @"
	using System;
	namespace CodeGate.Analyzer.Test
	{
		sealed class TypeName
		{   
			void MethodName(string argument)
			{ 
				Console.WriteLine(argument);
			}
		}
	}";
			VerifyCSharpFix(test, fixtest);
		}

		protected override CodeFixProvider GetCSharpCodeFixProvider()
		{
			return new NoRefsCodeFixProvider();
		}

		protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
		{
			return new NoRefsAnalyzer();
		}
	}
}