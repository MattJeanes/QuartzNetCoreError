using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using Quartz.Impl.Matchers;
using System;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace QuartzNetCoreError
{
    class Program
    {
        private static void Main(string[] args)
        {
            RunProgram().GetAwaiter().GetResult();
        }

        private static async Task RunProgram()
        {
            try
            {
                var logFactory = new LoggerFactory();
                logFactory.AddConsole(LogLevel.Debug);
                var logger = logFactory.CreateLogger("QuartNetCoreError");


                var factory = new StdSchedulerFactory();
                var scheduler = await factory.GetScheduler();


                // comment and unncomment below line to see the two different type of errors
                scheduler.ListenerManager.AddJobListener(new GlobalJobListener(logger), GroupMatcher<JobKey>.AnyGroup());

                scheduler.ListenerManager.AddSchedulerListener(new GlobalSchedulerListener(logger));

                await scheduler.Start();

                var job = JobBuilder.Create<HelloJob>()
                    .WithIdentity("job1", "group1")
                    .Build();

                var trigger = TriggerBuilder.Create()
                    .WithIdentity("trigger1", "group1")
                    .StartNow()
                    .WithSimpleSchedule(x => x
                        .WithIntervalInSeconds(5)
                        .RepeatForever())
                    .Build();

                await scheduler.ScheduleJob(job, trigger);

                await Task.Delay(TimeSpan.FromSeconds(60));

                await scheduler.Shutdown();
            }
            catch (SchedulerException se)
            {
                await Console.Error.WriteLineAsync(se.ToString());
            }
        }
    }
    public class HelloJob : IJob
    {
        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Greetings from HelloJob!");
            return null;
        }
    }


    internal class GlobalSchedulerListener : ISchedulerListener
    {
        private ILogger logger;

        public GlobalSchedulerListener(ILogger logger)
        {
            this.logger = logger;
        }

        public Task JobAdded(IJobDetail jobDetail)
        {
            logger.LogDebug($"Job added: {jobDetail.Key.Group} - {jobDetail.Key.Name}");
            return null;
        }

        public Task JobDeleted(JobKey jobKey)
        {
            logger.LogDebug($"Job deleted: {jobKey.Group} - {jobKey.Name}");
            return null;
        }

        public Task JobPaused(JobKey jobKey)
        {
            logger.LogDebug($"Job paused: {jobKey.Group} - {jobKey.Name}");
            return null;
        }

        public Task JobResumed(JobKey jobKey)
        {
            logger.LogDebug($"Job resumed: {jobKey.Group} - {jobKey.Name}");
            return null;
        }

        public Task JobScheduled(ITrigger trigger)
        {
            logger.LogDebug($"Job scheduled: {trigger.Key.Group} - {trigger.Key.Name}");
            return null;
        }

        public Task JobsPaused(string jobGroup)
        {
            logger.LogDebug($"Jobs paused: {jobGroup}");
            return null;
        }

        public Task JobsResumed(string jobGroup)
        {
            logger.LogDebug($"Jobs resumed: {jobGroup}");
            return null;
        }

        public Task JobUnscheduled(TriggerKey triggerKey)
        {
            logger.LogDebug($"Job unscheduled: {triggerKey.Group} - {triggerKey.Name}");
            return null;
        }

        public Task SchedulerError(string msg, SchedulerException cause)
        {
            logger.LogError($"Scheduler error: {msg}: {cause.Message}");
            return null;
        }

        public Task SchedulerInStandbyMode()
        {
            logger.LogDebug("Scheduler standby");
            return null;
        }

        public Task SchedulerShutdown()
        {
            logger.LogDebug("Scheduler shutdown");
            return null;
        }

        public Task SchedulerShuttingdown()
        {
            logger.LogDebug("Scheduler shutting down");
            return null;
        }

        public Task SchedulerStarted()
        {
            logger.LogDebug("Scheduler started");
            return null;
        }

        public Task SchedulerStarting()
        {
            logger.LogDebug("Scheduler starting");
            return null;
        }

        public Task SchedulingDataCleared()
        {
            logger.LogDebug("Scheduling data cleared");
            return null;
        }

        public Task TriggerFinalized(ITrigger trigger)
        {
            logger.LogDebug($"Trigger finalized: {trigger.Key.Group} - {trigger.Key.Name}");
            return null;
        }

        public Task TriggerPaused(TriggerKey triggerKey)
        {
            logger.LogDebug($"Trigger paused: {triggerKey.Group} - {triggerKey.Name}");
            return null;
        }

        public Task TriggerResumed(TriggerKey triggerKey)
        {
            logger.LogDebug($"Trigger resumed: {triggerKey.Group} - {triggerKey.Name}");
            return null;
        }

        public Task TriggersPaused(string triggerGroup)
        {
            logger.LogDebug($"Triggers paused: {triggerGroup}");
            return null;
        }

        public Task TriggersResumed(string triggerGroup)
        {
            logger.LogDebug($"Triggers resumed: {triggerGroup}");
            return null;
        }
    }

    public class GlobalJobListener : Quartz.IJobListener
    {
        private ILogger logger;
        public GlobalJobListener(ILogger logger)
        {
            this.logger = logger;
        }

        public virtual string Name
        {
            get { return "MainJobListener"; }
        }

        public Task JobToBeExecuted(IJobExecutionContext context)
        {
            logger.LogDebug($"Job to be executed {context.JobDetail.Description}");
            return null;
        }

        public Task JobExecutionVetoed(IJobExecutionContext context)
        {
            logger.LogDebug($"Job execution vetoed {context.JobDetail.Description}");
            return null;
        }

        public Task JobWasExecuted(IJobExecutionContext context, JobExecutionException jobException)
        {
            if (jobException != null)
            {
                // Log/handle error here
                logger.LogError($"Job Errored : {context.JobDetail.Description} - {jobException.ToString()}");
            }
            else
            {
                logger.LogInformation($"Job Executed : {context.JobDetail.Description}");
            }

            return null;
        }
    }
}