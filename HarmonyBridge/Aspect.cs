using System.Diagnostics;
using System.IO;
using Newtonsoft.Json;

namespace HarmonyBridge
{
    internal class AspectJson
    {
        
    }
    public class Aspect
    {
        public string ContextName;
        public string FunctionName;
        public string BeforeCode;
        public string AfterCode;

        public static Aspect OclToAspect(string ocl)
        {
            var p = new Process
            {
                StartInfo =
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    FileName = "/home/yuri/dc/htw/fe/ocl-to-csharp/cpp/conv",
                    Arguments = @"-d """ + ocl + @""""
                }
            };
            p.Start();
            var json = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            
            var deserializedAspect = JsonConvert.DeserializeObject<Aspect>(json);
            deserializedAspect.ContextName = "Designer." + deserializedAspect.ContextName;
            return deserializedAspect;
        }
    }
}