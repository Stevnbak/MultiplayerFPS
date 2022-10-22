using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.RemoteConfig.Editor.Core;
using UnityEditor;
using UnityEditor.Connect;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using Unity.RemoteConfig.Editor.UIComponents;

namespace Unity.RemoteConfig.Editor
{
    internal class RemoteConfigWindow : EditorWindow
    {
        //Window state
        public bool shouldFetchOnInit;
        public bool windowOpenOnInit;
        public string orgForeignKey;
        [NonSerialized] bool m_Initialized;
        SettingsTreeview settingsTreeview;
        [SerializeField] TreeViewState m_SettingsTreeViewState;
        [SerializeField] MultiColumnHeaderState m_SettingsMultiColumnHeaderState;
        RemoteConfigWindowController m_Controller;
        
        //GUI Content
        GUIContent m_pullButtonContent = new GUIContent("Pull");
        GUIContent m_pushButtonContent = new GUIContent("Push");
        GUIContent m_loadingMessage = new GUIContent("Loading, please wait.");
        GUIContent m_EnvironmentsLabelContent = new GUIContent("Environment Name:");
        GUIContent m_EnvironmentsIdContent = new GUIContent("Environment Id:");
        GUIContent m_EnvironmentsIsDefaultContent = new GUIContent("Default:");
        GUIContent m_AnalyticsNotEnabledContent = new GUIContent("To get started with Unity Remote Config, you must first link your project to a Unity Cloud Project ID. A Unity Cloud Project ID is an online identifier which is used across all Unity Services. These can be created within the Services window itself, or online on the Unity Services website. The simplest way is to use the Services window within Unity, as follows: \nTo open the Services Window, go to Window > General > Services.\nNote: using Unity Remote Config does not require that you turn on any additional, individual cloud services like Analytics, Ads, Cloud Build, etc.");

        //UI Style variables
        const float k_LineHeight = 22f;
        const float k_LineHeightBuffer = k_LineHeight - 2;
        const float k_LinePadding = 5f;
        const string m_NoSettingsContent = "To get started, please add a setting";
        private GUIStyle guiStyleLabel = new GUIStyle();
        private GUIStyle guiStyleSubLabel = new GUIStyle();

        Rect toolbarRect
        {
            get
            {
                return new Rect(0, 0, position.width, (k_LineHeight * 2));
            }
        }

        Rect configsViewRect
        {
            get
            {
                return new Rect(0, toolbarRect.y, toolbarRect.width, k_LineHeight * 2);
            }
        }

        Rect configHeaderRect
        {
            get
            {
                return new Rect(0, toolbarRect.y, toolbarRect.width, k_LineHeight);
            }
        }

        Rect configTableRect
        {
            get
            {
                return new Rect(0, configHeaderRect.y + configHeaderRect.height, toolbarRect.width, k_LineHeight);
            }
        }

        Rect detailsViewRect
        {
            get
            {
                return new Rect(1f, toolbarRect.height + k_LinePadding, position.width, position.height - (k_LineHeight * 2.35f));
            }
        }

        [MenuItem("Window/Remote Config")]
        public static void GetWindow()
        {
            var RCWindow = GetWindow<RemoteConfigWindow>();
            RCWindow.titleContent = new GUIContent("Remote Config");
            RCWindow.minSize = new Vector2(600, 300);
            RCWindow.windowOpenOnInit = true;
            RCWindow.Focus();
            RCWindow.Repaint();
        }
        
        private void OnEnable()
        {
            if (AreServicesEnabled())
            {
                InitIfNeeded();
            }
        }

        private void OnDisable()
        {
            if (m_Controller != null)
            {
                m_Controller.SetDataStoreDirty();

                try
                {
                    if(settingsTreeview != null)
                    {
                        settingsTreeview.OnSettingChanged -= SettingsTreeview_OnConfigSettingsChanged;
                    }
                    m_Controller.remoteSettingsStoreChanged -= OnRemoteSettingsStoreChanged;
                    EditorApplication.quitting -= m_Controller.SetDataStoreDirty;
                    EditorApplication.playModeStateChanged -= EditorApplication_playModeStateChanged;
                }
#pragma warning disable CS0168 // Variable is declared but never used
                catch (NullReferenceException e)
#pragma warning restore CS0168 // Variable is declared but never used
                { }
            }
        }

        private void OnGUI()
        {
            if (AreServicesEnabled(true))
            {
                InitIfNeeded();

                EditorGUI.BeginDisabledGroup(IsLoading());

                DrawToolbar(toolbarRect);
                DrawConfigsSettingsPane(detailsViewRect);

                EditorGUI.EndDisabledGroup();
                AddLoadingMessage();
            }
        }

