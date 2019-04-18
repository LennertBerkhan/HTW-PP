using System;
using Designer;

namespace HarmonyBridge
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var planner = new Planner();
            Console.WriteLine("Output without Harmony/Aspects");
            Planner.Plan();
            // Console.ReadKey();


            //CONSTRAINT =>  context: Operation pre: _duration > 0
            new CodeGenerator(new CodeGenerator.Options(
                    "DurationNotNegative",
                    typeof(Operation),
                    "SetTask",
                    "duration >= 0",
                    "true"
                ))
                .InvokeApplyMethod();


            //CONSTRAINT =>  context: Operation pre: _startTime < predecessor.getEndtime()
            new CodeGenerator(new CodeGenerator.Options(
                    "OverlappingProdTime",
                    typeof(Operation),
                    "SetTask",
                    "startTime >= GetValue(predecessor, \"EndTime\")",
                    "true"
                ))
                .InvokeApplyMethod();

            new CodeGenerator(new CodeGenerator.Options(
                    "MachineNotFree",
                    typeof(Machine),
                    "SetEntry",
                    "true",
                    "true"
                ))
                .InvokeApplyMethod();


            // if (!MachineNotFree.Apply()) throw new Exception("Applying aspect failed");
            // if (!CapacityCheck.Apply()) throw new Exception("Applying aspect failed");

            Console.WriteLine("\nOutput with Harmony/Aspects");
            Planner.Plan();
            // Console.ReadKey();
        }
    }
}