using Harmony;
using System;
using System.Reflection;
using Designer;
using System.Collections.Generic;

namespace HarmonyTest
{
    public class Tools
    {
        //Help method for getting value of private variables
        public dynamic getValue(dynamic instance, string variableName)
        {
            PropertyInfo prop = instance.GetType().GetProperty(variableName, BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            MethodInfo methInf = prop.GetGetMethod(nonPublic: true);
            dynamic workload = methInf.Invoke(instance, null);

            return workload;
        }
    }

    public class DurationNotNegativ
    {
        //private  static  object PrefixRuntime = CodeGenerator.Gernerate();
        
        /// <summary>
        /// Applying Aspects to a Methods
        /// </summary>
        /// <returns>return true if correct applied</returns>
        public static bool Apply()
        {
            /// Create a Harmony Instance that has an unique name
            var harmony = HarmonyInstance.Create("DurationNotNegativ");

            //harmony.PatchAll(Assembly.GetExecutingAssembly());

            /// get the method to override
            var original = typeof(Operation).GetMethod("setTask");

            /// gather the methodInfos for patching
            var prefix = typeof(DurationNotNegativ).GetMethod("BeforeCall");
            var postfix = typeof(DurationNotNegativ).GetMethod("AfterCall");

            /// Patch the method (Apply the aspect)
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            /// Check if methods are applied and return result
            return harmony.HasAnyPatches("DurationNotNegativ");
        }

        public static void BeforeCall(int _id, int _duration)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //CONSTRAINT =>  context: Operation inv: _duration > 0
            if (_duration < 0)
            {
                Console.WriteLine("PLANNING ERROR: Duration time from ID {0} is negativ.", _id);
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        /// <summary>
        /// Aspect after method call, catch the result
        /// </summary>
        /// <param name="__result">Return value of the Parameter</param>
        public static void AfterCall(Operation __instance)
        {
            // Even Private properties are accessable through reflection.
            PropertyInfo strProperty =
            __instance.GetType().GetProperty("endTime", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            MethodInfo strGetter = strProperty.GetGetMethod(nonPublic: true);
            int val = (int)strGetter.Invoke(__instance, null);
        }
    }

    public class OverlappingProdTime
    {
        static Tools tools = new Tools();

        public static bool Apply()
        {
            var harmony = HarmonyInstance.Create("OverlappingProdTime");
            var original = typeof(Operation).GetMethod("setTask");
            var prefix = typeof(OverlappingProdTime).GetMethod("BeforeCall");
            var postfix = typeof(OverlappingProdTime).GetMethod("AfterCall");
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            return harmony.HasAnyPatches("OverlappingProdTime");
        }

        public static void BeforeCall(int _id, int _startTime, Operation _predecessor)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //CONSTRAINT =>  context: Operation inv: _startTime < predecessor.getEndtime()
            if (_startTime < tools.getValue(_predecessor, "endTime"))
            {
                Console.WriteLine("PLANNING ERROR: Overlapping production times for ID {0} and ID {1}", _id, tools.getValue(_predecessor, "id").ToString());
            }
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void AfterCall(Operation __instance)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //SPACE FOR CODE
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public class MachineNotFree
    {
        static Tools tools = new Tools();
        public static bool Apply()
        {
            var harmony = HarmonyInstance.Create("MachineNotFree");
            var original = typeof(Machine).GetMethod("setEntry");
            var prefix = typeof(MachineNotFree).GetMethod("BeforeCall");
            var postfix = typeof(MachineNotFree).GetMethod("AfterCall");
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            return harmony.HasAnyPatches("MachineNotFree");
        }

        public static void BeforeCall(Machine __instance, Operation op)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;

            int startTime, endTime;
            List<Operation> workload = tools.getValue(__instance, "workload"); 
            int startTimeToAdd = tools.getValue(op, "startTime");
            int endTimeToAdd = tools.getValue(op, "endTime");

            foreach (Operation o in workload)
            {
                startTime = tools.getValue(o, "startTime");
                endTime = tools.getValue(o, "endTime");

                if (startTimeToAdd > startTime && startTimeToAdd < endTime)
                {
                    Console.WriteLine("PLANNING ERROR: Start time from Operation-ID {0} is within other production time.", tools.getValue(op, "id").ToString());
                }
                if(endTimeToAdd > startTime && endTimeToAdd < endTime)
                {
                    Console.WriteLine("PLANNING ERROR: End time from Operation-ID {0} is within other production time.", tools.getValue(op, "id").ToString());
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void AfterCall(Machine __instance)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //SPACE FOR CODE
            Console.ForegroundColor = ConsoleColor.White;
        }
    }

    public class TEMPLATE
    {
        static Tools tools = new Tools();
        public static bool Apply()
        {
            var harmony = HarmonyInstance.Create("TEMPLATE");
            var original = typeof(Machine).GetMethod("<METHODENAME>");
            var prefix = typeof(MachineNotFree).GetMethod("BeforeCall");
            var postfix = typeof(MachineNotFree).GetMethod("AfterCall");
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            return harmony.HasAnyPatches("TEMPLATE");
        }

        public static void BeforeCall()
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //SPACE FOR CODE
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void AfterCall(Operation __instance)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;
            //SPACE FOR CODE
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
