﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;

namespace VoatHub.Models.VoatHub.LoadingList
{
    /// <summary>
    /// An ObservableCollection container that manages its own Loading and HasItems states.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class IncrementalLoadingList<T, N> : LoadingListBase<T> where N : LoadingObservableCollection<T>, new()
    {
        /// <summary>
        /// Prevents using the constructor. Use <see cref="CreateList{N}(LoadingObservableCollection{N})"/> instead.
        /// </summary>
        public IncrementalLoadingList(LoadingObservableCollection<T> collection) : base()
        {
            List = collection;
        }

        private LoadingObservableCollection<T> _List;
        public override ObservableCollection<T> List
        {
            get
            {
                return _List;
            }

            set
            {
                Contract.Requires(value != null);
                SetProperty(ref _List, (LoadingObservableCollection<T>)value);
                HasItems = value.Count != 0;
                _List.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(ListChanged);
                _List.LoadingStart += new EventHandler(LoadingStartHandler);
                _List.LoadingFinish += new EventHandler(LoadingFinishHandler);
            }
        }

        protected override ObservableCollection<T> DefaultList
        {
            get
            {
                return new N();
            }
        }

        /// <summary>
        /// Change the loading state whenever the list changes.
        /// <para>Only affects HasItems</para>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ListChanged(object sender, EventArgs e)
        {
            var collection = sender as ObservableCollection<T>;
            // Somehow this is getting called in the UI thread by default so it avoid the access violation problem the other two
            // event handlers have. Maybe its because NotifyCollectionChangedEventHandler is a System level event?
            HasItems = collection.Count > 0;
        }

        private async void LoadingStartHandler(object sender, EventArgs e)
        {
            // http://stackoverflow.com/questions/16477190/correct-way-to-get-the-coredispatcher-in-a-windows-store-app
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Loading = true;
            });
        }

        private async void LoadingFinishHandler(object sender, EventArgs e)
        {
            // http://stackoverflow.com/questions/16477190/correct-way-to-get-the-coredispatcher-in-a-windows-store-app
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Loading = false;
            });
        }
    }
}
