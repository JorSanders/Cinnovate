# Cinnovate
Codebase for a Windows unival app that uses [Pozyx](www.pozyx.io) for positioning. 

# Setup guide

###### Required hardware
- Windows machine to run the application on
    - We used a Raspberry pi v3 which runs [Windows Iot](developer.microsoft.com/en-us/windows/iot/downloads)
- Windows 10 machine with visual studio.
- Pozyx shield
- at least 3 anchors, preferably 4+

###### Connect the pozyx
- We have chosen to connect the Pozyx via I2C with our Raspberry.
    - Connect the (5v, ground, scl and sda) pins
- Pozyx should also be abled to connect via a serial connection. However we couldn't manage to get this functional

###### Open the application

- Clone the project: ```$ git clone https://github.com/JorSanders/Cinnovate.git ```
- Open the project in visual studio: file > open > project/solution > (location where you cloned the project to) > click and open <name> *

###### Run the application

- Connect the machine with visual studio(Your pc or laptop) to the same network as the machine connected to the pozyx(Rasperry Pi)
- In visual studio open your project properties and go to the debug tab.
    - Set platform to "Active (Arm)"
    - Set target device to "Remove machine" 
    - Set the remote machine to the IP address
    - Set Authentication mode to "Universal (Unencrypted Protocol)
- Note: Our network didn't allow for a wifi connection to our Raspberry Pi. So we setup one of our laptops to host a hotspot and connected the Raspberry Pi to it.
