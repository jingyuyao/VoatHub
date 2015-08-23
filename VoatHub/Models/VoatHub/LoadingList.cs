using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// This class contains an <see cref="ObservableCollection{T}"/> and reveals its loading state as bindable properties.
    /// The loading state changes with the <see cref="ObservableCollection{T}"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoadingList<T> : BindableBase
    {
        public LoadingList()
        {
            List = new ObservableCollection<T>();
        }

        private ObservableCollection<T> _List;
        public ObservableCollection<T> List
        {
            get { return _List; }
            set
            {
                Contract.Requires(value != null);
                SetProperty(ref _List, value);
                Loading = value.Count == 0;
                HasItems = value.Count != 0;
                value.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ListChanged);
            }
        }

        private bool _Loading;
        public bool Loading
        {
            get { return _Loading; }
            set { SetProperty(ref _Loading, value); }
        }

        private bool _HasItems;
        public bool HasItems
        {
            get { return _HasItems; }
            set { SetProperty(ref _HasItems, value); }
        }

        public void Clear()
        {
            List = new ObservableCollection<T>();
        }

        /// <summary>
        /// Change the loading state whenever the list changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListChanged(object sender, EventArgs e)
        {
            var collection = sender as ObservableCollection<T>;
            if (collection.Count > 0)
            {
                Loading = false;
                HasItems = true;
            }
            else
            {
                Loading = true;
                HasItems = false;
            }
        }
    }
}
