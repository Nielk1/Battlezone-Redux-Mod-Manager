# Battlezone-Redux-Mod-Manager
Battlezone Redux Mod Manager

This application is designed to download and manage mods from the Steam Workshop and git repositories. This manager uses NTFS junctions to inject mod directories into game mod folders.

To utilize the manager, directories must be properly configured. These directories are set via the Settings tab and in most cases can be set by clicking the "Quick Find" button followed by the "Apply" button to save the found value. Steam directories do not need to be set if you do not wish to sync mods from an existing steam installation to a Gog installation or install git based mods into Steam.

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