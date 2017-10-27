# HellionExtendedServer
An Extended Dedicated Server for Hellion http://www.playhellion.com 

### Websites
~~**Jenkins** - jenkins.hellionextendedserver.com:8080~~

~~**Forums and Plugins** - https://hellionextendedserver.com~~

[![Build status](https://ci.appveyor.com/api/projects/status/phdspum8g0d3ics9/branch/master?svg=true)](https://ci.appveyor.com/project/yungtechboy1/hellionextendedserver/branch/master)

### Websites
~~**Jenkins** - jenkins.hellionextendedserver.com:8080~~

~~**Forums and Plugins** - https://hellionextendedserver.com~~

**Steam Discussion**  - http://steamcommunity.com/app/588210/discussions/0/133258593382366911/

**Hellion Forum Topic** - https://www.playhellion.com/forum/discussion/132/program-hellion-extended-server-hes-a-dedicated-server-extender

# FEATURES
     (WIP) You can edit the GameServerIni in the GUI
     Console commands to use (See the Console Command section below)
     you can chat with people on the server by just typing into the command prompt or on the Chat tab of the GUI.
     You can read the chat from all players that talk on the console, or on the Chat tab of the GUI.
     (WIP)You can build and use plugins.
     (WIP)Ingame chat commands.
     (WIP) Permission System

# INSTALLING

Just drop the 2 files and the folder "Hes" from the zip archive into your Hellion Dedicated Server directory

     HellionExtendedServer.exe
     HellionExtendedServer.exe.config  
     Hes/
          NLog.config
          localization/
          logs/
          plugins/
          bin/
               NLog.dll
               HellionExtendedServer.Common.dll
               
          
          
     

To start the server, just run HellionServerExtender.exe ( make sure the files and folder above exist with HellionExtendedServer.exe)

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
