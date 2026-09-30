# Battlezone-Redux-Mod-Manager
Battlezone Redux Mod Manager

This application is designed to download and manage mods from the Steam Workshop and git repositories. This manager uses NTFS junctions to inject mod directories into game mod folders.

To utilize the manager, directories must be properly configured. These directories are set via the Settings tab and in most cases can be set by clicking the "Quick Find" button followed by the "Apply" button to save the found value. Steam directories do not need to be set if you do not wish to sync mods from an existing steam installation to a Gog installation or install git based mods into Steam.

## Installing Updating

### Installing
Unzip the binary archive from the [Latest Release](https://github.com/Nielk1/Battlezone-Redux-Mod-Manager/releases/latest) page.  Extract it to the location you'd like the manage to sit.  This location is important because it is where all your mods will download and take up space.  Moving the mods after they are installed is not advised.  If you would like to move the manager after mods are installed you should uninstall all mods (but keep them downloaded) before moving the folder.  This will prevent the junction points from becoming mangled.

Example installation locations:
* `C:\BZR Mod Manager`
* `D:\Games\BZR\ModManger`
* `D:\Program Files (loose)\BZR Mod Manager` (This is what I use, though some programs refuse to work in folders that start with "Program Files"

Do not install this program to your Desktop or Documents or other general user location.  This is a data-heavy application (stores lots of mod data) so it should be properly located.  The directory paths should not be too deep just to keep things nice and stable.

### Updating
The update process is manual but it is not difficult.
1. Delete everything except the `steamcmd`, `git`, and `fixes` folders.
   * If you wish to delete these folders you should ensure all mods are uninstalled first or manually delete any junctions left in the game's mod folder.
   * The `steamcmd` folder is a self contained SteamCmd instance.
   * The `git` folder may not exist unless you used git based mods.
   * The `fixes` folder is a mini-mod that is supposed to only be injected when launching directly into a multiplayer session in BZCC on specific versions with a UI bug, thus it is actually likely safe to remove, but leaving it and overwriting it is perfectly safe as well.
2. **Optional**: Delete everything in the `steamcmd` folder except the `steamapps` directory.
   * Generally this is not required, but if you do be sure to keep the `steamapps` subfolder unless you also uninstall all mods.  This is where all the normally downloaded mods reside.
3. Extract the new release binary zip into the folder, ensure the files are positioned the same as the old removed files.

## Main Interface

`Download` - Attempt to download a mod from the URL in the Mod URL box. Steam workshop URLs, workshop ID#s, or git URLs accepted.

`Refresh List` - Rescan local storage for mods.

`Update Mods` - Run an update to all mods that need update.

`Hard Update` - Run an update on all mods regardless of if they are flagged as needing an update.

`Download Dependencies` - For BZCC mods downloaded with SteamCmd download any missing dependent asset type mods with SteamCmd.

## Installation Statuses
The small colored letters in the GOG and Steam columns indicate the status of a mod and when double clicked perform an action. Be sure you double click specificly on these cells as double clicking any other part of the list will do nothing.

`N` - Mod is not installed, double click to install.

`Y` - Mod is installed, double click to uninstall.

`C` - Mod cannot be installed because an installed mod shares the same ID.

`M` - Mod is missing but required for another installed mod. This only applies to Steam downloaded mods as missing dependencies under SteamCmd can easily be downloaded just by clicking the `Download Dependencies` button. Double click to open the workshop page in Steam to subscribe to the mod.

`X` - Nothing can be done.

Note that grayed our `Y` and `N` markers on mods means no action can be taken. These statuses apply to mods downloaded by Steam always being `Y` in Steam and mods download by SteamCmd always being blocked from being `N` because they cannot be placed into Steam without causing a conflict.

Generally, a normal user with all mods installed should see only green or grayed out statuses. Some users may see purple C statuses if maintaining a mod set in both Steam and GOG but this is uncommon.

## Git Mods
For git mods to function your system must have git installed and its application path set in the mod manager's settings.

For a git mod to be detected it must have a specific folder structure. The `baked` folder will be checked out in a sparse manner to reduce storage space usage, allowing for dev assets to be placed in another path such as `assets` which will not be downloaded. The meta-file `baked/config.json` must be present to list the mods contained within the repository. Mods should have unique IDs if their content differs so they can be installed simultaneously with release mods, for example a mod with ID `1364723281` on the Steam Workshop should indicate its ID as `1364723281-dev` or another non-numeric string. If the mod exactly matches that on the workshop then the purely numeric `1364723281` is acceptable because the workshop sourced entry may be substituted when installing/running the mod. An example of a properly configured git-mod: https://github.com/Nielk1/BZCC-Advanced-Lua-API/

## Antivirus Note
Antivirus programs may complain about `steamcmdprox.exe` and `steamcmdinj.dll`. The SteamCmdProxy application is used to read realtime output from SteamCmd wich normally prevents this. The SteamCmdInjection DLL is injected into SteamCmd by SteamCmdProxy to force it to always run in English. This is required for automation to work properly on non-english computers.

## Compile Notes:
To compile you must also use the project SteamVent.SteamCmd from https://github.com/Nielk1/SteamVent
