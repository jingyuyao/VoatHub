using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VoatHub.Models.VoatHub.LoadingList
{
    /// <summary>
    /// A base class that contains an <see cref="ObservableCollection{T}"/> and exposes its loading state properties.
    /// <para>NOTE: Child classes must initialize the List property upon construction to perserve the invariant.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LoadingListBase<T> : BindableBase, IDisposable
    {
        /// <summary>
        /// Creates a new <see cref="LoadingListBase{T}"/> where the initial loading state is set to true.
        /// </summary>
        public LoadingListBase()
        {
            Loading = true;
        }

        /// <summary>
        /// Invariant: Never null.
        /// </summary>
        public abstract ObservableCollection<T> List { get; }

        private bool _Loading;
        /// <summary>
        /// Indicates whether data is in the process of being loaded into the collection.
        /// </summary>
        public bool Loading
        {
            get { return _Loading; }
            set { SetProperty(ref _Loading, value); }
        }

        private bool _HasItems;
        /// <summary>
        /// Bindable bool that indicate whether the collection have any items.
        /// </summary>
        public bool HasItems
        {
            get { return _HasItems; }
            set { SetProperty(ref _HasItems, value); }
        }

        public virtual void Dispose()
        {
            List.Clear();
            Loading = true;
            HasItems = false;
        }
    }
}
