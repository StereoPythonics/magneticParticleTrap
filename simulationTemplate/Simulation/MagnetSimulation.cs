using Cloo;
using simulationTemplate.Simulation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace simulationTemplate
{
    public class MagnetSimulation : ISimulation
    {

        private float[] protomolecules;
        private ComputeBuffer<float> moleculesBuffer;
        private ComputeImage2D canvas;
        private Random randoCal;
        private OpenCLContext oclc;
        private Bitmap sourceTemplate;

        private ComputeKernel blackKernel;
        private ComputeKernel drawKernel;
        private ComputeKernel magKernel;
        public float temp { get; set; }

        public MagnetSimulation(OpenCLContext context, Bitmap Template)
        {
            sourceTemplate = Template.Clone(new Rectangle(0, 0, Template.Width, Template.Height), PixelFormat.Format32bppArgb);
            oclc = context;
            randoCal = new Random();
            setupCanvas();
            generateRandowPositions(1000000);

            blackKernel = oclc.program.CreateKernel("black");
            drawKernel = oclc.program.CreateKernel("draw");
            magKernel = oclc.program.CreateKernel("magnitate");

            loadDataOntoCard();
        }

        private void loadDataOntoCard()
        {
            blackKernel.SetMemoryArgument(0, canvas);
            blackKernel.SetMemoryArgument(1, canvas);

            drawKernel.SetMemoryArgument(0, moleculesBuffer);
            drawKernel.SetMemoryArgument(1, canvas);

            magKernel.SetMemoryArgument(0, moleculesBuffer);
        }

        private void generateRandowPositions(int count)
        {
            protomolecules = new float[6*count].Select(f => (float)(randoCal.NextDouble())).ToArray();
            for (int i=0; i < 6 * count; i++)
            {
                if (i % 6 == 1) protomolecules[i] *= sourceTemplate.Width;
                if (i % 6 == 2) protomolecules[i] *= sourceTemplate.Height;
                if (i % 6 == 3) protomolecules[i] = 0;
                if (i % 6 == 4 || i % 6 == 5) protomolecules[i] = (float)(Math.Sin(protomolecules[i]*Math.PI*2)* randoCal.NextDouble() * 0.1);

            }


            moleculesBuffer = new ComputeBuffer<float>(oclc.Context, ComputeMemoryFlags.CopyHostPointer | ComputeMemoryFlags.ReadWrite, protomolecules);
        }

        private void setupCanvas()
        {
            Graphics g = Graphics.FromImage(sourceTemplate);
            g.FillRectangle(Brushes.Black, new Rectangle(0,0,sourceTemplate.Width, sourceTemplate.Height));
            BitmapData bmd = sourceTemplate.LockBits(new Rectangle(0, 0, sourceTemplate.Width, sourceTemplate.Height), ImageLockMode.ReadWrite, sourceTemplate.PixelFormat);
            canvas = new ComputeImage2D(oclc.Context,ComputeMemoryFlags.CopyHostPointer | ComputeMemoryFlags.ReadWrite, new ComputeImageFormat(ComputeImageChannelOrder.Rgba, ComputeImageChannelType.UnsignedInt8), sourceTemplate.Width, sourceTemplate.Height, 0, bmd.Scan0);
            sourceTemplate.UnlockBits(bmd);
        }

        public void Render(WriteableBitmap target)
        {
            oclc.queue.Execute(drawKernel, null, new long[] { protomolecules.Count() / 6 }, null, null);
            oclc.queue.ReadFromImage(canvas, target.BackBuffer, true, null);
            //oclc.queue.ReadFromBuffer<float>(moleculesBuffer,ref protomolecules, true, null);
            //oclc.queue.Finish();
            oclc.queue.Execute(blackKernel, null, new long[] { sourceTemplate.Width, sourceTemplate.Height }, null, null);

            
            //List<float> temps = new List<float>();
            //float roller = 0;
            //float[] subs = protomolecules.Take(600000).Where(f => !float.IsNaN(f) && f<1000).ToArray();
            //int count = subs.Count();
            //for (int i = 1; i < count; i++)
            //{
            //    if (i % 6 == 0)
            //    {
            //        temps.Add(roller);
            //        roller = 0;
            //    }
            //    if (i % 6 == 3 || i % 6 == 4 || i % 6 == 5) roller += (subs[i]*subs[i]);
            //}
            //temp = temps.Average();
        }

        public void Run()
        {
            for (int i = 0; i < 1; i++)
            {
                oclc.queue.Execute(magKernel, null, new long[] { protomolecules.Count()/6 }, new long[] { 255 }, null);
            }
            oclc.queue.Finish();
            
        }
    }
}
