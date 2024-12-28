using Microsoft.Win32;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
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
            CheckSettings();
            CheckToolsExist(); // 도구가 기본값에 존재하는지 확인, 텍스트박스 설정
            LoadSettings(); // 설정 파일 유효성 확인 (이상하면 복구) + 요소 언어 설정
            
        }
        private void CheckSettings() // 실행함으로서 CheckToolsExist, LoadSettings => 설정 파일 검증 불필요하게 함.
        {
            try
            {
                var test = Properties.Settings.Default.Language;
            }
            catch (ConfigurationErrorsException) //설정 파일이 손상된 경우
            {
                try //설정 파일 제거 시도
                {
                    Directory.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ReAPK"), true);
                    MessageBox.Show("오류가 발생했으나 복구에 성공했습니다.\n변경 사항을 적용하기 위해 프로그램을 다시 실행해 주세요.");
                    Environment.Exit(0); // System.Reflection.TargetInvocationException: 'Exception has been thrown by the target of an invocation.' 발생함
                }
                catch (Exception exception)
                {
                    MessageBox.Show("설정 파일 초기화 중 오류가 발생했습니다.\nWin+R -> %localappdata% 입력 후 ReAPK 폴더를 삭제해 주세요.\nReAPK 폴더가 없는 경우, 프로그램을 재설치해 주세요.");
                    MessageBox.Show("오류 메시지는 다음과 같습니다: " + exception.Message);
                    Environment.Exit(0);
                }
            }
        }
        private void LoadSettings() //설정 파일 검사, 복구, 요소 언어 설정 호출
        {
            
            if (Properties.Settings.Default.Language == "EN") // 설정 파일이 정상적임 + 최초 실행 X, 설정 파일 오류 발생 후 첫 실행일 가능성 있음
            {
                SetUILanguage(Properties.Settings.Default.Language);
            }
            else if (Properties.Settings.Default.Language == "KO") // 설정 파일이 정상적임 + 최초 실행 X
            {
                SetUILanguage(Properties.Settings.Default.Language);
            }
            else if (Properties.Settings.Default.Language == "FirstRun") // 최초 실행임
            {
                appSettings.Language = "EN";
                Properties.Settings.Default.Language = "EN";
                
                //null 방지 기본 설정
                Properties.Settings.Default.AutoSign = true;
                LoadSettings();
            }
            else
            {
                MessageBox.Show("알 수 없는 오류가 발생했습니다. 프로그램을 다시 실행해 주세요.");
                Environment.Exit(0);
            }
            chkAutoSign.IsChecked = Properties.Settings.Default.AutoSign; // UI 반영
            tboxApktool.Text = Properties.Settings.Default.Apktool; // UI 반영
            tboxCert.Text = Properties.Settings.Default.Cert; // UI 반영
            tboxKey.Text = Properties.Settings.Default.Key; // UI 반영
            appSettings.AutoSign = Properties.Settings.Default.AutoSign; // 앱 설정
            appSettings.Apktool = Properties.Settings.Default.Apktool; // 앱 설정
            appSettings.Cert = Properties.Settings.Default.Cert; // 앱 설정
            appSettings.Key = Properties.Settings.Default.Key; // 앱 설정
        }
        public void CheckToolsExist() // 시작시 도구가 기본값에 존재하는지 확인, 텍스트박스 설정
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string Apktool = Path.Combine(exeDirectory, "Apktool", "apktool.jar");
            string Cert = Path.Combine(exeDirectory, "Resources", "testkey.x509.pem");
            string Key = Path.Combine(exeDirectory, "Resources", "testkey.pk8");
            if (!File.Exists(Apktool))
            {
                if (!File.Exists(Properties.Settings.Default.Apktool))
                {
                    MessageBox.Show("Error: apktool.jar이 존재하지 않습니다.\n설정에서 경로를 변경해주세요.");
                }
            }
            if (File.Exists(Apktool))
            {
                tboxApktool.Text = Apktool;
            }
            if (!File.Exists(Cert))
            {
                if (!File.Exists(Properties.Settings.Default.Cert))
                {
                    MessageBox.Show("Error: Cert가 존재하지 않습니다.\n설정에서 경로를 변경해주세요.");
                }
            }
            if (File.Exists(Cert))
            {
                tboxCert.Text = Cert;
            }
            if (!File.Exists(Key))
            {
                if (!File.Exists(Properties.Settings.Default.Key))
                {
                    MessageBox.Show("Error: Key가 존재하지 않습니다.\n설정에서 경로를 변경해주세요.");
                }
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
                labWarning.Content = "Please press 'Set' after making changes to save your settings.";
                labHowToUse.Content = "Simply drag and drop files onto the button to process them.";
            }
            else if (Language == "KO")
            {
                appSettings.Language = "KO";
                LanguageSelector.SelectedIndex = 1;
                btnDecompile.Content = "APK 디컴파일";
                btnCompile.Content = "APK 컴파일";
                btnSign.Content = "APK 서명";
                chkAutoSign.Content = "APK 컴파일 후 자동으로 서명";
                tabMain.Header = "메인";
                tabSettings.Header = "설정";
                labWarning.Content = "설정값을 저장하기 위해 설정값 변경 후 'Set' 버튼을 눌러주세요.";
                labHowToUse.Content = "파일을 버튼으로 Drag & Drop해서 작업을 수행합니다.";
            }
            else
            {
                MessageBox.Show("알 수 없는 오류가 발생했습니다. 프로그램이 정상적으로 작동하지 않을 수 있습니다.");
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
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string Apktool = Path.Combine(exeDirectory, "Apktool", "apktool.jar");
            string folderPath = Path.Combine(exeDirectory, "!Decompiled APKs", fileName);
            string command = $"-jar \"{Apktool}\" d -o \"{folderPath}\" \"{apk}\"";
            try
            {
                labState.Content = $"{fileName}.apk를 디컴파일 중..."; // 작동 안 함. 수정 필요.
                var result = await Run("java", command);
                if (result.ExitCode == 0)
                {
                    labState.Content = $"{fileName}.apk의 디컴파일이 완료되었습니다.";
                }
                else
                {
                    if (result.error.Contains("already exists."))
                    {
                        MessageBoxResult msgboxresult = MessageBox.Show("이미 디컴파일한 폴더가 존재합니다. 덮어쓸까요?", "폴더 덮어쓰기", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                        if (msgboxresult == MessageBoxResult.Yes)
                        {
                            command = $"-jar \"{Apktool}\" d -f -o \"{folderPath}\" \"{apk}\"";
                            try
                            {
                                result = await Run("java", command);
                                if (result.ExitCode == 0)
                                {
                                    labState.Content = $"{fileName}.apk의 디컴파일이 완료되었습니다.";
                                }
                                else
                                {
                                    MessageBox.Show($"디컴파일 중 문제가 발생했습니다:\n{result.error}");
                                }
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show($"예외 발생: {ex.Message}");
                            }
                        }
                        else if (msgboxresult == MessageBoxResult.No)
                        {
                            labState.Content = $"{fileName}.apk의 디컴파일이 취소되었습니다.";
                        }
                    }
                    else
                    {
                        MessageBox.Show($"디컴파일 중 문제가 발생했습니다:\n{result.error}");
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
                    string folderName = Path.GetFileNameWithoutExtension(droppedItem);

                    // 컴파일 실행
                    Compile(droppedItem, folderName);
                }
            }
        }

        private async void Compile(string folderPath, string folderName)
        {
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string outputApkPath = Path.Combine(exeDirectory, "!Compiled APKs", $"{folderName}_compiled.apk");
            string command = $"-jar \"{appSettings.Apktool}\" b \"{folderPath}\" -o \"{outputApkPath}\"";
            try
            {
                labState.Content = $"{folderName} 컴파일 중...";
                var result = await Run("java", command);

                if (result.ExitCode == 0)
                {
                    labState.Content = $"{folderName}의 컴파일이 완료되었습니다.";
                    if (chkAutoSign.IsChecked == true)
                    {
                        bool fromCompile = true;
                        Sign(outputApkPath, folderName, fromCompile);
                    }
                }
                else
                {
                    MessageBox.Show($"컴파일 중 문제가 발생했습니다:\n{result.error}");
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
            string apksigner = Path.Combine(exeDirectory, "Resources", "Android", "Sdk", "build-tools", "35.0.0", "apksigner.bat");
            string keyPath = appSettings.Key;
            string certPath = appSettings.Cert;

            string command = $"sign --key \"{keyPath}\" --cert \"{certPath}\" --v4-signing-enabled false --out \"{signedPath}\" \"{apk}\"";
            Trace.WriteLine(certPath);
            try
            {
                labState.Content = $"{fileName}.apk 서명 중...";
                var result = await Run(apksigner, command);

                if (result.ExitCode == 0)
                {
                    labState.Content = $"{fileName}.apk 서명이 완료되었습니다.";
                }
                else
                {
                    MessageBox.Show("서명 중 오류 발생: " + result.error);
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
                SetUILanguage("EN");
            }
            else if (LanguageSelector.SelectedIndex == 1) // 한국어
            {
                SetUILanguage("KO");
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
                MessageBox.Show("알 수 없는 오류가 발생했습니다.\n이 버튼 아래 보이는 체크박스를 클릭해보고 다시 이 버튼을 눌러보면 해결될 것 같네요!");
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

        private void btnDecompile_Click(object sender, RoutedEventArgs e)
        {
            (string fullapk, string fileName) = OpenFileDialog("Select an APK to decompile.", "APK|*.apk");
            Decompile(fullapk, fileName);
        }

        private void btnCompile_Click(object sender, RoutedEventArgs e)
        {
            (string fullfoldername, string foldername) = OpenFolderDialog("Select an APK to compile.");
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
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string folderName = "!Signed APKs";
            string folderPath = Path.Combine(exeDirectory, folderName);
            Directory.CreateDirectory(folderPath);
            (string fullapk, string fileName) = OpenFileDialog("Select an APK to sign.", "APK|*.apk");
            if (fullapk != null)
            {
                Sign(fullapk, fileName, false);
            }
        }

        private void btnOpenDecompiledFolder_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "!Decompiled APKs");
            if (Directory.Exists(folderPath))
            {
                Process.Start("explorer", folderPath);
            }
            else
            {
                MessageBox.Show("디컴파일한 APK가 없습니다.");
            }
        }

        private void btnOpenCompiledFolder_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "!Compiled APKs");
            if (Directory.Exists(folderPath))
            {
                Process.Start("explorer", folderPath);
            }
            else
            {
                MessageBox.Show("컴파일된 APK가 없습니다.");
            }
        }

        private void btnOpenSignedFolder_Click(object sender, RoutedEventArgs e)
        {
            string folderPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "!Signed APKs");
            if (Directory.Exists(folderPath))
            {
                Process.Start("explorer", folderPath);
            }
            else
            {
                MessageBox.Show("서명된 APK가 없습니다.");
            }
        }
    }
}