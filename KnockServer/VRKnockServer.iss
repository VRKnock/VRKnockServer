
[Setup]
#define AppVersion GetFileVersion("bin\Release\KnockServer.exe")
#define AppName "VRKnockServer"

AppName={#AppName}
AppVersion={#AppVersion}
AppVerName={#AppName} {#AppVersion}
VersionInfoVersion={#AppVersion}
WizardStyle=modern
DefaultDirName={autopf}\{#AppName}
DefaultGroupName={#AppName}
UninstallDisplayIcon={app}\KnockServer.exe
Compression=lzma2
SolidCompression=yes
SourceDir="bin\Release"
OutputDir="Installer"
OutputBaseFilename={#AppName}_{#AppVersion}_setup

[Files]
Source: "KnockServer.exe"; DestDir: "{app}"
Source: "KnockServer.exe.config"; DestDir: "{app}"
Source: "manifest.vrmanifest"; DestDir: "{app}"
Source: "openvr_api.dll"; DestDir: "{app}"
Source: "QRCoder.dll"; DestDir: "{app}"
Source: "Resources\*"; DestDir: "{app}"

[Code]
function UserDomainAndName(Param: String): String;
begin
  Result := GetEnv('UserDomain') + '\' + GetEnv('UserName');
end;

[Run]
Filename: "netsh.exe"; Parameters: "http add urlacl url=http://+:16945/ user={code:UserDomainAndName}" 


[Icons]
Name: "{group}\VRKnockServer"; Filename: "{app}\KnockServer.exe"
