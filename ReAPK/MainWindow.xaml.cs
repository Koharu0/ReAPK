﻿using Microsoft.Win32;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Resources;
using Path = System.IO.Path;
using System.Globalization;

namespace ReAPK
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        
        private void _Loaded_(object sender, RoutedEventArgs e)
        {
            //CultureInfo.CurrentCulture = new CultureInfo("en");
            //ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            //string test = rm.GetString("btnDecompile", CultureInfo.CurrentCulture);
            //Trace.WriteLine(CultureInfo.CurrentCulture);
            //MessageBox.Show(test);

            CheckSettings(); // 설정 파일 검증
            CheckToolsExist(); // 도구가 기본값에 존재하는지 확인, 텍스트박스 설정
            //CheckLanguage(); // 요소 언어 설정
            //LoadSettings(); // 설정 파일 유효성 확인 (이상하면 복구) + 요소 언어 설정 <- CheckLanguage()에서 호출함. [호출 순서 참고용]
            
        }
        private void CheckSettings() // 설정 파일 검증
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            try
            {
                var test = Properties.Settings.Default.Language;
            }
            catch (ConfigurationErrorsException) // 설정 파일이 손상된 경우
            {
                try // 설정 파일 제거 시도
                {
                    Directory.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ReAPK"), true);
                    MessageBox.Show(rm.GetString("ErrorButRecovered", CultureInfo.CurrentCulture));
                    // 오류가 발생했으나 복구에 성공했습니다.\n변경 사항을 적용하기 위해 프로그램을 다시 실행해 주세요.
                    Environment.Exit(0); // System.Reflection.TargetInvocationException: 'Exception has been thrown by the target of an invocation.' 발생함
                }
                catch (Exception exception)
                {
                    MessageBox.Show(rm.GetString("ResetFailed", CultureInfo.CurrentCulture));
                    // 설정 파일 초기화 중 오류가 발생했습니다.\nWin+R -> %localappdata% 입력 후 ReAPK 폴더를 삭제해 주세요.\nReAPK 폴더가 없는 경우, 프로그램을 재설치해 주세요.
                    MessageBox.Show(rm.GetString("FollowError", CultureInfo.CurrentCulture) + exception.Message);
                    // 오류 메시지는 다음과 같습니다: 
                    Environment.Exit(0);
                }
            }
        }
        
        public void CheckToolsExist() // 시작시 도구가 기본값에 존재하는지 확인, 텍스트박스 설정
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());

            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string Apktool = Path.Combine(exeDirectory, "Apktool", "apktool.jar");
            string apksigner = Path.Combine(exeDirectory, "Resources", "Android", "Sdk", "build-tools", "35.0.0", "lib", "apksigner.jar");
            string Cert = Path.Combine(exeDirectory, "Resources", "testkey.x509.pem");
            string Key = Path.Combine(exeDirectory, "Resources", "testkey.pk8");

            bool IsdefaultApktool = false;
            bool Isdefaultapksigner = false;
            bool IsdefaultCert = false;
            bool IsdefaultKey = false;
            if (!File.Exists(Apktool)) // Apktool 기본값 아님
            {
                if (!File.Exists(Properties.Settings.Default.Apktool)) // 설정값도 아님
                {
                    appSettings.Apktool = null;
                    Properties.Settings.Default.Apktool = null;
                    MessageBox.Show(rm.GetString("ApktoolNotFound", CultureInfo.CurrentCulture));
                    // 오류: apktool.jar이 존재하지 않습니다.\n설정에서 경로를 변경해주세요.
                }
                else // 설정값임
                {
                    appSettings.Apktool = Properties.Settings.Default.Apktool;
                }
            }
            if (File.Exists(Apktool)) // Apktool 기본값임
            {
                IsdefaultApktool = true;
                appSettings.Apktool = Apktool;
                tboxApktool.Text = Apktool;
            }

            if (!File.Exists(apksigner)) // apksigner 기본값 아님
            {
                if (!File.Exists(Properties.Settings.Default.apksigner)) // 설정값도 아님
                {
                    appSettings.apksigner = null;
                    Properties.Settings.Default.apksigner = null;
                    MessageBox.Show(rm.GetString("apksignerNotFound", CultureInfo.CurrentCulture));
                    // 오류: apksigner.jar이 존재하지 않습니다.\n설정에서 경로를 변경해주세요.
                }
                else // 설정값임
                {
                    appSettings.apksigner = Properties.Settings.Default.apksigner;
                }
            }
            if (File.Exists(apksigner)) // apksigner 기본값임
            {
                Isdefaultapksigner=true;
                appSettings.apksigner=apksigner;
                tboxApksigner.Text = apksigner;
            }

            if (!File.Exists(Cert)) // Cert 기본값 아님
            {
                if (!File.Exists(Properties.Settings.Default.Cert)) // 설정값도 아님
                {
                    appSettings.Cert = null;
                    Properties.Settings.Default.Cert = null;
                    MessageBox.Show(rm.GetString("CertNotFound", CultureInfo.CurrentCulture));
                    // 오류: Cert가 존재하지 않습니다.\n설정에서 경로를 변경해주세요.
                }
                else // 설정값임
                {
                    appSettings.Cert = Properties.Settings.Default.Cert;
                }
            }
            if (File.Exists(Cert)) // Cert 기본값임
            {
                IsdefaultCert = true;
                appSettings.Cert = Cert;
                tboxCert.Text = Cert;
            }

            if (!File.Exists(Key)) // Key 기본값 아님
            {
                if (!File.Exists(Properties.Settings.Default.Key)) // 설정값도 아님
                {
                    appSettings.Key = null;
                    Properties.Settings.Default.Key = null;
                    MessageBox.Show(rm.GetString("KeyNotFound", CultureInfo.CurrentCulture));
                    // 오류: Key가 존재하지 않습니다.\n설정에서 경로를 변경해주세요.
                }
                else // 설정값임
                {
                    appSettings.Key = Properties.Settings.Default.Key;
                }
            }
            if (File.Exists(Key)) // Key 기본값임
            {
                IsdefaultKey = true;
                appSettings.Key = Key;
                tboxKey.Text = Key;
            }
            Trace.WriteLine(IsdefaultApktool);
            Trace.WriteLine(Isdefaultapksigner);
            Trace.WriteLine(IsdefaultCert);
            Trace.WriteLine(IsdefaultKey);
            CheckLanguage(IsdefaultApktool, Isdefaultapksigner, IsdefaultCert, IsdefaultKey);
        }
        private void CheckLanguage(bool IsdefaultApktool, bool Isdefaultapksigner, bool IsdefaultCert, bool IsdefaultKey)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            if (Properties.Settings.Default.Language == "en") // 설정 파일이 정상적임 + 최초 실행 X, 설정 파일 오류 발생 후 첫 실행일 가능성 있음
            {
                CultureInfo.CurrentCulture = new CultureInfo("en");
                SetUILanguage(Properties.Settings.Default.Language);
            }
            else if (Properties.Settings.Default.Language == "ko") // 설정 파일이 정상적임 + 최초 실행 X
            {
                CultureInfo.CurrentCulture = new CultureInfo("ko");
                SetUILanguage(Properties.Settings.Default.Language);
            }
            else if (Properties.Settings.Default.Language == "FirstRun") // 최초 실행임
            {
                appSettings.Language = "en";
                Properties.Settings.Default.Language = "en";

                // null 방지용 기본값 설정
                Properties.Settings.Default.AutoSign = true;
                LoadSettings(IsdefaultApktool, Isdefaultapksigner, IsdefaultCert, IsdefaultKey);
            }
            else
            {
                MessageBox.Show(rm.GetString("UnknownError_RestartNeeded", CultureInfo.CurrentCulture));
                // 알 수 없는 오류가 발생했습니다. 프로그램을 다시 실행해 주세요.
                Environment.Exit(0);
            }
        }
        private void LoadSettings(bool IsdefaultApktool, bool Isdefaultapksigner, bool IsdefaultCert, bool IsdefaultKey) // 설정 파일 검사, 복구, 요소 언어 설정 호출
        {
            chkAutoSign.IsChecked = Properties.Settings.Default.AutoSign; // UI 반영

            if (IsdefaultApktool == false) //기본값 X
            {
                tboxApktool.Text = Properties.Settings.Default.Apktool; // UI 반영
            }
            
            if (Isdefaultapksigner == false) //기본값 X
            {
                tboxApksigner.Text = Properties.Settings.Default.apksigner; // UI 반영
            }

            if (IsdefaultCert == false) //기본값 X
            {
                tboxCert.Text = Properties.Settings.Default.Cert; // UI 반영
            }
            
            if (IsdefaultKey == false) //기본값 X
            {
                tboxKey.Text = Properties.Settings.Default.Key; // UI 반영
            }
            
            appSettings.AutoSign = Properties.Settings.Default.AutoSign; // 앱 설정
            //appSettings.Apktool = Properties.Settings.Default.Apktool; // 앱 설정
            //appSettings.apksigner = Properties.Settings.Default.apksigner; // 앱 설정
            //appSettings.Cert = Properties.Settings.Default.Cert; // 앱 설정
            //appSettings.Key = Properties.Settings.Default.Key; // 앱 설정
            MessageBox.Show(appSettings.Apktool);
            MessageBox.Show(appSettings.apksigner);
            MessageBox.Show(appSettings.Cert);
            MessageBox.Show(appSettings.Key);
        }

        public class Settings 
        {
            public string Language { get; set; }
            public bool AutoSign { get; set; }
            public string Apktool { get; set; }
            public string apksigner {  get; set; }
            public string Cert { get; set; }
            public string Key { get; set; }
        }
        public void SetUILanguage(string Language) // 요소의 언어 설정
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            if (Language == "en")
            {
                appSettings.Language = "en";
                LanguageSelector.SelectedIndex = 0;

                CultureInfo.CurrentCulture = new CultureInfo("en");
                btnDecompile.Content = rm.GetString("btnDecompile", CultureInfo.CurrentCulture);
                btnCompile.Content = rm.GetString("btnCompile", CultureInfo.CurrentCulture);
                btnSign.Content = rm.GetString("btnSign", CultureInfo.CurrentCulture);
                chkAutoSign.Content = rm.GetString("chkAutoSign", CultureInfo.CurrentCulture);
                tabMain.Header = rm.GetString("tabMain", CultureInfo.CurrentCulture);
                tabSettings.Header = rm.GetString("tabSettings", CultureInfo.CurrentCulture);
                labWarning.Content = rm.GetString("labWarning", CultureInfo.CurrentCulture);
                labHowToUse.Content = rm.GetString("labHowToUse", CultureInfo.CurrentCulture);
                btnOpenDecompiledFolder.Content = rm.GetString("btnOpenDecompiledFolder", CultureInfo.CurrentCulture);
                btnOpenCompiledFolder.Content = rm.GetString("btnOpenCompiledFolder", CultureInfo.CurrentCulture);
                btnOpenSignedFolder.Content = rm.GetString("btnOpenSignedFolder", CultureInfo.CurrentCulture);
            }
            else if (Language == "ko")
            {
                appSettings.Language = "ko";
                LanguageSelector.SelectedIndex = 1;

                CultureInfo.CurrentCulture = new CultureInfo("ko");
                btnDecompile.Content = rm.GetString("btnDecompile", CultureInfo.CurrentCulture);
                btnCompile.Content = rm.GetString("btnCompile", CultureInfo.CurrentCulture);
                btnSign.Content = rm.GetString("btnSign", CultureInfo.CurrentCulture);
                chkAutoSign.Content = rm.GetString("chkAutoSign", CultureInfo.CurrentCulture);
                tabMain.Header = rm.GetString("tabMain", CultureInfo.CurrentCulture);
                tabSettings.Header = rm.GetString("tabSettings", CultureInfo.CurrentCulture);
                labWarning.Content = rm.GetString("labWarning", CultureInfo.CurrentCulture);
                labHowToUse.Content = rm.GetString("labHowToUse", CultureInfo.CurrentCulture);
                btnOpenDecompiledFolder.Content = rm.GetString("btnOpenDecompiledFolder", CultureInfo.CurrentCulture);
                btnOpenCompiledFolder.Content = rm.GetString("btnOpenCompiledFolder", CultureInfo.CurrentCulture);
                btnOpenSignedFolder.Content = rm.GetString("btnOpenSignedFolder", CultureInfo.CurrentCulture);
            }
            else
            {
                MessageBox.Show(rm.GetString("UnknownError", CultureInfo.CurrentCulture));
                // 알 수 없는 오류가 발생했습니다. 프로그램이 정상적으로 작동하지 않을 수 있습니다.
            }
        }
        

        private Settings appSettings = new Settings();
        private void UpdateSettings()
        {
            Properties.Settings.Default.Language = appSettings.Language;
            Properties.Settings.Default.AutoSign = appSettings.AutoSign;
            Properties.Settings.Default.Apktool = appSettings.Apktool;
            Properties.Settings.Default.apksigner = appSettings.apksigner;
            Properties.Settings.Default.Cert = appSettings.Cert;
            Properties.Settings.Default.Key = appSettings.Key;
            Properties.Settings.Default.Save();
        }

        private void btnDeCompile_DragEnter(object sender, DragEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string extension = Path.GetExtension(files[0]).ToLower();
                if (extension == ".apk")
                {

                }
                else
                {
                    MessageBox.Show(rm.GetString("OnlyCanDropAPK", CultureInfo.CurrentCulture));
                    // 확장명이 .apk인 파일만 끌어 놓을 수 있습니다.
                }
            }
        }

        private void btnDecompile_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string droppedFile = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];

                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string extension = Path.GetExtension(files[0]).ToLower();
                string fileName = Path.GetFileNameWithoutExtension(droppedFile);

                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string folderName = "!Decompiled APKs";
                string folderPath = Path.Combine(exeDirectory, folderName);
                Directory.CreateDirectory(folderPath);
                if (extension == ".apk")
                {
                    Decompile(droppedFile, fileName);
                }
            }
        }

        public async Task<(string output, string error, int ExitCode)> Run(string fileName, string command)
        {
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = command;
            process.StartInfo.RedirectStandardOutput = true; // 표준 출력 캡처
            process.StartInfo.RedirectStandardError = true; // 표준 오류 캡처
            process.StartInfo.UseShellExecute = false; // 셸 사용 비활성화
            process.StartInfo.CreateNoWindow = true; // 별도 창 표시 안 함

            process.Start();
            string output = await process.StandardOutput.ReadToEndAsync(); // 표준 출력 읽기
            string error = await process.StandardError.ReadToEndAsync(); // 표준 오류 읽기
            await process.WaitForExitAsync();
            int ExitCode = process.ExitCode;
            return (output, error, ExitCode);
        }
        private async void Decompile(string apk, string fileName)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string Apktool = appSettings.Apktool;
            if (Apktool == null)
            {
                MessageBox.Show(rm.GetString("ApktoolNotFound_Retry", CultureInfo.CurrentCulture));
                // apktool.jar 경로가 잘못되었습니다.\n설정에서 수정한 뒤 다시 시도해 주세요.
                return;
            }
            string folderPath = Path.Combine(exeDirectory, "!Decompiled APKs", fileName);
            string command = $"-jar \"{Apktool}\" d -o \"{folderPath}\" \"{apk}\"";
            try
            {
                string DecompilingFormat = rm.GetString("Decompiling", CultureInfo.CurrentCulture);
                // {0}.apk를 디컴파일 중...
                labState.Content = string.Format(DecompilingFormat, fileName);
                var result = await Run("java", command);
                if (result.ExitCode == 0)
                {
                    string DecompileCompileteFormat = rm.GetString("DecompileComplete", CultureInfo.CurrentCulture);
                    // {0}.apk의 디컴파일이 완료되었습니다.
                    labState.Content = string.Format(DecompileCompileteFormat, fileName);
                }
                else
                {
                    if (result.error.Contains("already exists."))
                    {
                        MessageBoxResult msgboxresult = MessageBox.Show($"{rm.GetString("Question_Overwrite", CultureInfo.CurrentCulture)}", $"{rm.GetString("Overwrite", CultureInfo.CurrentCulture)}", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        // 이미 디컴파일한 폴더가 존재합니다. 덮어쓸까요? / 폴더 덮어쓰기

                        if (msgboxresult == MessageBoxResult.Yes)
                        {
                            command = $"-jar \"{Apktool}\" d -f -o \"{folderPath}\" \"{apk}\"";
                            try
                            {
                                result = await Run("java", command);
                                if (result.ExitCode == 0)
                                {
                                    string DecompilingFormat2 = rm.GetString("Decompiling", CultureInfo.CurrentCulture);
                                    // {0}.apk를 디컴파일 중...
                                    labState.Content = string.Format(DecompilingFormat2, fileName);
                                }
                                else
                                {
                                    MessageBox.Show($"{rm.GetString("WhileDecompileError", CultureInfo.CurrentCulture)}\n{result.error}");
                                    // 디컴파일 중 문제가 발생했습니다:
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"{rm.GetString("Exception", CultureInfo.CurrentCulture)} {ex.Message}");
                                // 예외 발생:
                            }
                        }
                        else if (msgboxresult == MessageBoxResult.No)
                        {
                            string DecompileCanceled = rm.GetString("DecompileCanceled", CultureInfo.CurrentCulture);
                            labState.Content = string.Format(DecompileCanceled, fileName);
                            // {0}.apk의 디컴파일이 취소되었습니다.
                        }
                    }
                    else
                    {
                        MessageBox.Show($"{rm.GetString("WhileDecompileError", CultureInfo.CurrentCulture)}\n{result.error}");
                        // 디컴파일 중 문제가 발생했습니다:
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{rm.GetString("Exception", CultureInfo.CurrentCulture)} {ex.Message}");
                // 예외 발생:
            }

        }
        
        private void btnCompile_DragEnter(object sender, DragEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // 드롭한 항목이 폴더인지 확인
                if (Directory.Exists(files[0]))
                {

                }
                else
                {
                    MessageBox.Show(rm.GetString("CanOnlyFolder", CultureInfo.CurrentCulture));
                    //폴더만 끌어 놓을 수 있습니다.
                }
            }
        }
        
        private void btnCompile_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                // 드롭된 데이터 가져오기
                string[] droppedItems = (string[])e.Data.GetData(DataFormats.FileDrop);
                string droppedItem = droppedItems[0]; // 첫 번째 드롭된 항목만 처리

                // 폴더인지 확인
                if (Directory.Exists(droppedItem))
                {
                    // 폴더 드롭 처리가 확인되면 컴파일 작업 시작
                    string folderName = Path.GetFileNameWithoutExtension(droppedItem);

                    // 컴파일 실행
                    Compile(droppedItem, folderName);
                }
            }
        }

        private async void Compile(string folderPath, string folderName)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string outputApkPath = Path.Combine(exeDirectory, "!Compiled APKs", $"{folderName}_compiled.apk");
            if (appSettings.Apktool == null)
            {
                MessageBox.Show(rm.GetString("apksignerNotFound_Retry", CultureInfo.CurrentCulture));
                // apktool.jar 경로가 잘못되었습니다.\n설정에서 수정한 뒤 다시 시도해 주세요.
                return;
            }
            string command = $"-jar \"{appSettings.Apktool}\" b \"{folderPath}\" -o \"{outputApkPath}\"";
            try
            {
                string CompilingFormat = rm.GetString("Compiling", CultureInfo.CurrentCulture);
                // {0} 컴파일 중...
                labState.Content = string.Format(CompilingFormat, folderName);
                var result = await Run("java", command);

                if (result.ExitCode == 0)
                {
                    string CompileComplete = rm.GetString("CompileComplete", CultureInfo.CurrentCulture);
                    // {0}의 컴파일이 완료되었습니다.
                    labState.Content = string.Format(CompileComplete, folderName);
                    if (chkAutoSign.IsChecked == true)
                    {
                        bool fromCompile = true;
                        Sign(outputApkPath, folderName, fromCompile);
                    }
                }
                else
                {
                    MessageBox.Show($"{rm.GetString("WhileCompileError", CultureInfo.CurrentCulture)}\n{result.error}");
                    // 컴파일 중 문제가 발생했습니다:
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{rm.GetString("Exception", CultureInfo.CurrentCulture)} {ex.Message}");
                // 예외 발생:
            }
        }

        private void btnSign_DragEnter(object sender, DragEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string extension = Path.GetExtension(files[0]).ToLower();
                if (extension == ".apk")
                {

                }
                else
                {
                    MessageBox.Show(rm.GetString("OnlyCanDropAPK", CultureInfo.CurrentCulture));
                    // 확장명이 .apk인 파일만 끌어 놓을 수 있습니다.
                }
            }
        }

        private void btnSign_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string droppedFile = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
                string fileName = Path.GetFileNameWithoutExtension(droppedFile);
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string extension = Path.GetExtension(files[0]).ToLower();

                string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string folderName = "!Signed APKs";
                string folderPath = Path.Combine(exeDirectory, folderName);
                Directory.CreateDirectory(folderPath);
                if (extension == ".apk")
                {
                    Sign(droppedFile, fileName, false);
                }
            }
        }

        public async void Sign(string apk, string fileName, bool fromCompile)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string signedPath;
            if (fromCompile == true)
            {
                signedPath = Path.Combine(exeDirectory, "!Compiled APKs", fileName + "_compiled.apk"); // 결과물 APK
            }
            else
            {
                signedPath = Path.Combine(exeDirectory, "!Signed APKs", fileName + ".apk"); // 결과물 APK
            }
            string apksigner = appSettings.apksigner;
            string keyPath = appSettings.Key;
            string certPath = appSettings.Cert;

            string command = $"-jar \"{apksigner}\" sign --key \"{keyPath}\" --cert \"{certPath}\" --v4-signing-enabled false --out \"{signedPath}\" \"{apk}\"";
            Trace.WriteLine(certPath);
            try
            {
                string SigningFormat = rm.GetString("Signing", CultureInfo.CurrentCulture);
                // {0}.apk 서명 중...
                labState.Content = string.Format(SigningFormat, fileName);
                var result = await Run("java", command);

                if (result.ExitCode == 0)
                {
                    string SignCompleteFormat = rm.GetString("SignComplete", CultureInfo.CurrentCulture);
                    // {0}.apk 서명이 완료되었습니다.
                    labState.Content = string.Format(SignCompleteFormat, fileName);
                }
                else
                {
                    MessageBox.Show(rm.GetString("WhileSignError", CultureInfo.CurrentCulture) + result.error);
                    // 서명 중 오류 발생: 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"{rm.GetString("Exception", CultureInfo.CurrentCulture)} {ex.Message}");
                // 예외 발생:
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            if (LanguageSelector.SelectedIndex == 0) // English
            {
                Trace.WriteLine("영어");
                SetUILanguage("en");
            }
            else if (LanguageSelector.SelectedIndex == 1) // 한국어
            {
                Trace.WriteLine("한국어");
                SetUILanguage("ko");
            }
            if (chkAutoSign.IsChecked == true) // 체크박스 설정 안 건들고 설정 버튼 누를 경우를 대비
            {
                appSettings.AutoSign = true;
            }
            else if (chkAutoSign.IsChecked == false)
            {
                appSettings.AutoSign = false;
            }
            else
            {
                MessageBox.Show(rm.GetString("UnknownError_CheckBox", CultureInfo.CurrentCulture));
                // 알 수 없는 오류가 발생했습니다.\n이 버튼 아래 보이는 체크박스를 클릭해보고 다시 이 버튼을 눌러보면 해결될 것 같네요!
            }
            appSettings.Apktool = tboxApktool.Text;
            appSettings.Cert = tboxCert.Text;
            appSettings.Key = tboxKey.Text;
            UpdateSettings();

        }
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        // @@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@
        private void btnApktool_Click(object sender, RoutedEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Apktool|apktool.jar",
                Title = $"{rm.GetString("SelectApktool", CultureInfo.CurrentCulture)}"
                //Apktool 파일을 선택해 주세요. (apktool.jar만 허용됩니다.)
            };

            if (openFileDialog.ShowDialog() == true)
            {
                tboxApktool.Text = openFileDialog.FileName;
            }
        }
        private void btnApksigner_Click(object sender, RoutedEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "apksigner|apksigner.jar",
                Title = $"{rm.GetString("Selectapksigner", CultureInfo.CurrentCulture)}"
                // apksigner.jar를 선택해 주세요.
            };

            if (openFileDialog.ShowDialog() == true)
            {
                tboxApksigner.Text = openFileDialog.FileName;
            }
        }

        private void btnCert_Click(object sender, RoutedEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Certificate|*.x509.pem",
                Title = $"{rm.GetString("SelectCert", CultureInfo.CurrentCulture)}"
                // APK 서명에 사용할 인증서(.x509.pem)를 선택하세요.
            };

            if (openFileDialog.ShowDialog() == true)
            {
                tboxCert.Text = openFileDialog.FileName;
            }
        }

        private void btnKey_Click(object sender, RoutedEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Private Key|*.pk8",
                Title = $"{rm.GetString("SelectKey", CultureInfo.CurrentCulture)}"
                // APK 서명에 사용할 비공개 키 파일(.pk8)을 선택하세요.
            };

            if (openFileDialog.ShowDialog() == true)
            {
                tboxKey.Text = openFileDialog.FileName;
            }
        }

        private void chkAutoSign_Checked(object sender, RoutedEventArgs e)
        {
            appSettings.AutoSign = true;
        }

        private void chkAutoSign_Unchecked(object sender, RoutedEventArgs e)
        {
            appSettings.AutoSign = false;
        }

        private void btnDecompile_Click(object sender, RoutedEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            (string fullapk, string fileName) = OpenFileDialog(rm.GetString("SelectDecompile", CultureInfo.CurrentCulture), "APK|*.apk");
            // 디컴파일할 APK를 선택하세요.
            if (fullapk != null)
            {
                Decompile(fullapk, fileName);
            }
        }

        private void btnCompile_Click(object sender, RoutedEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            (string fullfoldername, string foldername) = OpenFolderDialog(rm.GetString("SelectCompile", CultureInfo.CurrentCulture));
            // 컴파일할 APK를 선택하세요.
            if (fullfoldername != null)
            {
                Compile(fullfoldername, foldername);
            }
        }

        public (string, string) OpenFileDialog(string Title, string Filter)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = Title,
                Filter = Filter
            };
            if (openFileDialog.ShowDialog() == true)
            {
                string fileName = Path.GetFileNameWithoutExtension(openFileDialog.FileName);
                return (openFileDialog.FileName, fileName);
            }
            return (null, null);
        }

        public (string, string) OpenFolderDialog(string Title)
        {
            OpenFolderDialog openFolderDialog = new OpenFolderDialog
            {
                Title = Title
            };
            if (openFolderDialog.ShowDialog() == true)
            {
                string fullfoldername = openFolderDialog.FolderName;
                string foldername = Path.GetFileNameWithoutExtension(fullfoldername);
                return (fullfoldername, foldername);
            }
            return (null, null);
        }
        private void btnSign_Click(object sender, RoutedEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string folderName = "!Signed APKs";
            string folderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(folderPath);
            (string fullapk, string fileName) = OpenFileDialog(rm.GetString("SelectSign", CultureInfo.CurrentCulture), "APK|*.apk");
            //서명할 APK를 선택하세요.
            if (fullapk != null)
            {
                Sign(fullapk, fileName, false);
            }
        }

        private void btnOpenDecompiledFolder_Click(object sender, RoutedEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "!Decompiled APKs");
            if (Directory.Exists(folderPath))
            {
                Process.Start("explorer", folderPath);
            }
            else
            {
                MessageBox.Show(rm.GetString("NoDecompiled", CultureInfo.CurrentCulture));
                //디컴파일된 APK가 없습니다.
            }
        }

        private void btnOpenCompiledFolder_Click(object sender, RoutedEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "!Compiled APKs");
            if (Directory.Exists(folderPath))
            {
                Process.Start("explorer", folderPath);
            }
            else
            {
                MessageBox.Show(rm.GetString("NoCompiled", CultureInfo.CurrentCulture));
                //컴파일된 APK가 없습니다.
            }
        }

        private void btnOpenSignedFolder_Click(object sender, RoutedEventArgs e)
        {
            ResourceManager rm = new ResourceManager("ReAPK.Resources", Assembly.GetExecutingAssembly());
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "!Signed APKs");
            if (Directory.Exists(folderPath))
            {
                Process.Start("explorer", folderPath);
            }
            else
            {
                MessageBox.Show(rm.GetString("NoSigned", CultureInfo.CurrentCulture));
                //서명된 APK가 없습니다.
            }
        }
    }
}