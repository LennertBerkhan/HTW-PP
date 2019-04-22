using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Harmony;

namespace HarmonyBridge
{
    public class CodeGenerator
    {
        public struct Options
        {
            public readonly System.Type Context;
            public readonly string ClassName;
            public readonly string BeforeCode;
            public readonly string AfterCode;
            public readonly string HookedFuncName;

            public Options(string className, System.Type ctx, string hookedFuncName,
                string beforeCode, string afterCode)
            {
                Context = ctx;
                ClassName = className;
                BeforeCode = beforeCode;
                AfterCode = afterCode;
                HookedFuncName = hookedFuncName;
            }
        }

        private readonly Options _options;
        private readonly dynamic _runtimeCode;

        public CodeGenerator(Options options)
        {
            _options = options;

            // Console.WriteLine(GenerateCodeString());
            var param = new CompilerParameters
            {
                GenerateExecutable = false,
                IncludeDebugInformation = true,
                GenerateInMemory = true
            };
            param.ReferencedAssemblies.Add("System.dll");
            param.ReferencedAssemblies.Add("System.Xml.dll");
            param.ReferencedAssemblies.Add("System.Data.dll");
            param.ReferencedAssemblies.Add("System.Core.dll");
            param.ReferencedAssemblies.Add("System.Xml.Linq.dll");
            param.ReferencedAssemblies.Add("0Harmony.dll");
            param.ReferencedAssemblies.Add(typeof(HarmonyInstance).Assembly.Location);
            param.ReferencedAssemblies.Add("Microsoft.CSharp.dll");
            // param.ReferencedAssemblies.Add("Designer.dll"); // new System.Uri(System.Reflection.Assembly.GetExecutingAssembly().EscapedCodeBase).LocalPath);

            var codeProvider = new CSharpCodeProvider();
            var code = GenerateCodeString();
            var results = codeProvider.CompileAssemblyFromSource(param, code);

            if (!results.Errors.HasErrors)
                // _runtimeCode = results.CompiledAssembly.CreateInstance(_options.Context.Namespace + "." + _options.Context.Name).GetType();
                _runtimeCode =
                    results.CompiledAssembly.GetType("HookClass_" + _options.Context.Namespace + "." +
                                                     _options.ClassName);
            else
            {
                foreach (var error in results.Errors)
                    Console.WriteLine(error);
                throw new Exception("CodeGen failed!");
            }
            ResolveEventHandler @object = (object obj, ResolveEventArgs args) => results.CompiledAssembly;
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += @object.Invoke;
        }

        private string getMethodArgumentsList()
        {
            var original = _options.Context.GetMethod(_options.HookedFuncName);
            string args = "";
            foreach (var pi in original.GetParameters())
            {
                var pt = pi.ParameterType.ToString();
                if (!pt.StartsWith("System.Int"))
                    pt = "dynamic";
                args += ", " + pt + " " + pi.Name;
            }
            return args;
        }

        public void InvokeApplyMethod()
        {
            _runtimeCode.GetMethod("Apply").Invoke(null, new object[]
            {
                _options.Context
            });
        }

        private string GenerateCodeString()
        {
            var funcArgsAddStr = getMethodArgumentsList();
            return @"
using Harmony;
using System;
using System.Reflection;
using System.Collections.Generic;
namespace HookClass_" + _options.Context.Namespace + @"
{
public class " + _options.ClassName + @"
    {
        public static void Apply(System.Type ctx)
        {
            if (ctx == null)
                throw new Exception(""[" + _options.ClassName + @"] ctx is null!"");
            var harmony = HarmonyInstance.Create(""" + _options.ClassName + @""");
            var original = ctx.GetMethod(""" + _options.HookedFuncName + @""");
            if (original == null) 
                throw new Exception(""[" + _options.ClassName + @"] original method == null."");
            var prefix = typeof(" + _options.ClassName + @").GetMethod(""BeforeCall"");
            var postfix = typeof(" + _options.ClassName + @").GetMethod(""AfterCall"");
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
            if (!harmony.HasAnyPatches(""" + _options.ClassName + @"""))
                throw new Exception(""[" + _options.ClassName + @"] applying hook failed."");
        }

        public static void BeforeCall(object __instance" + funcArgsAddStr + @")
        {
            var self = __instance;
            if (!(" + _options.BeforeCode + @"))
            {
                // var col = Console.ForegroundColor
                // Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(""Planning Error (beforeCall): " + _options.ClassName + @"."");
                // Console.ForegroundColor = col;
            }
        }
        public static void AfterCall(object __instance" + funcArgsAddStr + @")
        {
            var self = __instance;

            if (!(" + _options.AfterCode + @"))
            {
                // var col = Console.ForegroundColor
                // Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(""Planning Error (afterCall): " + _options.ClassName + @"."");
                // Console.ForegroundColor = col;
            }
        }
        public static dynamic GetValue(dynamic instance, string variableName)
        {
            PropertyInfo prop = instance.GetType().GetProperty(variableName,
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            var methInf = prop.GetGetMethod(nonPublic: true);
            var workload = methInf.Invoke(instance, null);
            return workload;
        }
    }
}
            ";
        }
    }
}