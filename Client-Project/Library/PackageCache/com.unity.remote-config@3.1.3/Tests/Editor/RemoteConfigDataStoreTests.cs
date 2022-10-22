using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Unity.RemoteConfig.Editor.Tests
{
    internal class RemoteConfigDataStoreTests
    {
        [TearDown]
        public void TearDown()
        {
            var path = typeof(RemoteConfigUtilities)
                .GetField("k_PathToDataStore", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null) as string;
            AssetDatabase.DeleteAsset(path);
            AssetDatabase.SaveAssets();
        }

        [Test]
        public void InitDataStore_InitsAll()
        {
            var dataStore = RCTestUtils.GetDataStore();
            var initDataStoreMethod =
                typeof(RemoteConfigDataStore).GetMethod("InitDataStore", BindingFlags.NonPublic |
                                                                         BindingFlags.Instance);
            initDataStoreMethod?.Invoke(dataStore, new object[] { });

            Assert.That(dataStore.rsKeyList != null);
            Assert.That(dataStore._rsKeyList != null);
            Assert.That(dataStore._rsLastCachedKeyList != null);
            Assert.That(dataStore._environments != null);
        }

        [Test]
        public void CheckAndCreateAssetFolder_CreatesAssetFolder()
        {
            var dataStore = RCTestUtils.GetDataStore();
            var path = typeof(RemoteConfigUtilities)
                .GetField("k_PathToDataStore", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(dataStore) as string;
            Directory.Delete(path, true);
            Assert.That(!Directory.Exists(path));
        }

        [Test]
        public void RSList_ReturnsCorrectKeysTypesAndValues()
        {
            var dataStore = RCTestUtils.GetDataStore();
            dataStore.rsKeyList = new JArray(RCTestUtils.rsListWithMetadata);
            foreach(var settingWithMetadata in dataStore.rsKeyList)
            {
                Assert.That(!string.IsNullOrEmpty(settingWithMetadata["metadata"]["entityId"].Value<string>()));
                switch (settingWithMetadata["rs"]["type"].Value<string>())
                {
                    case "bool":
                        Assert.That(settingWithMetadata["rs"]["key"].Value<string>().Equals(RCTestUtils.settingBool["key"].Value<string>()));
                        Assert.That(settingWithMetadata["rs"]["type"].Value<string>().Equals(RCTestUtils.settingBool["type"].Value<string>()));
                        Assert.That(settingWithMetadata["rs"]["value"].Value<string>().Equals(RCTestUtils.settingBool["value"].Value<string>()));
                        break;
                    case "int":
                        Assert.That(settingWithMetadata["rs"]["key"].Value<string>().Equals(RCTestUtils.settingInt["key"].Value<string>()));
                        Assert.That(settingWithMetadata["rs"]["type"].Value<string>().Equals(RCTestUtils.settingInt["type"].Value<string>()));
                        Assert.That(settingWithMetadata["rs"]["value"].Value<string>().Equals(RCTestUtils.settingInt["value"].Value<string>()));
                        break;
                    case "float":
                        Assert.That(settingWithMetadata["rs"]["key"].Value<string>().Equals(RCTestUtils.settingFloat["key"].Value<string>()));
                        Assert.That(settingWithMetadata["rs"]["type"].Value<string>().Equals(RCTestUtils.settingFloat["type"].Value<string>()));
                        Assert.That(settingWithMetadata["rs"]["value"].Value<string>().Equals(RCTestUtils.settingFloat["value"].Value<string>()));
                        break;
                    case "long":
                        Assert.That(settingWithMetadata["rs"]["key"].Value<string>().Equals(RCTestUtils.settingLong["key"].Value<string>()));
                        Assert.That(settingWithMetadata["rs"]["type"].Value<string>().Equals(RCTestUtils.settingLong["type"].Value<string>()));
                        Assert.That(settingWithMetadata["rs"]["value"].Value<string>().Equals(RCTestUtils.settingLong["value"].Value<string>()));
                        break;
                    case "string":
                        Assert.That(settingWithMetadata["rs"]["key"].Value<string>().Equals(RCTestUtils.settingString["key"].Value<string>()));
                        Assert.That(settingWithMetadata["rs"]["type"].Value<string>().Equals(RCTestUtils.settingString["type"].Value<string>()));
                        Assert.That(settingWithMetadata["rs"]["value"].Value<string>().Equals(RCTestUtils.settingString["value"].Value<string>()));
                        break;
                    case "json":
                        Assert.That(settingWithMetadata["rs"]["key"].Value<string>().Equals(RCTestUtils.settingJson["key"].Value<string>()));
                        Assert.That(settingWithMetadata["rs"]["type"].Value<string>().Equals(RCTestUtils.settingJson["type"].Value<string>()));
                        Assert.That(settingWithMetadata["rs"]["value"].Value<string>().Equals(RCTestUtils.settingJson["value"].Value<string>()));
                        break;
                }
            }
        }

        [Test]
        public void SetDefaultEnvironment_ShouldSetDefaultEnvironment()
        {
            var dataStore = RCTestUtils.GetDataStore();
            var newDefaultEnvironment = RCTestUtils.environment2;
            dataStore.environments = RCTestUtils.environments;
            dataStore.SetDefaultEnvironment(newDefaultEnvironment["id"].Value<string>());
            Assert.That(Equals(dataStore.environments[1]["isDefault"].Value<bool>(), true));
        }

        [Test]
        public void SetCurrentEnvironment_SetsCurrentEnvironment()
        {
            var dataStore = RCTestUtils.GetDataStore();
            var currentEnvironment = new JObject();
            currentEnvironment["name"] = RCTestUtils.currentEnvironment;
            currentEnvironment["id"] = RCTestUtils.currentEnvironmentId;
            dataStore.SetCurrentEnvironment(currentEnvironment);

            Assert.That(Equals(dataStore.currentEnvironmentName, currentEnvironment["name"].Value<string>()));
            Assert.That(Equals(dataStore.currentEnvironmentId, currentEnvironment["id"].Value<string>()));
        }

        [Test]
        public void SetDataStoreConfig_SetsDataStoreConfigWhenAListIsPassedIn()
        {
            var dataStore = RCTestUtils.GetDataStore();
            var config = new JObject();
            config["type"] = "settings";
            config["id"] = "someId";
            config["value"] = RCTestUtils.rsListNoMetadata;
            dataStore.config = config;
            Assert.That(RCTestUtils.rsListNoMetadata.Count.Equals(dataStore.rsKeyList.Count));
            for(int i = 0; i < RCTestUtils.rsListNoMetadata.Count; i++)
            {
                Assert.That(!string.IsNullOrEmpty(dataStore.rsKeyList[i]["metadata"]["entityId"].Value<string>()));
                Assert.That(dataStore.rsKeyList[i]["rs"]["key"].Equals(RCTestUtils.rsListNoMetadata[i]["key"]));
                Assert.That(dataStore.rsKeyList[i]["rs"]["value"].Equals(RCTestUtils.rsListNoMetadata[i]["value"]));
            }
        }

        [Test]
        public void AddSetting_AddsSettingToListAndDict()
        {
            var dataStore = RCTestUtils.GetDataStore();
            dataStore.rsKeyList = new JArray(RCTestUtils.rsListWithMetadata);

            var newSetting = new JObject();
            newSetting["metadata"] = new JObject();
            newSetting["metadata"]["entityId"] = "new-setting-entitity-id";
            newSetting["rs"] = new JObject();
            newSetting["rs"]["key"] = "NewSetting";
            newSetting["rs"]["value"] = "NewValue";
            newSetting["rs"]["type"] = "string";

            dataStore.AddSetting(newSetting);
            var settingsFromList = new JArray(dataStore.rsKeyList.Where(s => s["rs"]["key"].Value<string>() == newSetting["rs"]["key"].Value<string>()));
            var addedSetting = settingsFromList[0];
            Assert.That(Equals(addedSetting["rs"]["key"], newSetting["rs"]["key"]));
            Assert.That(Equals(addedSetting["rs"]["value"], newSetting["rs"]["value"]));
            Assert.That(Equals(addedSetting["rs"]["type"], newSetting["rs"]["type"]));
        }

        [Test]
        public void AddSettingFormattedAsDate_AddsSettingToListAndDict()
        {
            var dataStore = RCTestUtils.GetDataStore();
            dataStore.rsKeyList = new JArray(RCTestUtils.rsListWithMetadata);

            var newSetting = new JObject();
            newSetting["metadata"] = new JObject();
            newSetting["metadata"]["entityId"] = "new-setting-entitity-id";
            newSetting["rs"] = new JObject();
            newSetting["rs"]["key"] = "NewSetting";
            newSetting["rs"]["value"] = "2020-04-03T10:01:00Z";
            newSetting["rs"]["type"] = "string";

            dataStore.AddSetting(newSetting);
            var settingsFromList = new JArray(dataStore.rsKeyList.Where(s => s["rs"]["key"].Value<string>() == newSetting["rs"]["key"].Value<string>()));
            var addedSetting = settingsFromList[0];
            Assert.That(Equals(addedSetting["rs"]["key"], newSetting["rs"]["key"]));
            Assert.That(Equals(addedSetting["rs"]["value"], newSetting["rs"]["value"]));
            Assert.That(Equals(addedSetting["rs"]["type"], newSetting["rs"]["type"]));
        }

        [Test]
        public void AddSettingStringFormattedAsJson_AddsSettingToListAndDict()
        {
            var dataStore = RCTestUtils.GetDataStore();
            dataStore.rsKeyList = new JArray(RCTestUtils.rsListWithMetadata);

            var newSetting = new JObject();
            newSetting["metadata"] = new JObject();
            newSetting["metadata"]["entityId"] = "new-setting-entitity-id";
            newSetting["rs"] = new JObject();
            newSetting["rs"]["key"] = "NewSetting";
            newSetting["rs"]["value"] = "{\"a\":\"b\"}";
            newSetting["rs"]["type"] = "string";

            dataStore.AddSetting(newSetting);
            var settingsFromList = new JArray(dataStore.rsKeyList.Where(s => s["rs"]["key"].Value<string>() == newSetting["rs"]["key"].Value<string>()));
            var addedSetting = settingsFromList[0];
            Assert.That(Equals(addedSetting["rs"]["key"], newSetting["rs"]["key"]));
            Assert.That(Equals(addedSetting["rs"]["value"], newSetting["rs"]["value"]));
            Assert.That(Equals(addedSetting["rs"]["type"], newSetting["rs"]["type"]));
        }

        [Test]
        public void AddSetting_AddsJsonSettingToListAndDict()
        {
            var dataStore = RCTestUtils.GetDataStore();
            dataStore.rsKeyList = new JArray(RCTestUtils.rsListWithMetadata);

            var newSetting = new JObject();
            newSetting["metadata"] = new JObject();
            newSetting["metadata"]["entityId"] = "new-setting-entitity-id";
            newSetting["rs"] = new JObject();
            newSetting["rs"]["key"] = "NewSetting";
            newSetting["rs"]["value"] = "{'a':'b'}";
            newSetting["rs"]["type"] = "json";

            dataStore.AddSetting(newSetting);
            var settingsFromList = new JArray(dataStore.rsKeyList.Where(s => s["rs"]["key"].Value<string>() == newSetting["rs"]["key"].Value<string>()));
            var addedSetting = settingsFromList[0];
            var jsonKey = JObject.Parse(addedSetting["rs"]["value"].Value<string>());
            Assert.That(Equals(addedSetting["rs"]["key"], newSetting["rs"]["key"]));
            Assert.That(Equals(addedSetting["rs"]["value"], newSetting["rs"]["value"]));
            Assert.That(Equals(addedSetting["rs"]["type"], newSetting["rs"]["type"]));
            Assert.That(Equals(jsonKey["a"].Value<string>(), "b"));
        }

        [Test]
        public void DeleteSetting_DeletesSettingFromListAndDict()
        {
            var dataStore = RCTestUtils.GetDataStore();
            dataStore.rsKeyList = new JArray(RCTestUtils.rsListWithMetadata);

            dataStore.DeleteSetting(RCTestUtils.rsListWithMetadata[0]["metadata"]["entityId"].Value<string>());
            Assert.That(!dataStore.rsKeyList.Contains(RCTestUtils.rsListWithMetadata[0]));
        }

        [Test]
        public void UpdateSetting_UpdatesCorrectSetting()
        {
            var dataStore = RCTestUtils.GetDataStore();
            dataStore.rsKeyList = new JArray(RCTestUtils.rsListWithMetadata);

            var newSetting = new JObject();
            newSetting["metadata"] = new JObject();
            newSetting["metadata"]["entityId"] = "new-setting-entitity-id";
            newSetting["rs"] = new JObject();
            newSetting["rs"]["key"] = "NewSetting";
            newSetting["rs"]["value"] = "NewValue";
            newSetting["rs"]["type"] = "string";

            var oldSetting = (JObject)dataStore.rsKeyList[0];

            dataStore.UpdateSetting(oldSetting, newSetting);
            Assert.That(!dataStore.rsKeyList.Contains(oldSetting));
            Assert.That(dataStore.rsKeyList.Contains(newSetting));

            var settingsFromList = new JArray(dataStore.rsKeyList.Where(s => s["rs"]["key"].Value<string>() == newSetting["rs"]["key"].Value<string>()));
            var updatedSetting = settingsFromList[0];
            Assert.That(Equals(updatedSetting["rs"]["key"], newSetting["rs"]["key"]));
            Assert.That(Equals(updatedSetting["rs"]["value"], newSetting["rs"]["value"]));
            Assert.That(Equals(updatedSetting["rs"]["type"], newSetting["rs"]["type"]));
        }

        [Test]
        public void ConfigID_ShouldReturnConfigIDFromDataStore()
        {
            var dataStore = RCTestUtils.GetDataStore();
            Assert.That(string.IsNullOrEmpty(dataStore.configId));
            dataStore.configId = "someId";
            Assert.That(string.Equals(dataStore.configId, "someId"));
        }
    }

    internal static class RCTestUtils
    {
        public const string EntityIdBool = "bool0000-0000-0000-0000-000000000000";
        public const string EntityIdInt = "int00000-0000-0000-0000-000000000000";
        public const string EntityIdLong = "long0000-0000-0000-0000-000000000000";
        public const string EntityIdFloat = "float000-0000-0000-0000-000000000000";
        public const string EntityIdString = "string00-0000-0000-0000-000000000000";
        public const string EntityIdJson = "json0000-0000-0000-0000-000000000000";
        public const int IntValue = 1;
        public const float FloatValue = 1.0f;
        public const bool BoolValue = true;
        public const long LongValue = 32L;
        public const string StringValue = "test-string";
        public const string JsonValue = "{'keyA': [{ 'subkeyB': 'subValueB','subKeyC': {'subSubKeyD': 0}}]}";

        public const string currentEnvironment = "test-environment";
        public const string currentEnvironmentId = "test-environment-id";

        public static JObject settingBool = new JObject
        {
            {"key", "SettingBool"},
            {"type", "bool"},
            {"value", BoolValue}
        };

        public static JObject settingInt = new JObject
        {
            {"key", "SettingInt"},
            {"type", "int"},
            {"value", IntValue}
        };

        public static JObject settingLong = new JObject
        {
            {"key", "settingLong"},
            {"type", "long"},
            {"value", LongValue}
        };

        public static JObject settingFloat = new JObject
        {
            {"key", "SettingFloat"},
            {"type", "float"},
            {"value", FloatValue}
        };

        public static JObject settingString = new JObject
        {
            {"key", "SettingString"},
            {"type", "string"},
            {"value", StringValue}
        };

        public static JObject settingJson = new JObject
        {
            {"key", "settingJson"},
            {"type", "json"},
            {"value", JsonValue}
        };

        public static JObject settingBoolWithMetadata = new JObject
        {
            {"metadata", new JObject {{"entityId", EntityIdBool}}},
            {"rs", settingBool}
        };

        public static JObject settingIntWithMetadata = new JObject
        {
            {"metadata", new JObject {{"entityId", EntityIdInt}}},
            {"rs", settingInt}
        };

        public static JObject settingLongWithMetadata = new JObject
        {
            {"metadata", new JObject {{"entityId", EntityIdLong}}},
            {"rs", settingLong}
        };

        public static JObject settingFloatWithMetadata = new JObject
        {
            {"metadata", new JObject {{"entityId", EntityIdFloat}}},
            {"rs", settingFloat}
        };

        public static JObject settingStringWithMetadata = new JObject
        {
            {"metadata", new JObject {{"entityId", EntityIdString}}},
            {"rs", settingString}
        };

        public static JObject settingJsonWithMetadata = new JObject
        {
            {"metadata", new JObject {{"entityId", EntityIdJson}}},
            {"rs", settingJson}
        };


        // settings
        public static JArray rsListWithMetadata = new JArray
        {
            settingBoolWithMetadata,
            settingIntWithMetadata,
            settingLongWithMetadata,
            settingFloatWithMetadata,
            settingStringWithMetadata,
            settingJsonWithMetadata
        };

        public static JArray rsListNoMetadata = new JArray {
            settingBool,
            settingInt,
            settingLong,
            settingFloat,
            settingString,
            settingJson
        };

        // environments
        public static JObject environment1 = new JObject
        {
            {"id", "env-id-1"},
            {"project_id", "app-id-1"},
            {"name", "env-name-1"},
            {"description", "env-description-1"},
            {"created-at", "2019-07-10T23:15:14.000-0700"},
            {"updated-at", "2019-08-12T08:15:14.000+0430"},
            {"isDefault", true}
        };

        public static JObject environment2 = new JObject
        {
            {"id", "env-id-2"},
            {"project_id", "app-id-2"},
            {"name", "env-name-2"},
            {"description", "env-description-1"},
            {"created-at", "2019-07-10T23:15:14.000-0700"},
            {"updated-at", "2019-08-12T08:15:14.000+0430"},
            {"isDefault", false}
        };

        public static JArray environments = new JArray
        {
            environment1,
            environment2
        };

        public static RemoteConfigDataStore GetDataStore()
        {
            var dataStore = RemoteConfigUtilities.CheckAndCreateDataStore();
            dataStore.rsKeyList = new JArray();
            dataStore.environments = new JArray();
            dataStore.rsLastCachedKeyList = new JArray();
            dataStore.currentEnvironmentName = "Please create an environment.";
            dataStore.currentEnvironmentId = null;
            return dataStore;
        }
    }
}