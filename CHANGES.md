# Module Manager /L Experimental :: Changes
*Lasciate ogne speranza, voi ch'intrate*
- - -

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
