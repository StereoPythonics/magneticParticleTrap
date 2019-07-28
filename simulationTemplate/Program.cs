using simulationTemplate.Simulation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace simulationTemplate
{
    public class Program : Bindatable
    {

        MainWindow mainWindow;
        OpenCLContext openCLContext;
        public WriteableBitmap test { get; set; }
        public WriteableBitmap back { get; set; }
        public string Readout { get; set; }
        private Bitmap testbm;
        public float temperature = 0;

        private int i = 0;

        public Program(MainWindow mw)
        {
            mainWindow = mw;
            mainWindow.WindowState = WindowState.Maximized;
            testbm = new Bitmap(1843,1060);
            Graphics g = Graphics.FromImage(testbm);
            g.FillRectangle(Brushes.Black, new Rectangle(0, 0, testbm.Width, testbm.Height));
            BitmapSource testabm = new AliasedBitmapSource(testbm);
            back = new WriteableBitmap(testabm);
            test = new WriteableBitmap(testabm);
            openCLContext = new OpenCLContext();
            Thread T = new Thread(() => runSimulation()) { IsBackground = true };
            T.Start();
        }

        private void runSimulation()
        {
            Stopwatch timer = new Stopwatch();
            timer.Start();
            ISimulation sim = new MagnetSimulation(openCLContext, testbm);
            
            while (true)
            {
                
                try
                {
                    if (timer.ElapsedMilliseconds < 30)
                    {
                        i++;
                        sim.Run();
                    }
                    else
                    {
                        Readout = $"{i.ToString()} iterations/frame  temperature:{temperature.ToString()}";
                        Notify("Readout");
                        mainWindow.Dispatcher.Invoke(new Action(() =>
                        {

                            test.Lock();
                            sim.Render(test);
                            temperature = sim.temp;
                            test.AddDirtyRect(new Int32Rect(0, 0, test.PixelWidth, test.PixelHeight));
                            test.Unlock();

                        }));
                        timer.Restart();

                        
                        i = 0;
                    }
                }
                catch
                { }

            }




        }

    }
}
