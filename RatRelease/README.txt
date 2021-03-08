1 Step - configure backdoor config with main port over which connection will happen + port for file download (to which port backdoor should knock in order to upload file on your PC/server)

2 Step - merge configuration into .exe by using merge program. It will append encrypted JSON file at the end of RAT.exe, which will be decrypted and red by RAT during launch.
NOTE/S:
  RAT will be unable to start without configuration! 
  Use only JSON configuration!

3 Step - you're set and ready :)

Lauch RAT.exe on some PC and you will be able to connect to it remotely and will get remote cmd shell.
RAT.exe is self-contained application and compatible with any win10.

Perfectly works with netcat.
