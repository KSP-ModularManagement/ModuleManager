# Module Manager /L Experimental :: Changes
*Lasciate ogne speranza, voi ch'intrate*
- - -

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
