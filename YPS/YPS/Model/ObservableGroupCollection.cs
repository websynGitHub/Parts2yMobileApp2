using System.Collections.ObjectModel;
using System.Linq;

namespace YPS.Model
{
    public class ObservableGroupCollection<K, T> : ObservableCollection<T>
    {
        private readonly K _key;

        public ObservableGroupCollection(IGrouping<K, T> group)
            : base(group)
        {
            _key = group.Key;
        }

        public K Key
        {
            get { return _key; }
        }
    }
}  
  
