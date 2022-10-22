# Changelog
All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [3.1.3] - 2022-06-17

- using ENABLE_CLOUD_SERVICES_ANALYTICS flag for platforms using Analytics within 2.x namespace
- Updated `com.unity.remote-config-runtime` dependency to `3.1.3`

## [3.1.2] - 2022-06-09

- Whitelisting platforms for getting userId from Analytics package within 2.x namespace
- Updated `com.unity.remote-config-runtime` dependency to `3.1.2`

## [3.1.1] - 2022-06-08

- Allowing projectId and userId params to be in the payload for consoles
- Updated `com.unity.remote-config-runtime` dependency to `3.1.1`

## [3.1.0] - 2022-05-31

- Updated `com.unity.services.core` dependency from `1.3.2` to `1.4.0`
- Updated `com.unity.remote-config-runtime` dependency to `3.1.0`
- Added 2.x namespace and corresponding API's for backward compatibility with 2.x implementation
- Promote Candidate Experimental package to Verified Production

## [3.0.0] - 2022-05-18

- Updated `com.unity.services.core` dependency from `1.3.1` to `1.3.2`
- Updated `com.unity.remote-config-runtime` dependency to `3.0.0`
- Updated SDK documentation
- Promote Candidate Experimental package to Verified Production

## [3.0.0-pre.16] - 2022-04-21

- Updated `com.unity.remote-config-runtime` dependency to `3.0.0-pre.30`
- Updated `com.unity.services.authentication` dependency from `1.0.0-pre.37` to `2.0.0`

## [3.0.0-pre.15] - 2022-04-08

- Updated `com.unity.remote-config-runtime` dependency to `3.0.0-pre.29`
- Updated `com.unity.services.core` dependency from `1.2.0` to `1.3.1`
- Updated `com.unity.nuget.newtonsoft-json` dependency from `3.0.1` to `3.0.2`

## [3.0.0-pre.14] - 2022-03-27

- Added `RemoteConfigService.Instance` as an access pattern in Remote Config Runtime instead of static class `ConfigManager`
- Added API Updater [Obsolete] for ConfigManager
- Remote Config Runtime namespace changed from `Unity.RemoteConfig` to `Unity.Services.RemoteConfig`
- Updated `com.unity.remote-config-runtime` dependency to `3.0.0-pre.28`

## [3.0.0-pre.13] - 2022-03-17

- Fixed bug for origin value if there is no internet
- Updated `com.unity.remote-config-runtime` dependency to `3.0.0-pre.27`

## [3.0.0-pre.12] - 2022-03-09

- Updated `com.unity.nuget.newtonsoft-json` dependency from `2.0.2` to `3.0.1`
- Updated `com.unity.remote-config-runtime` dependency to `3.0.0-pre.26`
- Updated documentation regarding metadata parameters within web response

## [3.0.0-pre.11] - 2022-02-16

- changed encoding from ASCII to UTF8 when reading from cache file
- Updated `com.unity.remote-config-runtime` dependency to `3.0.0-pre.25`

## [3.0.0-pre.10] - 2022-02-11

- Updated `com.unity.remote-config-runtime` dependency to `3.0.0-pre.23`
- Bypassing cache mechanism for consoles
- Refactored unit tests

## [3.0.0-pre.9] - 2022-01-13

- Updated `com.unity.remote-config-runtime` dependency to `3.0.0-pre.19`
- Fixed bug for successive requests with different config types
- Removed obsolete chunkedTransfer property from UnityWebRequest
- Added playerId and installationId in the request headers
- Added warnings if core / auth services are not initialized

## [3.0.0-pre.8] - 2021-12-06

- Updated `com.unity.remote-config-runtime` dependency to `3.0.0-pre.17`
- Bypassed exception error from auth if token or playerId is not available
- Retrieving projectId from UnityEngine Application.cloudProjectId static property
- Updated integration docs for using different configType

## [3.0.0-pre.7] - 2021-11-24

- Retrieving projectId from core services, removing preprocessor directives for consoles

