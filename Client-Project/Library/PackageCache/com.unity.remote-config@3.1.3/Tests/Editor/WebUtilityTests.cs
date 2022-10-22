using NUnit.Framework;
using System;
using Newtonsoft.Json.Linq;

namespace Unity.RemoteConfig.Editor.Tests
{
    internal class WebUtilityTests
    {

        [Test]
        public void FetchEnvironments_ThrowsArgumentExceptionOnBadArgs()
        {
            try
            {
                RemoteConfigWebApiClient.FetchEnvironments(null);
            }
            catch(ArgumentException ex)
            {
                Assert.That(ex.GetType() == typeof(ArgumentException));
            }
        }

        [Test]
        public void FetchDefaultEnvironment_ThrowsArgumentExceptionOnBadArgs()
        {
            try
            {
                RemoteConfigWebApiClient.FetchDefaultEnvironment(null);
            }
            catch(ArgumentException ex)
            {
                Assert.That(ex.GetType() == typeof(ArgumentException));
            }
        }

        [Test]
        public void FetchConfigs_ThrowsArgumentExceptionOnBadArgs()
        {
            try
            {
                RemoteConfigWebApiClient.FetchConfigs(null, null);
            }
            catch (ArgumentException ex)
            {
                Assert.That(ex.GetType() == typeof(ArgumentException));
            }
        }

        [Test]
        public void PutConfig_ThrowsArgumentExceptionOnBadArgs()
        {
            try
            {
                RemoteConfigWebApiClient.PutConfig(null, null, null, null);
            }
            catch (ArgumentException ex)
            {
                Assert.That(ex.GetType() == typeof(ArgumentException));
            }
        }

        [Test]
        public void PostConfig_ThrowsArgumentExceptionOnBadArgs()
        {
            try
            {
                RemoteConfigWebApiClient.PostConfig(null, null, null);
            }
            catch (ArgumentException ex)
            {
                Assert.That(ex.GetType() == typeof(ArgumentException));
            }
        }

        [Test]
        public void DeleteConfig_ThrowsArgumentExceptionOnBadArgs()
        {
            try
            {
                RemoteConfigWebApiClient.DeleteConfig(null,null);
            }
            catch (ArgumentException ex)
            {
                Assert.That(ex.GetType() == typeof(ArgumentException));
            }
        }
    }
}

