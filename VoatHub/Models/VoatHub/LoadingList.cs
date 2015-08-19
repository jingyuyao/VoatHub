using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// Container for a list with its loading state.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoadingList<T> : BindableBase
    {
        private bool _loading;
        private bool _hasItems;
        private List<T> _list;

        public LoadingList()
        {
            _list = new List<T>();
            _loading = true;
        }

        public List<T> List
        {
            get { return _list; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();

                SetProperty(ref _list, value);
                HasItems = _list.Count != 0;
            }
        }

        public bool Loading
        {
            get { return _loading; }
            set { SetProperty(ref _loading, value); }
        }

        public bool HasItems
        {
            get { return _hasItems; }
            set { SetProperty(ref _hasItems, value); }
        }
    }
}
