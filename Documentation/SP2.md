# `SP2` File format

The `.sp2` file format is a file format specific to the SM64DSe editor, it allows to define a series of instructions
that will be executed in sequence.

It **must** be a text file, with a _command_ per line.

## Instructions

**generate_file_list**

Generate a `C` header at the given destination path with the game files listed.

````
generate_file_list  "[DEST-FILE]"
````

**generate_sound_list**

Generate a `C` header at the given destination path with the game sounds listed.

````
generate_sound_list  "[DEST-FILE]"
````

**compile**

The compile instruction has two options, the `dl` (aka dynamic library) and `overlay`.

````
compile (dl|overlay) "[INTERNAL-FILE]" "[SOURCE-FOLDER]" "[SOURCE-SUB-DIRECTORY]"
````

a `clean` option also exist, it is mainly used to ensure we are building from a clean project

````
compile clean
````

**add_or_replace**

````
add_or_replace "[INTERNAL-DIRECTORY]" "[INTERNAL-NAME]" "[LOCAL-FILE]"

# Example

add_or_replace "data/custom_obj/peach_npc/" "peach_low.bmd" "Assets/PeachNPC/peach_low.bmd"

+ "data/custom_obj/peach_npc/" is the directory inside my rom
+ "peach_low.bmd" is the name that will be used for the file inside the rom
+ "Assets/PeachNPC/peach_low.bmd" is the local directory relative to our .sp2 file where the file we want to insert is located.
````

**replace**

Similar to `add_or_replace` but will throw an error if the file does not exist.

**rename**

Rename an internal file from the rom filesystem.

````
rename "[INTERNAL-FILE-PATH]" "[INTERNAL-NEW-NAME]"

# Example

rename "data/custom_obj/peach_npc/peach_low.bmd" "peach.bmd"
````

**remove**

Remove an internal file from the rom filesystem.

````
remove "[INTERNAL-FILE-PATH]"
````

**add_dir**

Add a directory in the rom filesystem.

````
add_dir "[INTERNAL-DIRECTORY]" "[DIRECTORY-NAME]"

# Example

add_dir "data/" "dynamic"
````

**rename_dir**

Rename a directory in the rom filesystem.

````
rename_dir "[INTERNAL-DIRECTORY]" "[NEW-DIRECTORY-NAME]"

# Example

rename_dir "data/dynamic" "very_dynamic"
````

**remove_dir**

Remove an internal directory from the rom filesystem.

````
remove_dir "[INTERNAL-DIRECTORY-PATH]"
````
