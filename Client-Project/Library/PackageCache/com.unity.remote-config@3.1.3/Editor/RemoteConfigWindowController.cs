using System;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;
using Newtonsoft.Json.Linq;

namespace Unity.RemoteConfig.Editor
{
    internal class RemoteConfigWindowController
    {
        public event Action remoteSettingsStoreChanged;
        public event Action environmentChanged;

        RemoteConfigDataStore m_DataStore;

        bool m_PostSettingsEventSubscribed = false;
        bool m_PutConfigsEventSubscribed = false;
        bool m_PostConfigEventSubscribed;
        bool m_IsLoading = false;
        public string environmentId { get { return m_DataStore.currentEnvironmentId; } }
        public string environmentName { get { return m_DataStore.currentEnvironmentName; } }
        public bool environmentIsDefault { get { return m_DataStore.currentEnvironmentIsDefault; } }

        // DialogBox variables
        public readonly string k_RCWindowName = "Remote Config";
        public readonly string k_RCDialogUnsavedChangesTitle = "Unsaved Changes";
        public readonly string k_RCDialogUnsavedChangesMessage = "You have unsaved changes. \n \n" +
                                                                 "If you want them saved, click 'Cancel' then 'Push'.\n" +
                                                                 "Otherwise, click 'OK' to discard the changes \n";
        public readonly string k_RCDialogUnsavedChangesOK = "OK";
        public readonly string k_RCDialogUnsavedChangesCancel = "Cancel";

        public bool isLoading
        {
            get { return m_IsLoading; }
            set { m_IsLoading = value; }
        }

        public RemoteConfigWindowController(bool shouldFetchOnInit = true, bool windowOpenOnInit = false)
        {
            m_DataStore = RemoteConfigUtilities.CheckAndCreateDataStore();
            m_DataStore.RemoteSettingDataStoreChanged += OnRemoteSettingDataStoreChanged;
            m_DataStore.EnvironmentChanged += OnEnvironmentChanged;
            RemoteConfigWebApiClient.rcRequestFailed += OnFailedRequest;
            RemoteConfigWebApiClient.fetchEnvironmentsFinished += FetchDefaultEnvironment;
            if (shouldFetchOnInit && windowOpenOnInit)
            {
                FetchEnvironments();
            }
        }

        private void OnFetchEnvironmentsFinished(JArray environments)
        {
            SetEnvironmentData(environments);
        }

        public void SetDataStoreDirty()
        {
            EditorUtility.SetDirty(m_DataStore);
        }

        private bool SetEnvironmentData(JArray environments)
        {
            if (environments != null && environments.Count > 0)
            {
                m_DataStore.environments = environments;
                var currentEnvironment = LoadEnvironments(environments, environmentName);
                m_DataStore.SetCurrentEnvironment(currentEnvironment);
                m_DataStore.SetLastSelectedEnvironment(currentEnvironment["name"].Value<string>());
                return true;
            }
            else
            {
                m_DataStore.environments = environments;
            }

            return false;
        }

        public JObject LoadEnvironments(JArray environments, string currentEnvName)
        {
            if (environments.Count > 0)
            {
                var currentEnvironment = environments[0];
                foreach (var environment in environments)
                {
                    if (environment["name"].Value<string>() == currentEnvName)
                    {
                        currentEnvironment = environment;
                        break;
                    }
                }
                return (JObject)currentEnvironment;
            }
            else
            {
                Debug.LogWarning("No environments loaded. Please restart the Remote Config editor window");
                return new JObject();
            }
        }

        public JArray GetSettingsList()
        {
            var settingsList = m_DataStore.rsKeyList ?? new JArray();
            return (JArray)settingsList.DeepClone();
        }

        public JArray GetLastCachedKeyList()
        {
            var settingsList = m_DataStore.rsLastCachedKeyList ?? new JArray();
            return (JArray)settingsList.DeepClone();
        }

        public int GetEnvironmentsCount()
        {
            return m_DataStore.environments.Count;
        }

        public GenericMenu BuildPopupListForEnvironments()
        {
            var menu = new GenericMenu();

            for (int i = 0; i < GetEnvironmentsCount(); i++)
            {
                string name = m_DataStore.environments[i]["name"].Value<string>();
                menu.AddItem(new GUIContent(name), name == environmentName, EnvironmentSelectionCallback, name);
            }

            return menu;
        }