## [3.0.0-pre.6] - 2021-11-23
- Updated `com.unity.remote-config-runtime` dependency to `3.0.0-pre.15`
- Bypassed exception error from core if there is no internet connection
- Updated `com.unity.nuget.newtonsoft-json` dependency from `2.0.0` to `2.0.2`
- Fixed bug for returning incorrect request origin in case of a failed request in Remote Config Runtime
- Updated ExampleSample and `CodeIntegration.md` docs

## [3.0.0-pre.5] - 2021-11-03
- Updated `com.unity.remote-config-runtime` dependency to `3.0.0-pre.13`
- Updated integration guides for FetchConfigs() and FetchConfigsAsync() methods from Remote Config Runtime
- Updated ExampleSample
- Cleaned up manifest.json
- Updated link to the dashboard

## [3.0.0-pre.4] - 2021-09-21
- Updated `com.unity.remote-config-runtime` dependency to `3.0.0-pre.10`
- Fixed links in Documentation, and updated integration guides
- Updated ExampleSample
- Removed unneeded APIs Docs with Docs Flag

## [3.0.0-pre.3] - 2021-08-30

- Updated Rules Documentation to Campaigns workflow
- Added `upgrade-guide.md` with directions for existing Remote Config users to upgrade to 3.0.x Remote Config
- Updated Environments Documentation to new Game Services Environment workflow
- Updated internal dependency for the Remote Config Runtime package for additional Platform Support

## [3.0.0-pre.2] - 2021-08-27

- Removed ability to Create, Update and Delete environments
- Added initial pull for environments and configs

## [3.0.0-pre.1] - 2021-07-28

- Minimum Editor version is now 2019.4 with the update of `com.unity.remote-config-runtime` dependency
- Platform support is currently restricted to PC, Mac, Android, iOS with the 3.0.x versions of Remote Config
- Updated internal dependency for the Remote Config Runtime package to support new Environments system
- Updated Documentation to include the new Initialization flow for Unity Game Services in `CodeIntegration.md`

## [2.1.1-exp.1] - 2021-06-28

- Updated internal dependency for the Remote Config Runtime package in order to support filtering settings

## [2.1.0-exp.2] - 2021-05-17

- Updated link to the dashboard

## [2.1.0-exp.1] - 2021-04-22

- Removed "rules" UI from the Editor package. We will continue to support this functionality on our Campaigns dashboard and our public API. Existing rules runtime delivery will not be impacted by this change.
- Please visit https://dashboard.unity3d.com/remote-config and https://dashboard.unity3d.com/campaigns for more info.
## [2.0.2-exp.1] - 2021-02-09

- Adjusted upm files for tests in isolation
- Added documentation for Apple Privacy Survey
- Added .sample.json file
## [2.0.1] - 2020-12-10

- Documented previously undocumented methods within RemoteConfigDataStore, RemoteConfigWebApiClient, SegmentationRulesTreeview and SettingsTreeview for the editor
- Updated yamato files

## [2.0.0] - 2020-10-21

- Promote Candidate Preview package to Verified Production

## [2.0.0-preview.2] - 2020-10-21

- Updated documentation and soft files

## [2.0.0-preview.1] - 2020-10-06

- Runtime is now separate depencency

## [1.4.0-preview.5] - 2021-01-26

- Adjusted upm files for tests in isolation
- Added documentation for Apple Privacy Survey
- Added .sample.json file

## [1.4.0-preview.4] - 2020-12-07

- Documented previously undocumented methods within IRCUnityWebRequest and ConfigManagerImpl for the runtime
- Documented previously undocumented methods within RemoteConfigDataStore, RemoteConfigWebApiClient, SegmentationRulesTreeview and SettingsTreeview for the editor
- Updated yamato files

## [1.4.0] - 2020-10-21

- Promote Candidate Preview package to Verified Production

## [1.4.0-preview.3] - 2020-10-21

- Updated documentation and soft files

## [1.4.0-preview.2] - 2020-10-03

- JSON setting now allows array of jsons as a value

## [1.4.0-preview.1] - 2020-08-28

