using System.Xml;
using NUnit.Framework;

namespace Unity.Notifications.Tests
{
    internal class PostprocessorTests
    {
        [Test]
        public void DummmyTest()
        {
            Assert.AreEqual(true, true);
        }

        const string kManifestTemplate = @"<?xml version=""1.0"" encoding=""utf-8""?>
<manifest xmlns:android=""http://schemas.android.com/apk/res/android"" package=""com.UnityTestRunner.UnityTestRunner"" xmlns:tools=""http://schemas.android.com/tools"" android:installLocation=""preferExternal"">
  <supports-screens android:smallScreens=""true"" android:normalScreens=""true"" android:largeScreens=""true"" android:xlargeScreens=""true"" android:anyDensity=""true"" />
  <application android:theme=""@style/UnityThemeSelector"" android:icon=""@mipmap/app_icon"" android:label=""@string/app_name"" android:isGame=""true"" android:banner=""@drawable/app_banner"">
    <activity android:label=""@string/app_name"" android:screenOrientation=""fullSensor"" android:launchMode=""singleTask"" android:configChanges=""mcc|mnc|locale|touchscreen|keyboard|keyboardHidden|navigation|orientation|screenLayout|uiMode|screenSize|smallestScreenSize|fontScale|layoutDirection|density"" android:hardwareAccelerated=""false"" android:name=""com.UnityTestRunner.UnityTestRunner.UnityPlayerActivity"">
      <intent-filter>
        <action android:name=""android.intent.action.MAIN"" />
        <category android:name=""android.intent.category.LAUNCHER"" />
        <category android:name=""android.intent.category.LEANBACK_LAUNCHER"" />
      </intent-filter>
      <meta-data android:name=""unityplayer.UnityActivity"" android:value=""true"" />
    </activity>
    <meta-data android:name=""unity.build-id"" android:value=""8a616d3b-0433-49d8-bbaf-fd1415e8701e"" />
    <meta-data android:name=""unity.splash-mode"" android:value=""0"" />
    <meta-data android:name=""unity.splash-enable"" android:value=""True"" />{0}
  </application>
  <uses-feature android:glEsVersion=""0x00020000"" />
  <uses-permission android:name=""android.permission.INTERNET"" />
  <uses-permission android:name=""android.permission.WRITE_EXTERNAL_STORAGE"" android:maxSdkVersion=""18"" />
  <uses-permission android:name=""android.permission.READ_EXTERNAL_STORAGE"" android:maxSdkVersion=""18"" />{1}
  <uses-feature android:name=""android.hardware.touchscreen"" android:required=""false"" />
  <uses-feature android:name=""android.hardware.touchscreen.multitouch"" android:required=""false"" />
  <uses-feature android:name=""android.hardware.touchscreen.multitouch.distinct"" android:required=""false"" />
</manifest>";
      const string kRescheduleOnRestartFalse = "<meta-data android:name=\"reschedule_notifications_on_restart\" android:value=\"false\" />";
      const string kRescheduleOnRestartTrue = "<meta-data android:name=\"reschedule_notifications_on_restart\" android:value=\"true\" />";
      const string kReceiveBookCompletedPermission = "<uses-permission android:name=\"android.permission.RECEIVE_BOOT_COMPLETED\" />";

      string GetSourceXml(string metaDataExtra, string permissionExtra)
      {
          if (metaDataExtra == null)
              metaDataExtra = "";
          if (permissionExtra == null)
              permissionExtra = "";
          return string.Format(kManifestTemplate, metaDataExtra, permissionExtra);
      }

#if UNITY_ANDROID
        [Test]
        public void AppendMetadataToManifest_WhenSameValue_Works()
        {
            string sourceXmlContent = GetSourceXml(kRescheduleOnRestartTrue, null);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sourceXmlContent);


            AndroidNotificationPostProcessor.AppendAndroidMetadataField(null, xmlDoc, "reschedule_notifications_on_restart", "true");

            Assert.IsTrue(xmlDoc.InnerXml.Contains(kRescheduleOnRestartTrue));
        }

        [Test]
        public void AppendMetadataToManifest_WhenOtherValue_Works()
        {
            string sourceXmlContent = GetSourceXml(kRescheduleOnRestartFalse, null);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sourceXmlContent);


            AndroidNotificationPostProcessor.AppendAndroidMetadataField(null, xmlDoc, "reschedule_notifications_on_restart", "true");

            Assert.IsFalse(xmlDoc.InnerXml.Contains(kRescheduleOnRestartFalse));
            Assert.IsTrue(xmlDoc.InnerXml.Contains(kRescheduleOnRestartTrue));
        }

        [Test]
        public void AppendMetadataToManifest_WhenNotPresent_Works()
        {
            string sourceXmlContent = GetSourceXml(null, null);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sourceXmlContent);


            AndroidNotificationPostProcessor.AppendAndroidMetadataField(null, xmlDoc, "reschedule_notifications_on_restart", "true");

            Assert.IsTrue(xmlDoc.InnerXml.Contains(kRescheduleOnRestartTrue));
        }

        [Test]
        public void AppendMetadataToManifest_WhenOtherFieldPresentWorks()
        {
            string sourceXmlContent = GetSourceXml(kRescheduleOnRestartTrue, null);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sourceXmlContent);


            AndroidNotificationPostProcessor.AppendAndroidMetadataField(null, xmlDoc, "do_something", "true");

            Assert.IsTrue(xmlDoc.InnerXml.Contains(kRescheduleOnRestartTrue));
            Assert.IsTrue(xmlDoc.InnerXml.Contains("<meta-data android:name=\"do_something\" android:value=\"true\" />"));
        }

        [Test]
        public void AppendPermissionToManifest_WhenNoPresentWorks()
        {
            string sourceXmlContent = GetSourceXml(null, null);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sourceXmlContent);


            AndroidNotificationPostProcessor.AppendAndroidPermissionField(null, xmlDoc, "android.permission.RECEIVE_BOOT_COMPLETED");

            Assert.IsTrue(xmlDoc.InnerXml.Contains(kReceiveBookCompletedPermission));
        }

        [Test]
        public void AppendPermissionToManifest_WhenAlreadyPresentWorks()
        {
            string sourceXmlContent = GetSourceXml(null, kReceiveBookCompletedPermission);
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(sourceXmlContent);


            AndroidNotificationPostProcessor.AppendAndroidPermissionField(null, xmlDoc, "android.permission.RECEIVE_BOOT_COMPLETED");

            Assert.IsTrue(xmlDoc.InnerXml.Contains(kReceiveBookCompletedPermission));
        }

#endif
    }
}
