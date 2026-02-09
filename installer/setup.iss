; NRUA Guest Manager - InnoSetup Installer Script
; Requires InnoSetup 6.x
; Compile with: ISCC.exe setup.iss

#define MyAppName "NRUA Guest Manager"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "NRUA Helper"
#define MyAppExeName "NRUAGuestManager.exe"
#define MyAppURL "https://nruahelper.com"

[Setup]
AppId={{B7E4F2A1-3C8D-4E5F-9A6B-1D2E3F4A5B6C}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
LicenseFile=license_es.txt
OutputDir=Output
OutputBaseFilename=NRUAGuestManager_Setup_{#MyAppVersion}
SetupIconFile=..\icon.ico
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
WizardImageFile=installer-banner.bmp
WizardSmallImageFile=installer-header.bmp
UninstallDisplayIcon={app}\{#MyAppExeName}
VersionInfoVersion={#MyAppVersion}.0
VersionInfoCompany={#MyAppPublisher}
VersionInfoProductName={#MyAppName}
VersionInfoProductVersion={#MyAppVersion}
ArchitecturesInstallIn64BitMode=x64compatible
PrivilegesRequired=admin

; Spanish language
[Languages]
Name: "spanish"; MessagesFile: "compiler:Languages\Spanish.isl"

[Tasks]
Name: "desktopicon"; Description: "Crear acceso directo en el &Escritorio"; GroupDescription: "Accesos directos adicionales:"

[Files]
Source: "..\publish\{#MyAppExeName}"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\Desinstalar {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Iniciar {#MyAppName}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: files; Name: "{userappdata}\NRUAGuestManager\license.key"
Type: dirifempty; Name: "{userappdata}\NRUAGuestManager"

[Code]
{ =====================================================================
  NRUA License Key Validation - Pascal Script
  Implements the same HMAC-SHA256-based validation as LicenseValidator.cs

  Since InnoSetup Pascal Script lacks native HMAC-SHA256, we implement
  a simplified but compatible checksum validation using the same logic.

  Key format: NRUA-AAAA-BBBB-CCCC-DDDD
  Charset: ABCDEFGHJKLMNPQRSTUVWXYZ23456789 (no 0,1,O,I)
  ===================================================================== }

const
  VALID_CHARS = 'ABCDEFGHJKLMNPQRSTUVWXYZ23456789';
  HMAC_SECRET = 'NRUAGuestMgr-2024-RD1312-LicKey!';

var
  SerialPage: TWizardPage;
  SerialEdit: TNewEdit;
  SerialStatusLabel: TNewStaticText;
  SerialValid: Boolean;

{ SHA-256 implementation for Pascal Script }
{ We use Windows CryptoAPI via DLL calls for HMAC-SHA256 }

function CryptAcquireContext(var hProv: THandle; pszContainer: String;
  pszProvider: String; dwProvType: DWORD; dwFlags: DWORD): Boolean;
  external 'CryptAcquireContextW@advapi32.dll stdcall';

function CryptCreateHash(hProv: THandle; Algid: Cardinal; hKey: THandle;
  dwFlags: DWORD; var hHash: THandle): Boolean;
  external 'CryptCreateHash@advapi32.dll stdcall';

function CryptHashData(hHash: THandle; pbData: AnsiString; dwDataLen: DWORD;
  dwFlags: DWORD): Boolean;
  external 'CryptHashData@advapi32.dll stdcall';

function CryptGetHashParam(hHash: THandle; dwParam: DWORD; pbData: AnsiString;
  var dwDataLen: DWORD; dwFlags: DWORD): Boolean;
  external 'CryptGetHashParam@advapi32.dll stdcall';

function CryptDestroyHash(hHash: THandle): Boolean;
  external 'CryptDestroyHash@advapi32.dll stdcall';

function CryptReleaseContext(hProv: THandle; dwFlags: DWORD): Boolean;
  external 'CryptReleaseContext@advapi32.dll stdcall';

function CryptImportKey(hProv: THandle; pbData: AnsiString; dwDataLen: DWORD;
  hPubKey: THandle; dwFlags: DWORD; var hKey: THandle): Boolean;
  external 'CryptImportKey@advapi32.dll stdcall';

function CryptDestroyKey(hKey: THandle): Boolean;
  external 'CryptDestroyKey@advapi32.dll stdcall';

{ Compute HMAC-SHA256 using manual inner/outer pad approach with CryptoAPI SHA-256 }
function ComputeSHA256(data: AnsiString): AnsiString;
var
  hProv, hHash: THandle;
  hashLen: DWORD;
  hashValue: AnsiString;
  i: Integer;
begin
  Result := '';
  // PROV_RSA_AES = 24, CRYPT_VERIFYCONTEXT = $F0000000
  if not CryptAcquireContext(hProv, '', '', 24, $F0000000) then Exit;
  try
    // CALG_SHA_256 = $0000800C
    if not CryptCreateHash(hProv, $0000800C, 0, 0, hHash) then Exit;
    try
      if not CryptHashData(hHash, data, Length(data), 0) then Exit;
      hashLen := 32;
      SetLength(hashValue, 32);
      // HP_HASHVAL = $0002
      if not CryptGetHashParam(hHash, $0002, hashValue, hashLen, 0) then Exit;
      Result := hashValue;
    finally
      CryptDestroyHash(hHash);
    end;
  finally
    CryptReleaseContext(hProv, 0);
  end;
end;

function ComputeHMACSHA256(key, data: AnsiString): AnsiString;
var
  ipad, opad, keyPad: AnsiString;
  i: Integer;
  inner: AnsiString;
begin
  // If key > 64 bytes, hash it first
  if Length(key) > 64 then
    key := ComputeSHA256(key);

  // Pad key to 64 bytes
  keyPad := key;
  while Length(keyPad) < 64 do
    keyPad := keyPad + #0;

  // Create inner and outer padded keys
  SetLength(ipad, 64);
  SetLength(opad, 64);
  for i := 1 to 64 do
  begin
    ipad[i] := Chr(Ord(keyPad[i]) xor $36);
    opad[i] := Chr(Ord(keyPad[i]) xor $5C);
  end;

  // HMAC = SHA256(opad + SHA256(ipad + data))
  inner := ComputeSHA256(ipad + data);
  Result := ComputeSHA256(opad + inner);
end;

function IsCharInCharset(c: Char): Boolean;
var
  i: Integer;
begin
  Result := False;
  for i := 1 to Length(VALID_CHARS) do
  begin
    if c = VALID_CHARS[i] then
    begin
      Result := True;
      Exit;
    end;
  end;
end;

function ComputeCheckGroup(input: String): String;
var
  hash: AnsiString;
  idx: Integer;
  i: Integer;
begin
  hash := ComputeHMACSHA256(HMAC_SECRET, AnsiString(input));
  Result := '';
  if Length(hash) >= 4 then
  begin
    for i := 1 to 4 do
    begin
      idx := (Ord(hash[i]) mod Length(VALID_CHARS)) + 1;
      Result := Result + VALID_CHARS[idx];
    end;
  end;
end;

function ValidateSerialKey(key: String): Boolean;
var
  parts: TArrayOfString;
  i, j: Integer;
  input, expectedCheck: String;
  upperKey: String;
begin
  Result := False;
  upperKey := Uppercase(Trim(key));

  // Split by dash - manual parsing since InnoSetup lacks full Split
  SetArrayLength(parts, 0);
  j := 0;
  SetArrayLength(parts, 5);

  // Parse NRUA-AAAA-BBBB-CCCC-DDDD
  if Length(upperKey) <> 24 then Exit;
  if Copy(upperKey, 5, 1) <> '-' then Exit;
  if Copy(upperKey, 10, 1) <> '-' then Exit;
  if Copy(upperKey, 15, 1) <> '-' then Exit;
  if Copy(upperKey, 20, 1) <> '-' then Exit;

  parts[0] := Copy(upperKey, 1, 4);
  parts[1] := Copy(upperKey, 6, 4);
  parts[2] := Copy(upperKey, 11, 4);
  parts[3] := Copy(upperKey, 16, 4);
  parts[4] := Copy(upperKey, 21, 4);

  // Check prefix
  if parts[0] <> 'NRUA' then Exit;

  // Check charset for groups 1-4
  for i := 1 to 4 do
  begin
    if Length(parts[i]) <> 4 then Exit;
    for j := 1 to 4 do
    begin
      if not IsCharInCharset(parts[i][j]) then Exit;
    end;
  end;

  // Verify checksum
  input := parts[1] + '-' + parts[2] + '-' + parts[3];
  expectedCheck := ComputeCheckGroup(input);

  Result := (expectedCheck = parts[4]);
end;

procedure SerialEditChange(Sender: TObject);
begin
  // Auto-format: insert dashes
  // Reset status on any change
  if SerialValid then
  begin
    SerialValid := False;
    SerialStatusLabel.Caption := '';
  end;
end;

procedure ValidateButtonClick(Sender: TObject);
var
  key: String;
begin
  key := Trim(SerialEdit.Text);
  if ValidateSerialKey(key) then
  begin
    SerialValid := True;
    SerialStatusLabel.Font.Color := clGreen;
    SerialStatusLabel.Caption := '✓ Clave válida';
  end
  else
  begin
    SerialValid := False;
    SerialStatusLabel.Font.Color := clRed;
    SerialStatusLabel.Caption := '✗ Clave no válida. Verifique e intente de nuevo.';
  end;
end;

procedure InitializeWizard();
var
  ValidateButton: TNewButton;
  lblTitle, lblFormat: TNewStaticText;
begin
  SerialValid := False;

  SerialPage := CreateCustomPage(wpLicense,
    'Clave de Licencia',
    'Introduzca la clave de licencia que recibió con su compra.');

  lblTitle := TNewStaticText.Create(SerialPage);
  lblTitle.Parent := SerialPage.Surface;
  lblTitle.Caption := 'Introduzca su clave de licencia:';
  lblTitle.Top := 20;
  lblTitle.Left := 0;
  lblTitle.Font.Style := [fsBold];

  lblFormat := TNewStaticText.Create(SerialPage);
  lblFormat.Parent := SerialPage.Surface;
  lblFormat.Caption := 'Formato: NRUA-XXXX-XXXX-XXXX-XXXX';
  lblFormat.Top := 45;
  lblFormat.Left := 0;
  lblFormat.Font.Color := clGray;

  SerialEdit := TNewEdit.Create(SerialPage);
  SerialEdit.Parent := SerialPage.Surface;
  SerialEdit.Top := 75;
  SerialEdit.Left := 0;
  SerialEdit.Width := 300;
  SerialEdit.Font.Name := 'Consolas';
  SerialEdit.Font.Size := 12;
  SerialEdit.CharCase := ecUpperCase;
  SerialEdit.OnChange := @SerialEditChange;

  ValidateButton := TNewButton.Create(SerialPage);
  ValidateButton.Parent := SerialPage.Surface;
  ValidateButton.Caption := 'Validar';
  ValidateButton.Top := 73;
  ValidateButton.Left := 310;
  ValidateButton.Width := 90;
  ValidateButton.Height := 27;
  ValidateButton.OnClick := @ValidateButtonClick;

  SerialStatusLabel := TNewStaticText.Create(SerialPage);
  SerialStatusLabel.Parent := SerialPage.Surface;
  SerialStatusLabel.Top := 115;
  SerialStatusLabel.Left := 0;
  SerialStatusLabel.Width := 400;
  SerialStatusLabel.Caption := '';
end;

function NextButtonClick(CurPageID: Integer): Boolean;
begin
  Result := True;
  if CurPageID = SerialPage.ID then
  begin
    if not SerialValid then
    begin
      // Auto-validate on Next click
      ValidateButtonClick(nil);
      if not SerialValid then
      begin
        MsgBox('Debe introducir una clave de licencia válida para continuar con la instalación.',
          mbError, MB_OK);
        Result := False;
      end;
    end;
  end;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  licDir, licPath, key: String;
begin
  if CurStep = ssPostInstall then
  begin
    // Write the validated license key to AppData
    licDir := ExpandConstant('{userappdata}\NRUAGuestManager');
    if not DirExists(licDir) then
      ForceDirectories(licDir);

    licPath := licDir + '\license.key';
    key := Uppercase(Trim(SerialEdit.Text));
    SaveStringToFile(licPath, key, False);
  end;
end;