- Added attribution for ConfigManager constructor

## [1.3.2-preview.10] - 2020-08-25

- Settings of type string can handle double quotes, so it can be used as json

## [1.2.4-preview.4] - 2020-08-25

- Settings of type string can handle double quotes, so it can be used as json

## [1.3.2-preview.9] - 2020-08-17

- Fixed bug where Json.net unexpectedly formats date-looking string to Date by default

## [1.2.4-preview.3] - 2020-08-17

- Fixed bug where Json.net unexpectedly formats date-looking string to Date by default

## [1.2.4-preview.2] - 2020-07-31

- Version bump for updates to internal pipeline fixes

## [1.2.4-preview.1] - 2020-07-31

- Upping the version as 1.2.3 verified exists

## [1.3.2-preview.8] - 2020-07-31

- UI fixes

## [1.2.3-preview.2] - 2020-07-31

- UI fixes

## [1.3.2-preview.7] - 2020-07-24

- Removed StateMachine from the codebase

## [1.3.2-preview.6] - 2020-07-16

- Resolved type casting bug when changing setting type from/to Json in Settings Config View
- Added members and methods to IRCUnityWebRequest to fully support UnityWebRequest
- Timeout added to the DoRequest method within ConfigManagerImpl in runtime

## [1.3.2-preview.5] - 2020-07-09

- Resolved intermittent null reference bug on Json modal close

## [1.3.2-preview.4] - 2020-07-02

- ConfigManager is now a thin wrapper for ConfigManagerImpl class
- ConfigManagerImpl constructor now accepts cache file names as parameters, useful for writing to different cache files if multiple instances of ConfigManager exist
- Resolved casting bug occuring when json settings are created on the dashboard and in the editor

## [1.3.2-preview.3] - 2020-06-18

- Updates to internal test fixtures, external requests are mocked now
- Text area in the JSON editor modal is now dynamically resized if component dropdown exist

## [1.3.2-preview.2] - 2020-05-28

- Fix for sending platform information in runtime.

## [1.2.3] - 2020-05-28

- Updates to internal test fixtures

## [1.2.3-preview.1] - 2020-05-28

- Fix for sending platform information in runtime.

## [1.3.2-preview.1] - 2020-05-21

- The JSON editor modal now supports json conversion of following types:
  - Text Assets
  - Scriptable Objects
  - Custom Scripts attached to Game Objects

## [1.3.1-preview.4] - 2020-05-13

- EditorWindow type is now passed to json modal, to be able to hold reference even to parent window types other than RemoteConfigWindow.
- Deleting Environment is now a single request; the service does the cleanup internally for corresponding rules and configs.

## [1.2.2] - 2020-05-13

- Updates to internal test fixtures
- Doc Updates

## [1.3.1-preview.3] - 2020-05-06

- Addressed bug where JSON keys edited from the dashboard were not being handled correctly in the editor and at runtime.

## [1.3.1-preview.2] - 2020-04-30

- Resolved UI bug for saving modal content for json settings

## [1.2.1] - 2020-04-28

- Promote Candidate Preview package to Verified Production

## [1.2.1-preview.1] - 2020-04-28

- Rev due to internal publishing error

## [1.3.1-preview.1] - 2020-04-24

- Completely removed DataManager in order to avoid unnecessary delegation of methods and events to DataStore
- Singleton pattern applied on DataStore scriptable object

## [1.2.0] - 2020-04-14

- Promote Candidate Preview package to Verified Production
- Docs Update

## [1.3.0-preview.2] - 2020-04-10

- Fixed bug that caused the add setting button to appear while viewing a rule, causing a null reference error when clicked
- Fixed bug in runtime for correct order for reading settings from different sources in ConfigManager
- Resolved CS0108 warning after importing package due to modal inheritance

## [1.3.0-preview.1] - 2020-03-27

- Added support for json as a new setting type.
- Added method GetJson() to utilize json in runtime.
- Added UI Json Window in Editor for manipulating json setting value (works via Edit button instead of text box).
- Added ability to format and validate json within the Json Window.
- Added ability to load json from a local file.

