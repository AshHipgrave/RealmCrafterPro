# Realm Crafter Pro

Archive of the Realm Crafter Pro engine source code from May 2013.

https://web.archive.org/web/20130509024229/http://www.realmcrafter.com/

## What is Realm Crafter?

Realm Crafter was an MMORPG creation engine developed by Solstar Games and sold in two variants:

* Realm Crafter Standard - A beginner/user friendly game engine for creating MMORPG's. This version of Realm Crafter was written in Blitz Basic.
* Realm Crafter Professional - A rewritten version of Realm Crafter Standard written primarily in C++, with some tools and applications written in C#. This version was targeted at more advanced projects and users.

Around 2013 Solstar Games started providing source code access to licensees. 

## Where did I get this?

I purchased a license for both standard and professional editions sometime in 2011 which enabled me to obtain the source code. Whilst going through some old PC backups I came across an unmodified checkout of their SVN repository and decided to publish the code to GitHub for archival purposes.

According to the SVN log and file modification times, this code is from 31st May 2013.

# Building and Running

## Prerequisites

* [Visual Studio 2010 with Service Pack 1](https://archive.org/details/en_vs_2010_ult)
* [DirectX June 2010 SDK](https://www.microsoft.com/en-gb/download/details.aspx?id=6812)
* [PhysX 2.8.1 SDK](https://github.com/AshHipgrave/RealmCrafterPro/releases/download/svn-unmodified/PhysX_2.8.1_SDK_Core.msi)
	* Check the installer created a `PHYSXSDK` environment variable pointing to `C:\Program Files (x86)\NVIDIA Corporation\NVIDIA PhysX SDK\v2.8.1\SDKs` (manually create this if it doesn't exist).
* [PhysX 9.13.1220 System Software](https://github.com/AshHipgrave/RealmCrafterPro/releases/download/svn-unmodified/PhysX-9.13.1220-SystemSoftware.msi)
* [irrKlang Pro 1.6.0](https://www.ambiera.com/irrklang/irrklang_pro.html)
	* Create an `IRRKLANG_SDK` environment variable pointing to your irrKlang installation folder
	* The free version can be used but you'll need to make the following modifications to the BBDX2 project so that it uses the irrKlang DLL file and not the static library:
		* Navigate to `Properties` > `Configuration` > `VC++ Directories` > Edit `Library Directories` and replace `$(IRRKLANG_SDK)\bin\win32-visualstudio_lib` with `$(IRRKLANG_SDK)\lib\win32-visualstudio`
* [Realm Crafter Pro 2.53 Retail](https://github.com/AshHipgrave/RealmCrafterPro/releases/download/svn-unmodified/Realm.Crafter.PRO.2.53.Beta.FULL.VERSION.exe)

## Compiling

* Install all of the prerequisites
* Copy the `Data` folder from `C:\Program Files (x86)\Solstar Games\Realm Crafter 2\Data\Default Project` to `<Checkout_Root>\Sandbox`
* Run `<Checkout_Root>\Sandbox\SQLiteConvert.bat`
* Create an environment variable named `RCP_SDK` and point it to `<Checkout_Root>\Sandbox`
* Open `RC Pro.sln`
* Right-click the solution and select `Build Solution`
* Once complete, navigate to `<Checkout_Root>\Sandbox` and run `Realm Crafter GE.exe` to start the editor

# Demo project

The [Releases](https://github.com/AshHipgrave/RealmCrafterPro/releases) page contains a demo project Solstar Games provided to licensees to showcase some of the engine's features and provide a starting point for projects.

# License

There is no license for this code. Technically it is still copyright to Solstar Games, however as far as I can tell the company is all but dead. I'm sharing this for archival/historical purposes only.
