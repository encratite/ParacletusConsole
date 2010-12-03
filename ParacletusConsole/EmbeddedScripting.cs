using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.CodeDom.Compiler;

namespace ParacletusConsole
{
	class EmbeddedScripting
	{
		Microsoft.CSharp.CSharpCodeProvider CodeProvider;
		CompilerParameters Parameters;

		public EmbeddedScripting()
		{
			CodeProvider = new Microsoft.CSharp.CSharpCodeProvider();
			Parameters = new CompilerParameters();
			Parameters.GenerateExecutable = false;
			Parameters.GenerateInMemory = true;
			Parameters.ReferencedAssemblies.Add(Assembly.GetExecutingAssembly().Location);
		}

		public CompilerResults Compile(string code)
		{
			return CodeProvider.CompileAssemblyFromSource(Parameters, code);
		}
	}
}
