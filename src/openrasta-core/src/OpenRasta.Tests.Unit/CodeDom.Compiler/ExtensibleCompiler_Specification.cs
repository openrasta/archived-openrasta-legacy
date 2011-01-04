using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using OpenRasta.CodeDom.Compiler;
using OpenRasta.Testing;

namespace ExtensibleCompiler_Specification
{
    public class when_dependency_manager_is_not_available : context
    {
        [Test]
        public void there_are_no_snippet_modifiers()
        {
            ExtensibleCSharpCodeProvider.SnippetModifiers.Count().ShouldBe(0);
        }
    }
}
