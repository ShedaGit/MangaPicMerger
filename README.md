# MangaPicMerger WPF Application

MangaPicMerger is a Windows Presentation Foundation (WPF) application that allows users to merge two images and create a new image with a bar between the two merged images if needed. The application has been refactored to use the Model-View-ViewModel (MVVM) architecture to improve its readability, maintainability, and testability.

## Installation

The application does not require installation, simply download and run the executable file. Alternatively, you can compile the source code and run it from your local machine.

### System Requirements

- Windows 10 or later
- .NET 7.0 or later

## How to Use

1. Click on the "Browse" button and select two images you want to merge. They can be in PNG or JPEG format.
2. If two images are selected, they will be displayed in the left and right image viewers.
3. If needed, click on the "Switch" button to switch the left and right images.
4. Select the type of bar that you want to appear between the two merged images:
    - No Bar
    - White Bar
    - Black Bars
5. If you selected a white or black bar, select the width of the bar using the slider or by typing a value into the textbox.
6. Click on the "Merge" button to create the merged image.
7. If prompted, choose a name and location for the merged image and select a file type (PNG or JPEG).

*Note: Both images should have the same dimensions, otherwise the smaller of the images will be attached to the top of the bigger.*

## Future Features

In addition to the existing functionality, the following features are planned for future releases of MangaPicMerger:

- **WebP image format support:** Ability to merge images in the `WebP` format.
- **Upscaling images:** Capability to upscale an image to match the resolution of the other image while maintaining the same proportions.
- **Merged image preview:** Preview the merged image before saving it.
- **Zooming in and out of the preview image:** Allow users to zoom in and out of the merged image preview to examine it more closely.
- **Image rotation:** Ability to rotate images before merging.
- **Showing image metadata:** Display image metadata such as resolution, file size, and format.
- **Dark mode:** Option to switch between light and dark mode.
- **Grouping images:** Capability to merge more than two images at once by grouping images in pairs.
- **Keyboard shortcuts:** Support for keyboard shortcuts to speed up user interactions.

### Contributing

If you want to contribute to the development of this application, feel free to create a pull request. If you encounter any bugs or have any feature requests, please create an issue in the repository.
