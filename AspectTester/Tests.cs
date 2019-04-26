using System;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using Designer;
using HarmonyBridge;
using Xunit;

namespace AspectTester
{
    public class Tests
    {
        [Fact]
        private void OverlappingProdTimeTest()
        {
            RunAspectTest(
                "Designer.Operation",
                "SetTask",
                "startTime >= GetValue(predecessor, \"EndTime\")"
            );
        }

        [Fact]
        private void DurationNotNegativeTest()
        {
            RunAspectTest(
                "Designer.Operation",
                "SetTask",
                "duration >= 0"
            );
        }

        public Tests()
        {
            _assembly = typeof(Operation).Assembly;
        }

        private static Assembly _assembly;

        private string CSharpToOcl(string ocl)
        {
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "conv.exe";
            p.StartInfo.Arguments = ocl;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return output;
        }

        private static System.Type ContextNameToType(string ctxName)
        {
            return _assembly.GetType(ctxName, true);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void RunAspectTest(string contextClassName, string functionName, string beforeCode,
            string afterCode = "true")
        {
            var planner = new Planner();

            string aspectName = GetCurrentTestName();
            Console.WriteLine("0, " + aspectName);
            Console.WriteLine("1, " + contextClassName);
            System.Type ctx = ContextNameToType(contextClassName);
            Console.WriteLine("2, " + ctx.ToString());

            var cgen = new CodeGenerator(new CodeGenerator.Options(
                aspectName,
                ctx,
                functionName,
                beforeCode,
                afterCode
            ));
            cgen.InvokeApplyMethod();
            planner.Plan();

            Assert.False(cgen.HasPlanningError);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private string GetCurrentTestName()
        {
            var st = new StackTrace();
            var sf = st.GetFrame(2);

            return sf.GetMethod().Name;
        }
    }
}