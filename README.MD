# Download Manager
![GitHub Workflow Status](https://img.shields.io/github/workflow/status/Soniczac7/Download-Manager/CodeQL) ![GitHub issues](https://img.shields.io/github/issues/Soniczac7/Download-Manager) ![GitHub all releases](https://img.shields.io/github/downloads/Soniczac7/Download-Manager/total) ![GitHub](https://img.shields.io/github/license/Soniczac7/Download-Manager)

A fast download manager for Windows to download your files.
Download Manager now contains a browser extention to help you download your files as easily as possible.
Just install the extention and download a file!
For requests for file extentions please make a [Feature Request](https://github.com/Soniczac7/Download-Manager/issues/new?assignees=&labels=enhancement&template=feature_request.md&title=).

If you have any issues with Download Manager please make an [issue](https://github.com/Soniczac7/Download-Manager/issues/new/choose).

*To check for any updates regarding the next update of Download Manager go to the [Discussions](https://github.com/Soniczac7/Download-Manager/discussions) page.*

## Compatability
Older versions of Download Manager do not support high DPI monitors.
If you are experiecing display issues with older versions of Download Manager please use the following compatability settings:

<img src=".github/images/CompatabilitySettings.png" width="196.5px" height="246px">

## Installation Instructions
1) Download the latest version from the [releases page](https://github.com/Soniczac7/Download-Manager/releases).
2) Extract the zip anywhere.
4) Run the executable `DownloadManagerInstaller.exe`.
5) You can also install the browser extention [here](https://microsoftedge.microsoft.com/addons/detail/download-manager/facopbimneimllhcabghncloejfeficd?hl=en-GB).

## Silent Install
If you choose to install the program silently using the `--install` argument please refer to the following exit codes:
 - `0` - The installation completed successfully.
 - `1` - The default drive `C:\` could not be found.
 - `2` - The installer could not install due to insufficent permission.
 - `3` - The installer could not install due to an unknown IO error.
 - `4` - The application is already installed.
 - `5` - The disk is full.
 - `6` - The installer could not install due to an unknown exception.

## Manually Updating
If you want to manually initiate a update to Download Manager through the command line then you can use the `--update` argument in the installer. The exit codes for the updater are:
 - `0` - The installation completed successfully.
 - `1` - Setup determined that Download Manager was already running the latest version.
 - `2` - Setup exited before the installation could complete.
 - `3` - The update XML file is malformed.
 - `4` - The update failed due to an error during the installation process.
 - `5` - Setup could not retrieve the existing Download Manager installation. (Ensure the installer is in the same path as Download Manager and try again.)
 
## Contribution
Any contribution to Download Manager no matter how small will be appreciated.
There are multiple ways to contribute to Download Manager. They are:

### Option 1 (Creating a issue)
Opening issues are essential for open source software so the developers know what can be worked on and what bugs need to be fixed or what features the community wants added.
There are 3 different types of issues that can be opened on Download Manager.

1) **Bug reports** are very useful because they help the community to help create a more stable application for everyone.
2) **Feature requests** are very useful because they help the community maintainers to figure out what to make next in the application.
3) **Security Vulnerability** reports are very important because it helps the developer patch a security issue.

### Option 2 (Contributing code)
If you would like to contribute code you can do the following to get started:
1) [Fork](https://github.com/Soniczac7/Download-Manager/fork) the repository.
2) Create a [pull request](https://github.com/Soniczac7/Download-Manager/pulls) to the `master` branch.
3) When you are ready request `Soniczac7` as a reviewer.
4) If the pull request is ok it will be merged with the `master` branch.
5) If there are improvements that need to be made before the branch can be merged, make them and request `Soniczac7` as a reviewer again.

*To build Download Manager successfully you may have to build the application in the `Debug` build configuration as the `Release` configuration does not work.*

### Option 3 (Sharing and starring)
If you dont want to do any of the above but still want to contribute in some way, then leave a star on this repository and share it to others to help other people find this program.
