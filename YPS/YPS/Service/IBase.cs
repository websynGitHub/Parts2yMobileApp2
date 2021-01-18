using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace YPS.Service
{
    public class IBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets called when changes is taking place to any properties.
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName]string propertyName = "")
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Gets called when changes is taking place to any properties.
        /// </summary>
        /// <param name="name"></param>
        public void RaisePropertyChanged(String name)
        {
            if (PropertyChanged != null)
            this.PropertyChanged(this, new PropertyChangedEventArgs(name));
        }
    }
}
