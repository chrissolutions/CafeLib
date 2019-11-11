using System;
using System.Collections.Generic;
using CafeLib.Core.Eventing;
using CafeLib.Core.IoC;
using CafeLib.Mobile.Extensions;
using CafeLib.Mobile.Services;
using CafeLib.Mobile.UnitTest.Core.Fakes;
using Microsoft.Extensions.Logging;
using Moq;
using Xamarin.Forms;

// ReSharper disable UnusedMember.Global

namespace CafeLib.Mobile.UnitTest.Core
{
    public abstract class MobileUnitTest
    {
        protected List<Guid> Subscribers;

        protected Mock<IAlertService> AlertService;
        protected Mock<IDeviceService> DeviceService;
        protected Mock<INavigationService> NavigationService;
        protected Mock<IPageService> PageService;

        protected IServiceRegistry Registry { get; private set; }

        protected IServiceResolver Resolver => Registry.GetResolver();

        protected FakeApplication App { get; private set; }

        public void Initialize()
        {
            SetupTest();
            CreateApplication();
            InitTest();
        }

        public void CreateApplication()
        {
            Subscribers = new List<Guid>();
            Device.PlatformServices = new FakePlatformServices();
            Device.Info = new FakeDeviceInfo();
            App = new FakeApplication(Registry);
        }

        public void Terminate()
        {
            Subscribers.ForEach(x => Resolver.Resolve<IEventService>().Unsubscribe(x));
            Subscribers.Clear();
            Resolver.Dispose();
        }

        /// <summary>
        /// Publish an event message.
        /// </summary>
        /// <typeparam name="T">event message type</typeparam>
        /// <param name="message">event message</param>
        protected void PublishEvent<T>(T message) where T : IEventMessage
        {
            Resolver.Resolve<IEventService>().Publish(message);
        }

        /// <summary>
        /// Subscribe an action to an event message.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        protected void SubscribeEvent<T>(Action<T> action) where T : IEventMessage
        {
            Subscribers.Add(Resolver.Resolve<IEventService>().SubscribeOnMainThread(action));
        }

        /// <summary>
        /// Test entry point.
        /// </summary>
        protected virtual void InitTest()
        {
        }

        /// <summary>
        /// Setup test.
        /// </summary>
        protected virtual void SetupTest()
        {
            // fake set up of the IoC
            SetupRegistry();

            // Setup for all tests.
            AlertService = new Mock<IAlertService>();
            NavigationService = new Mock<INavigationService>();
            PageService = new Mock<IPageService>();

            Registry
                .AddSingleton<IDeviceService, FakeDeviceService>()
                .AddSingleton(x => AlertService.Object)
                .AddSingleton(x => NavigationService.Object)
                .AddSingleton(x => PageService.Object);
        }

        /// <summary>
        /// Setup the service registry.
        /// </summary>
        private void SetupRegistry()
        {
            Registry = IocFactory.CreateRegistry()
                .AddLogging(builder => builder.AddConsole().AddDebug());
        }
    }
}