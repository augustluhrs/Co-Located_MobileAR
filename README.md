# Co-Located_MobileAR
Basic Unity Template for using ARFoundation and Photon for co-located mobile localization


### Set-up Steps
1. Create a new Unity 2020.3 project and default scene (MobileQR_Test).
2. Install the packages as found here: [ARFoundation Packages](https://github.com/Unity-Technologies/arfoundation-samples), minus "ARKit Face Tracking".
3. Install Photon (PUN) according to the [set up guide](https://doc.photonengine.com/en-us/pun/v2/demos-and-tutorials/pun-basics-tutorial/intro). Log in / setup account with PUN Wizard to get AppID. Make sure the AppID is correct in Resources/PhotonServerSettings.
4. Set up the basic networking scripts and gameobjects (Launcher.cs and PUN_Launcher) with basic connection to random room.
5. Set up the prefabs representing the connected clients and attach Photon View components.