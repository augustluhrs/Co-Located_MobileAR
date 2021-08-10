# Co-Located_MobileAR
Basic Unity Template for using ARFoundation and Photon for co-located mobile localization

### Why Co-Located AR is So Tricky


### Setup Steps For Using This Project as a Starting Template


### Setup Steps For Replicating This From Scratch
1. Create a new Unity 2020.3 project and default scene (MobileQR_Test).
2. Install the packages as found here: [ARFoundation Packages](https://github.com/Unity-Technologies/arfoundation-samples), minus "ARKit Face Tracking".
3. Install Photon (PUN) according to the [set up guide](https://doc.photonengine.com/en-us/pun/v2/demos-and-tutorials/pun-basics-tutorial/intro). Log in / setup account with PUN Wizard to get AppID. Make sure the AppID is correct in Resources/PhotonServerSettings.
4. Setup the basic networking scripts and gameobjects (Launcher.cs and PUN_Launcher) with basic connection to random room.
5. Setup the prefabs representing the connected clients and attach Photon View components.
6. Create custom Network Position script component for sending relative position to QRCode via Photon View.
7. Setup the ImageTracking script from ARFoundation and call Launcher.Connect() when scanning, but only after we set the anchorPos variable of our NetworkPosition component.
8. Create the XR Reference Image Library of images to track.