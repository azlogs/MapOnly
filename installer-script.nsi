; Name of the installer
OutFile "GOM FbUploader Installer.exe"

; Default installation directory
InstallDir "$PROGRAMFILES\GOM FbUploader"

; Request application admin privileges for installation
RequestExecutionLevel admin

; Installer pages
Page directory
Page instfiles

; Uninstaller pages
UninstPage uninstConfirm
UninstPage instfiles

; Section for installation
Section "Install"
    ; Set installation directory
    SetOutPath "$INSTDIR"

    ; Copy the application executable
    File "dist\GOM FbUploader.exe"

    ; Create shortcuts
    CreateShortcut "$DESKTOP\GOM FbUploader.lnk" "$INSTDIR\GOM FbUploader.exe"
    CreateShortcut "$SMPROGRAMS\GOM FbUploader\GOM FbUploader.lnk" "$INSTDIR\GOM FbUploader.exe"

SectionEnd

; Section for uninstallation
Section "Uninstall"
    ; Delete application files
    Delete "$INSTDIR\GOM FbUploader.exe"

    ; Delete shortcuts
    Delete "$DESKTOP\GOM FbUploader.lnk"
    Delete "$SMPROGRAMS\GOM FbUploader\GOM FbUploader.lnk"

    ; Remove installation directory
    RMDir "$INSTDIR"
SectionEnd
