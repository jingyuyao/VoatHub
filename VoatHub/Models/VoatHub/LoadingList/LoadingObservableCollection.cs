using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace VoatHub.Models.VoatHub.LoadingList
{
    /// <summary>
    /// An <see cref="ObservableCollection{T}"/> that supports <see cref="ISupportIncrementalLoading"/> and fires
    /// events on when loading new content starts and ends.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class LoadingObservableCollection<T> : ObservableCollection<T>, ISupportIncrementalLoading
    {
        public event EventHandler LoadingStart;
        public event EventHandler LoadingFinish;

        public abstract bool HasMoreItems { get; }
        public abstract IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count);

        public virtual void OnLoadingStart(EventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler handler = LoadingStart;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public virtual void OnLoadingFinish(EventArgs e)
        {
            // Make a temporary copy of the event to avoid possibility of 
            // a race condition if the last subscriber unsubscribes 
            // immediately after the null check and before the event is raised.
            EventHandler handler = LoadingFinish;
            if (handler != null)
            {
                handler(this, e);
            }
        }
    }
}
