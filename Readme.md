# KingmakerPortraitManager
This mod is designed to provide a way of better portrait management in Pathfinder Kingmaker.

## Install
1. Download and install [Unity Mod Manager](https://www.nexusmods.com/site/mods/21)
2. Download the [mod](https://www.nexusmods.com/pathfinderkingmaker/mods/195)
3. Extract the archive and put the mod folder into 'Mods' folder of the Game (\Steam\steamapps\common\Pathfinder Kingmaker\Mods)
4. Open the interface (Ctrl+f10)

## Tags
Note: portraits included in the game are not affected by tags.
Using the Mod UI you can add/remove tags to portraits. Tags are assigned to portrait based on the portrait id (basically, the folder where portrait images are stored). 
Tags are json strings, converted to lowercase. They are stored in "tags" subdirectory of the mod itself as json fileis, separate for each portrait. "all", "recent", "favorite", "filter" are reserved and cannot be manually entered through the in-game mod UI.
Tag file structure:
{
	"tags": [ List of tags ],
	"Hash": pseudohash integer, stored as string though.
	"CustomId": Portrait Id. stored as string as well, added just because of some edge duplication cases.
}

## Exporting and importing portraits
Portraits are exported to the "Export" directory in the mod folder and imported from the "Import" directory. Portraits are stored in "Id" folders and tags - in a "tags" subdirectory, as json files with their names corresponding to portrait Ids.
In the end, the mod folder structure should look like this:
KingmakerPortraitManager
│
├── tags
│   └── *.json
│
├── Export
│   ├── tags
│   │   └── *.json
│   └── <PortraitID>
│		└── Portrait images
│
└── Import
    ├── tags
    │   └── *.json
    └── <PortraitID>
		└── Portrait images


## Compilation notes
Github page: https://github.com/ilkar399/KingmakerPortraitManager
Published assembly is required.
This project depends on [ModMaker](https://github.com/hsinyuhcan/KingmakerModMaker), you need both repos in the same folder, and a folder called `KingmakerLib` including the Dll files. The folder structure should look like:
```
Repos
│
├── KingmakerLib
│   ├── UnityModManager
│   │   ├── 0Harmony.dll.dll
│   │   └── UnityModManager.dll
│	├── Assembly-CSharp_publicized.dll
│   └── *.dll
│
├── KingmakerModMaker
│   ├── ModMaker
│   │   └── ModMaker.shproj
│   └── ModMaker.sln
│
└── PortraitManager
    ├── PortraitManager
    │   └── KingmakerPortraitManager.csproj
    └── KingmakerPortraitManager.sln
```

## Credits to 
Spacehamster, Hambeard, Holic92, Hsinyu Chan etc.

## TODO

* next
	* apply modUI filter into the game UI

* tests
	* check that game UI updates correctly on update in mod UI

* settings
	* enable/disable filters
	* reset to default
	* clear recent

* tags
	* recent&favorite tags

* separate tag management tab
	* rename tags (with combining)
	* ?remove tagless portraits

* portrait folder 
	* test ppack export/import