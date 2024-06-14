﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using DownKyi.Events;
using DownKyi.Images;
using DownKyi.Utils;
using DownKyi.ViewModels.Friends;
using DownKyi.ViewModels.PageViewModels;
using Prism.Commands;
using Prism.Events;
using Prism.Regions;

namespace DownKyi.ViewModels
{
    public class ViewFriendsViewModel : ViewModelBase
    {
        public const string Tag = "PageFriends";

        private readonly IRegionManager regionManager;

        private long mid = -1;

        #region 页面属性申明

        private VectorImage arrowBack;

        public VectorImage ArrowBack
        {
            get => arrowBack;
            set => SetProperty(ref arrowBack, value);
        }

        private ObservableCollection<TabHeader> tabHeaders;

        public ObservableCollection<TabHeader> TabHeaders
        {
            get => tabHeaders;
            set => SetProperty(ref tabHeaders, value);
        }

        private int selectTabId = -1;

        public int SelectTabId
        {
            get => selectTabId;
            set => SetProperty(ref selectTabId, value);
        }

        #endregion

        public ViewFriendsViewModel(IRegionManager regionManager, IEventAggregator eventAggregator) : base(
            eventAggregator)
        {
            this.regionManager = regionManager;

            #region 属性初始化

            ArrowBack = NavigationIcon.Instance().ArrowBack;
            ArrowBack.Fill = DictionaryResource.GetColor("ColorTextDark");

            TabHeaders = new ObservableCollection<TabHeader>
            {
                new() { Id = 0, Title = DictionaryResource.GetString("FriendFollowing") },
                new() { Id = 1, Title = DictionaryResource.GetString("FriendFollower") },
            };

            #endregion
        }

        #region 命令申明

        // 返回事件
        private DelegateCommand backSpaceCommand;

        public DelegateCommand BackSpaceCommand =>
            backSpaceCommand ?? (backSpaceCommand = new DelegateCommand(ExecuteBackSpace));

        /// <summary>
        /// 返回事件
        /// </summary>
        private void ExecuteBackSpace()
        {
            //InitView();

            ArrowBack.Fill = DictionaryResource.GetColor("ColorText");

            NavigationParam parameter = new NavigationParam
            {
                ViewName = ParentView,
                ParentViewName = null,
                Parameter = null
            };
            EventAggregator.GetEvent<NavigationEvent>().Publish(parameter);
        }

        // 顶部tab点击事件
        private DelegateCommand<object> tabHeadersCommand;

        public DelegateCommand<object> TabHeadersCommand => tabHeadersCommand ??
                                                            (tabHeadersCommand =
                                                                new DelegateCommand<object>(ExecuteTabHeadersCommand));

        /// <summary>
        /// 顶部tab点击事件
        /// </summary>
        /// <param name="parameter"></param>
        private void ExecuteTabHeadersCommand(object parameter)
        {
            if (!(parameter is TabHeader tabHeader))
            {
                return;
            }

            // TODO
            // 此处应该根据具体状态传入true or false
            NavigationView(tabHeader.Id, true);
        }

        #endregion

        /// <summary>
        /// 进入子页面
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isFirst"></param>
        private void NavigationView(long id, bool isFirst)
        {
            // isFirst参数表示是否是从PageFriends的headerTable的item点击进入的
            // true表示加载PageFriends后第一次进入
            // false表示从headerTable的item点击进入
            var param = new NavigationParameters()
            {
                { "mid", mid },
                { "isFirst", isFirst },
            };

            switch (id)
            {
                case 0:
                    regionManager.RequestNavigate("FriendContentRegion", ViewFollowingViewModel.Tag, param);
                    break;
                case 1:
                    regionManager.RequestNavigate("FriendContentRegion", ViewFollowerViewModel.Tag, param);
                    break;
            }
        }

        /// <summary>
        /// 导航到页面时执行
        /// </summary>
        /// <param name="navigationContext"></param>
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);

            ArrowBack.Fill = DictionaryResource.GetColor("ColorTextDark");

            // 根据传入参数不同执行不同任务
            var parameter = navigationContext.Parameters.GetValue<Dictionary<string, object>>("Parameter");
            if (parameter == null)
            {
                return;
            }

            mid = (long)parameter["mid"];
            SelectTabId = (int)parameter["friendId"];

            PropertyChangeAsync(() =>
            {
                NavigationView(SelectTabId, true);
            });
        }
    }
}