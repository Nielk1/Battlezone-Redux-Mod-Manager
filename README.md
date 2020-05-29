# Battlezone-Redux-Mod-Manager
Battlezone Redux Mod Manager

This application is designed to download and manage mods from the Steam Workshop and git repositories. This manager uses NTFS junctions to inject mod directories into game mod folders.

To utilize the manager, directories must be properly configured. These directories are set via the Settings tab and in most cases can be set by clicking the "Quick Find" button followed by the "Apply" button to save the found value. Steam directories do not need to be set if you do not wish to sync mods from an existing steam installation to a Gog installation or install git based mods into Steam.

## Main Interface

`Download` - Attempt to download a mod from the URL in the Mod URL box.

`Refresh List` - Rescan local storage for mods.

`Update Mods` - Run an update to all mods that need update.

`Hard Update` - Run an update on all mods reguardless of if they are flagged as needing an update.

`Download Dependencies` - For BZCC mods downloaded with SteamCmd download any missing dependent asset type mods with SteamCmd.

## Installation Statuses

`N` - Mod is not installed, double click to insatll.

`Y` - Mod is installed, double click to uninstall.

`C` - Mod cannot be insatlled because an installed mod shares the same ID.

`M` - Mod is missing but required for another insatlled mod. This only applies to Steam downloaded mods as missing dependencies under SteamCmd can easily be downloaded just by clicking the `Download Dependencies` button. Double click to open the workshop page in Steam to subscribe to the mod.

`X` - Nothing can be done.

Note that greyed our `Y` and `N` markers on mods means no action can be taken. These statues apply to mods downloaded by Steam always being `Y` in Steam and mods download by SteamCmd always being blocked from being `N` because they cannot be placed into Steam without causing a conflict.

## Git Mods
For git mods to function your system must have git installed or `git.exe` must be placed in the application directory.

For a git mod to be detected it must have a `config.json` meta-file.  Any branches can be selected though `baked` or a branch starting with `baked-` will auto-select in the branch selection list. `baked-something` branches should have unique IDs so they can be installed simultaneously with release mods.  An example of a properly configured git-mod: https://github.com/Nielk1/BZCC-Advanced-Lua-API/tree/baked

## Compile Notes:
To compile you must also use the project SteamVent.SteamCmd from https://github.com/Nielk1/SteamVent