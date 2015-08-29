using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace VoatHub.Models.VoatHub.LoadingList
{
    /// <summary>
    /// This class contains an <see cref="ObservableCollection{T}"/> and reveals its loading state as bindable properties.
    /// <para>Only the HasItems state is managed by this class. The Loading state should be managed by the client.</para>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LoadingList<T> : LoadingListBase<T>
    {
        public LoadingList()
        {
            _List = new ObservableCollection<T>();
            _List.CollectionChanged += ListChanged;
        }

        private ObservableCollection<T> _List;
        /// <summary>
        /// Change the list and triggers corresponding changes in the HasItems property.
        /// </summary>
        public override ObservableCollection<T> List
        {
            get { return _List; }
        }

        /// <summary>
        /// Change the loading state whenever the list changes
        /// <para>Both Loading and HasItems are determined by the current length of the collection.</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <seealso cref="IncrementalLoadingList.ListChanged(object, EventArgs)"/>
        private void ListChanged(object sender, EventArgs e)
        {
            var collection = sender as ObservableCollection<T>;
            HasItems = collection.Count > 0;
        }
    }
}