## [1.2.0-preview.4] - 2020-03-13

- Fixed bug where date was automatically formatted due to default JSON.net DateParseHandling

## [1.2.0-preview.3] - 2020-02-28

- Fixed UI bug where input box value for rollout percentage was not updated correctly if left empty.
- "Select all" button for settings in variant Rules is now a toggable checkbox
- Added "Select all" checkbox for settings in segmentation Rules

## [1.2.0-preview.2] - 2020-02-14

- Refactored the settings tree view into it's own public class, so it can be used to create custom GUI.
- All tree view columns should now autoresize by default.
- Added confirmation dialog when switching from rule type 'variant' to 'segmentation'
- Made Unity.RemoteConfig.Editor.RemoteConfigWebApiClient a public class.
- Config object in RuntimeConfig class gets initialised on creation
- Package now exposes all server errors as warnings in the console
- Moved the checkbox to add a setting to a segmentation rule to the left side of the treeview for consistency.
- Now the Remote Config window will block you from changing key names, types, and deleting settings that are being used in a rule to mimic the backend APIs.

## [1.2.0-preview.1] - 2020-02-03

- Fixed a bug where configId and environmentId were not pushed on creating / updating the rule.

## [1.2.0-preview] - 2020-01-31

- The configs and rules for a config are now displayed separately, and appropriately labeled.
- Added support for Variant rules in the Remote Config GUI.
	- Added a rule type dropdown next to the rule name, use this to change between segmentation and variant rules.
	- *NOTE*: When switching from a variant rule to a segmentation rule, it will result in a loss of local changes to variants.
	- Updates to the settings table for variant rules:
		- Added an `Add Variant` button that will add a variant.
		- Each variant has a name, and a variant weight.
		- The weights are `null` by default, which will result in a even balancing on the backend at time of assignment.
- Added a public get to `config` as a `JObject` to `RuntimeConfig`, so entire config can be retreived as a `JObject`.
- Added a "Select All" button to the variant rules settings view that will add all settings to the rule.
- A slew of UI stability fixes and refactors.

## [1.1.0-preview.4] - 2020-01-09

- Fixed a bug where the environment management window would incorrectly report an error when trying to set the Default environment as the default environment.

## [1.1.0-preview.3] - 2020-01-08

- Server errors for creation, updating and deleting of environments are now surfaced in Environment Window.
- Fixed bug with async read/write from and to the cache. File IO will be syncronous until a proper queueing system is in place.

## [1.1.0-preview.2] - 2019-12-27

- Fixed bug where adding Remote Config to a new project for the first time resulted in an endless loop of 404 errors.
- Added some text telling developers that when installing for the first time, they need to create their first environment.
- Added help button and Jexl syntax label to Rules Condition Field

## [1.1.0-preview.1] - 2019-12-16

- Environment Window can not be docked or resized now
- Disabling 'Update' button in Environment Window if both `is Default` checkbox and environment name is unchanged
- Disabling 'Create' button in Environment Window if environment name is empty
- Added tooltip for `is Default` checkbox in Environment Window if environment is default and checkbox is locked
- Added more intuitive icons for environment's `is Default` parameter in Remote Config Window
- Added button in Environment Window for copying Environment Id in the buffer
- Added loading states for Environment Window
- Remote Config Window is disabled while Environment Window is active
- Added a cancel button to the Environment Management Window
- Made the minimum window size a bit larger in the x to make room for new environment details GUI

## [1.1.0-preview] - 2019-12-12

- `NOTE:` This is a minor update which will cause changes to your Remote Config Environment which are not backward compatible with <1.1.x versions, please read the update notes prior to updating. https://docs.unity3d.com/Packages/com.unity.remote-config@1.1/manual/Environments.html
- `NOTE:` Added JSON.NET as a dependency for Editor and Runtime JSON Parsing, using package: com.unity.nuget.newtonsoft-json
- Added Developer Defined Environments additional information is included in the documentation for version 1.1.x
- Added environment id to the main Remote Config Window (RC Window)
- Added environment default status to the RC Window
- Added buttons on the RC Window to launch a popup dialog for Environment creation, editing, and deletion
- Environments will sync immediately upon modification

