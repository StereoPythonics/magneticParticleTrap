using System.Windows.Media.Imaging;

namespace simulationTemplate
{
    public interface ISimulation
    {

        float temp { get; set; }

        void Run();
        void Render(WriteableBitmap target);
    }
}