        private void EnvironmentSelectionCallback(object obj)
        {
            var environmentName = (string)obj;
            var defaultEnvironmentIndex = -1;
            for (int i = 0; i < m_DataStore.environments.Count; i++)
            {
                if (((JObject)m_DataStore.environments[i])["name"].Value<string>() == environmentName)
                {
                    defaultEnvironmentIndex = i;
                }
            }
            var env = (JObject)m_DataStore.environments[defaultEnvironmentIndex];

            //TODO: Move this logic checking if changes to env and fetch settings if current settings are not modified
            if (CompareJArraysEquality(GetSettingsList(), GetLastCachedKeyList()))
            {
                m_DataStore.SetCurrentEnvironment(env);
                FetchSettings(m_DataStore.environments);
            }
            else
            {
                if (EditorUtility.DisplayDialog(k_RCDialogUnsavedChangesTitle, k_RCDialogUnsavedChangesMessage, k_RCDialogUnsavedChangesOK, k_RCDialogUnsavedChangesCancel))
                {
                    m_DataStore.SetCurrentEnvironment(env);
                    FetchSettings(m_DataStore.environments);
                }
            }
        }

        public void Fetch()
        {
            m_IsLoading = true;
            FetchEnvironments();
        }

        private void FetchEnvironments()
        {
            RemoteConfigWebApiClient.fetchEnvironmentsFinished += FetchSettings;
            try
            {
                RemoteConfigWebApiClient.FetchEnvironments(Application.cloudProjectId);
            }
            catch
            {
                RemoteConfigWebApiClient.fetchEnvironmentsFinished -= FetchSettings;
                DoCleanUp();
            }
        }
        private void FetchDefaultEnvironment(JArray environments)
        {
            if(environments.Count > 0)
            {
                RemoteConfigWebApiClient.fetchDefaultEnvironmentFinished += OnFetchDefaultEnvironmentFinished;
                try
                {
                    RemoteConfigWebApiClient.FetchDefaultEnvironment(Application.cloudProjectId);
                }
                catch
                {
                    DoCleanUp();
                }
            }
            else
            {
                DoCleanUp();
            }
        }

        private void FetchSettings(JArray environments)
        {
            RemoteConfigWebApiClient.fetchEnvironmentsFinished -= FetchSettings;
            if (SetEnvironmentData(environments))
            {
                RemoteConfigWebApiClient.fetchConfigsFinished += OnFetchRemoteSettingsFinished;
                try
                {
                    RemoteConfigWebApiClient.FetchConfigs(Application.cloudProjectId, m_DataStore.currentEnvironmentId);
                }
                catch
                {
                    RemoteConfigWebApiClient.fetchConfigsFinished -= OnFetchRemoteSettingsFinished;
                    DoCleanUp();
                }
            }
            else
            {
                DoCleanUp();
            }
        }

        public void Push()
        {

            if (m_DataStore.dataStoreStatus == RemoteConfigDataStore.m_DataStoreStatus.Error)
            {
                Debug.LogError("There are errors in the Local Data Rules and or Settings please resolve them before pushing changes");
            }
            else
            {
                string environmentId = m_DataStore.currentEnvironmentId;
                if (string.IsNullOrEmpty(m_DataStore.configId))
                {
                    RemoteConfigWebApiClient.postConfigRequestFinished += OnConfigPostFinishedPushHandler;
                    m_PostConfigEventSubscribed = true;
                }
                PushSettings(environmentId);
            }
        }

        private void OnConfigPostFinishedPushHandler(string configId)
        {
            string environmentId = m_DataStore.currentEnvironmentId;
            m_DataStore.configId = configId;
            if (m_PostConfigEventSubscribed)
            {
                RemoteConfigWebApiClient.postConfigRequestFinished -= OnConfigPostFinishedPushHandler;
                m_PostConfigEventSubscribed = false;
            }
        }

