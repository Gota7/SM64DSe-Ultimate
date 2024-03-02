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

### **🔨 compile**

The `compile` command aims to provide an easy way to build and insert binaries and overlays from source code.

> Currently only the dynamic libraries support insertion. 
> An utility option called `TARGET` allow you to simply run make in a given folder.
> This can be useful when running commands in batches.

````
SM64DSe.exe compile (DL|OVERLAY|TARGET) [SOURCE-CODE] [INTERNAL-PATH]

# flags
+ `--rom` path to the rom (nds format)

+ `--force` force the editor to use the required patch on the rom
+ `--create` if the internal path does not exist, the file will be created, by default it replaces an existing one.
+ `--recursive` if the internal path provided does not exist, create the parent directory.
+ `--env` environment variable to provide to the make command. E.g. `--env SOURCES=code/peach --env INCLUDE=includes` 

# Example

SM64DSe.exe compile DL ./src data/dynamic/peach_npc.dylb --force --create --recursive --rom="./EUROPE.nds"
SM64DSe.exe compile TARGET ./src 
````

### **📁 filesystem**

The filesystem or in short `fs` command allows you to interact with the rom's filesystem.

To make the difference between your computer filesystem and the rom filesystem, we use URI (Uniform Resource Identifier) to indicate when a rom filesystem is targeted.
> E.g. `rom://file/data/message/msg_data_eng.bin` is an URI with `rom` as scheme and `file` as authority. the remaining path is the internal path of the targeted file/directory.
>
> ⚠️ Currently only `file` authority is supported. Later we could support `overlay` if some people needs it.

````
SM64DSe.exe fs ( rm | cp ) [TARGET] [DESTINATION]

# flags
+ `--rom` path to the rom (nds format)
+ `--force` force the editor to use the required patch on the rom
+ `--create` if the internal path does not exist, the file will be created, by default it replaces an existing one.
+ `--recursive` if the internal path provided does not exist, create the parent directory.

# Example

# Copy a local `msg_data_eng.bin` file inside the rom filesystem in `data/message/msg_data_eng.bin `
SM64DSe.exe fs cp ./msg_data_eng.bin rom://file/data/message/msg_data_eng.bin --rom="./EUROPE.nds"
# Delete the rom file data/message/msg_data_eng.bin 
SM64DSe.exe fs rm rom://file/data/message/msg_data_eng.bin --rom="./EUROPE.nds"
# Copy one rom file to another path of the rom
SM64DSe.exe fs cp rom://file/data/message/msg_data_eng.bin rom://file/data/message/msg_data_eng.bin.bak --rom="./EUROPE.nds"
````

### **📘 generate**

The editor is often used to generate headers file for sounds and files. You can do the same using the `generate` command.

````
SM64DSe.exe generate ( sound | filesystem ) [OUTPUT]

# flags
+ `--rom` path to the rom (nds format)

# Example

SM64DSe.exe generate filesystem files.h --rom="./EUROPE.nds"
SM64DSe.exe generate sound sound.h --rom="./EUROPE.nds"
````

### **📥 insertDLs**

The `insertDLs` command will use already built binaries to generate one or many dynamics libraries and insert them inside the rom filesystem.

````
SM64DSe.exe insertDLs [BUILD-FOLDER] [TARGETS-FILE]

# flags
+ `--rom` path to the rom (nds format)
+ `--newcode-lo` (Default `newcode_lo`)
+ `--newcode-hi` (Default `newcode_hi`)

+ `--force` force the editor to use the required patch on the rom
+ `--create` if the internal path does not exist, the file will be created, by default it replaces an existing one.
+ `--recursive` if the internal path provided does not exist, create the parent directory.

# Example

SM64DSe.exe insertDLs ./build ./targets.json --newcode-lo=newcode --newcode-hi=newcode1 --rom="./EUROPE.nds"
````

The options `--newcode-lo` and `--newcode-hi` can be used to specify the names of the input files **without extensions**. The command assumes that each targeted folder contains two `.bin` files with the given filenames (`newcode_lo.bin` and `newcode_hi.bin` by default), and a `.sym` file with the name specified with `--newcode-lo` (or `newcode_lo.sym` by default).

⚠️ The editor **will not** build the binaries for you, you need to execute `make` or any building process ahead. This will only combine for each target the existing `newcode_lo` and `newcode_hi` in a dynamic library.

> The `TARGETS-FILE` values must point to a text file either **plain text** or **json**.
> 
> For plain text, the format is per line `[directory]: [internal-path]`. E.g. 
> ```
> infinite_floor: data/enemy/peach/peach_jump.bca
> test_cutscene:  data/enemy/peach/peach_jump_end.bca
> ```
> 
> For json format, we simply require a dictionnary string: string. E.g.
> 
> ```{"infinite_floor": "data/enemy/peach/peach_jump.bca", "test_cutscene": "data/enemy/peach/peach_jump_end.bca"}```

You can find an example in the repository [pants64DS/Misc-SM64DS-Patches/dynamic_lib](https://github.com/pants64DS/Misc-SM64DS-Patches/tree/master/dynamic_lib)

### **📦 batches**

When dealing with many files, such as assets, dynamics libraries, we want to automatically assemble everything to ease the testing. 
This CLI allows you to makes it easy to interact with the filesystem of the game. However, having to run several time the CLI can be 
time consuming, each start having a delay etc. 

The `batches` command is here to speed up this process. Given a text file, on each line a CLI command, the editor will run all of them one by one.
````
SM64DSe.exe batches [TEXT-FILE]

# flags
+ `--rom` path to the rom (nds format)
+ `--force` force the editor to use the required patch on the rom
````

Here is an example of the content of a text file that can be provided.
````
# Support comments ! (this line will not be run)
fs cp ./sound_data.sdat rom://file/data/sound_data.sdat

compile TARGET Code
insertDLs Code/build Code/targets.json --create --recursive
````

> [!NOTE]
> Inside the text file provided, you should not set the --rom flag, as you should provide it with the batches command.

### ~~**🩹 patch**~~

> [!WARNING]
> The .sp2 format is deprecated and hopefully will be removed from the editor soon.

The `patch` command is used to execute a `.sp2` file containing instructions.
````
SM64DSe.exe patch [SP2-FILE]

# flags
+ `--rom` path to the rom (nds format)

When dealing with vanilla ROM, the default behavior of the CLI is to exit the program, as it will not edit vanilla rom by default. 
You can force the editor to install the required patch for vanilla rom to be edited using the following flag.

+ `--force` force the editor to use the required patch on the rom
````

> Lean more about the `.sp2` format in [Documentation/SP2](Documentation/SP2.md).

