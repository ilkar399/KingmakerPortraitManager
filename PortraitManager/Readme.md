# KingmakerPortraitManager
This mod is designed to provide a way of better portrait management in Pathfinder Kingmaker.
##TODO
*next
	filters
*tests
*PortraitList ui
	Don't load all portraits?

*settings
	enable/disable injections
	enable/disable UI injections
	enable/disable filters
	update tags based on portrait hashes
	clear saved tag data
	clear recent
*tags
	Dictionary {<string>,<tagdata>}
	<tagdata>:
		string hash;
		List<string> tags;

	stored in modDirectory/tags/
	json format in storage, plaintext in portrait directory
	only jsonstring allowed, converted to lowercase
	tags are loaded into dictionary {string, List<string>}
*filters
	Filter is a List<string> of portraitIDs that won't be shown in UI.
	Filter is based upon the tag selection in the Mod UI/Game UI
	First stage: use the same filters as in portrait list

	save recent separately from tags, in settings? Queue<string>(10)

*separate tag management tab
	rename tags (with combining)
	remove tag
	?remove tagless portraits

*portrait folder 
	add portrait pack merging
	add ppack export/import
	tagdata is saved in separate folder

## Install
1. Download and install [Unity Mod Manager](https://www.nexusmods.com/site/mods/21)
2. Download the [mod](https://github.com/)
3. Extract the archive and put the mod folder into 'Mods' folder of the Game (\Steam\steamapps\common\Pathfinder Kingmaker\Mods)
4. Open the interface (Ctrl+f10)

##Credits to 
Spacehamster, Hambeard, Holic92, etc.