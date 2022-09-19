# Module Manager /L Experimental
*Lasciate ogne speranza, voi ch'intrate*
- - -

ModuleManager is mod that let you write patch file that edit other part at load time.

This is Lisias' Experimental fork for Module Mamager.


## Installation Instructions

To install, place the GameData folder inside your Kerbal Space Program folder. Optionally, you can also do the same for the PluginData (be careful to do not overwrite your custom settings):

* **REMOVE ANY OLD VERSIONS OF THE PRODUCT BEFORE INSTALLING**, including any other fork:
	+ Delete `<KSP_ROOT>/ModuleManager*`
		- Yes. Every single file that starts with this name.
		- Older MM/L versions used to shove all files on `GameData`. This changed on 4.2.2.4.
	+ Delete the directory`<KSP_ROOT>/ModuleManager/` if existant
		- This is the new deployment model as 4.2.2.4
* Extract the package's `GameData` folder into your KSP's root:
	+ `<PACKAGE>/GameData` --> `<KSP_ROOT>/GameData`

The following file layout must be present after installation:

```
<KSP_ROOT>
	[GameData]
		[ModuleManager]
			[PluginData]
				...
			CHANGE_LOG.md
			LICENSE
			ModuleManager.version
			NOTICE
			README.md
		000_KSPe.dll
		ModuleManager.dll
		...
	[PluginData]
		[ModuleManager]
			...
	KSP.log
	PartDatabase.cfg
	...
```

Note: the `<KSP_ROOT>/PluginData/ModuleManager` folder will be automatically created and populated after the first run.

Additionally, don't mangle with `<KSP_ROOT>/ModuleManager/PluginData` as it contents is used by `ModuleManagerWatchDog` to keep `MM/L` healthy.

### Dependencies

* [KSP API Extensions/L](https://github.com/net-lisias-ksp/KSPAPIExtensions)
	+ Hard dependency, it will not work without it. 
* [Module Manager Watch Dog](https://github.com/net-lisias-ksp/ModuleManagerWatchDog)
	+ Soft Dependency. It's possible to run `MM/L` without `MMWD`, but it's highly recommended to install it and let it care about the `MM` (being it my fork or not) healthiness. 

