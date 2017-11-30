# Freewheels
Codebase for a Windows unival app that uses [Pozyx](www.pozyx.io) for indoor positioning. Using  Pozyx firmware version 1.1. Developed to support a self driving wheelchair

# Overview
- Setup guide
    - Required hardware
    - Connecting the pozyx
    - Opening the application
    - Running the application
- Tips and tricks

# Setup guide

###### Required hardware
- Windows machine to run the application on
    - We used a Raspberry pi v3 which runs [Windows Iot](developer.microsoft.com/en-us/windows/iot/downloads)
- Windows 10 machine with visual studio.
- Pozyx shield
- at least 3 Pozyx anchors, 4 are required for 3d positioning

###### Connecting the pozyx
- We have chosen to connect the Pozyx via I2C with our Raspberry.
    - Connect the (5v, ground, scl and sda) pins
- Pozyx should also be abled to connect via a serial connection. However we couldn't manage to get this functional

###### Opening the application
- Clone the project: ```$ git clone https://github.com/JorSanders/Cinnovate.git ```
- Open the project in visual studio: file > open > project/solution > (location where you cloned the project to) > click and open <name> *

###### Running the application
- Connect the machine with visual studio(Your pc or laptop) to the same network as the machine connected to the pozyx(Rasperry Pi)
- In visual studio open your project properties and go to the debug tab.
    - Set platform to "Active (Arm)"
    - Set target device to "Remove machine" 
    - Set the remote machine to the IP address
    - Set Authentication mode to "Universal (Unencrypted Protocol)

# Tips and tricks
- Open a webbrowser and go to the ipaddress of your Windows Iot device and port 8080 example ```192.168.0.2:8080``` To access more settings and the files explorer of your device.
- Setting ```Pozyx.ConfigurationsRegisters.PosNumAnchors()``` seems to result in Pozyx always raising the error "POZYX_ERROR_NOT_ENOUGH_ANCHORS"
- Setting ```Pozyx.ConfigurationsRegisters.PosAlg()``` to tracking seems to set the algorithm to tracking although when requested it returns "UWB-only" is the set algorithm
- Our network didn't allow for a wifi connection to our Raspberry Pi. So we setup one of our laptops to host a hotspot and connected the Raspberry Pi to that network.

