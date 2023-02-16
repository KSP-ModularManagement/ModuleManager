# Module Manager /L :: Change Log
*Lasciate ogne speranza, voi ch'intrate*
- - -

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
* 2021-0801: 4.2.1 (Sarbian) for KSP 1.8.0
	+ Fix off-by-one string indexing in constraint checking Also change string
	+ comparison type to `StringComparison.Ordinal`, which should be the
	+ correct type according to
	+ https://docs.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings.
	+ Undo string comparison change.
* 2021-0801: 4.2.0 (Sarbian) for KSP 1.8.0
	+ Normalize KSP root path
	+ KSP makes it weird
	+ Addresses confusion in #164
	+ Set modded physics and reload earlier
	+ Do it in post patch, this allows the part loader to pick up changes
	+ (e.g. rigidbody min mass)
	+ mark dependencies as copy local false
	+ prevents them from showing up in the target directory
	+ fix remaining .NET 3.5 nuget packages
	+ upgrade visual studio runner
	+ Update remaining NuGet packages
	+ Add specific language markers to all project files
	+ seems to get confused otherwise
	+ Ensure string comparison is culture invariant
	+ And get rid of message suppresions related to it
	+ Always replace physics
	+ On a database reload this will already be true but we still want physics
	+ reloading to happen
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
* 2020-0822: 4.1.4.2 (Lisias) for KSP >= 1.3.1
	+ **Withdrawn** due a KSPe bork. 
* 2020-0711: 4.1.4.1 (Lisias) for KSP >= 1.3.1
	+ Merging upstream updates:
		- Fix a bug with LAST[modx] when modx is not present
		- some code cleanups
* 2020-0707: 4.1.4.0 (sarbian) for KSP >= 1.8
	+ Fix a bug with LAST[modx] when modx is not present
	+ some code cleanups
* 2020-0616: 4.1.3.2 (Lisias) for KSP >= 1.3.1
	+ A nasty mishap from an old merge (dated 2018-11) was detected and fixed.
		- See Issue [#4](https://github.com/net-lisias-ksp/ModuleManager/issues/4). 
* 2020-0526: 4.1.3.1 (Lisias) for KSP >= 1.3.1
	+ Adding a nice 'Houston' GUI message
	+ Merging upstream updates:
		- Adding an Exception interceptor to catch `ReflectionTypeLoadException` and properly blame DLLs
		- Cleanup the InterceptLogHandler, remove double logging and avoid any risk of throwing more
	+ These things were reworked to keep them compatible with previous KSP versions.
* 2019-1130: 4.1.3.0 (sarbian) for KSP 1.8
	+ Cleanup the InterceptLogHandler, remove double logging and avoid any risk of throwing more and improve the badly worded warning
	+ Start the Handler earlier
* 2019-1115: 4.1.2.0 (sarbian) for KSP 1.8
	+ Add an Exception interceptor to catch ReflectionTypeLoadException and properly blame broken DLLs
* 2019-1027: 4.1.1.0 (sarbian) for KSP 1.8
	+ Outdated Firespitter warning
* 2019-1016: 4.1.0.0 (sarbian) for KSP 1.8
	+ Extract escaping/unescaping of node values
	+ Fix dump database to files
	+ Fix reload dialog showing 100% when it shouldn't
	+ KSP 1.8 ! With less cat bugs
* 2019-0807: 4.0.3.1 (Lisias) for KSP >= 1.3.1
	+ Updating code to the latest KSPe API
		- Getting rid of deprecated warnings
	+ Getting rid of the overcomplicated logging code
		- Relying on KSPe to that
		- Logging into a file will be implemented later on KSPe itself. Place holders were kept.
	+ Merging upstream updates:
		- Improvements on thread safety
		- Small improvements on performance (regex caching and preserving config cache when just whitespaces were changed)
		- Better handling of Escape Chars, SHAs and URLs
		- Some code cleaning and better test cases
* 2019-0807: 4.0.3.0 (sarbian) for 1.4 <= KSP <= 1.7.3
	+ add thread-safe KeyValueCache
	+ use new KeyValueCache class as regex cache
	+ add test for ProtoUrlConfig
	+ Add name value to applying patch messages
	+ Fix StreamLogger ignoring messages
	+ Redo logging interface
	+ Initialize LogMessage from old LogMessage
	+ Don't put date on every log message
	+ Initialize timestamp with log message
	+ ModLogger -> PrefixLogger
	+ Unnecessary using
	+ Use better assertion
	+ Mark disposables as sealed
	+ Another unnecessary using
	+ Use custom code to lookup file by url
	+ Put extension on cache URLs
	+ Alphabetize .csproj files
	+ improve file sha generator
	+ Convert UrlBuilder cfg test to theory
	+ Set FileType based on common types
	+ unnecessary using
	+ make StreamLoggerTest.TestLog work on \n platforms
	+ Improve TestConfigNodeTest
	+ Remove redundant parent class
	+ Fix TestConfigNode's handling of escaped chars
	+ Clean up test a bit
	+ Fix DeepCopy handling of escaped values
	+ Fix ModifyNode handling of escaped characters
	+ Ensure tabs and newlines don't break cache
* 2019-0405: 4.0.2.1 (Lisias) for KSP >= 1.3.1
	+ Reverting change on MMPatchLoader to allow it to be findable by `UnityEngine.Object.FindObjectOfType`
	+ Merging upstream updates:
		- Allow assemblies to add to mod list (Fix for Kerbalism compatibility)
		- Add -mm-dont-copy-logs command line flag if you do not want to copy MM log back into the game log
		- Fix for KSP runs in debug mode.
* 2019-0203: 4.0.2.0 (sarbian) for KSP 1.3 & 1.4
	+ Fix datetime format
	+ Use KSP's directory listing rather than file system
	+ Better status updates in post patch
	+ Add -mm-dont-copy-logs command line flag
	+ Allow assemblies to add to mod list
	+ wait another frame for plugins to finish initializing
	+ Don't use Unity's time since startup in thread
* 2019-0125: 4.0.1.0 (sarbian) for KSP 1.3 & 1.4
	+ Delete unnecessary space
	+ Fix modded physics and tech treegithubweb)
	+ Make physics and tech tree node names constant
