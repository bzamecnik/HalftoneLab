# Halftone Laboratory

**Abstract**

A modular library for digital halftoning methods. The aim were photorealistic
methods utilizing space-filling curves and blue-noise, artistic methods were
considered as well. From the point of view of software design, emphasis
was on modularity and extensibility, in order to support experimenting, even
with new algorithms. The library along with a graphical user interface has
been integrated as a plug-in with the framework of a popular image-processing
application GIMP.

- Author: [Bohumir Zamecnik](http://zamecnik.me)
- Release Date: 2009-08-03
- License: The MIT License, see the LICENSE file
- Project website (on SourceForge):
	- [wiki](https://sourceforge.net/apps/trac/halftonelab/wiki)
	- [download page](http://sourceforge.net/projects/halftonelab/)
- Thesis text: [Advanced Halftoning Methods](https://www.dropbox.com/s/m7x22sj68vz288a/advanced-halftoning-methods_zamecnik_thesis_2009.pdf?dl=0)

## System Requirements

Halftone Laboratory should run everywhere where Gimp# is able to run.
Currently, it was tested sucessfully on Windows XP SP2 with .NET 3.5,
GIMP 2.4.6, GIMP# 0.14 and GTK# 2.10.

* Operating system: Windows XP, Linux
* Microsoft .NET Framework 3.5 / Mono 2.4
* GIMP >= 2.4
* GIMP# >= 0.13

## Installation Instructions

First make sure you have Microsoft .NET Framework 3.5 installed, otherwise
HalftoneLab will not work. You can eventually use the enclosed incremental
installers of .NET 2.0, 3.0, 3.5, and optionally 3.5 SP1. Note that some of
those installer might need internet accesss.

Then GIMP and GIMP# must be installed, run `gimp-2.4.6-i686-setup.exe` and
then `gimp-sharp-setup-0.14-gimp-2.4.exe`. After that you can install
HalftoneLab with `halftonelab-setup.exe`. All installers act as wizards, so the
installation is straightforward.

HalftoneLab stores its configuration in halftonelab.cfg file, located in the
Application Data directory. On Windows platform it can be Application Data in
the user directory, on other systems .config in the home directory, but it
depends on specific operating system and its configuration. If the
configuration file does not exist, it is automatically created and filled with
some example configuration.

## User's Guide

The basic graphical user interface for HalftoneLab project is available in
form of a GIMP plug-in. So at first launch the GIMP application and open an
image. Currently, HalftoneLab is able only to process GIMP images in greyscale
mode. To convert an image to that mode click in the menu on *Image -> Mode ->
Greyscale*. Then you can start HalftoneLab plug-in: *Filters -> Distorts ->
Halftone Laboratory*.

A main dialog opens. It consist of several panels:

* Configuration saving and loading
* Pre-processing
* Halftone method
* Post-processing

### Configuration saving and loading

This panel enables you to manage algorithm configurations - save current one
along with a name and description to a persistent storage and load it again.
Select a name from the combobox to load that configuration. There are two
special configurations: `_DEFAULT` reverts all modules to their default
settings, `_LAST` is the last configuration automatically saved at the end of
the previous session.

To save the current configuration hit the Save button on the configuration
panel. A dialog prompting for configuration name and description appears. A
name is mandatory and cannot be `"_DEFAULT"` or `"_LAST"` or the configuration is
not saved, a description is optional, but can help to quickly see what the
algorithm does. The configurtion selected in the combobox can be removed
hitting the Delete button.

### Pre-processing

In the pre-preprocessing panel you can configure how the image will be altered
before halftoning itself takes place. Modules are ordered as they will be
exeuted. Each module can be enabled or disabled.

#### Resize

With this module you can scale the image by a given factor using selected
interpolation method - *Nearest neighbour, Bilinear,
Bicubic, Lanczos.*

#### Sharpen

Sharpening prior to halftoning can help preserving image details as most
halftoning methods slightly blur the image. The amount of sharpening can be
set.

#### Dot gain

Dot gain (see page dotgain) caused by the behaviour of some printing processes
can be corrected here. Simple gamma correction acts as a rough approximation
of dot gain curves.

### Post-processing

Post-processing panel embraces modules executed after the halftoning. It acts
similarly to the pre-processing panel.

#### Resize

The post-processing resize module acts the same way as its pre-processing
counterpart. It is typically used to down-sample the image. You can perform
supersampling technique with the two resize modules - in the pre-processing
one set the desired upsample factor and in the other module enable
Supersampling checkbutton. The downsample factor will be computed
automatically. Bilinear interpolation is a recommended method for
supersampling. 

#### Smoothen

If you want to smoothen the halftoned image without the computation overhead
of supersamping you can use the Smoothen module. First it Gaussian-blurs the
image with given radius and then it applies *Levels* GIMP command. Thus, the
rough black and white contours get perfectly smooth. However, the cost is
bluring some fine details.  

### Halftone method

This is the most important control panel in the plug-in. There you can set
the type of halftoning method to be used and its details. The panel itself
follows a concept of submodule selector, which appears on many places inside
specific module configuration dialogs. It consists of three elements: In a
combobox there are submodule types, one of which can be selected. On hitting
the *Edit* button a configuration dialog for that submodule type opens (provided
there is anything to configure). Sometimes, the submodule is optional and it
is possible that no type is set. To achieve this there is a null checkbutton.
If it gets checked the module is unset and its internal settings get deleted.

#### Thresholding halftone method

In thresholding methods the image is processed pixel-wise along a scanning 
order given by a `ScanningOrder` module. Each pixel is quantized using threshold
computed by a `ThresholdFilter` module and the quantization error is optionally
diffused by a `ErrorFilter` module. Each submodule can be configured via a
submodule configuration panel. As for `ErrorFilter`, there is a *Use error
filter?* checkbutton to enable or disable the module without removing its
configuration.

#### SFC clustering halftone method

SFC clustering method, described on page sfc:clustering, works differently.
The main parameter, Maximum cell size, controls the coarseness of the halftone
(or the spatial vs. tonal resolution trade-off) - lower values give finer
detais, but higher values enable representing more tones. There are
checkbuttons for controlling the use of adaptive clustering (ie. varying cell
sizes according to local detail amount) and cluster positioning techniques.
Setting Minimum cell size value is meaningful only when using adaptive
clustering.

The type of space-filling curve to scan along can be selected via Scanning
order submodule panel. Currently only Hilbert SFC is supported. An optional
`VectorErrorFilter` can be used.
	
#### Matrix threshold filter

Matrix threshold filter enables you to edit a single thresholding matrix.
The matrix dimensions can be changed using the *Resize* button.

#### Dynamic matrix threshold filter

Dynamic matrix threshold filter allows a different matrix for each pixel
intensity or a range of intensities as well as perturbing their threshold
coefficients with random noise. There is a table of records consisting of
a threshold matrix, starting intensity of that rangethe range spans up to the
next record and noise amplitude. You can add a new record hitting the New
button, which opens a separate dialog. Selected record can be edited using the
Edit button in the same dialog or deleted with the Delete button. To delete
all records at once hit the *Clear all* button. Applying perturbation noise can
be enabled or disabled for the whole table with the Noise enabled? checkbutton.

#### Spot function threshold filter

A spot function analytically defines threshold values for digital screening
(see page spotfunc). There are several presets for most common spot functions:
Euclid dotwith growth: circle -> square at intensity 0.5 -> circle, Euclid dot
with random perturbation, Square dot, Line, Triangle dot. For each preset two
parameters can be set: Screen angle (in radians) - the angle of screen
rotation, Screen line distance (in pixels) - distance between adjacent screen
elements.

#### Image threshold filter

Image threshold filter behaves much like {Spot function threshold filter,
except that spot functions are not evaluated at run-time but rather a big
threshold matrix is pre-generated - to a GIMP image. The advantage is that
the image can be then distorted with GIMP image filters. The possibilities are
tremendous. A few examples are provided in the form of presets. The initial
spot function can be configured the same way as in Spot function threshold
filter. Currently, the effects applied cannot be controled, only a description
is shown. One of the presets use GIMP Patterns to tile the plane, histogram is
then equalized - this with an error filter enabled give approximately the
Veryovka-Buchanan method Veryovka99 veryovka. To select a pattern, go to
*Dialogs -> Pattern* in *Image* window menu and select the desired pattern.

#### Matrix error filter

Matrix error filter performs error-diffusion with a single error matrix.
The way of its editing is similar to Matrix threshold filter dialog.
Dimensions of the matrix can be changed using the Resize button.

Error matrix coefficients are represented as fractions - numerators are in the
table and a common denominator is in the Divisor spinbutton. As the sum of all
error matrix coefficients must be equal to $1.0$, they must be scaled using
the their sum. However, in case you want to experiment, you can choose a
different divisor. Just enable the Use a custom divisor? checkbutton and fill
the Divisor spinbutton.

The relative position of source pixel in the matrix can be controlled by the
Source offset X (the Y offset is always the same - the first row).

#### Dynamic matrix error filter

This is a concept similar to Dynamic matrix threshold filter (and the controls
are almost identical) - there can be different error matrices for different
pixel intensities or intensity ranges, however, there is no noise added.

#### Randomized matrix error filter

Error matrix coefficients can be generated randomly for each pixel. There is
a 'template' matrix which defines the dimensions of the final matrix and where
the coefficients would be generated. A constraint is that error can be
distributed only to places after the source pixel (following the scanning
order). There are two possible modes controlled by the Randomize coefficient
count? checkbox: If it is enabled, coefficients will be generated exactly in
place of all non-zero coefficients in the template matrix, maintaining a
constant number of them, otherwise the number and position of coefficients
will be varied up to the available capacity of the matrix. 

#### Perturbed matrix error filter

For perturbing error matrix coefficients with random noise a separate filter
is used. Inside it has a MatrixErrorFilter child filter as a submodule
controlled by a standard submodule panel. In addition to it the amount of
noise can be controlled by the Perturbation amplidute scale.

Currently, Perturbed matrix error filter does not support Dynamic matrix
error filter as its child filter. 

#### Vector error filter

For 1-D scanning, such as along an space-filling curve, a vector error filter
is suitable. Its interface is similar to that of Matrix error filter, except
that it can be resized only in one dimension (Length).

#### Scanning order

For general scanning order you can select among Scanline, Serpentine and
Hilbert scanning, for SFC scanning only Hilbert is available. Scanline
processes pixels line by line in the same direction, serpentine in a zig-zag
direction. Hilbert scans along an approximation of the Hilbert space-filling
curve.
