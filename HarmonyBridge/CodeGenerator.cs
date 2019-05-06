﻿using System;
using System.CodeDom.Compiler;
using Microsoft.CSharp;
using Harmony;
using System.Reflection;

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

        public CodeGenerator(string className, System.Type ctx, Aspect aspect) : this(new Options(className, ctx,
            aspect.FunctionName, aspect.BeforeCode, aspect.AfterCode))
        {
        }

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
            {
                _assembly = results.CompiledAssembly;
                _instance = _assembly.CreateInstance("HookClass_" + _options.Context.Namespace + "." +
                                                     _options.ClassName);
                if (_instance != null) _runtimeCode = _instance.GetType();
                //_runtimeCode =
                //    results.CompiledAssembly.GetType("HookClass_" + _options.Context.Namespace + "." +
                //                                     _options.ClassName);
            }
            else
            {
                foreach (var error in results.Errors)
                    Console.WriteLine(error);
                throw new Exception("CodeGen failed!");
            }
        }

        private readonly dynamic _instance;
        private readonly Assembly _assembly;

        private string GetMethodArgumentsList()
        {
            var original = _options.Context.GetMethod(_options.HookedFuncName);
            string args = "";
            if (original == null) return args;
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
            ResolveEventHandler @object = (object obj, ResolveEventArgs args) => _assembly;
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.AssemblyResolve += @object.Invoke;
            var method = _runtimeCode.GetMethod("Apply");
            Tuple<HarmonyInstance, MethodInfo, HarmonyMethod, HarmonyMethod> tup = method.Invoke(_instance, new object[]
            {
                _options.Context
            });
            tup.Item1.Patch(tup.Item2, tup.Item3, tup.Item4);
        }

        public bool HasPlanningError
        {
            get
            {
                var method = _runtimeCode.GetProperty("HasPlanningError");
                return method.GetValue(_instance);
            }
        }

        private string GenerateCodeString()
        {
            var funcArgsAddStr = GetMethodArgumentsList();
            return @"
using Harmony;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace HookClass_" + _options.Context.Namespace + @"
{
public class " + _options.ClassName + @"
    {
private static ConditionalWeakTable<object, object> oset = new ConditionalWeakTable<object, object>();
        public " + _options.ClassName + @"() {}
        public Tuple<HarmonyInstance, MethodInfo, HarmonyMethod, HarmonyMethod> Apply(System.Type ctx)
        {
            if (ctx == null)
                throw new Exception(""[" + _options.ClassName + @"] ctx is null!"");
            var harmony = HarmonyInstance.Create(""" + _options.ClassName + @""");
            var original = ctx.GetMethod(""" + _options.HookedFuncName + @""");
            if (original == null) 
                throw new Exception(""[" + _options.ClassName + @"] original method == null."");
            var prefix = typeof(" + _options.ClassName + @").GetMethod(""BeforeCall"");
            var postfix = typeof(" + _options.ClassName + @").GetMethod(""AfterCall"");
            // harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
            // if (!harmony.HasAnyPatches(""" + _options.ClassName + @"""))
            //     throw new Exception(""[" + _options.ClassName + @"] applying hook failed."");
            return new Tuple<HarmonyInstance, MethodInfo, HarmonyMethod, HarmonyMethod>(harmony, original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));
        }

        public static void BeforeCall(object __instance" + funcArgsAddStr + @")
        {
            var self = __instance;

            if (!(" + _options.BeforeCode + @"))
            {
                SetPlanningError();
            }

            Type newObjectType = self.GetType();
            object newObject = Activator.CreateInstance(newObjectType);
            foreach (var propInfo in self.GetType().GetProperties())
            {
                object orgValue = propInfo.GetValue(self, null);
                propInfo.SetValue(newObject, orgValue, null);
            }
            oset.Add(self, newObject);
        }
        public static void AfterCall(object __instance" + funcArgsAddStr + @")
        {
            var self = __instance;
            object pre;
            oset.TryGetValue(self, out pre);

            if (!(" + _options.AfterCode + @"))
            {
                SetPlanningError();
            }
        }

        public static bool HasPlanningError { get; private set; }
        private static void SetPlanningError()
        {
            Console.WriteLine(""Planning Error " + _options.ClassName + @"."");
            HasPlanningError = true;
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