* 2019-0125: 4.0.0.2 (Lisias) for KSP >= 1.3.1
	+ Merging upstream updates:
		- [blowfish](https://forum.kerbalspaceprogram.com/index.php?/profile/119688-blowfish/) worked his magic once more and now MM does the patching while the game loads the models and textures.
		- Fix tech tree and modded physics
	+ (my) Fixes to the upstream:
		- Fixing the Logging system, restoring the (sane) previous behaviour. The new logs were preserved, and can be found on `<KSP_ROOT>/Logs/ModuleManager`
		- Allowing "stock" compatibility to 1.3.1 by avoiding >= 1.4 specifics #hurray :)
		- Monitoring changes to TechTree and Physics
		- Preventing hijacking them when another Add'On changes them.
			- They are set up only **one** at first time Space Center is loaded. From there, it only logs if they were changed.
	+ Stating **Official** support for KSP 1.3.1 :)
* 2019-0124: 4.0.0.0 (sarbian) for KSP 1.3 & 1.4
	+ Update NuGet packages where available
	+ Accessory projects on .NET 4.7.1
	+ Update NSubstitute to latest
	+ Update Xunit
	+ Fix Xunit warnings
	+ implement stream logger
	+ Unnecessary using
	+ These are already run by MMPatchLoader
	+ Extract post patch
	+ Operate on a copy of the game database then apply
	+ Move path initialization to static initializer
	+ Make sure cache also uses copy of game db
	+ Extract paths to their own class
	+ patch in background
	+ Remove PurgeUnused
	+ Put log paths in path repository
	+ Rename file to reflect class name
	+ Dump ModuleManager log to main log after patching
	+ Don't count insert nodes as patches
	+ Only display whole percentages
	+ Add subdir under Logs
	+ Replace reloading screen messages with dialog
	+ add LogSplitter
	+ Save patch log and dump when loading from cache
	+ Get rid of stream logger's exception logger
	+ Get rid of trailing whitespace
	+ use international date format
	+ extract common parts of reading logs from queue to its own class
* 2019-0110: 3.1.3.1 (Lisias) for KSP >= 1.4.1
	+ Merging 1.6.0 DragCube workaround from uptream
	+ Certifying for use on 1.5.x and 1.6.x series
	+ Bumping up version to catch upstream's
* 2019-0108: 3.1.3.0 (sarbian) for KSP 1.4
	+ Implement :LAST[mod] pass
	+ return iterator rather than array
	+ Workaround for 1.6.0 PartDatabase rebuild
* 2018-1125: 3.1.2.0 (sarbian) for KSP 1.4
	+ Fix color tag
	+ Clear filesSha before generating sha
	+ Move election check up
	+ Make MM a once addon
	+ Get rid of unnecessary using