        private void DrawToolbar (Rect toolbarSize)
        {
            var currentY = toolbarSize.y;
            DrawEnvironmentDropdown(currentY);
            DrawPushPullButtons(currentY);
            currentY += k_LineHeight;

            DrawEnvironmentDetails(currentY);

            if (GUI.Button(new Rect(position.width - (position.width / 5), currentY + 2, (position.width / 5) - 5, 20), "View in Dashboard"))
            {
                if (string.IsNullOrEmpty(m_Controller.environmentId))
                {
                    Help.BrowseURL(string.Format(RemoteConfigEnvConf.dashboardEnvironmentsPath, Application.cloudProjectId, orgForeignKey));
                }
                else
                {
                    Help.BrowseURL(string.Format(RemoteConfigEnvConf.dashboardConfigsPath, Application.cloudProjectId, m_Controller.environmentId, orgForeignKey));
                }
            }
        }

        private void InitIfNeeded()
        {
            if (!m_Initialized)
            {
                settingsTreeview = new SettingsTreeview();
                settingsTreeview.OnSettingChanged += SettingsTreeview_OnConfigSettingsChanged;
                m_Controller = new RemoteConfigWindowController();
                EditorApplication.quitting += m_Controller.SetDataStoreDirty;
                EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;

                if (m_SettingsTreeViewState == null)
                {
                    m_SettingsTreeViewState = new TreeViewState();
                }

                settingsTreeview.settingsList = m_Controller.GetSettingsList();
                settingsTreeview.activeSettingsList = m_Controller.GetSettingsList();

                m_Controller.remoteSettingsStoreChanged += OnRemoteSettingsStoreChanged;

                // get an orgForeignKey via reflection
                Assembly assembly = Assembly.GetAssembly(typeof(UnityEditor.EditorWindow));
                var unityConnectInstance = assembly.CreateInstance("UnityEditor.Connect.UnityConnect", false, BindingFlags.NonPublic | BindingFlags.Instance, null, null, null, null);

                Type t = unityConnectInstance.GetType();
                var projectInfo = t.GetProperty("projectInfo").GetValue(unityConnectInstance, null);
                Type projectInfoType = projectInfo.GetType();
                orgForeignKey = projectInfoType.GetProperty("organizationForeignKey").GetValue(projectInfo, null) as string;

                m_Initialized = true;
                m_Controller.Fetch();
            }
        }

        private void SettingsTreeview_OnConfigSettingsChanged(JObject arg1, JObject arg2)
        {
            if(arg1 == null && arg2 != null)
            {
                //new setting added
                m_Controller.AddSetting();
            }
            else if(arg2 == null && arg1 != null)
            {
                //setting removed/deleted
                OnDeleteSetting(arg1["metadata"]["entityId"].Value<string>());
            }
            else if(arg1 != null && arg2 != null)
            {
                //update the setting
                OnUpdateSetting(arg1, arg2);
            }
        }

        private void OnDestroy()
        {
            if (m_Controller != null)
            {
                if (!(m_Controller.CompareJArraysEquality(m_Controller.GetSettingsList(),
                          m_Controller.GetLastCachedKeyList())))
                {
                    if (!EditorUtility.DisplayDialog(m_Controller.k_RCDialogUnsavedChangesTitle,
                        m_Controller.k_RCDialogUnsavedChangesMessage,
                        m_Controller.k_RCDialogUnsavedChangesOK,
                        m_Controller.k_RCDialogUnsavedChangesCancel))
                    {
                        CreateNewRCWindow();
                    }
                }
            }
        }

        private void CreateNewRCWindow()
        {
            RemoteConfigWindow newWindow = (RemoteConfigWindow) CreateInstance(typeof(RemoteConfigWindow));
            newWindow.titleContent.text = m_Controller.k_RCWindowName;
            newWindow.shouldFetchOnInit = true;
            newWindow.Show();
        }

        private void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        {
            if(obj == PlayModeStateChange.EnteredPlayMode)
            {
                m_Controller.SetDataStoreDirty();
            }
        }

        private bool AreServicesEnabled(bool calledFromOnGui = false)
        {
            if (string.IsNullOrEmpty(CloudProjectSettings.projectId) || string.IsNullOrEmpty(CloudProjectSettings.organizationId))
            {
                if(calledFromOnGui)
                {
                    GUIStyle style = GUI.skin.label;
                    style.wordWrap = true;
                    EditorGUILayout.LabelField(m_AnalyticsNotEnabledContent, style);
                }
                return false;
            }
            return true;
        }

        private void OnDeleteSetting(string entityId)
        {
            m_Controller.DeleteRemoteSetting(entityId);
        }

        private void OnUpdateSetting(JObject oldItem, JObject newitem)
        {
            m_Controller.UpdateRemoteSetting(oldItem, newitem);
        }

