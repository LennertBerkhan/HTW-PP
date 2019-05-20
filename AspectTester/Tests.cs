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
        public static void Main(string[] args)
        {
            var test = new Tests();
            test.RunAspectTests();
        }

        [Fact]
        private void RunAspectTests()
        {
            var planner = new Planner();

            var ocls = new List<string>();
            ocls.Add("context Operation::SetTask() pre OverlappingProdTime: startTime >= predecessor.EndTime");
            ocls.Add("context Operation::SetTask() pre DurationNotNegativ: duration >= 0");
            ocls.Add("context Machine::SetEntry(op: Operation) pre StartTimeCollision: self.Workload->forAll(v|v.StartTime < op.StartTime and v.EndTime > op.StartTime)");
            ocls.Add("context Machine::SetEntry(op: Operation) pre EndTimeCollision: self.Workload->forAll(v|v.StartTime < op.EndTime and v.EndTime > op.EndTime)");
            ocls.Add("context Machine::SetEntry() post CapacityCheck: self.Workload.collect(wl|wl.Duration).sum() <= self.Capacity");
            // ocls.Add("context Planner::Plan() post CheckProductionTime: self.Operations.collect(wl|wl.Duration).sum() <= self.ProductionTime");

            var gens = new List<CodeGenerator>();
            foreach (var ocl in ocls)
                gens.Add(GenCode(Aspect.OclToAspect(ocl)));


            try
            {
                planner.Plan();
            }
            catch (Exception e)
            {
                Console.WriteLine("Plan Exception: " + e.ToString());
            }


            // Assert.DoesNotContain(gens, gen => gen.HasPlanningError);
        }

        public Tests()
        {
            _assembly = typeof(Operation).Assembly;
        }

        private static Assembly _assembly;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private CodeGenerator GenCode(Aspect aspect)
        {
            Console.WriteLine("Aspect: " + aspect.ToString());

            var gen = new CodeGenerator(
                aspect,
                _assembly
            );
            gen.InvokeApplyMethod();
            return gen;
        }
    }
}