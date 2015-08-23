using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VoatHub.Api.Voat;
using VoatHub.Models.Voat.v1;
using Windows.Foundation;
using Windows.UI.Xaml.Data;
using Windows.UI.Core;
using Windows.UI.Xaml;

using VoatHub.Models.VoatHub.LoadingList;

namespace VoatHub.Models.VoatHub
{
    /// <summary>
    /// References:
    /// https://marcominerva.wordpress.com/2013/05/22/implementing-the-isupportincrementalloading-interface-in-a-window-store-app/
    /// </summary>
    public class IncrementalSubmissionList : LoadingObservableCollection<ApiSubmission>
    {
        private VoatApi api;
        private bool hasMoreItems;
        private string subverse;

        /// <summary>
        /// For <see cref="IncrementalLoadingList{T, N}"/>
        /// </summary>
        public IncrementalSubmissionList() : base()
        {

        }

        public IncrementalSubmissionList(VoatApi api, string subverse)
        {
            this.api = api;
            this.subverse = subverse;
            hasMoreItems = true;
            api.SubmissionSearchOptions.page = 1;  // Voat's page starts with 1
        }

        #region IncrementalLoadingBase
        public override bool HasMoreItems
        {
            get { return hasMoreItems; }
        }
        
        public override IAsyncOperation<LoadMoreItemsResult> LoadMoreItemsAsync(uint count)
        {
            OnLoadingStart(EventArgs.Empty);

            return Task.Run<LoadMoreItemsResult>(async () =>
            {
                uint resultCount = 0;
                var response = await api.GetSubmissionList(subverse);
                if (response != null && response.Success)
                {
                    // http://stackoverflow.com/questions/16477190/correct-way-to-get-the-coredispatcher-in-a-windows-store-app
                    await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        foreach (var submission in response.Data)
                        {
                            this.Add(submission);
                        }
                    });
                    
                    resultCount = (uint)response.Data.Count;
                }

                if (resultCount == 0) hasMoreItems = false;

                OnLoadingFinish(EventArgs.Empty);
                return new LoadMoreItemsResult { Count = resultCount };
            }).AsAsyncOperation<LoadMoreItemsResult>();
        }
        #endregion
    }
}
