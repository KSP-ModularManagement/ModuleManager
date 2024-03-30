# Module Manager /L :: Changes
*Lasciate ogne speranza, voi ch'intrate*
- - -

* 2024-0311: 4.2.3.2 (Lisias) for KSP >= 1.2
	+ Logging revamp/refactoring
		- Saner code
		- MOAR LOGS
		- Previous `LOGSPAM` conditional compiling is now pushed into the mainstream as TRACE messages.
			- Edit the relevant ModuleManager section on `<KSP>/PluginData/KSPe.cfg` file to set the desired log level.
* 2023-0716: 4.2.3.1 (Lisias) for KSP >= 1.2
	+ Bumps versioning, catching up with upstream and formalising he fixes already implemented - ome for ages... :)
	+ Removes yet some more deprecated calls from `KSPe`.
* 2023-0521: 4.2.2.6 (Lisias) for KSP >= 1.2
	+ (Hopefully) mitigates a pretty weird problem happening on some new Intel CPU's with asymmetric cores (and on a less extent, faster Intel and probably AMD with symmetric ones).
		- Initially discovered by [LinuxGuruGamer](https://github.com/sarbian/ModuleManager/pull/180), then (pretty badly) misdiagnosed by me until I got in my senses and did some testings.
	+ Removes some deprecated calls from `KSPe`.
* 2023-0316: 4.2.2.5 (Lisias) for KSP >= 1.2
	+ Drops the Experimental status
	+ Relicense the whole shebang to GPL-3.0
	+ Make the thing properly distributable.
	+ **Needs** KSPe 2.5 or higher.
* 2022-0918: 4.2.2.4b (Lisias) for KSP >= 1.2
	+ Updates the [`INSTALL.md`](https://github.com/net-lisias-ksp/ModuleManager/blob/master/INSTALL.md) file to reflect the new deployment model.
		- A new `MM/L` release wasn't really **needed**, but since I failed to update the install instructions and there're people using this, I choose to be conservative on the matter.
		- There're no changes on the DLL, so I choose to shove a "b" on the same release number.
* 2022-0809: 4.2.2.4 (Lisias) for KSP >= 1.2
	+ Fixed a pretty lame mistake on the 1.2.2 port.
		- And, yeah. I should had tested this thing on KSP 1.2.2 recently... #facePalm 	
* 2022-0719: 4.2.2.3 (Lisias) for KSP >= 1.2
	+ More resilient handling of a potentially corrupted (or old) cache.
* 2022-0716: 4.2.2.2 (Lisias) for KSP >= 1.2
	+ Removes a memory leak, and promotes some key functions reusability. 
	+ Mitigates false positives while checking the ConfigCache, aiming to avoid the need of "deleting the cache when something weird happens" after load.
		- It's pretty rare, but not that much, that a change on a file ends up getting the same SHA256.
		- Checking also the file size now, as it's **way more** improbable that we would had a hash collision on a file with the same size. 
* 2022-0620: 4.2.2.1 (Lisias) for KSP >= 1.2
	+ Catch up with upstream:
		- v4.2.2 
			- Support wildcards in nodetype matching so you can do @*,* {}
			- Support # in value names since loc names start with #
			- Tell Localizer to reload the language after MM finishes
	+ More orthodoxous deploy model
		- **COMPLETELY REMOVE ALL OLDER FILES FROM `GameData`** before updating !!
* 2022-0306: 4.2.1.3 (Lisias) for KSP >= 1.2
	+ More sensible performance logging.
* 2021-0927: 4.2.1.2 (Lisias) for KSP >= 1.2
	+ Code base updated to the newest KSPe 2.4 series
	+ **ATTENTION**
		- This release will only work on the new KSPe 2.4 series!
* 2021-0907: 4.2.1.1 (Lisias) for KSP >= 1.2
	+ Catch up with upstream:
		- 4.2.1
			- Fix off-by-one string indexing in constraint checking Also change string
			- comparison type to `StringComparison.Ordinal`, which should be the correct type according to [best-practices-strings](https://docs.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings).
			- Undo string comparison change.
		- 4.2.0
			- Set modded physics and reload earlier
			- Ensure string comparison is culture invariant
			- Always replace physics
* 2021-0822: 4.1.4.8 (Lisias) for KSP >= 1.2
	+ Fix a performance issue on KSP >= 1.8, due a change on when the GameSettings were being applied (that ended you screwing my fork's restoring point) 
* 2021-0227: 4.1.4.7 (Lisias) for KSP >= 1.2
	+ Resurrects the Patch Logger on file
* 2020-1216: 4.1.4.6 (Lisias) for KSP >= 1.2
	+ Fixing a regression I introduced on 4.1.4.3 when I refactored MM to allow suporting KSP 1.2.2 again.
		- Yep... Pretty stupid... #facePalm 
* 2020-0922: 4.1.4.5 (Lisias) for KSP >= 1.2
	+ Making sure that eventual Database reloading borks don't screw up the KSP Performance Settings.
	+ The performance gain claimed by 4.1.4.4 may be inaccurate - the measures on my rig enhanced a bit, but a closer inspection of the original code revealed that (obviously) it was already doing (almost) the same.
		- Until I have more (and better) information about what exactly is happening, I suggest to take any measurement improvement with a huge grain of salt.
	+ The support for KSP 1.2 is still beta!
		- Some less used features are still Work In Progress. 
* 2020-0921: 4.1.4.4 (Lisias) for KSP >= 1.2
	+ Squeezing the last possible second from the Loading Scene by pumping up Unity's FPS settings (and restoring user's settings on finish)
		- Your mileage will vary, but I got about 10% faster loading times on my rig when using the MM cache, and even a bit more while rebuilding it on heavily modded instalments. 
	+ Beta support for KSP 1.2 is on the wild!
		- Some less used features are still Work In Progress. 
* 2020-0825: 4.1.4.3 (Lisias) for KSP >= 1.3.1
	+ Preventing KSPe to use Thread Safe logging.
		- MM doesn't need it (yet), and it's somewhat verbose on the logging.
		- KSPe thread safe logging will also mangle a bit the timestamps (as the writings are delayed until the next frame), what may be undesirable on MM.
	+ Needs KSPe 2.2.1 or newer.
* 2020-0711: 4.1.4.1 (Lisias) for KSP >= 1.3.1
	+ Merging upstream updates:
		- Fix a bug with LAST[modx] when modx is not present
		- some code cleanups
* 2020-0707: 4.1.4.0 (sarbian) for KSP >= 1.8
	+ Fix a bug with LAST[modx] when modx is not present
	+ some code cleanups
