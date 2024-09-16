# TeamsHider

A very simple tool to hide the overlays shown by teams during calls.

![overlays](https://github.com/mroter93/TeamsHider/assets/156033398/31047bbd-0da0-4779-affd-c1aa927f4524)

## Download

The latest version can be found here: https://github.com/mroter93/TeamsHider/releases (Release.zip)


## Config

Open settings.conf to configure TeamsHider

![image](https://github.com/mroter93/TeamsHider/assets/156033398/ba7a2b8b-6468-4862-b14d-bffc02eb10e4)

**HideTopBar**  
Set to true to hide the bar which appears as soon as you share your screen.

**HideBottomOverlay**  
Hide the bottom right overlay which displays as soon as you minimize an active call window.

## Tray

A tray icon is shown while running the tool. Rightclick and select Quit to stop TeamsHider from running.

![image](https://github.com/mroter93/TeamsHider/assets/156033398/f90d5bfe-9542-40f2-baa8-24cdf88d9fc8)

## Autostart

Hit Windows + R and enter shell:startup.
Place a shortcut to the TeamsHider.exe in the previously opened folder.

## Dependencies

Requires net 8 to be run. Download it here:
https://dotnet.microsoft.com/en-us/download/dotnet/8.0 (.NET Desktop Runtime 8.0.1)

## Video Showcase

Head over to YouTube for a short video:  
https://youtu.be/xbHCp9kb0vc

## Credits

Thanks to flaticon for the icon (https://www.flaticon.com/free-icons/hidden)

## Changelog

**v.3**  
Adjusted to internal changes in Teams to fix https://github.com/mroter93/TeamsHider/issues/3.
Also from now on the release will be self-contained (https://github.com/mroter93/TeamsHider/issues/1)

**v.3**  
Use GetWindowDisplayAffinity to make checking the window title and GetWindows obsolete aswell hiding the right window 100% reliable

**v.2**  
In conference with multiple people it is possible that the overlay name doesnt match the call window title exactly. Implemented a way to also work around this.

**v.1**  
Initial release