* 2018-1112: 3.1.1.1 (Lisias) for KSP 1.4; 1.5
	+ Adding KSPe logging facilities
	+ Syncing source with upstream latest fixes.  
		- more Internal code improvement by @blowfish  
* 2018-1112: 3.1.1.0 (sarbian) for KSP 1.4
	+ Make patch stopwatch local
	+ Make useCache local
	+ Make Patch an object
	+ Simplify a bit
	+ Extra semicolon
	+ Extract interface for PatchList, modify PatchExtractor
	+ Allow null value in wildcard match
	+ Extract node matcher
	+ Extract IPatch interface, split up root patches
	+ Fix tests depending on line endings
	+ Whitespace
	+ Allow warnings
	+ Make ArrayEnumerator more versatile
	+ Tag, TagList, TagListParser
	+ Restructure patch building
	+ Make extra colons a warning rather than an error 
* 2018-1019: 3.1.0.1 (Lisias) for KSP 1.4
	+  Declaring this thing **EXPERIMENTAL**.
		- I will properly maintain it, but it still Experimental (and non Standard)
	+ Syncing source with upstream latest fixes.  
* 2018-1015: 3.1.0.0 (sarbian) for KSP 1.4
	+ Stupid typo
	+ Remove unused variable
	+ Make const stuff const
	+ Fix typo in test 
* 2018-1011: 3.0.7.4 (Lisias) for KSP 1.4
	+ We have moved to the properly maintained /L Division! :)
	+ No code changes from the last release.
* 2018-1010: 3.0.7.3 (Lisias) for KSP 1.4
	+ Moving the datafiles	 to <KSP_ROOT>/PluginData , where God intended it to be.
	+ Using KSPe facilities
		- (some) Logging
		- File System and Data Files
	+ Some small glitches and what appears to be a behavior inconsistency fixed. 
	+ Doing things the right way this time.
* 2018-1010: 3.0.7.2 (Lisias) for KSP 1.4 -- WITHDRAWN
* 2018-0930: 3.0.7.1 (Lisias) for KSP 1.4
	+ fixing MissingMethodException: Method not found: 'UnityEngine.Texture2D.LoadImage'.
