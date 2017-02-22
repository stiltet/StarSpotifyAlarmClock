# Star Spotify Alarm Clock

[![Download Star Spotify Alarm Clock](https://a.fsdn.com/con/app/sf-download-button)](https://sourceforge.net/projects/starspotifyalarmclock/files/latest/download)

### What is this?
Star Spotify Alarm Clock is a lightweight Windows application to run Spotify, play the playlist/artist/album/track of your choise and smoothly fade in the computers volume in the amount of time you desire. Star Spotify Alarm Clock is completely open source, both source code and software, and free to use, both for comercial and personal purposes.

### How do I use it?
[//]: # (https://github.com/stiltet/StarSpotifyAlarmClock/raw/master/Executebles/latest/StarSpotifyAlarmClock.zip)
Just download the software from [here](https://sourceforge.net/projects/starspotifyalarmclock/files/latest/download) unzip it into the folder of your choise and run "StarSpotifyAlarmClock.exe" to se witch attributes you need to send to the application.
I also recomend you to create a Windows scheduled task that runs the software at your desired wakeup time and with the arguments needed.

[![Download Star Spotify Alarm Clock](https://img.shields.io/sourceforge/dt/starspotifyalarmclock.svg)](https://sourceforge.net/projects/starspotifyalarmclock/files/latest/download)

### Release Notes

#### v. 1.2.2.0

* Fixed a bug with the RegExp for SpotifyURL caused by only allowing small caps in usernames.
* Added version number in header when starting application.
* Done a lot of refactoring.

#### v. 1.2.1.0

* Fixed some stability and reliability issues. 
* Updated dependencies.

#### v. 1.1.0.0

* Updated failure handling to check if Spotify is installed and that a user is logged in.
* Improved exception handling. 
* Switched to minutes to fade in system volume instead of seconds to sleep beteen increese of volume by 2. 

#### v. 1.0.0.0

First public release of the application.