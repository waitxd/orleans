using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Runtime;
using UnitTests.GrainInterfaces;

namespace UnitTests.Grains
{
    internal class LivenessTestGrain : Grain, ILivenessTestGrain
    {
        private string label;
        private ILogger logger;
        private Guid uniqueId;

        public LivenessTestGrain(ILoggerFactory loggerFactory)
        {
            this.logger = loggerFactory.CreateLogger($"{this.GetType().Name}-{this.IdentityString}");
        }

        public override Task OnActivateAsync()
        {
            if (this.GetPrimaryKeyLong() == -2)
                throw new ArgumentException("Primary key cannot be -2 for this test case");

            uniqueId = Guid.NewGuid();
            label = this.GetPrimaryKeyLong().ToString();
            logger.LogInformation("OnActivateAsync");

            return base.OnActivateAsync();
        }

        public override Task OnDeactivateAsync()
        {
            logger.LogInformation("!!! OnDeactivateAsync");
            return base.OnDeactivateAsync();
        }

        public Task<string> GetLabel()
        {
            return Task.FromResult(label);
        }

        public Task SetLabel(string label)
        {
            this.label = label;
            logger.LogInformation("SetLabel {0} received", label);
            return Task.CompletedTask;
        }

        public Task StartTimer()
        {
            logger.LogInformation("StartTimer.");
            base.RegisterTimer(TimerTick, null, TimeSpan.Zero, TimeSpan.FromSeconds(10));
            
            return Task.CompletedTask;
        }

        private Task TimerTick(object data)
        {
            logger.LogInformation("TimerTick.");
            return Task.CompletedTask;
        }

        public Task<string> GetRuntimeInstanceId()
        {
            return Task.FromResult(this.RuntimeIdentity);
        }

        public Task<string> GetUniqueId()
        {
            return Task.FromResult(uniqueId.ToString());
        }

        public Task<ILivenessTestGrain> GetGrainReference()
        {
            return Task.FromResult(this.AsReference<ILivenessTestGrain>());
        }
    }
}