        private void AddLoadingMessage()
        {
            if (IsLoading())
            {
                GUI.Label(new Rect(0, position.height - k_LineHeight, position.width, k_LineHeight), m_loadingMessage);
            }
        }

        private bool IsLoading()
        {
            bool isLoading = m_Controller.isLoading;
            settingsTreeview.isLoading = isLoading;
            return isLoading;
        }
        
        private void OnRemoteSettingsStoreChanged()
        {
            settingsTreeview.settingsList = m_Controller.GetSettingsList();
            settingsTreeview.activeSettingsList = m_Controller.GetSettingsList();
        }

        private void DrawEnvironmentDropdown(float currentY)
        {
            var totalWidth = position.width / 2;
            EditorGUI.BeginDisabledGroup(m_Controller.GetEnvironmentsCount() <= 1 || IsLoading());
            GUI.Label(new Rect(0, currentY, 120, 20), m_EnvironmentsLabelContent);
            var environmentName = string.IsNullOrEmpty(m_Controller.environmentName) ? "" : m_Controller.environmentName;
            GUIContent ddBtnContent = new GUIContent(Regex.Replace(environmentName, @"[/]+", "\u2215"));
            Rect ddRect = new Rect(120 , currentY, totalWidth - 120, 20);
            if (GUI.Button(ddRect, ddBtnContent, EditorStyles.popup))
            {
                m_Controller.BuildPopupListForEnvironments().DropDown(ddRect);
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawEnvironmentDetails(float currentY)
        {
            var envIdLabelLength = 120;
            var envIdValueLength = 260;
            var envIsDefaultLength = 58;
            var isDefaultIcon = m_Controller.environmentIsDefault ? EditorGUIUtility.FindTexture("CollabNew") : EditorGUIUtility.FindTexture("Grid.BoxTool");
            GUI.Label(new Rect(0, currentY, envIdLabelLength, k_LineHeight), m_EnvironmentsIdContent);
            var envIdRect = new Rect(envIdLabelLength, currentY, envIdValueLength, k_LineHeight);
            EditorGUI.SelectableLabel(envIdRect, m_Controller.environmentId);
            GUI.Label(new Rect(envIdLabelLength + envIdValueLength, currentY, envIsDefaultLength, 20), m_EnvironmentsIsDefaultContent);
            GUI.DrawTexture(new Rect(envIdLabelLength + envIdValueLength + envIsDefaultLength, currentY + 2, 12, 12), isDefaultIcon);
        }

        private void DrawNoEnvironmentsWarning(float currentY)
        {
            showMessage(new Rect(0, currentY, 380f, k_LineHeight), "Please click Create above to create your first environment.");
        }

        private void DrawPushPullButtons(float currentY)
        {
            float boundingBoxPadding = 8;
            var paddedRect = new Rect((position.width / 2) + boundingBoxPadding, currentY,(position.width / 2) - (2 * boundingBoxPadding), 20);
            var buttonWidth = (paddedRect.width / 4);

            EditorGUI.BeginDisabledGroup(m_Controller.GetEnvironmentsCount() == 0);

            if (GUI.Button(new Rect(paddedRect.x + 2*(buttonWidth + (2 * boundingBoxPadding)), paddedRect.y, buttonWidth - (2 * boundingBoxPadding), 20),
                m_pushButtonContent))
            {
                m_Controller.Push();
            }

            if (GUI.Button(new Rect(paddedRect.x + 3*(buttonWidth) + (2.2f*boundingBoxPadding), paddedRect.y, buttonWidth - (2 * boundingBoxPadding), 20),
                    m_pullButtonContent))
            {
                m_Controller.Fetch();
            }

        }

        private void DrawConfigsSettingsPane(Rect configsDetailsRect)
        {
            DrawConfigsSettingsTreeView(new Rect(configsDetailsRect.x, configsDetailsRect.y, configsDetailsRect.width, configsDetailsRect.height));
        }

        void DrawConfigsSettingsTreeView(Rect treeViewRect)
        {
            settingsTreeview.enableEditingSettingsKeys = true;
            settingsTreeview.settingsList = m_Controller.GetSettingsList();
            settingsTreeview.activeSettingsList = m_Controller.GetSettingsList();

            //TODO: Figure out what to do here
            if (!m_Controller.GetSettingsList().Any())
            {
                settingsTreeview.settingsList = null;
                var messageRect = new Rect(treeViewRect.x + 1f, treeViewRect.y + k_LineHeight + 6f, treeViewRect.width - 3f, k_LineHeight);
                showMessage(messageRect, m_NoSettingsContent);
            }
            settingsTreeview.OnGUI(treeViewRect);
        }

        private void showMessage(Rect messageRect, string messageText)
        {
            EditorGUI.HelpBox(messageRect, messageText, MessageType.Warning);
        }

    }

}
