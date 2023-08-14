This repo contains the source code for the Xenonauts 2 Mod Patcher (XMP) and an example Mod.

_Both the Mod Patcher and example Mod were inspired by a thread on the Goldhawk Interactive forum posted by asdfcyber. Most of the TestMod code was taken directly from their code._

_The thread can be found here:_ https://www.goldhawkinteractive.com/forums/index.php?/topic/28131-first-v25-modding-impressions/#comment-192760

The Xenonauts Mod Patcher (XMP) is used to automatically patch the Xenonaut 2 Assembly-CSharp.dll to allow custom Mod dlls to be loaded when the game starts.

The TestMod project is an example of how to create a Mod that can be loaded by the XMP. It also shows a simple example of how to use harmony (https://github.com/pardeike/Harmony) to override an existing Xenonaut 2 method to allow you to print a debug message when the game starts.

## Xenonauts 2 Mod Patcher (XMP)

### Configuration 
The Mod Patcher requires the Xenonauts install path and Mod directory to be added to the Settings.json file.

#### How does it work
Once started the Mod Patcher uses mono.cecil to inject several changes into Assembly-CSharp.dll. 

It adds an IMod interface to the Xenonauts namespace (which any mods will need to implement) and adds a LoadMods method to the Xenonauts.XenonautsMain class. Finally, it updates the Xenonauts.XenonautsMain constructor to call LoadMods.

The LoadMods method will recursively check the specified mods directory for dlls that contain Types that implement the IMods interface. It will then instantiate these types and call their Initialise method. 

## TestMod

For the test mod to build correctly you will need .net framework 3.5 to be installed (as this is the version being used by the version of Unity Xenonauts 2 is using). You will also need to copy in at least the Assembly-CSharp.dll and Assembly-CSharp-firstpass.dll files from the Xenonauts2_Data\Managed data directory into the debug directory.

You will need to have run the Mod Patcher at least once so the Xenonauts.IMod interface has been injected into the Assembly-CSharp.dll.

The TestMod includes just one patch, a postfix patch of XenonautsMain.Start(). This patch adds a "XenonautsMain.Start called" message to the output_log.txt file located in the Xenonauts2_Data folder.
