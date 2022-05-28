using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChatRejoinToolCore
{
    internal class MainWindowViewModel : INotifyPropertyChanged
    {
        readonly List<Visit> visits;
        int currentIndex;

        public int CurrentIndex
        {
            get => currentIndex;
            set 
            {
                if ((uint)value < (uint)visits.Count)
                {
                    currentIndex = value;
                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(CurrentIndex)));
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(Selected)));
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(HasNewer)));
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(HasOlder)));
                        PropertyChanged(this, new PropertyChangedEventArgs(nameof(CanOpenUserDetail)));
                    }
                }
                
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public Visit Selected => visits[currentIndex];
        public bool HasNewer => currentIndex < visits.Count - 1;
        public bool HasOlder => currentIndex > 0;
        public bool CanOpenUserDetail => Selected.Instance.OwnerId is not null;
        public void SelectNewer() => CurrentIndex++;
        public void SelectOlder() => CurrentIndex--;
        public MainWindowViewModel()
        {
            visits = new List<Visit>();
            Visit.LoadVisits(visits);
            visits.Sort((a, b) => a.TimeStamp.CompareTo(b.TimeStamp));
            CurrentIndex = visits.Count - 1;
        }
    }
}