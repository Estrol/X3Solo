# X3Solo
![Preview](https://cdn.discordapp.com/attachments/664111023493742595/808934793441443850/X3Solo_9kpo11AOSo.png)

O2Jam Singleplayer Server version based on [X3JAM](https://github.com/Estrol/X3JAM)

## Current status
This singleplayer server software already stable enough because it's easy to make than [X3JAM](https://github.com/Estrol/X3JAM) \
because this only contain necesarry packets to handle singleplayer.

## How to run the software
- You need copy of O2-JAM 1.8 (You can get one from O2-JAM Interval)
- Place X3Solo in game directory
- Run X3Solo and click Start Game

## Building the software
There already pre-build binaries for this software in Github Releases or in O2-JAM Interval Discord Server

But if you want build the software from the source code here how:

### Prequisites
- .NET 5 SDK
- Visual Studio 2019

### Building
Currently only providing build through Visual Studio Solution.
- Open solution file `.sln` using Visual Studio
- Select `Estrol.X3Solo`
- Press Build

## License
This software licensed under [MIT License](/license.txt).

This software using the code from the following repository to work 
* O2MusicList SDK from [SirusDoma](https://github.com/SirusDoma/O2MusicList) under [MIT License](https://github.com/SirusDoma/O2MusicList/LICENSE) (For it's OJN SDK)
* StatefulXOR Decryption from [sebastian-heinz](https://github.com/sebastian-heinz/Arrowgene.Baf/blob/master/Arrowgene.Baf.Server/Packet/PacketFactory.cs) under [MIT License](https://github.com/sebastian-heinz/Arrowgene.Baf/LICENSE) (For it's packet decyption method)