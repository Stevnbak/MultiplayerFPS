# Remote Config upgrade guide

* Do not upgrade this package if you're using Unity Editor 2018 LTS or Tech Stream 2019. Versions below 2019.4 (LTS) are not supported.

* There's a new initialization for Unity Game Services which will require code updates for those who already implemented Remote Config in their Unity Project. Please review the **Initialization** section of [CodeIntegration](CodeIntegration.md) for the new code to be added to your implementation.

* Upgrading a project from Unity 2018 or pre LTS 2019; developers no longer need to set the Scripting Runtime in the Editor. The option to change the Scripting Runtime version was removed after Unity 2019.2 https://docs.unity.cn/Manual/UpgradeGuide2019LTS.html#NET

* The latest information on the Service features can be found in the [REST API documentation](https://services.docs.unity.com/remote-config/v1)

For a full list of changes and updates in this version, see the [Remote Config Runtime](../changelog/CHANGELOG.html) package changelog.
