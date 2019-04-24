﻿#define DONT_USE_CODEGENERATOR 

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
            

#if DONT_USE_CODEGENERATOR
            if (!DurationNotNegativ.Apply()) throw new Exception("Applying aspect failed");
            if (!OverlappingProdTime.Apply()) throw new Exception("Applying aspect failed");
            if (!MachineNotFree.Apply()) throw new Exception("Applying aspect failed");
            if (!CapacityCheck.Apply()) throw new Exception("Applying aspect failed");
            if (!CheckMaterialQuantity.Apply()) throw new Exception("Applying aspect failed");
            Console.WriteLine("\nOutput with Harmony/Aspects");
            Planner.Plan();
            Console.ReadKey();
            
#else
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
            Planner.Plan();
            Console.ReadKey();
            
#endif
        }
    }
}