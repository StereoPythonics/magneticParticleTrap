using System.ComponentModel;

namespace simulationTemplate
{
    public class Bindatable : INotifyPropertyChanged
    {

        public Bindatable() { }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Notify(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}