# GoDiceBeginnerDemo

This is a beginner-friendly demo with a minimal abstraction layers and features.

<img src="https://github.com/user-attachments/assets/a6e1ccd1-02e6-4600-a51e-a965d782accf"  width="600">

The application will:
- Scan for GoDice when Scan button is pressed
- Automatically connect to all found devices 
- Print full log of actions performed in order to connect and initialize. 

More advanced and features reach demo can be found [here](https://github.com/ParticulaCode/GoDiceUnityDemo).

### Setup
To run the demo, you will need an Android device and [Bluetooth LE for iOS, tvOS and Android](https://assetstore.unity.com/packages/tools/network/bluetooth-le-for-ios-tvos-and-android-26661) plugin. With a minor changes, you can make project to run on iOS device as well. You can also use another bluetooth plugin, if you want to, but it will require some code changes.

The project will not compile without a bluetooth plugin!

DO NOT IMPORT `AndroidManifest.xml` from the plugin. It will override existing manifest and your application will fail to install on device.

Once you add BLE plugin, you will need to create an `assembly definition` for its codebase and reference it in `GoDice.asmdef`.

For more details, please take a look at [this video](https://youtu.be/SjlsfxbiFwc?si=vHPahms5iEVYLXYL&t=45) with step by step explanation. 

### Permissions
In order for demo to work on your device - you need to manually grant it permissions to use bluetooth. Otherwise, you will not be able to run a scan. In production-ready application app would request the permission in runtime, but to keep the demo simple we decided not to include this logic.

<img src="https://github.com/user-attachments/assets/9f6c60d6-ac3a-4103-a624-cd2d19492ec0" width="600">

