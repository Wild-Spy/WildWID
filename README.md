# WildWID
An open-source active RFID system for wildlife research

## Requirements
Some device firmware is written in C and requires the AVR/Atmel Studio IDE - https://www.microchip.com/mplab/avr-support/avr-and-sam-downloads-archive

Some device firmware is written in C and requires the IAR Embedded Workbench for 8051 IDE - https://www.iar.com/iar-embedded-workbench/#!?architecture=8051

Programming software is written in Visual Basic and requires the Microsoft Visual Studio IDE - https://visualstudio.microsoft.com

## Repository Structure
The WildWID repository is organised around the two components of the WildWID system: tags and loggers. Within the `Logger` and `Tag` folders you will find two sub-folders:

* Device firmware: contains firmware for the respective device.
* Production: contains the schematics and bill of materials required for PCB fabrication.

Within the `PC_Software` folder you will find multiple sub-folders containing the enduser software required to configure, test, and deploy tags and loggers.

## Further Information
A more detailed overview of WildWID and its use in wildlife biology is covered in: 

Rafiq, K., Appleby, R.G., Edgar, J.P., Jordan, N.R., Radford, C., Smith, B., Dexter, C.E., Jones, D.N., Blacker, A.R.F., and Cochrane, M., 'WildWID: An open-source active RFID system for wildlife research', *Methods in Ecology and Evolution*, in publication.

[![DOI](https://zenodo.org/badge/260139150.svg)](https://zenodo.org/badge/latestdoi/260139150)

## License
This project is licensed under the terms of the GNU General Public License v3.0. See [LICENSE](LICENSE) for details.
