using System;
using System.Collections.Generic;
using System.Text;
using System.CodeDom.Compiler;

namespace ParacletusConsole
{
	class EmbeddedScriptingException: Exception
	{
		public CompilerErrorCollection Errors;

		public EmbeddedScriptingException(CompilerErrorCollection errors)
		{
			Errors = errors;
		}
	}
}
