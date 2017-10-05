# Cinnovate
Codebase for the self driving wheelchair

# Requirements

##### Required hardware
- Raspberry Pi v3 x1
- Pozyx Shield x1
- UTP cable x2
- Micro USB cable (Power supply Raspberry Pi) x1 
- Keyboard x1
- Mouse x1
- Display x1
- HDMI cable x1
- Jumper cables male to female x4
- Laptop/PC (laptop is recommended for testing) x1

##### Required software
- Visual studio (latest version)
- Windows 10 (latest version)
- UTP cable


# How to connect the Raspberry Pi to the Pozyx shield
1. Connect the UTP cable to the Raspberry Pi (see number .. on image..)
2. Connect the HDMI cable from the display to the Raspberry Pi (see number .. on image..)
3. Connect the Micro USB cable to the Raspberry Pi, this will function as the power supply for the Raspberry Pi (see number .. on image ..)  
4. Connect the Raspberry Pi and Pozyx shield pins with the jumper cables (x4)
	a. Connect the male end of one jumper cable to the pozyx SCL pin (see number .. on image ..)     connect the female end to the Raspberry Pi (see number .. on image ..)
    b. Connect the male end of one jumper cable to the pozyx SDA pin (see number .. on image ..)     connect the female end to the Raspberry Pi (see number .. on image ..)
    c. Connect the male end of one jumper cable to the pozyx 5v pin (see number .. on image ..)     connect the female end to the Raspberry Pi (see number .. on image ..)
    d. Connect the male end of one jumper cable to the pozyx GND pin (see number .. on image ..)     connect the female end to the Raspberry Pi (see number .. on image ..)
5. Make sure everything is plugged in and powered, to test this turn on the display, you should now see a page with information about the Raspberry Pi and an IPv4 address, this IP is later used in 'How to connect with the Prozyx Shield' (see image ..)

# How to clone and open the application

1. Clone the project: $ git clone https://github.com/JorSanders/Cinnovate.git
2. Open the project in visual studio: file > open > project/solution > (location where you cloned the project to) > click and open <name> *

# How to connect with the Pozyx Shield using the application

1. Connect your Laptop/PC with the same network as the Pozyx Shield (UTP cable) make sure your IP matches that of the IP (IPv4) you got from 'How to connect the Raspberry Pi to the Pozyx shield' #5
2. In visual studio right click the project <name> in the solution explorer > properties > debug > under 'remote machine' click 'find' . Under 'manual configuration' type the IP addres of the Pozyx Shield in the address field  (your own IP should match this). Set the authentication mode to 'universal (unencrypted protocol)' and click the 'select' button. 
3. Back in Visual studio run the application by clicking the 'remote machine' button, with the setting 'Debug' and 'ARM' selected. The project should now open in a new window, from this point you can start using the pozyx
