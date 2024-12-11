using Microsoft.Win32;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Path = System.IO.Path;

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
            LoadSettings(); //설정 파일 유효성 확인 (이상하면 복구) + 요소 언어 설정
            CreateFolder(); //디컴파일, 컴파일, 사인 폴더 생성
            CheckToolsExist(); //도구가 기본값에 존재하는지 확인, 텍스트박스 설정
        }
        public void CreateFolder() //시작시 디컴파일, 컴파일, 사인 폴더 생성
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string decompileFolderPath = Path.Combine(exeDirectory, "!Decompiled APKs");
            string compileFolderPath = Path.Combine(exeDirectory, "!Compiled APKs");
            string signFolderPath = Path.Combine(exeDirectory, "!Signed APKs");

            // 존재하지 않으면 폴더 생성
            if (!Directory.Exists(decompileFolderPath)) // 디컴파일 폴더
            {
                Directory.CreateDirectory(decompileFolderPath);
            }
            if (!Directory.Exists(compileFolderPath)) //컴파일 폴더
            {
                Directory.CreateDirectory(compileFolderPath);
            }
            if (!Directory.Exists(signFolderPath)) //서명 폴더
            {
                Directory.CreateDirectory(signFolderPath);
            }
        }
        public void CheckToolsExist() //시작시 도구가 기본값에 존재하는지 확인, 텍스트박스 설정
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string Apktool = Path.Combine(exeDirectory, "Apktool", "apktool.jar");
            string Cert = Path.Combine(exeDirectory, "Resources", "testkey.x509.pem");
            string Key = Path.Combine(exeDirectory, "Resources", "testkey.pk8");
            if (!File.Exists(Apktool))
            {
                MessageBox.Show("Error: apktool.jar이 존재하지 않습니다.\n설정에서 경로를 변경해주세요.");
            }
            if (File.Exists(Apktool))
            {
                tboxApktool.Text = Apktool;
            }
            if (!File.Exists(Cert))
            {
                MessageBox.Show("Error: Cert가 존재하지 않습니다.\n설정에서 경로를 변경해주세요.");
            }
            if (File.Exists(Cert))
            {
                tboxCert.Text = Cert;
            }
            if (!File.Exists(Key))
            {
                MessageBox.Show("Error: Key가 존재하지 않습니다.\n설정에서 경로를 변경해주세요.");
            }
            if (File.Exists(Key))
            {
                tboxKey.Text = Key;
            }
        }
        public class Settings
        {
            public string Language { get; set; }
            public bool AutoSign { get; set; }
            public string Apktool { get; set; }
            public string Cert { get; set; }
            public string Key { get; set; }
        }
        public void SetUILanguage(string Language) // 요소의 언어 설정
        {
            if (Language == "EN")
            {
                appSettings.Language = "EN";
                LanguageSelector.SelectedIndex = 0;
                btnDecompile.Content = "Decompile APK";
                btnCompile.Content = "Compile APK";
                btnSign.Content = "Sign APK";
                chkAutoSign.Content = "Automatically Sign APK After Compile";
                tabMain.Header = "Main";
                tabSettings.Header = "Settings";
            }
            if (Language == "KO")
            {
                appSettings.Language = "KO";
                LanguageSelector.SelectedIndex = 1;
                btnDecompile.Content = "APK 디컴파일";
                btnCompile.Content = "APK 컴파일";
                btnSign.Content = "APK 서명";
                chkAutoSign.Content = "APK 컴파일 후 자동으로 서명";
                tabMain.Header = "메인";
                tabSettings.Header = "설정";
            }
            else
            {
                MessageBox.Show("알 수 없는 오류가 발생했습니다. 프로그램이 정상적으로 작동하지 않을 수 있습니다.");
            }
        }
        private void LoadSettings() //설정 파일 검사, 복구, 요소 언어 설정 호출
        {
            try
            {
                var test = Properties.Settings.Default.Language;
            }
            catch (ConfigurationErrorsException ex) //설정 파일이 손상된 경우
            {
                try //설정 파일 제거 시도
                {
                    Directory.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ReAPK"), true);
                    MessageBox.Show("오류가 발생했으나 복구에 성공했습니다.\n변경 사항을 적용하기 위해 프로그램을 다시 실행해 주세요.");
                    Environment.Exit(0);
                }
                catch (Exception exception)
                {
                    MessageBox.Show("설정 파일 초기화 중 오류가 발생했습니다.\nWin+R -> %localappdata% 입력 후 ReAPK 폴더를 삭제해 주세요.\nReAPK 폴더가 없는 경우, 프로그램을 재설치해 주세요.");
                    MessageBox.Show("오류 메시지는 다음과 같습니다: " + exception.Message);
                    Environment.Exit(0);
                }                
            }
            if (Properties.Settings.Default.Language == "EN") //설정 파일이 정상적임 + 최초 실행 X, 설정 파일 오류 발생 후 첫 실행일 가능성 있음
            {
                SetUILanguage(Properties.Settings.Default.Language);
            }
            else if (Properties.Settings.Default.Language == "KO") //설정 파일이 정상적임 + 최초 실행 X
            {
                SetUILanguage(Properties.Settings.Default.Language);
            }
            else if (Properties.Settings.Default.Language == "FirstRun") //최초 실행임
            {
                appSettings.Language = "EN";
                Properties.Settings.Default.Language = "EN";
                LoadSettings();
            }
            else
            {
                MessageBox.Show("알 수 없는 오류가 발생했습니다. 프로그램을 다시 실행해 주세요.");
                Environment.Exit(0);
            }
        }

        private Settings appSettings = new Settings();
        private void UpdateSettings()
        {
            Properties.Settings.Default.Language = appSettings.Language;
            Properties.Settings.Default.AutoSign = appSettings.AutoSign;
            Properties.Settings.Default.Apktool = appSettings.Apktool;
            Properties.Settings.Default.Cert = appSettings.Cert;
            Properties.Settings.Default.Key = appSettings.Key;
            Properties.Settings.Default.Save();
            //string Language = Properties.Settings.Default.Language;
            //MessageBox.Show("언어가 다음으로 설정됨: " + Language);
        }

        private void btnDeCompile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string extension = Path.GetExtension(files[0]).ToLower();
                if (extension == ".apk")
                {

                }
                else
                {
                    MessageBox.Show("확장명이 .apk인 파일만 Drop이 가능합니다.");
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

        private void Decompile(string apk, string fileName)
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string Apktool = Path.Combine(exeDirectory, "Apktool", "apktool.jar");
            string folderName = "!Decompiled APKs";
            string folderPath = Path.Combine(exeDirectory, folderName, fileName);
            string command = $"-jar \"{Apktool}\" d -o \"{folderPath}\" \"{apk}\"";

            Process process = new Process();
            process.StartInfo.FileName = "java";
            process.StartInfo.Arguments = command;
            process.StartInfo.RedirectStandardOutput = true; // 명령어 출력을 캡처
            process.StartInfo.RedirectStandardError = true; // 에러 캡처
            process.StartInfo.UseShellExecute = false; // 셸 사용 비활성화
            process.StartInfo.CreateNoWindow = true; // 별도 창 표시 없음

            try
            {
                // Process 실행
                labState.Content = $"{fileName}.apk를 디컴파일 중...";
                process.Start();
                string error = process.StandardError.ReadToEnd();   // 실행 중 발생한 에러 출력
                process.WaitForExit(); // 명령어가 끝날 때까지 대기

                if (process.ExitCode == 0)
                {
                    MessageBox.Show("디컴파일 완료!");
                    labState.Content = $"{fileName}.apk의 디컴파일이 완료되었습니다.";
                }
                else
                {
                    if (error.Contains("already exists."))
                    {
                        MessageBoxResult result = MessageBox.Show(
            "이미 디컴파일한 폴더가 존재합니다. 덮어쓸까요?",
            "폴더 덮어쓰기",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning
        );
                        if (result == MessageBoxResult.Yes)
                        {
                            command = $"-jar \"{Apktool}\" d -f -o \"{folderPath}\" \"{apk}\"";
                            process.StartInfo.Arguments = command;
                            try
                            {
                                process.Start();
                                error = process.StandardError.ReadToEnd();   // 실행 중 발생한 에러 출력
                                process.WaitForExit(); // 명령어가 끝날 때까지 대기
                                if (process.ExitCode == 0)
                                {
                                    labState.Content = $"{fileName}.apk의 디컴파일이 완료되었습니다.";
                                }
                                else
                                {
                                    MessageBox.Show($"디컴파일 중 문제가 발생했습니다:\n{error}");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"예외 발생: {ex.Message}");
                            }
                        }
                        else if (result == MessageBoxResult.No)
                        {
                            labState.Content = $"{fileName}.apk의 디컴파일이 취소되었습니다.";
                        }
                    }
                    else
                    {
                        MessageBox.Show($"디컴파일 중 문제가 발생했습니다:\n{error}");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예외 발생: {ex.Message}");
            }

        }
        
        private void btnCompile_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                // 드롭한 항목이 폴더인지 확인
                if (Directory.Exists(files[0]))
                {

                }
                else
                {
                    MessageBox.Show("폴더만 Drop이 가능합니다.");
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
                    string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string folderName = Path.GetFileNameWithoutExtension(droppedItem);
                    string Apktool = Path.Combine(exeDirectory, "Apktool", "apktool.jar");
                    string outputApkPath = Path.Combine(exeDirectory, "!Compiled APKs", $"{folderName}_compiled.apk");

                    // 컴파일 실행
                    Compile(droppedItem, outputApkPath, Apktool, folderName);
                }
            }
        }

        private void Compile(string folderPath, string outputApkPath, string Apktool, string folderName)
        {
            string command = $"-jar \"{Apktool}\" b \"{folderPath}\" -o \"{outputApkPath}\"";
            
            Process process = new Process();
            process.StartInfo.FileName = "java";
            process.StartInfo.Arguments = command;
            process.StartInfo.RedirectStandardOutput = true; // 명령어 출력을 캡처
            process.StartInfo.RedirectStandardError = true; // 에러 캡처
            process.StartInfo.UseShellExecute = false; // 셸 사용 비활성화
            process.StartInfo.CreateNoWindow = true; // 별도 창 표시 없음
            
            
            try
            {
                labState.Content = $"{folderName} 컴파일 중...";
                process.Start();
                string error = process.StandardError.ReadToEnd();   // 실행 중 발생한 에러 출력
                process.WaitForExit(); // 명령어가 끝날 때까지 대기

                if (process.ExitCode == 0)
                {
                    MessageBox.Show("컴파일 완료!");
                    labState.Content = $"{folderName}의 컴파일이 완료되었습니다.";
                    if (chkAutoSign.IsChecked == true)
                    {
                        bool fromCompile = true;
                        Sign(outputApkPath, folderName, fromCompile);
                    }
                }
                else
                {
                    MessageBox.Show($"컴파일 중 문제가 발생했습니다:\n{error}");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예외 발생: {ex.Message}");
            }
        }

        private void btnSign_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                string extension = Path.GetExtension(files[0]).ToLower();
                if (extension == ".apk")
                {

                }
                else
                {
                    MessageBox.Show("확장명이 .apk인 파일만 Drop이 가능합니다.");
                }
            }
        }

        private void btnSign_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                bool fromCompile = false;
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
                    MessageBox.Show(droppedFile);
                    Sign(droppedFile, fileName, fromCompile);
                }
            }
        }

        private void Sign(string apk, string fileName, bool fromCompile)
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string signedPath;
            if (fromCompile == true)
            {
                signedPath = Path.Combine(exeDirectory, "!Compiled APKs", fileName + "_compiled.apk"); //결과물 APK
            }
            else
            {
                signedPath = Path.Combine(exeDirectory, "!Signed APKs", fileName + ".apk"); //결과물 APK
            }
            string apksigner = Path.Combine(exeDirectory, "Resources", "Android", "Sdk", "build-tools", "35.0.0", "apksigner.bat");
            //string signedPath = Path.Combine(exeDirectory, "!Signed APKs", fileName + ".apk"); //결과물 APK
            string keyPath = Path.Combine(exeDirectory, "Resources", "testkey.pk8");
            string certPath = Path.Combine(exeDirectory, "Resources", "testkey.x509.pem");

            string command = $"sign --key \"{keyPath}\" --cert \"{certPath}\" --v4-signing-enabled false --out \"{signedPath}\" \"{apk}\"";

            Process process = new Process();
            process.StartInfo.FileName = apksigner; // .bat 경로를 지정
            process.StartInfo.Arguments = command; // 명령어 인수 전달
            process.StartInfo.UseShellExecute = false; // CLI 환경에서 실행
            process.StartInfo.RedirectStandardOutput = true; // 표준 출력 캡처
            process.StartInfo.RedirectStandardError = true; // 표준 에러 캡처
            process.StartInfo.CreateNoWindow = true; // 창 표시 안 함

            try
            {
                // 프로세스 실행
                process.Start();
                string output = process.StandardOutput.ReadToEnd(); // 실행 결과 읽기
                string error = process.StandardError.ReadToEnd(); // 실행 중 에러 읽기
                process.WaitForExit(); // 종료 대기

                // 결과 확인
                if (process.ExitCode == 0)
                {
                    labState.Content = $"{fileName}.apk 서명이 완료되었습니다.";
                }
                else
                {
                    MessageBox.Show("서명 중 에러 발생:");
                    MessageBox.Show(error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"예외 발생: {ex.Message}");
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            if (LanguageSelector.SelectedIndex == 0) // English
            {
                btnDecompile.Content = "Decompile APK";
                btnCompile.Content = "Compile APK";
                btnSign.Content = "Sign APK";
                chkAutoSign.Content = "Automatically Sign APK After Compile";
                tabMain.Header = "Main";
                tabSettings.Header = "Settings";

                appSettings.Language = "EN";
            }
            else if (LanguageSelector.SelectedIndex == 1) // 한국어
            {
                btnDecompile.Content = "APK 디컴파일";
                btnCompile.Content = "APK 컴파일";
                btnSign.Content = "APK 서명";
                chkAutoSign.Content = "APK 컴파일 후 자동으로 서명";
                tabMain.Header = "메인";
                tabSettings.Header = "설정";

                appSettings.Language = "KO";
            }
            appSettings.Apktool = tboxApktool.Text;
            appSettings.Cert = tboxCert.Text;
            appSettings.Key = tboxKey.Text;
            UpdateSettings();

        }

        private void btnApktool_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Apktool|apktool.jar",
                Title = "Please select the Apktool file. (Only apktool.jar is allowed.)"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                tboxApktool.Text = openFileDialog.FileName;
            }
        }

        private void btnCert_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Certificate|*.x509.pem",
                Title = "Select X509 Certificate (.x509.pem) for APK Signing"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                tboxCert.Text = openFileDialog.FileName;
            }
        }

        private void btnKey_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Private Key|*.pk8",
                Title = "Select Private Key File (.pk8) for APK Signing"
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
    }
}