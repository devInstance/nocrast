﻿using NoCrast.Client.Services.Api;
using NoCrast.Shared.Logging;
using NoCrast.Shared.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NoCrast.Client.Services
{
    public class AccountService : BaseService
    {
        private IUserProfileApi UserProfileApi { get; }

        public AccountService(NotificationService notificationService,
                                ILogProvider logProvider,
                                IUserProfileApi api) : base(notificationService)
        {
            UserProfileApi = api;
            Log = logProvider.CreateLogger(this);
            Log.D("constructor");
        }

        public async Task<UserProfileItem> GetAccountAsync()
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                try
                {
                    var response = await UserProfileApi.GetProfileAsync();
                    ResetNetworkError();
                    return response;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return null;
            }
        }

        public async Task<UserProfileItem> UpdateAccountNameAsync(UserProfileItem item, string newName)
        {
            using (var l = Log.DebugScope())
            {
                ResetUIError();
                try
                {
                    item.Name = newName;
                    var response = await UserProfileApi.UpdateProfileAsync(item);
                    ResetNetworkError();
                    return response;
                }
                catch (Exception ex)
                {
                    NotifyNetworkError(ex);
                }
                return item;
            }
        }
    }
}
