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
* Extract the package's `GameData` folder into your KSP's root:
	+ `<PACKAGE>/GameData` --> `<KSP_ROOT>/GameData`

The following file layout must be present after installation:

```
<KSP_ROOT>
	[GameData]
		000_KSPe.dll
		ModuleManager.CHANGE_LOG.md
		ModuleManager.LICENSE
		ModuleManager.README.md
		ModuleManager.dll
		ModuleManager.version
		...
	[PluginData]
		[ModuleManager]
			...
	KSP.log
	PartDatabase.cfg
	...
```

Note: the `PluginData/ModuleManager` folder will be automatically created and populated after the first run.

### Dependencies

* [KSP API Extensions/L](https://github.com/net-lisias-ksp/KSPAPIExtensions)

