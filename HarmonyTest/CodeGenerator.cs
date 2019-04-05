using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CSharp;

namespace HarmonyTest
{
    public class CodeGenerator
    {
        public static dynamic Gernerate()
        {
            var param = new CompilerParameters
            {
                GenerateExecutable = false,
                IncludeDebugInformation = false,
                GenerateInMemory = true
            };
            param.ReferencedAssemblies.Add("System.dll");
            param.ReferencedAssemblies.Add("System.Xml.dll");
            param.ReferencedAssemblies.Add("System.Data.dll");
            param.ReferencedAssemblies.Add("System.Core.dll");
            param.ReferencedAssemblies.Add("System.Xml.Linq.dll");

            var codeProvider = new CSharpCodeProvider();
            var results = codeProvider.CompileAssemblyFromSource(param, Code());

            if (results.Errors.HasErrors)
            {
                foreach (var error in results.Errors)
                {
                    Console.WriteLine(error);
                }

                throw new Exception("CodeGen Failed!");
            }

            return results.CompiledAssembly.CreateInstance("HarmonyTest.Ocl");


            // o.Prefix(5);

            //var o = results.CompiledAssembly.GetType("HarmonyTest.Ocl").GetMethod("Prefix");
            // return o;
        }

        private static string Code()
        {
            return @"using System;
                    namespace HarmonyTest
                    {
                        public class Ocl
                        {
                            public static void Prefix(int number) { Console.WriteLine(""CodeGenerated : number is : "" + number + "": CodeGenerated""); }
                        }
                    }";
        }
    }
}
