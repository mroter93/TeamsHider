# TeamsHider

A very simple tool to hide the overlays shown by teams during calls.

## Config

Open settings.conf to configure TeamsHider

![image](https://github.com/mroter93/TeamsHider/assets/156033398/774fae73-cb97-4832-aee1-ab13e1956be5)

**HideTopBar**  
Set to true to hide the bar which appears as soon as you share your screen.

**HideBottomOverlay**  
Hide the bottom right overlay which displays as soon as you minimize an active call window.

**GetWindows & TopBarWindowName**  
To hide the screen sharing bar the tool needs to know its window name. The german aswell english title is hardcoded however you can also add other languages easily. Set GetWindows to true and run the tool once while being in a call and sharing your screen. A file called windows.txt will be created:

![image](https://github.com/mroter93/TeamsHider/assets/156033398/c48bfded-5293-4101-9deb-e02aca1d3738)

Enter the title of the scren sharing bar into TopBarWindowName (it is enough to enter "Sharing control bar" instead of the full name).
After setting the name change "GetWindows" back to false or otherwise the tool wont do anything.

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
[https://www.youtube.com/watch?v=kBcTrQhyuo4](https://youtu.be/2CUZDFNQd0A)

## Credits

Thanks to flaticon for the icon (https://www.flaticon.com/free-icons/hidden)
