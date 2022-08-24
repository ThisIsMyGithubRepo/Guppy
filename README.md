# Guppy
3D Printer Serial Communication Tool, mostly for Marlin. Why Guppy? Marlin is a fish, and Guppies are much smaller fish. Plus I'm not great at naming apps.

Guppy is a Windows .Net WPF Core 3.1 App. Honestly, I kinda hate the implementation...but I don't have a better idea for how to deal with the serial stream, and that fact that there is no easy way to pair a command with it's response (no correlation Id, and you see responses to commands from all sources...). Ah well, it works and was an experiment.

<!-- ABOUT THE PROJECT -->
## About The Project

![Product Name Screen Shot][product-screenshot]

I like to connect to my 3D printer via serial to control it for tuning and adjusting. I'm happy to print via SD, and don't really need something like OctoPrint.

I really don't know what I'm doing, in particular around publishing an app. So, if you have a suggestion or see me doing something wrong - please let me know.

<!-- USAGE EXAMPLES -->
## Usage
### Basic Features
* Connect/Disconnect using...wait for it...the Connect and Disconnect buttons. The app will try to autoconnect if you send a command while disconnected.
* Type commands directly into the command bar. The last 5 commands are remembered and you can scroll through them with the up/down arrow keys to reuse them.
* When you've scrolled to a command, you can use the left arrow to get to the end of it for editing - I find I often go back to a command and want to tweak a parameter. This makes that easier.

### Macros
The app has 20 macro buttons, which can you assign any commands you want. Macros can have more than one command (multi-line). It comes with default commands including some UBL mesh management, but you can edit them to make them as you see fit.
* Single click a macro button to send the commands.
* Right click a macro button to edit it.
* You can drag/drop text from the output window onto a macro button. This will open a macro edit window with your dragged text inserted. The app will sanitize the text, stripping out any "echo" statements to make them runnable (useful for capturing your M501/503 config):
![Product Name Screen Shot][text-drag-drop-screenshot]

### Special Command Responses
The app will notice if certain responses are received from the printer, currently just M501/M503 config output and G29 T1 (deliminated mesh) output.
* M501/503 results can be dragged/dropped onto a macro. It will strip off all the non-command stuff leaving you with just the commands you would send to re-set your conifg. This should make it easier to bridge a firmware upgrade.
![config-to-marco-screenshot]
* G29 T1 Mesh Result will be processed, and let you double-click to see a mesh visualization.
![mesh-view-screenshot]
* List Files and Start Prints: M20 command results will be recognized as a list of files, and doubleclicking on a file will start printing it.
![file-list-screenshot]

<!-- ROADMAP -->
## Roadmap
Literally while typing this, I realize I should make the app process mesh gcode results (I don't think a lot of people know about this feature? You can have marlin output the gcode necessary to re-create your mesh. It's like an export you do prior to a firmware update, since the update will erase your mesh). You can highlight and drag/drop G29 S-1 output and it'll "commandify" it, but a nice processed result would be better.

<!-- Install -->
## ~~Install~~ ALL PREBUILT EXEs ARE OUT OF DATE - NOT RECOMMENDED!
I have no idea how I should distribute this - maybe send a hint my way?
I've created an Installers folder, where I've put some install options:

#### Single Exe 
You can also download [Guppy.zip][exe-link] and inside you'll find a single-file-exe built for Win x64 .Net 3.1, that might run for you? Runs for me. I have no idea how to make you feel even remotely safe about downloading and running a random exe from the internet.

* Name: GuppySingleExe.zip
* Size: 61888135 bytes (59 MiB)
* SHA256: AEB1C0B62C15BF71CF974B86A88FCBA3FBA9B74B3BF6B5899F86EE21D50EE21A

#### ClickOnceWinx64
ClickOnce Installer, no auto-update. Win x64 and Framework Dependent. Prerequesites should download from MS.
ZIP File of this installer has the following CRC:

* Name: ClickOnceWinx64.zip
* Size: 2539440 bytes (2479 KiB)
* SHA256: CF6B9DF74340609BDCD6D64F1869FC2F8AC14A8BCADDBFC9277BB3B700711132




#### Winx64FrameworkDependent
This is the slimmest option. It's only for Win x64. It might expect you to already have the .Net Framework? Or maybe it'll install it for you from Microsoft?
ZIP File of this installer has the following CRC:

* Name: Winx64FrameworkDependent.zip
* Size: 1116270 bytes (1090 KiB)
* SHA256: 56C7227EA06E1FCA0E520ACB467AA420E9CE77B83106419FC0861690FC84576B

<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
* [WPFSurfacePlot3D](https://github.com/brittanybelle/wpf-surfaceplot3d)
* [Best Readme Template](https://github.com/othneildrew/Best-README-Template) which I largely ignored :(


<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->
[product-screenshot]: Images/M503ProcessedResponse.jpg
[text-drag-drop-screenshot]: Images/TextDragDropCommandified.jpg
[config-to-marco-screenshot]: Images/ConfigTurnedIntoMacro.jpg
[mesh-view-screenshot]: Images/MeshView.jpg
[file-list-screenshot]: Images/FileListAndPrintStart.jpg
[exe-link]: /Installers/GuppySingleExe.zip
