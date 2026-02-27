; PDF Merger Inno Setup Script
; Replaces WiX MSI installer for .NET 8 self-contained deployment

#define MyAppName "PDF Merger"
#define MyAppVersion "3.0.0"
#define MyAppPublisher "Ugur CORUH"
#define MyAppURL "https://ucoruh.github.io/pdf-merger/"
#define MyAppExeName "PdfMerger.exe"
#define MyAppSourceURL "https://github.com/ucoruh/pdf-merger"
#define MyAppUpdatesURL "https://github.com/ucoruh/pdf-merger/releases"

[Setup]
AppId={{EFE25C54-FF47-46BE-8389-082CF73E8F82}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppSourceURL}
AppUpdatesURL={#MyAppUpdatesURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
LicenseFile=..\src\PdfMergerSetup\res\Readme.rtf
WizardImageFile=..\src\PdfMergerSetup\img\pdfmerger_bg.bmp
WizardSmallImageFile=..\src\PdfMergerSetup\img\banner.bmp
WizardImageStretch=yes
OutputDir=output
OutputBaseFilename=PdfMergerSetup-{#MyAppVersion}
SetupIconFile=..\src\PdfMerger\pdfmergerlogo.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
Compression=lzma2
SolidCompression=yes
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
WizardStyle=modern

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"
Name: "startmenuicon"; Description: "Create a Start Menu shortcut"; GroupDescription: "{cm:AdditionalIcons}"; Flags: checkedonce

[Files]
Source: "..\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: startmenuicon
Name: "{group}\Uninstall {#MyAppName}"; Filename: "{uninstallexe}"; Tasks: startmenuicon
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent
