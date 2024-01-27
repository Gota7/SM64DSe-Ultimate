# Installation

You can download the latest version from releases section and download the `release.zip`.

Simply unzip the content in a folder, and launch `SM64DSe.exe`.

# Usage

The SM64DSe editor has 2 usage the traditional through its GUI (Graphical User Interface) or through CLI (Command Line Interface).

## 🖼️ Graphical User Interface

You can simply double click on the `SM64DSe.exe` file you downloaded, and it will open the interface.

> The editor support drag and drop a .nds file.

## #️⃣ Command Line Interface

The CLI is mainly work in progress as it only support a few elements.

> The CLI is aimed to provide tools for developers to 

**patch**

The `patch` command is used to execute a `.sp2` file containing instructions.
````
SM64DSe.exe patch [ROM-FILE] [SP2-FILE]

# flags

When dealing with vanilla ROM, the default behavior of the CLI is to exit the program, as it will not edit vanilla rom by default. 
You can force the editor to install the required patch for vanilla rom to be edited using the following flag.

+ `--force` force the editor to use the required patch on the rom
````

> Lean more about the `.sp2` format in [Documentation/SP2](Documentation/SP2.md).