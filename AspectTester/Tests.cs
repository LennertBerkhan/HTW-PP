using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Designer;
using HarmonyBridge;
using Newtonsoft.Json;
using Xunit;

namespace AspectTester
{
    public class Tests
    {
        [Fact]
        private void RunAspectTests()
        {
            var planner = new Planner();
            
            var gens = new List<CodeGenerator>();
            gens.Add(OverlappingProdTimeTest());
            gens.Add(DurationNotNegativeTest());

            planner.Plan();

            Assert.DoesNotContain(gens, gen => gen.HasPlanningError);
        }
        private CodeGenerator OverlappingProdTimeTest()
        {
            var aspect = Aspect.OclToAspect("context Operation::SetTask() pre: startTime >= predecessor.EndTime");
            return GenCode(aspect);
        }

        private CodeGenerator DurationNotNegativeTest()
        {
            var aspect = Aspect.OclToAspect("context Operation::SetTask() pre: duration >= 0");
            return GenCode(aspect);
        }
        
        // private CodeGenerator DurationNotNegativeTest()
        // {
        //     var a = new Aspect
        //     {
        //         ContextName = "Designer.Operation",
        //         FunctionName = "SetTask",
        //         BeforeCode = "duration >= 0",
        //         AfterCode = "true"
        //     };
        //     RunAspectTest(a);
        // }

        public Tests()
        {
            _assembly = typeof(Operation).Assembly;
        }

        private static Assembly _assembly;


        private static System.Type ContextNameToType(string ctxName)
        {
            return _assembly.GetType(ctxName, true);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private CodeGenerator GenCode(Aspect aspect)
        {
            string aspectName = GetCurrentTestName();
            Console.WriteLine("AspectName: " + aspectName);
            System.Type ctx = ContextNameToType(aspect.ContextName);
            Console.WriteLine("ContextName: " + ctx.ToString());

            var cgen = new CodeGenerator(
                aspectName,
                ctx,
                aspect
            );
            cgen.InvokeApplyMethod();
            return cgen;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static string GetCurrentTestName()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(2);

            return sf.GetMethod().Name;
        }
    }
}