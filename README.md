## Overview

* C# Windows application that captures and displays Mixture data from the Microsoft Flight Simulator 2020.
* The application automatically connects to the running instance of MSFS.
* Its main control window can be hidden or moved to another screen.

## Notes

* A cold engine (or one started at height from the map) will not have stable EGT temperature until the engine is warmed.
* The application amends the mixture lever and could potentially starve the engine. Safety Thresholds are adjustable

## Operation

* The application status need to be Connected before it can function.
* Adjustments are only made to Engine 1
* Graph button
	- Takes the current Mixture value as its rich base
	- decreases (leans) the mixture in 5% steps capturing engine EGT and RPM after 10 seconds
		+ EGT takes long time (10's of seconds) to stabilise if  mixture changes significantly
		+ RPM stabilises pretty quickly (couple seconds)
	- leaning continues to a 15% mixture, but halts if RPM drops more than 50 RPM from its peak
	- The initial mixture % is restored after the computations & graph is shown


## Settings

* RpmDropThreshold, default 50, While adjusting the mixture if the RPM drops by this amount from its peak (seen during the graphing process), then the process is terminated.
* EgtChangeWaitDelayMSec, default 10000 (10 seconds) - the time the EGT is alowed to stabalise before moving to the next value. Shortening the value means the measured RGT will lag the stable EGT more. The RPM will be stable within a few seconds.
* 