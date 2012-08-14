namespace Okazuki.UI.Flyouts.Internal
{
    using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

    public class NotificationObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual bool SetProperty<T>(ref T store, T value, [CallerMemberName]string propertyName = null)
        {
            if (object.Equals(store, value))
            {
                return false;
            }

            store = value;
            this.RaisePropertyChanged(propertyName);
            return true;
        }

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            var h = this.PropertyChanged;
            if (h != null)
            {
                h(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
