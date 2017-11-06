# HellionExtendedServer
An Extended Dedicated Server for Hellion http://www.playhellion.com 

**Discord Group** - https://discord.gg/Sr48Vbe

[![Build status](https://ci.appveyor.com/api/projects/status/phdspum8g0d3ics9/branch/master?svg=true)](https://ci.appveyor.com/project/yungtechboy1/hellionextendedserver/branch/master)

### Websites
~~**Jenkins** - jenkins.hellionextendedserver.com:8080~~

**Forums and Plugins** - https://hellionextendedserver.com

**Steam Discussion**  - http://steamcommunity.com/app/588210/discussions/0/133258593382366911/

**Hellion Forum Topic** - https://www.playhellion.com/forum/discussion/132/program-hellion-extended-server-hes-a-dedicated-server-extender

# FEATURES
     You can edit the GameServerIni in the GUI
     Console commands to use (See the Console Command section below)
     you can chat with people on the server by just typing into the command prompt or on the Chat tab of the GUI.
     You can read the chat from all players that talk on the console, or on the Chat tab of the GUI.
     (WIP)You can build and use plugins.
     I-ngame chat commands.
     (WIP) Permission System

# INSTALLING

Just drop the 2 files from the zip archive into your Hellion Dedicated Server directory or drop it into an empty folder and HES will automaticly install Hellion Dedicated and create all the required files.

     HellionExtendedServer.exe
     HellionExtendedServer.exe.config  
                          
To start the server, just run HellionServerExtender.exe ( make sure the files above exist with HellionExtendedServer.exe)


# Command Line Arguments
     
     -nogui - Disables the GUI
     -autostart - The server automaticly starts when HES is ran
     -noupdatehellion - HES will not automaticly update Hellion Dedicated
     -noupdatehes - HES will not update itself



# Console Commands
commands start with /

     Type directly into the console to talk to players as the server.
     /help - lists these commands
     /players -list - returns the full list of connected players
     /players -count - returns the current amount of online players
     /players -all - returns every player that has ever been on the server. And if they're online.
     /opengui - reopens the GUI if it was closed
     /start - starts the server
     /stop - stops the server
     /kick - kicks the specified player from the server
     /save - forces a universe save
     /save -send - forces a universe save, and tells connected players
     /send (name) text - send a message to the specified player


# Contributors
Generalwrex
TheCaptain
Yungtechboy1

If you would like to help, fork or message me! 
