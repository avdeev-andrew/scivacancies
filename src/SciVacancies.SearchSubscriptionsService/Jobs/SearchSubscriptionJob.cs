﻿using System;
using Microsoft.Framework.Configuration;
using Quartz;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Autofac;
using Npgsql;
using NPoco;
using SciVacancies.Domain.Enums;
using Microsoft.Framework.Logging;
using SciVacancies.Services.Elastic;
using Nest;

namespace SciVacancies.SearchSubscriptionsService.Jobs
{
    public class SearchSubscriptionJob : IJob
    {
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogger Logger;

        public SearchSubscriptionJob(ILifetimeScope lifetimeScope, ILoggerFactory loggerFactory)
        {
            _lifetimeScope = lifetimeScope;
            this.Logger = loggerFactory.CreateLogger<SearchSubscriptionJob>();
        }

        /// <summary>
        /// выполнить работу
        /// </summary>
        /// <param name="context"></param>
        public void Execute(IJobExecutionContext context)
        {
            ExecuteJob();
        }

        /// <summary>
        /// выполнить работу
        /// </summary>
        public void ExecuteJob()
        {
            Logger.LogInformation("SearchSubscriptionJob started");
            try
            {
                var dataBase = _lifetimeScope.Resolve<IDatabase>();

                Logger.LogInformation($"SearchSubscriptionJob: Select Subscriptions from DB");
                Queue<SciVacancies.ReadModel.Core.SearchSubscription> subscriptionQueue = new Queue<ReadModel.Core.SearchSubscription>(dataBase.Fetch<SciVacancies.ReadModel.Core.SearchSubscription>(new Sql($"SELECT * FROM res_searchsubscriptions ss WHERE ss.status = @0", SearchSubscriptionStatus.Active)));
                Logger.LogInformation($"SearchSubscriptionJob: Found {subscriptionQueue.Count} Subscriptions in DB");

                //int poolCount = 20;
                //var threadSize = subscriptionQueue.Count > poolCount
                //    ? (subscriptionQueue.Count / poolCount) + 1
                //    : 1;

                //Logger.LogInformation($"SearchSubscriptionJob: threadSize = {threadSize}");
                //var i = 0;

                //var actions = new List<Action>();

                //while (subscriptionQueue.Skip(threadSize * i).Take(threadSize).Any())
                //{
                //    var y = i;
                //    var scanner = _lifetimeScope.Resolve<ISearchSubscriptionScanner>();
                //    actions.Add(() =>
                //    {
                //        scanner.Initialize(subscriptionQueue.Skip(threadSize * y).Take(threadSize));
                //        scanner.PoolHandleSubscriptions();
                //    });
                //    i++;
                //}
                //var actionsArray = actions.ToArray();

                Logger.LogInformation("SearchSubscriptionJob: Обработка потоков началась");

                //Parallel.Invoke(actionsArray);
                var scanner = _lifetimeScope.Resolve<ISearchSubscriptionScanner>();

                Logger.LogInformation("SearchSubscriptionJob: Scanner initializaing");
                scanner.Initialize(subscriptionQueue);
                Logger.LogInformation("SearchSubscriptionJob: Scanner initialized");
                Logger.LogInformation("SearchSubscriptionJob: Scanner subsciptions starting to invoke");
                scanner.PoolHandleSubscriptions();
                Logger.LogInformation("SearchSubscriptionJob: Scanner subsciptions finished to invoke");

                Logger.LogInformation("SearchSubscriptionJob: Обработка потоков завершена");
            }
            catch (Exception e)
            {
                Logger.LogError(e.Message, e);
            }
        }
    }
}