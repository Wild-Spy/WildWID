# WildWID
An open-source active RFID system for wildlife research

## Requirements
Some device firmware is written in C and requires the AVR/Atmel Studio IDE - https://www.microchip.com/mplab/avr-support/avr-and-sam-downloads-archive

Some device firmware is written in C and requires the IAR Embedded Workbench for 8051 IDE - https://www.iar.com/iar-embedded-workbench/#!?architecture=8051

Programming software is written in Visual Basic and requires the Microsoft Visual Studio IDE - https://visualstudio.microsoft.com

## Repository Structure
The WildWID repository is organised around the two components of the WildWID system: tags and loggers. WildWID tag and logger folders can be found in the root level of the repository. 

Within the logger directory you will find three further folders:
* Device firmware: contains firmware for loggers.
* PC Software: contains the enduser software required to configure and test WildWID tags.
* Production: contains the schematics and bill of materials required for logger PCB fabrication.

Within the tag directory, you will find two folders:
* Device firmware: contains firmware for tags.
* Production: contains the schematics and bill of materials required for tag PCB fabrication.

## Further Information
A more detailed overview of WildWID and its use in wildlife biology is covered in: 

Rafiq, K., Appleby, R.G., Edgar, J.P., Jordan, N.R., Radford, C., Smith, B., Dexter, C.E., Jones, D.N., Blacker, A.R.F., and Cochrane, M., 'WildWID: An open-source active RFID system for wildlife research', *Methods in Ecology and Evolution*, in publication.

## License
This project is licensed under the terms of the GNU General Public License v3.0. See [LICENSE](LICENSE) for details.
