# Objective
Aims to periodically reads over IR the register of a Landis+Gyr E230 Power Meter and displays some statistics in a console app

# Overview
![image](https://user-images.githubusercontent.com/232387/220758298-e1080e02-64c4-42b2-bfac-e10d6da8f096.png)

# Material used

- Using an IR Reader ()
- Using a RaspberryPi ()
- Using a 3.2-inch HAT-Screen ()

# Principle

1. Triggers a read using
 ```echo -n -e '\x2F\x3F\x21\x0D\x0A' > /dev/ttyUSB0```
2. Reads the output into a ```C#``` application
 _manually the output can be read while having run this command before chating with the unit._
 ```cat /dev/ttyUSB0```
3. At first the application will only show the latest values. The statistics will be added later on.

# Sample Output

This output has been read using the `cat`-command.

```
/LGZ4ZMR120AC.K750
/LGZ4ZMR120AC.K750
F.F.0(00000000)
0.0.2(  175225)
0.0.0(13267131)
0.0.3(        )
1.8.1(024581.111*kWh)
1.8.2(019882.194*kWh)
2.8.1(000000.000*kWh)
2.8.2(000000.000*kWh)
1.8.0(044463.305*kWh)
2.8.0(000000.000*kWh)
15.8.0(044463.305*kWh)
C.7.0(0053)
32.7.0(233)
52.7.0(235)
72.7.0(236)
31.7.0(001.06)
51.7.0(001.93)
71.7.0(000.12)
p
```

Corresponding OBIS-Code: <https://www.promotic.eu/en/pmdoc/Subsystems/Comm/PmDrivers/IEC62056_OBIS.htm>


# Python Implementation

To ease the string-processing and posting results to http-based API I transform the bash-base sample into a python application.

[Programatic read using Python](doc/programmatic-read.md)

# Sources / References

Without those sources and references I would not have been able to achieve this result.

- <https://microclub.ch/2019/06/14/2019-06-14-interface-optique-pour-compteurs-denergie-electrique-landisgyr-e230/>
- <https://github.com/bossjl/readlandisgyr_e230/>


# How to develop

## Using codespace

