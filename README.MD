# Download Manager
<p align="center">
<img alt="CodeQL workflow status" src="https://img.shields.io/github/actions/workflow/status/Download-Manager-Community/Download-Manager/codeql-analysis.yml?branch=master&event=push">
<img alt="Actions Workflow Status" src="https://img.shields.io/github/actions/workflow/status/Download-Manager-Community/Download-Manager/dotnet-desktop.yml?label=Artifacts%20Build">
<img alt="Open issues" src="https://img.shields.io/github/issues/Lilyy2565/Download-Manager"> 
<img alt="Total downloads" src="https://img.shields.io/github/downloads/Lilyy2565/Download-Manager/total">
<img alt="License" src="https://img.shields.io/github/license/Lilyy2565/Download-Manager">
<img alt="GitHub Sponsors" src="https://img.shields.io/github/sponsors/Lilyy2565">
<img alt="Commit activity" src="https://img.shields.io/github/commit-activity/m/Download-Manager-Community/Download-Manager">
</p>

A fast download manager for Windows to download your files.
Download Manager now contains a [browser extention](https://microsoftedge.microsoft.com/addons/detail/download-manager/facopbimneimllhcabghncloejfeficd) to help you download your files as easily as possible.
Just install the extention and download a file!
For requests for file extentions please make a [Feature Request](https://github.com/Lilyy2565/Download-Manager/issues/new?assignees=&labels=enhancement&template=feature_request.md&title=).

If you have any issues with Download Manager please make an [issue](https://github.com/Lilyy2565/Download-Manager/issues/new/choose).

*To check for any updates regarding the next update of Download Manager go to the [Discussions](https://github.com/Lilyy2565/Download-Manager/discussions) page.*

## Compatability
Older versions of Download Manager do not support high DPI monitors.
If you are experiecing display issues, try updating your version of Download Manager or please use the following compatability settings:

<img src=".github/images/CompatabilitySettings.png" width="196.5px" height="246px">

## Installation Instructions
### Winget Install
1) Open a CMD or Powershell window.
2) Type `winget install Lilyy2565.DownloadManager`.
3) Wait for installation to complete.
4) You can also install the browser extention [here](https://microsoftedge.microsoft.com/addons/detail/download-manager/facopbimneimllhcabghncloejfeficd?hl=en-GB).
### Manual Install (Portable)
1) Download the latest portable version from the [releases page](https://github.com/Lilyy2565/Download-Manager/releases).
2) Run the installer
3) Follow the on screen instructions.
4) You can also install the browser extention [here](https://microsoftedge.microsoft.com/addons/detail/download-manager/facopbimneimllhcabghncloejfeficd?hl=en-GB). 
### Manual Install (Zip)
1) Download the latest unpacked installer version from the [releases page](https://github.com/Lilyy2565/Download-Manager/releases).
2) Extract the zip anywhere.
4) Run the executable `DownloadManagerInstaller.exe`.
5) Follow the on screen instructions.
6) You can also install the browser extention [here](https://microsoftedge.microsoft.com/addons/detail/download-manager/facopbimneimllhcabghncloejfeficd?hl=en-GB).

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
1) [Fork](https://github.com/Lilyy2565/Download-Manager/fork) the repository.
2) To be able to properly build Download Manager and have it behave correctly you must run the "First-Time Setup" debug option before building *(figure 1 below)*
3) Make your changes
4) Create a [pull request](https://github.com/Lilyy2565/Download-Manager/pulls) to the `master` branch.
5) When you are ready request `Lilyy2565` as a reviewer.
6) If the pull request is ok it will be merged with the `master` branch.
7) If there are improvements that need to be made before the branch can be merged, make them and request `Lilyy2565` as a reviewer again.

*To build Download Manager successfully you may have to build the application in the `Debug` build configuration as the `Release` configuration does not work.*

<!-- .github/images/SetupBuild.png -->

| <img src="https://github.com/Download-Manager-Community/Download-Manager/blob/master/.github/images/SetupBuild.png" width="350px" height="auto"> |
|----------|
|*Figure 1*|


### Option 3 (Sharing and starring)
If you dont want to do any of the above but still want to contribute in some way, then leave a star on this repository and share it to others to help other people find this program.
