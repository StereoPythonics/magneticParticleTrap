using Cloo;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace simulationTemplate.Simulation
{
    public class OpenCLContext
    {
        ComputePlatform platform;
        public ComputeContext Context { get; }
        public ComputeCommandQueue queue;
        public ComputeProgram program { get; }


        public OpenCLContext()
        {
            platform = ComputePlatform.Platforms[0];
            Context = new ComputeContext(ComputeDeviceTypes.Gpu, new ComputeContextPropertyList(platform), null, IntPtr.Zero);
            queue = new ComputeCommandQueue(Context, Context.Devices[0], ComputeCommandQueueFlags.None);

            string clSource;

            using (StreamReader sr = new StreamReader("displayKernels.cl"))
            {
                clSource = sr.ReadToEnd();
            }

            using (StreamReader sr = new StreamReader("simulationKernels.cl"))
            {
                clSource += sr.ReadToEnd();
            }

            program = new ComputeProgram(Context, clSource);
            program.Build(null, null, null, IntPtr.Zero);
            

        

        }
    }
}
