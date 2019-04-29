using System;
using System.Diagnostics;
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
        private void OverlappingProdTimeTest()
        {
            var a = new Aspect
            {
                ContextName = "Designer.Operation",
                FunctionName = "SetTask",
                BeforeCode = "startTime >= GetValue(predecessor, \"EndTime\")",
                AfterCode = "true"
            };
            RunAspectTest(a);
        }

        [Fact]
        private void DurationNotNegativeTest()
        {
            var a = new Aspect
            {
                ContextName = "Designer.Operation",
                FunctionName = "SetTask",
                BeforeCode = "duration >= 0",
                AfterCode = "true"
            };
            RunAspectTest(a);
        }

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
        private static void RunAspectTest(Aspect aspect)
        {
            var planner = new Planner();

            string aspectName = GetCurrentTestName();
            Console.WriteLine("0, " + aspectName);
            Console.WriteLine("1, " + aspect.ContextName);
            System.Type ctx = ContextNameToType(aspect.ContextName);
            Console.WriteLine("2, " + ctx.ToString());

            var cgen = new CodeGenerator(
                aspectName,
                ctx,
                aspect
            );
            cgen.InvokeApplyMethod();
            planner.Plan();

            Assert.False(cgen.HasPlanningError);
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