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
	- leaning continues to a 15% mixture, but halts if RPM drops more than a safe RPM from its peak
	- The initial mixture % is restored after the computations & graph is shown


## Configuration

* The file Lside-Mixture.exe.config contains configuration properties for the above. If you edit it, you need to follow the files obvious pattern.
   - RpmDropThreshold
     * max acceptable drop in RPM
   -  EgtDropThreshold   
     * max acceptable drom in Peak EGT, from observed peak
   - TempStabilisedThreshold 
     * difference between 2 successive reading, need to be below this value to be 'stable'
   - EgtChangeWaitDelayMSec 
     * max time we will wait for egt to stabilise
    
## To Install (Tested on 64 bit windows only)

Link to executable:  https://github.com/JJ-blip/lside/releases/download/v1.0.0/Lside-Mixture.V100.zip

* unzip the application zip (e.g. Lside-Mixture.V100.zip)
* execute Lside-Mixture.exe

Lside.exe.config contains user changeable properties. 

## Known issues
* To early to say

## License
Distributed under the GNU General Public License v3.0 License.