# Battlezone-Redux-Mod-Manager
Battlezone Redux Mod Manager

This application is designed to download and manage mods from the Steam Workshop and git repositories. This manager uses NTFS junctions to inject mod directories into game mod folders.

To utilize the manager, directories must be properly configured. An example of such settings is given below:

![My image](https://github.com/Nielk1/Battlezone-Redux-Mod-Manager/blob/master/images/image2.jpg?raw=true)

## Main Interface
![My image](https://github.com/Nielk1/Battlezone-Redux-Mod-Manager/blob/master/images/image1.jpg?raw=true)

`[R]` - Refresh Mod List

`[U]` - Update All Mods

`[U] + SHIFT` - Update All Mods (agressive, will ask SteamCmd again for each mod)

`[D]` - Download Dependencies for SteamCmd mods if they are not present

`[Download]` - Download the mod in the URL box.  Workshop ID numbers, Workshop page URLs, and git URLs are valid

## Installation Statuses
![My image](https://github.com/Nielk1/Battlezone-Redux-Mod-Manager/blob/master/images/image3.png?raw=true)

`N` status indicators may be double clicked to install a mod.

`Y` status indicators may be double clicked to uninstall a mod.

## Git Mods
For git mods to function your system must have git installed or `git.exe` must be placed in the application directory.