        public void AddSetting()
        {
            var jSetting = new JObject();
            jSetting["metadata"] = new JObject();
            jSetting["metadata"]["entityId"] = Guid.NewGuid().ToString();
            jSetting["rs"] = new JObject();
            jSetting["rs"]["key"] = "Setting" + m_DataStore.settingsCount.ToString();
            jSetting["rs"]["value"] = "";
            jSetting["rs"]["type"] = "";
            m_DataStore.AddSetting(jSetting);
        }

        private void OnSettingsRequestFinished()
        {
            m_DataStore.rsLastCachedKeyList = new JArray(m_DataStore.rsKeyList);
            DoCleanUp();
        }

        private void OnPostConfigRequestFinished(string configId)
        {
            m_DataStore.configId = configId;
            DoCleanUp();
        }

        private void OnFailedRequest(long errorCode, string errorMsg)
        {
            DoCleanUp();
        }

        JArray StripMetadataFromRSList(JArray rsJArray)
        {
            var strippedJArray = new JArray();
            for (int i = 0; i < rsJArray.Count; i++)
            {
                strippedJArray.Add(rsJArray[i]["rs"]);
            }
            return strippedJArray;
        }

        private void PushSettings(string environmentId)
        {
            m_IsLoading = true;
            if (string.IsNullOrEmpty(m_DataStore.configId))
            {
                RemoteConfigWebApiClient.postConfigRequestFinished += OnPostConfigRequestFinished;
                m_PostSettingsEventSubscribed = true;
                try
                {
                    RemoteConfigWebApiClient.PostConfig(Application.cloudProjectId, environmentId, StripMetadataFromRSList(m_DataStore.rsKeyList));
                }
                catch
                {
                    DoCleanUp();
                }
            }
            else
            {
                RemoteConfigWebApiClient.settingsRequestFinished += OnSettingsRequestFinished;
                m_PutConfigsEventSubscribed = true;
                try
                {
                    RemoteConfigWebApiClient.PutConfig(Application.cloudProjectId, environmentId, m_DataStore.configId, StripMetadataFromRSList(m_DataStore.rsKeyList));
                }
                catch
                {
                    DoCleanUp();
                }
            }
        }

        private void OnFetchRemoteSettingsFinished(JObject config)
        {
            DoCleanUp();
            RemoteConfigWebApiClient.fetchConfigsFinished -= OnFetchRemoteSettingsFinished;
            m_DataStore.config = config;
            m_DataStore.rsLastCachedKeyList = new JArray(m_DataStore.rsKeyList);
        }

        private void OnRemoteSettingDataStoreChanged()
        {
            remoteSettingsStoreChanged?.Invoke();
        }

        private void OnEnvironmentChanged()
        {
            m_IsLoading = true;
            environmentChanged?.Invoke();
        }

        private void OnFetchDefaultEnvironmentFinished(string defaultEnvironmentId)
        {
            RemoteConfigWebApiClient.fetchDefaultEnvironmentFinished -= OnFetchDefaultEnvironmentFinished;
            m_DataStore.SetDefaultEnvironment(defaultEnvironmentId);
        }

        private void DoCleanUp()
        {
            if (RemoteConfigWebApiClient.webRequestsAreDone)
            {
                if (m_PostSettingsEventSubscribed)
                {
                    RemoteConfigWebApiClient.postConfigRequestFinished -= OnPostConfigRequestFinished;
                    m_PostSettingsEventSubscribed = false;
                }
                if (m_PutConfigsEventSubscribed)
                {
                    RemoteConfigWebApiClient.settingsRequestFinished -= OnSettingsRequestFinished;
                    m_PutConfigsEventSubscribed = false;
                }
                if (m_PostConfigEventSubscribed)
                {
                    RemoteConfigWebApiClient.postConfigRequestFinished -= OnConfigPostFinishedPushHandler;
                    m_PostConfigEventSubscribed = false;
                }
                m_IsLoading = false;
            }
        }

        public bool CompareJArraysEquality(JArray objectListNew, JArray objectListOld)
        {
            return RemoteConfigUtilities.CompareJArraysEquality(objectListNew, objectListOld);
        }

        public void UpdateRemoteSetting(JObject oldItem, JObject newItem)
        {
            m_DataStore.UpdateSetting(oldItem, newItem);
        }

        public void DeleteRemoteSetting(string entityId)
        {
            m_DataStore.DeleteSetting(entityId);
        }

    }
}
