using Harmony;
using StandardCode;
using System;
using System.Reflection;
using Designer;

namespace HarmonyTest
{
    public class Aspect
    {
        //Erstmal ohne Code Generator
        //private  static  object PrefixRuntime = CodeGenerator.Gernerate();

        /// <summary>
        /// Applying Aspects to a Methods
        /// </summary>
        /// <returns>return true if correct applied</returns>
        public static bool Apply()
        {
            /// Create a Harmony Instance that has an unique name
            var harmony = HarmonyInstance.Create("StandardCode.V1");
            /// get the method to override
            var original = typeof(Plan).GetMethod("AddQuantity");

            /// gather the methodInfos for patching
            var prefix = typeof(Aspect).GetMethod("BeforeCall");
            var postfix = typeof(Aspect).GetMethod("AfterCall");

            /// Patch the method (Apply the aspect)
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            /// Check if methods are applied and return result
            return harmony.HasAnyPatches("StandardCode.V1");
        }

        public static bool Apply2()
        {
            /// Create a Harmony Instance that has an unique name
            var harmony = HarmonyInstance.Create("DurationNotNegativ");

            //harmony.PatchAll(Assembly.GetExecutingAssembly());

            /// get the method to override
            var original = typeof(Operation).GetMethod("setTask");

            /// gather the methodInfos for patching
            var prefix = typeof(Aspect).GetMethod("BeforeCall2");
            var postfix = typeof(Aspect).GetMethod("AfterCall2");

            /// Patch the method (Apply the aspect)
            harmony.Patch(original, new HarmonyMethod(prefix), new HarmonyMethod(postfix));

            /// Check if methods are applied and return result
            return harmony.HasAnyPatches("DurationNotNegativ");
        }


        /// <summary>
        /// Intercepts the Specified Method, gaining its instance and Property to modify
        /// </summary>
        /// <param name="__instance">Instance that tries to Execute the Method</param>
        /// <param name="number">Property of that Method, ad ref keyword to modify</param>
        public static void BeforeCall(Plan __instance, int number)
        {
            Console.WriteLine("-- Begin Interception --", "Aspect");
            Console.WriteLine(" -Before Method is Called", "Aspect");
            Console.WriteLine("  -My method parameter 'number' is " + number, "Aspect");

            // Test to access Runtime generated Code.
            //PrefixRuntime.GetType().GetMethod("Prefix").Invoke(null, new object[] { number });


            // Even Private properties are accessable through reflection.
            PropertyInfo strProperty =
            __instance.GetType().GetProperty("PrivateStr", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.GetProperty);
            MethodInfo strGetter = strProperty.GetGetMethod(nonPublic: true);
            string val = (string)strGetter.Invoke(__instance, null);
            Console.WriteLine(" -" + val);
        }

        public static void BeforeCall2(int _id, int _startTime, int _duration, Operation _predecessor, Machine _machId)
        {
            Console.ForegroundColor = ConsoleColor.DarkRed;

            Console.WriteLine(""); 
            if (_duration < 0)
            {
                Console.WriteLine("Planning error: duration time from ID {0} is negativ.", _id);

            }

            if (_startTime < _predecessor.getEndTime())
            {
                Console.WriteLine("Planning error: overalpping production times for ID {0} and ID {1}", _id, _predecessor.getId());
            }



            Console.ForegroundColor = ConsoleColor.White;

        }

        /// <summary>
        /// Aspect after method call, catch the result
        /// </summary>
        /// <param name="__result">Return value of the Parameter</param>
        public static void AfterCall(bool __result)
        {
            Console.WriteLine(" -After Method is Called", "Aspect");
            Console.WriteLine(" Result was " + __result, "Aspect");
            Console.WriteLine("--- End Interception ---", "Aspect");
            Console.WriteLine("", "Aspect");
        }

        public static void AfterCall2(Operation __instance)
        {
            Console.WriteLine(" -After Method- Endtime of the Operation is: {0}", __instance.getEndTime());
            // Console.WriteLine(" Result was " + endTime, "Aspect");
        }

    }
}