* 2018-0505: 3.0.7 (sarbian) for KSP 1.4
	+ Fix deprecation
	+ Turn some semi-redundant methods into extensions
	+ Keeps having to reimplement them for every IBasicLogger implementation
	+ Split up prefixing and translating logs for unity
	+ Should be separate classes.
	+ Allow parentheses in value name
	+ Allow spaces in value names
	+ Addresses #107
	+ Fix operators
	+ Addresses #110
	+ Operators are now parsed like commands, removed from the regex.
	+ Fix value assignment with * indexer
	+ Broken in #111 - probably an unusual case but it would have worked
	+ before.
	+ Added tests to ensure that this fixes it.  Tests are not and will
	+ probably never cover all of MMPatchLoader.ModifyNode but useful to add
	+ bugfix cases here as they occur.
	+ Reflection fields should be readonly
	+ Create special GameData subdirectory
	+ It's special
	+ Allow checking needs against directories
	+ If the needs string contains a / it will check for a directory with that
	+ path in GameData.  Notes:
		- PluginData folders are excluded
		- Leading and trailing slashes are allowed
		- Multiple slashes together will be treated as a single slash
		- Comaprison is case sensitive
	+ Require at least one space before the operator (#119)
	+ Fixes wildcards in value names.  If * appears in at the end of value
	+ name without a space it should be interpreted as a wildcard rather than
	+ the multiplication operator
	+ Fix SHA generation for DLL - Fix #120
	+ Make sure TransformFinalBlock is called *after* the last block
* 2018-0313: 3.0.6 (sarbian) for KSP 1.4
	+ Cats trails fix and better text position
* 2018-0307: 3.0.5 (sarbian) for KSP 1.4
	+ Fix :NEEDS clause sometimes not getting removed
	+ Fixes #94
	+ remove ModuleManager.csproj from .gitignore
	+ It was under version control anyway
	+ Make post build into a shell script, allowing cross-platform
	+ compatibility
	+ Just requires sh to be installed on Windows.  Relies on $PDB2MDB to find
	+ the pdb2mdb executable, and $KSPDIR to find where to copy to.  If either
	+ of these are absent, it will be ignored (but the build will still work)
* 2018-0211: 3.0.4 (sarbian) for KSP 1.3
	+ Allow operator-like character in value name
	+ Means that the operator needs a space before it in most cases
	+ Already was the case with - and * but now also the case for other
	+ operator-like characters + / ^ !
* 2018-0209: 3.0.3 (sarbian) for KSP 1.3
	+ Fix for #92 - non-US Linux decimal separator fix
* 2018-0208: 3.0.2 (sarbian) for KSP 1.3
	+ No changelog provided
* 2017-1206: 3.0.1 (sarbian) for KSP 1.3
	+ Add a -mm-dump cmd line option and redo the export
	+ Now uses the same directory sub tree as GameData
	+ Fix NEEDS checking for inner nodes/values
	+ Didn't work if you had both top level NEEDS and NEEDS on a subnode/value
	+ since it was checking NEEDS on the wrong node in that case
* 2017-1202: 3.0.0 (sarbian) for KSP 1.3
	+ Begin splitting files up (#76)
		- rename file
		- most of it is MMPatchLoader so that's what it'll be
			- Remove corrupt #region
		- It starts in one class and ends in another, I can't tell where it's
	+ really supposed to go
			- Move addon to its own file
			- Put cats in a box
			- Can has namespace
			- Promote business cat to manager
			- Unnecessary now
			- Old stuff
	+ Change debug C# version to default
	+ VS, why u do dis?
	+ Add ImmutableStack class
	+ Add PatchContext struct
	+ Use ImmutableStack and PatchContext in MM
	+ Remove unused code
	+ Apparently had to do with texture replacer corruption, but not called
	+ anywhere
	+ Implement IEnumerable<T>
	+ Add Depth property
	+ Use immutability in CheckNeeds
	+ Forgot a using directive
	+ Ged rid of Win64 specific code
	+ Doesn't matter anymore
	+ Needs to be included in the project too
	+ Add logging interface
	+ Extract progress into its own object
	+ Use logger and progress
	+ Make some things static that no longer depend on the patch loader's
	+ state
	+ Remove blocking option
	+ It's no longer used
	+ Use inline variable declaration
	+ Make log messages consistent
	+ Make deletes and copies count toward patch count
	+ Make names more accurate
	+ These are called before the patch is applied
	+ Simplify null check
	+ move main project to its own directory
	+ Allows more to be added
	+ Better output dir for debug
	+ Do not copy local
	+ Add test project
	+ Add MM, Assembly-CSharp, UnityEngine refs
	+ Add console runner
	+ Will be needed eventually
	+ Yo dawg, I heard you like tests
	+ Add TestConfigNode class
	+ Makes testing with ConfigNodes by simplifying creating them
	+ Reference TestUtils
	+ Don't reference a specific version of System
	+ Add test for ImmutableStack
	+ Add test for GetPath
	+ Add NSubstitute
	+ Add tests for ModLogger
	+ Fix an error
	+ Add UrlBuilder
	+ Hackily creates UrlDir, UrlFile, UrlConfig for testing purposes
	+ Progress shouldn't depend on deleted subnodes
	+ The number of needs unsatisfied nodes it should be counting is the
	+ number of root nodes that have been removed, not subnodes as well
	+ These should use actual URLs
	+ Since all the calls were just using .url anyway
	+ These too
	+ Minor logging tweak
	+ Add tests for PatchProgress
	+ Replace DeepCopy with ConfigNode.CreateCopy
	+ It does 100% the same thing (and is recursive)
	+ Inline out variable declarations
	+ Yay C#7
	+ Obey naming conventions
	+ Pull Command and ParseCommand out of MMPatchLoader
	+ Would be nice if enums allowed static methods
	+ Extract ShallowCopy
		- this" is the node you're copying to so that the extension method is
	+ only modifying "its" node
	+ Don't create duplicates in UrlBuilder
	+ Add ArrayEnumerator
	+ Enumerates arrays in a garbage-free way
	+ PatchList
	+ list of patches, 'nuff said
	+ Add PatchExtractor
	+ Extracts patches from the game database and sorts them
	+ Add SafeUrl extension method for UrlConfig
	+ Makes sure logging doesn't mess up, and fixes the weird quirk where a
	+ node with a name value ends up displaying that instead of its actual
	+ name
	+ Use SafeUrl in logging
	+ Remove unused
	+ Doesn't really have any benefit
	+ Log when BEFORE or AFTER patch deleted
	+ This is pretty much equivalent to unsatisfied NEEDS, so it should be
	+ noted as such.  Also log on an unsatisfied FOR, although this shouldn't
	+ happen (make it a warning)
	+ Fix case issues
	+ Mods may not be lowercase to begin with, need to handle this
	+ Extract IsBracketBalanced
	+ Remove bracket unbalanced nodes when sorting
	+ Unused method
	+ Bring back DeepCopy
	+ Apparently KSP's default implementation fails on badly formed nodes
	+ Fix bad region
	+ Make sure badly formed mod passes are an error
	+ That's a bug
	+ Add some explanatory comments
	+ Unnecessary using directives
	+ Use sorted patches when applying
	+ Improves performance somewhat
	+ Verified that sorting patches takes almost no time even for a fairly
	+ large number of patches
	+ Remove now-unnecessary try-catch
	+ There's already one around it and we no longer care about removing
	+ patches from the database at this stage
	+ Replace big if with guard clause
	+ Reduces indentation.  Insert nodes shouldn't exist here anyway
	+ Simplify this
	+ It no longer has to look in actual passes here, so we can just use the
	+ name we want it to display.
	+ It does change the way it displays in the loading screen but that seems
	+ fine.
	+ Invalid command = error on the patch extractor
	+ This seems like the right place to check it
	+ Extract RemoveWS
	+ Fix logging
	+ Extract PrettyPrint
	+ Get rid of unnecesary using directives
	+ Don't run PrePatchInit if cache is being used
	+ Mod list is not necessary
	+ Eliminate mods instance variable
	+ Use method param rather than instance var
	+ Makes things easier to disentagle
	+ Eliminate Update
	+ Status will be updated when necessary anyway
	+ Eliminate redundant logging
	+ MMPatchLoader logs this info itself
	+ Keep track of progress fraction independently
	+ Make StatusUpdate less general
	+ If cache is used, status only needs to be set once, no need to check it
	+ every time
	+ Move this
	+ What I get for trying to make a bunch of changes and then split them
	+ into small commits
	+ Eliminate Progress instance variable
	+ Make it local, inject where needed
	+ Make more methods static
	+ All their instance variable dependencies have been eliminated
	+ This is no longer necessary
	+ And will probably result in an error anyway
	+ Move exception handling outside of PrettyConfig
	+ Callers really shouldn't be trying to print the result if it resulted in
	+ an exception anyway
	+ Tweak test
	+ This isn't the case it was trying to test
	+ Allow adding a ConfigNode.Value in initializer
	+ Not useful yet but maybe at some point
	+ Extract PrettyConfig (for UrlConfig)
	+ Add one more test
	+ Make CheckNeeds static
	+ Can now be extracted
	+ This can already be static
	+ Remove unnecessary Using
	+ Improve url and node printing
			- Handle null name explicitly
			- Include url when printing a UrlConfig
	+ Extract CheckNeeds
	+ Equality vs sameness mostly not tested for now, need to determine
	+ desired behavior
	+ Ensure that final string printed to the screen is the actual status
	+ Fix up mod list logging
			- Use a string builder
			- Print assemblies in a nicer format (table)
	+ Use Path.Combine
	+ It's more concise then concatenating with the separator char
	+ Unnecessary now
	+ Improve assembly list
			- Get rid of unused code
			- Include KSPAssembly version
	+ Accidentally removed
	+ Move tracking number of patches
	+ from mod list to sorting patches
	+ Put progress counts in their own object
	+ Allows the same counts to be used with a different logger.  Also remove
	+ unused setter for NeedsUnsatisfiedRootCount
	+ Move exception handling out of FIleSHA
	+ Callers should be aware of exceptions anyway
	+ Extract FileSHA
	+ Interacts with the file system so difficult to test unfortunately
	+ Fix unassigned variable
	+ Make this extractable
	+ Extract GenerateModList
	+ Unfortunately interacts with AssemblyLoader and the file system so not
	+ really testable
	+ Add MessageQueue
	+ Add QueueLogger and supporting classes
	+ Allows logging to a queue
	+ Don't keep track of non-root needs unsatisfied
	+ Isn't used anywhere
	+ Add FatalErrorHandler
	+ Allows us to display a message to the user and quit when an
	+ unrecoverable error occurs.
	+ Can't really be tested unfortunately.
	+ Add background task support
	+ Allows a background task to be run and monitored, including if it exits
	+ due to an exception
	+ Begin creating Progress namespace
	+ Finish creating Progress namespace
	+ Unnecessary directives
	+ Add needs test for and/or and capitalization
	+ Separate out progress counter
	+ Make it so that all the values can be incremented but not otherwise
	+ messed with.
	+ Allow a new progress tracker to be initialized that shares a counter
	+ with another but uses a different logger
	+ Ensure Counter behaves like an int
	+ More unnecessary using
	+ Add test for ! (not) in :NEEDS
	+ More unnecessary using directives
	+ Extract application of patches to its own thread
	+ Allows it to not be bound by logging which can be slow
	+ Test and fix PatchProgress.ProgressFraction
	+ Patches are now only counted after needs are checked, so this shouldn't
	+ consider needs unsatisfied nodes
	+ Tweak
	+ Only convert to array once per pass
	+ This is expensive
	+ Make node matching its own method
	+ Saves a level of indentation
	+ Loop only applies to edit patches
	+ Saves another indentation level.  Also remove MM_PATCH_LOOP {} after
	+ done
	+ Don't convert to an array at all
	+ It's not necessary.  Also don't use switch - makes things cleaner.  It's
	+ only 3 cases anyway
	+ Ensure that user gets updates during long passes
	+ The patcher can potentially generate log messages faster than the main
	+ thread can log them, causing frames that are noticeably long with no
	+ updates.  This ensures that yields still happen then.
	+ Verified that this does not meaningfully affect performance.  Previous
	+ tests suggest that the time wasted by waiting until the next frame is
	+ relatively small.
	+ Without switch, i is valid here
	+ Ensure time between each check of the log queue
	+ This prevents the queue from being locked too often, slowing down the
	+ patching thread
	+ Convert to an array initially
	+ Apparently it saves a bit of time, and this won't be changed while
	+ patches run
	+ Having an actual array here no longer necessary
	+ Apparently Linq slows things down
	+ I guess it matters at scale
	+ Improve access of name a bit
	+ Looks like GetValue("name") has a bit of overhead, instead we can check
	+ if the UrlConfig's type == name
	+ Move loop out of loop
	+ This is all a bit loopy
	+ case should match filename
	+ matters on some filesystems
* 2017-0629: 2.8.1 (Sarbian) for KSP 1.3
	+ Improve some cat related code and add -ncats cmd line option
	+ Improve logging related to some exceptions
	+ Update project file
* 2017-0526: 2.8.0 (Sarbian) for KSP 1.3
	+ Revert "Temp revert of 1.3 changes to release a 1.2 patch"
	+ This reverts commit 29df624348391373485a82fec75e273ceed30648.
* 2017-0506: 2.7.6 (sarbian) for KSP 1.3
	+ KSP 1.3 changes (#66)
		- Add names to dialog windows
		- Now required
			- Adjust MMPatchLoaderIndex
		- A new LoadingSystem was added at the beginning (FontLoader).  This
	+ change ensures that MM will always be after the GameDatabase regardless.
			- Fix position of MM info in loading screen
		- Things seem to have moved
			- Remove unused field
			- Press Alt+F11 again to dismiss the menu
		- Apparently this wasn't a feature before (at least not recently) but
	+ pretty simple to implement
	+ Reload PartUpgrade System after patching (#70)
	+ As the part-upgrade data is initially populated prior to ModuleManager
	+ patching, this fix allows for the patches that are applied to the
	+ PARTUPGRADE nodes to be reloaded for use by the PartUpgrade system.
	+ With this fix in place the tech-nodes, names, descriptions, etc, for the
	+ part-upgrade parts located on the tech tree will use the proper
	+ post-patching config data.
		- This solution has been tested to work properly when used directly from
	+ a ModuleManagerPostLoad callback.
		- Fix for problems discovered in KSP-RO/RealismOverhaul/#1628
	+ Temp revert of 1.3 changes to release a 1.2 patch
* 2016-1129: 2.7.5 (sarbian) for KSP 1.2.1
	+ No changelog provided
* 2016-1114: 2.7.4 (sarbian) for KSP 1.2.1
	+ Fix typos (#63)
	+ Fix #64 - Targeting all values applied the operation more than it should
* 2016-1105: 2.7.3 (sarbian) for KSP 1.2.1
	+ No changelog provided
* 2016-1011: 2.7.2 (sarbian) for KSP 1.2
	+ No changelog provided
* 2016-1008: 2.7.1 (sarbian) for KSP 1.2
	+ Fix the problem with setting value name that include comma (unless the
	+ comma is followed by a number)
	+ Dispaly how many exception were encountered
	+ Remove some debug spam
	+ Lower garbage by removing implicit allocation in CheckConstraints
	+ Disable some warning that I am getting tired of seeing
	+ Prevent garbage generated by debug string that we do not display or
	+ print
	+ Prevent cache genration when there are exception and display the files
	+ that generated them
	+ Improved feedback on what is going on
	+ Minor cleanup
* 2016-1005: 2.7.0 (sarbian) for KSP 1.1.3
	+ No changelog provided
* 2016-0519: 2.6.25 (sarbian) for KSP 1.1.2
	+ Fix Exception for variable searching a value that does not exist
* 2016-0430: 2.6.24 (sarbian) for KSP 1.1.2
	+ 2.6.64 - Rebuild for 1.1.2
* 2016-0424: 2.6.23 (sarbian) for KSP 1.1.1
	+ No point of updating the status outside the loading screen
	+ Add & operator: insert only if it doesn't already exist
	+ Doesn't work with root nodes right now, same as insert (%)
	+ Make the game always load in background
	+ Fix the insert NODE at position that blowfish found
	+ Fix nested node constraints only checking the first set
	+ Test for HAS
* 2016-0419: 2.6.22 (sarbian) for KSP 1.1
	+ Fix for #50
* 2016-0330: 2.6.21 (sarbian) for KSP 1.0.5
	+ No changelog provided
* 2016-0218: 2.6.20 (sarbian) for KSP 1.0.5
	+ Remove the debug spam of the out of node value edit
	+ Prevents the creation of a cache if there were errors while patching
* 2016-0216: 2.6.19 (sarbian) for KSP 1.0.5
	+ Add a special "*MM_PATCH_LOOP" node that when found tries to apply the
	+ patch once more on the same NODE
	+ Fix the patch loop id to "MM_PATCH_LOOP"
	+ Allow for out of node editing of values Like :  *@TEST[Test]/copy -= 1
* 2016-0214: 2.6.18 (sarbian) for KSP 1.0.5
	+ Update README.md
* 2016-0110: 2.6.17 (sarbian) for KSP 1.0.5
	+ Prevents nightingale from trying to breaking some stuff. Fix #44
	+ Add a warning for KSP build 1.0.5.1024
	+ Logging the Exception may be smarter
	+ Store SHA for each cfg and log added/changed/removed cfg
* 2016-0101: 2.6.16 (sarbian) for KSP 1.0.5
	+ Added code for !key,* = DEL and fixed group nr. 5
	+ Bumped Assembly Version
* 2015-1231: 2.6.15 (sarbian) for KSP 1.0.5
	+ Fix a bug with #34
	+ remove some tabs
	+ Comments are nice, let s update them
* 2015-1231: 2.6.14 (sarbian) for KSP 1.0.5
	+ Implemented @key,* = something to resolve #37
	+ Implemented Vector Editing, using @key,*[0]
	+ There are options for editing all keys [*] and specifing a seperator
	+ (defaults to ,): [0, ] (here we use a space as the seperator). Math also
	+ works.
	+ Fixed the Regex
	+ Bumped AssemblyVersion
* 2015-1109: 2.6.13 (sarbian) for KSP 1.0.5
	+ 2.6.13 - let's just change the number and pretend all is fine for 1.0.5
* 2015-1104: 2.6.12 (sarbian) for KSP 1.0.4
	+ No changelog provided
* 2015-0917: 2.6.11 (sarbian) for KSP 1.0.4
	+ Added try/catch to FileSHA.
* 2015-0914: 2.6.10 (sarbian) for KSP 1.0.4
	+ Return empty string if a character-separated list has fewer than the
	+ requested number of elements.
	+ Update version so sarbian can merge.
* 2015-0905: 2.6.9 (sarbian) for KSP 1.0.4
	+ Prevents NullReferenceException when saving the cache to pause the
	+ loading
	+ 30 FPS patching
* 2015-0829: 2.6.8 (sarbian) for KSP 1.0.4
	+ Fix a bug with nested :NEEDS when the top node also used a :NEEDS
* 2015-0804: 2.6.7 (sarbian) for KSP 1.0.4
	+ No changelog provided
* 2015-0625: 2.6.6 (sarbian) for KSP 1.0.4
	+ Add a Quick Reload for ALT F11 menu (skip PartDatabase.cfg generation)
	+ Ignore the cache (and force a PartDatabase.cfg generation) on KSP
* 2015-0523: 2.6.5 (sarbian) for KSP 1.0.3
	+ KSP "-nyan-nyan" option detection for the true believers
	+ Clear the partDatabase if the cache is expired
	+ Display the useful log info even if we use the cache
	+ Do not use the cache if the techtree cache is not present
	+ Format and cleanup
* 2015-0514: 2.6.4 (sarbian) for KSP 1.0.2
	+ Improve the loaded mod listing
	+ Do not change the Tech & physic file patch if they are already OK
	+ Make the error messages more consistent
* 2015-0504: 2.6.3 (sarbian) for KSP 1.0.2
	+ Count the error for math operations
* 2015-0429: 2.6.2 (sarbian) for KSP 1.0.2
	+ No changelog provided
* 2015-0427: 2.6.1 (sarbian) for KSP 1.0.2
	+ No changelog provided
* 2015-0401: 2.6.0 (sarbian) for KSP 1.0.2
	+ No changelog provided
* 2015-0325: 2.5.13 (sarbian) for KSP 1.0.1
	+ DB Corruption check code kept in case of need
	+ Change for #28
	+ Bullet proof PrettyPrint
	+ Remove debug stuff
	+ Forgot 2 lines
	+ Reformat
	+ < and > for value HAS check ( #mass[<100]  ~mass[>100] )
	+ operator for nodes to copy-paste whole nodes
* 2015-0223: 2.5.12 (sarbian) for KSP 1.0.1
	+ No changelog provided
* 2015-0217: 2.5.10 (sarbian) for KSP 1.0.1
	+ Fix the NODE,*:HAS[xxxx] reported by NathanKell
	+ More exception hunting and poor man debuging
* 2015-0427: 2.2.0 (sarbian) for KSP 1.0.1
	+ No changelog provided
* 2014-0522: 2.1.5 (sarbian) for KSP 0.23.5
	+ Fix a bug when inserting a name less node
* 2014-0518: 2.1.4 (Swamp-Ig) for KSP 0.23.5
	+ Allow GameData subdir in NEEDS / BEFORE / AFTER
* 2014-0517: 2.1.3 (Swamp-Ig) for KSP 0.23.5
	+ Removed non-essential backups
	+ Improvements for if multiple copies of the same version are installed.
* 2014-0510: 2.1.2 (Swamp-Ig) for KSP 0.23.5
	+ Lots of improvements and bug fixes in this release.
	+ [Full details here](http://forum.kerbalspaceprogram.com/threads/55219-Module-Manager-2-1-0-%28May-04%29-please-read-the-orange-text-in-first-post?p=1149933&viewfull=1#post1149933)
* 2014-0504: 2.0.9 (Swamp-Ig) for KSP 0.23.5
	+ So I've done my own enhancements to module manager.
	+ They seem to work pretty nice!  I'd really appreciate ppl testing it out on their various MM files prior to me pushing it back to the MM repo.
	+ Any issues you have - take them up with me not with sarbian
	+ Features:
	+ Order Preserving
		- now preserves the original order, both for nodes and for values.
	+ Use of NEEDS
```
PART:NEEDS[RealFuels] {
    name = dummyPartIgnore
    module = Part
    DOG {
        name = First
        key1:NEEDS[ProceduralParts|StretchySRB] = Original Value
        key2:NEEDS[!RealFuels] = Some other value
    }
}
```
	+ In the above, the part will only be defined if you have RealFuels loaded. You can do this on keys, values, patches, anywhere.
	+ key1 will be defined if (ProceduralParts OR StretchySRB are loaded) and RealFuels is NOT loaded.  You can still use , and it is treated like &. Not has highest precedence, then or, then and.
	+ Define an insertion point for any nodes or values
```
@PART[dummyPartIgnore]
{
    DOG {
        name = AddLast
        string = Will insert at the end
    }
    DOG,0 {
        name = AddFirst
        string = Will insert at the beginning
    }
    @DOG,0 { // Edits node zero as before, but ends up editing the above node
        string,0 = Insert before the string already at the beginning
    }          
}
```
	+ The index after the comma is where it will be inserted, this is relative to other nodes or values with the same name.
	+ Regexp replaces
```
@PART[dummyPartIgnore]
{
    @DOG[First] 
    {
        @string ^= :^.*$:First dog edit $& in place
    }
}
```
	+ Breaking this down, the first character defines the separator, the section between the first : and the second : is the match string, and the section following the second : is the replacement.
	+ For details on how to use regexp [see the documentation here](http://msdn.microsoft.com/en-us/library/hs600312%28v=vs.110%29.aspx)
