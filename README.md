# HellionExtendedServer
An Extended Dedicated Server for Hellion http://www.playhellion.com 


[![Build status](https://ci.appveyor.com/api/projects/status/phdspum8g0d3ics9/branch/master?svg=true)](https://ci.appveyor.com/project/yungtechboy1/hellionextendedserver/branch/master)

### Websites
**Jenkins** - http://jenkins.hellionextendedserver.com:8080

**Forums and Plugins** - https://hellionextendedserver.com

**Steam Discussion**  - http://steamcommunity.com/app/588210/discussions/0/133258593382366911/

**Hellion Forum Topic** - https://www.playhellion.com/forum/discussion/132/program-hellion-extended-server-hes-a-dedicated-server-extender


_***THIS IS A WORK IN PROGRESS.***_

It's very minimalistic right now, 
you can chat with people on the server by just typing into the command prompt
You can read the chat from all players that talk.

# INSTALLING

Just drop the 5 files and the folder from the zip archive into your Hellion Dedicated Server directory

     HellionExtendedServer.exe
     HellionExtendedServer.exe.config
     HellionExtendedServer.Common.dll
     NLog.dll
     NLog.config
     Hes/
          localization/
          logs/
          plugins/
          
          
     

To start the server, just run HellionServerExtender.exe ( make sure the files and folder above exist with HellionExtendedServer.exe)


commands start with /

     /players -list - returns the full list of connected players
     /players -count - returns the current amount of online players
     /players -all - returns every player that has ever been on the server. And if they're online.
     /help - Shows all available commands 
     /save - forces a universe save
     /save -send - forces a universe save, and tells connected players
     /send (name) text - send a message to the specified player


If you would like to help, fork or message me! 

# Contributors
Generalwrex

TheCaptain

Yungtechboy1