## [1.0.9-preview] - 2019-11-27

- Adjusted the delete button for settings to the right most column; previously it was on the left.
- Fixed bug where float values were parsed incorrectly

## [1.0.8] - 2019-11-14

- Added confirmation dialog for unsaved changes on closing Remote Config window and changing environments
- Fixed bug where `RemoteConfigDataManager` would try set the `RemoteConfigDataStoreAsset` dirty when it doesn't exist
- Added documentation for `unity.model` predefined attribute
- The Remote Config window now supports having duplicate setting key names, but the backend will still reject duplicate setting keys
- On a failed push to servers, the Remote Config window will re-fetch everything, so that users see the state of their environment on the server
- Added a button to the Remote Config window that will take you to the Remote Config dashboard
- Now the release environment will have "Default" next to it in the environment dropdown
- The cursor now turns into an editing cursor when hovering over editable fields in the Remote Config window
- Changed "-" remove button to trashcan icon
- Fixed bug on Windows which would cause the name of a rule to wrap onto the lower line when the Remote Config window is small

## [1.0.7] - 2019-10-10

- Boolean checkbox is now a dropdown with `True` and `False`
- The "Rollout Percentage" slider label is now an editable text field as well
- Added support for more unity attributes : `unity.cpu`, `unity.graphicsDeviceVendor` and `unity.ram`
- Fixed floating point rounding issue
- The editor window now saves the last fetched environment, configs and rules on editor close, and on playmode enter

## [1.0.6] - 2019-09-18

- Updated with Privacy ToS
- Updated Documentation to clarify install and integration instructions
- Updated a Screenshot in Documentation
- Fixed formatting in UI onboarding text

## [1.0.5] - 2019-09-14

- Updated License Documentation
- Removed unneeded markdown files to clean up for release

## [1.0.4] - 2019-09-04

- Added a label under start & end date/time to show the format that is expected.
- Added some warning messages in the RC management window:
	- Warning for when there are no settings
	- Warning for when there are no settings in a rule
- Changed the name of the "Default Config" to "Settings Config".
- Updated documentation to reflect the change of string value character limits from 1024 to 10000 characters.
- The enable/disable, delete, and priority fields are now hidden for the "Settings Config" in RC Management since they cannot be edited.

## [1.0.3] - 2019-08-22

- Fixed instability with the RC Management Window that would keep it in a loading state after pushing a new rule.

## [1.0.2] - 2019-08-21

- This is a version bump to prepare the package for verification for 2019.3

## [1.0.0] - 2019-08-13

- Added JSON.Net as a dependency of Remote Config
- Under the hood, Remote Config now uses a UnityWebRequest to fetch configs, then uses JSON.Net to parse the response.

## [0.3.2] - 2019-07-26

- The input field on each setting is now the type of that setting, so developers don't have to worry about having incorrect values.
- Added warning when a setting key name reached 255 characters.

## [0.3.1] - 2019-07-11

- Fixed bug that caused the Remote Config Management Window to not display correctly after a domain reload.
- Added support for settings of type 'long'
- Added slider control for rollout percentage
- The UI will now properly recover from any server-side errors.
- The UI will now reject duplicate rule names rather than depending on the Service API
- Moved Remote Config configuration requests to new URIs in the API Gateway
- Removed reliance on API Gateway URL that needs `-prd` at the end, so developers _always_ interact with production backend services.
- The "All Users" pseudo-rule, is now named what it actually is, "Default Config."

## [0.3.0] - 2019-07-01

- New runtime wrapper added for easier integration of Unity Remote Config. Please see documentation for more info.
    - New runtime classes are: `Unity.RemoteConfig.ConfigMananger` and `Unity.RemoteConfig.RuntimeConfig`.
    - `ConfigManager` is meant the be the primary way developers interact with Unity Remote Config.
