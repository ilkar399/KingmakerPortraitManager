# KingmakerPortraitManager
This mod is designed to provide a way of better portrait management in Pathfinder Kingmaker.
##TODO
*tests
check that game UI updates correctly on update in mod UI


*next
	tests

*settings
	enable/disable filters
	reset to default
	clear recent
*tags
	Dictionary {<string>,<tagdata>}
	<tagdata>:
		string hash;
		List<string> tags;

	stored in modDirectory/tags/
	json format in storage, plaintext in portrait directory
	only jsonstring allowed, converted to lowercase
	reserved tags: all, recent, "", favorite.
	tags are loaded into dictionary {string, List<string>}
*filters
	Filter is a List<string> of portraitIDs that won't be shown in UI.
	Filter is based upon the tag selection in the Mod UI/Game UI
	First stage: use the same filters as in portrait list

	save recent separately from tags, in settings? Queue<string>(10)

*separate tag management tab
	rename tags (with combining)
	?remove tagless portraits

*portrait folder 
	add ppack export/import

##Install
1. Download and install [Unity Mod Manager](https://www.nexusmods.com/site/mods/21)
2. Download the [mod](https://github.com/)
3. Extract the archive and put the mod folder into 'Mods' folder of the Game (\Steam\steamapps\common\Pathfinder Kingmaker\Mods)
4. Open the interface (Ctrl+f10)

##Exporting and importing portraits
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


##Compilation notes
Published assembly is required (add link to publisher)
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



##Credits to 
Spacehamster, Hambeard, Holic92, Hsinyu Chan etc.