#  GMOD Commit Tracker (WIP)

## What is this?
This is a discord bot that will send commit messages (from:https://commits.facepunch.com/r/Garrys%20Mod) to a dedicated channel in your discord. This is useful if you want to stay update to date with Garry's Mod developement with very little effort :)

Coming soon: Facepunch Commit tracker - this bot will be refactored into a generic facepunch commit tracker with more functionality. 

## Installion / Help


## How to use


## Commands
*permission: adminstrator*
~setchannel `channelid` - Sets the channel to send the commits to. 

*permission: adminstrator*
~deletechannel `channelid` - Deletes the attachment where the commits are send to. (Does **not** delete the channel) 

~userinfo `user/userid` - Prints out useful information about the Discord user

~help - Shows all commands for the bot


## Info
If you delete the channel you assigned and a commit comes through; your guild will be deleted from the list and you will be notified to re-add a channel. This is also true if the Bot doesn't have permission to write to the channel.

Feel free to clone the bot yourself and self-host. Notice: Create ``config.json`` with your bot-token as the key, value. ``token: value``

(Builds coming soon)