- Fixed a bug where deleting a setting also deletes it from the rules that reference the deleted setting. Now, deleting a setting will not delete it from the corresponding rules.
- Removed errant Debug Logs.
- Window loader will now appear when settings are pushed (previously it was only happening if a rule was pushed as well).

## [0.2.1] - 2019-05-28

- Analytics no longer needs to be enabled in order to use Unity Remote Config. We now only require the project to be assiociated with an organization. In order to do so, go to Window > Services and follow the prompts.
- General UI and stability fixes
- Fixed bug which allowed settings to be deleted when they are in an active rule

## [0.2.0] - 2019-05-08

- Package name has changed to Unity Remote Config.
- Name spaces are now: `Unity.RemoteConfig`
   - `UnityEditor.RemoteSettings` -> `Unity.RemoteConfig.Editor` for example

## [0.1.0] - 2019-05-06

- startDate and endDate have been added to rules to allow for calendarization
- startDate and endDate Default is null
- Supported format is ISO8601 in the format "yyyy-MM-dd'T'HH:mm:ssZ" example: "2019-04-29T15:01:43Z"
- .AI prefix removed from codebase, new namespaces name is now UnityEditor.RemoteSettings
- bug fixes
- better handling of server response errors
- The type field for Remote Settings now have a dropdown containing all supported types
- Added a rule priority field, which determines which rules will overwrite the values of other rules.
    - Rule priority ranges from 0-1000, 0 being the highest priority rule (will overwrite all other rule values), and 1000 is the lowest priority rule.
    - If two or more rules have the same priority, they will be evaluated from newest to oldest (the oldest rule will overwrite the matching keys of newer rules)
    - The rule priority can be set by the new column in the RS Management window in the list of rules.
- The workflow for adding a Remote Setting to a rule has changed: No more dropdown. Now, on a Rule, all Remote Settings will be visible, and in order to add it to a rule, just click the checkbox to the left of the key. And to remove it, just uncheck the same box.
- GUI performance improvements, and general stability.
- Removed ability to sort from all RS Management window headers, since it didn't work

## [0.0.6] - 2019-04-19

- The Rules Management and Remote Settings Management Windows have been merged. The new window can be found through Window > Remote Settings > Remote Settings Management
- Deafult Remote Settings are now under the "All Users" rule.
- Pull/Push now syncs both Rules and Remote Settings
- The Remote Settings Data Stores are now all merged into one RemoteSettingsDataStore
- RemoteSettingsDataManager can be used to access the RemoteSettingsDataStore in a secure way
- Major stability fixes.
- Updated message when Analytics is disabled to reflect no longer needing a project secret key.

## [0.0.5] - 2019-04-17

- UI is now blocked while web operations are in progress
- Stability and bug fixes

## [0.0.4] - 2019-03-29

- Added the capability to add, remove, and edit rules
- Updated namespaces: all editor code is now under `UnityEditor.AI.RemoteSettings`, and runtime code is now under `UnityEngine.AI` instead of `UnityEngine.AI.RemoteSettings`.
- General stability and bug fixes

## [0.0.3] - 2019-03-19

- Added more Unit Tests throughout UI code
- Implemented TreeView in the Remote Settings Editor Window
- General optimizations
- Added the ability to push Remote Settings to any environment from the Editor
- Moved UI under Window > Remote Settings > Remote Settings Management

## [0.0.2] - 2019-02-26

### Added
- Local overrides checkbox in RS Editor Window, that will force the editor to use local values instead of the cloud.
- Created initial RemoteSettings runtime API wrapper under `UnityEngine.AI.RemoteSettings`
- Added Unit Tests through Unity Test Runner. Each test class needs to run individually due to a limitation in the PrebuildSetup step of Unity Test Runner. Note: The RS Editor window needs to be closed in order to run the tests.

## [0.0.1] - 2019-02-20

### Added
- UI under Window > Unity Analytics > Remote Settings
- Added buttons for pushing settings, but not hooked up to APIs (not ready)
- Added ability to create and delete keys locally
- Added ability to update key names, types, and values locally
