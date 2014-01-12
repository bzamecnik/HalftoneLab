# Notes

## Running GIMP

If you want to see console with GIMP stdandard output, run GIMP with the `-c` parameter:

```
"C:\Program Files\Gimp-2.0\bin\gimp-2.4.exe" -c
```

To run GIMP in English locale:

```
set LANG="C"
"C:\Program Files\Gimp-2.0\bin\gimp-2.4.exe"
```

You can save a lot time when running GIMP if you delete the other GIMP#
plug-ins or move them somewhere else.

## Deployment for development

For manual deploying the plugin just copy files `halftone.dll`, `halftonegui.dll`,
`HalftoneLab.exe` from `src/GimpSharpPlugin/bin/Release/` to
`C:\Program Files\Gimp-2.0\lib\gimp\2.0\plug-ins`.

If you want to see more debugging information (eg. lines on stack trace), also
copy `halftone.pdb`, `halftonegui.pdb`, `HalftoneLab.pdb` to the same directory.

Gimp needn't be closed and run again, this can save you a LOT of time during
the development cycle. 

The `halftonelab.cfg` configuration file is located usually at
`C:\Documents and Settings\<username>\Application Data\halftonelab.cfg`

If you modify the module structure (they differ in serialization) the
`halftone.cfg` file may not be compatible and have to be moved somewhere else
or deleted.

## Building

The HalftoneLab project can be build using the Microsoft Visual C# 2008.
The only references (GIMP#, GTK#) needed to build the project are in `src/lib/`
directory. Also, it is possible to create some makefiles and build using
Mono, although it is not tested for the current project version.

## Doxygen

To generate the API docs we use [Doxygen](http://www.stack.nl/~dimitri/doxygen/).
Run the `doxygen` command in the `src/` directory where the `Doxyfile` is located.

## Installer

The windows installer is compiled by [NSIS](http://nsis.sourceforge.net)
using the `src/installer/halftonelab.nsi` script.

Just copy the files:
  `halftone.dll`, `halftonegui.dll`, `HalftoneLab.exe`,
  `halftone.pdb`, `halftonegui.pdb`, `HalftoneLab.pdb`
from:
  `src/GimpSharpPlugin/bin/Release/`
to:
  `installer/`
and run NSIS.
