@echo off
setlocal

rem 7-Zip
set "SEVEN_ZIP_INSTALL_PATH=%ProgramFiles%\7-Zip"
if exist "%SEVEN_ZIP_INSTALL_PATH%\7z.exe" (
    echo 7-Zip is already installed at %SEVEN_ZIP_INSTALL_PATH%.
) else (
    echo 7-Zip is not installed. Proceeding with installation...
    set SEVEN_ZIP_URL=https://www.7-zip.org/a/7z2409-x64.exe
    set SEVEN_ZIP_INSTALLER=%~dp07z2409-x64.exe

    echo Downloading 7-Zip Installer...
    powershell -Command ^
        "$wc = New-Object net.webclient; $wc.DownloadFile('%SEVEN_ZIP_URL%', '%SEVEN_ZIP_INSTALLER%')"

    if not exist "%SEVEN_ZIP_INSTALLER%" (
        echo Download failed: 7-Zip installer not found!
        pause
        exit /b 1
    )

    echo Installing 7-Zip...
    "%SEVEN_ZIP_INSTALLER%" /S

    if not exist "%SEVEN_ZIP_INSTALL_PATH%\7z.exe" (
        echo 7-Zip installation failed!
        pause
        exit /b 1
    )

    echo 7-Zip has been installed successfully at %SEVEN_ZIP_INSTALL_PATH%.
)

rem Apktool
set APKTOOL_URL=https://bitbucket.org/iBotPeaches/apktool/downloads/apktool_2.10.0.jar
set APKTOOL_DIR=%~dp0Apktool
set APKTOOL_FILE=%APKTOOL_DIR%\apktool.jar

echo Downloading Apktool...
if not exist "%APKTOOL_DIR%" mkdir "%APKTOOL_DIR%"
powershell -Command ^
    "$wc = New-Object net.webclient; $wc.DownloadFile('%APKTOOL_URL%', '%APKTOOL_FILE%')"

if exist "%APKTOOL_FILE%" (
    echo Successfully downloaded Apktool to: %APKTOOL_FILE%
) else (
    echo Download failed: Apktool file not found!
    pause
    exit /b 1
)

rem Java
set "JAVA_INSTALL_PATH=C:\Program Files\Java\jdk-21"
if exist "%JAVA_INSTALL_PATH%" (
    echo Java is already installed at %JAVA_INSTALL_PATH%.
) else (
    echo Java is not installed. Proceeding with installation...
    set JDK_DOWNLOAD_URL=https://download.oracle.com/java/21/archive/jdk-21.0.4_windows-x64_bin.exe
    set JDK_INSTALLER=%~dp0jdk-21.0.4_windows-x64_bin.exe

    echo Downloading Java SE Development Kit 21.0.4 Windows x64 Installer...
    powershell -Command ^
        "$wc = New-Object net.webclient; $wc.DownloadFile('%JDK_DOWNLOAD_URL%', '%JDK_INSTALLER%')"

    if not exist "%JDK_INSTALLER%" (
        echo Download failed: Java SE Development Kit installer file not found!
        pause
        exit /b 1
    )

    echo Java SE Development Kit 21 installer downloaded successfully to: %JDK_INSTALLER%
    echo [!] DO NOT press any key before completing the installation of the downloaded file 'jdk-21.0.4_windows-x64_bin.exe'.
    echo [!] After the JDK installation is complete, press any key to continue dependencies installation.
    pause
)


rem Android SDK
set SDK_DOWNLOAD_URL=https://dl.google.com/android/repository/commandlinetools-win-11076708_latest.zip
set SDK_INSTALLER=%~dp0commandlinetools.zip
set SDK_INSTALL_DIR=%~dp0Resources\Android\Sdk

echo Downloading Android SDK Command-Line Tools...
powershell -Command ^
    "$wc = New-Object net.webclient; $wc.DownloadFile('%SDK_DOWNLOAD_URL%', '%SDK_INSTALLER%')"

echo Extracting Android SDK Command-Line Tools...
if not exist "%SDK_INSTALL_DIR%" mkdir "%SDK_INSTALL_DIR%"
"%SEVEN_ZIP_INSTALL_PATH%\7z.exe" x "%SDK_INSTALLER%" -o"%SDK_INSTALL_DIR%" -y

echo Installing Build Tools...
"%SDK_INSTALL_DIR%\cmdline-tools\bin\sdkmanager.bat" --sdk_root=%SDK_INSTALL_DIR% "build-tools;35.0.0"

if exist "%SDK_INSTALL_DIR%\build-tools\35.0.0" (
    echo Android SDK Command-Line Tools and Android SDK Build Tools installation completed successfully!
) else (
    echo Android SDK Build Tools installation failed. Please check the logs.
    pause
    exit /b 1
)

pause

endlocal