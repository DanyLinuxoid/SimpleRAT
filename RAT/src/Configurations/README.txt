Contains configurations for RAT.
Currently has configurations for:
1. Main port on which RAT will listen once is launched on victim's PC (default 8888)
2. Port for file download, which is used to download files from victim's PC (default 8889)

NOTE: Currently feature for JSON configuration merge into exe is not yet implemented, which means that for RAT to work properly, during program launch 
configuration should be in same path where RAT exe file is located, otherwise configuration will not be readed. This will be changed in future.
