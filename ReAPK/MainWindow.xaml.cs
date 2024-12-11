using Microsoft.Win32;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
            LoadSettings();
            string exeDirectory = AppDomain.CurrentDomain.BaseDirectory;
            



            // 생성할 폴더 이름
            string folderName = "!Decompiled APKs";

            // 최종 폴더 경로
            string folderPath = Path.Combine(exeDirectory, folderName);

            // 폴더 생성
            if (!Directory.Exists(folderPath)) // 폴더가 존재하지 않을 경우에만 생성
            {
                Directory.CreateDirectory(folderPath);
            }
            string Apktool = Path.Combine(exeDirectory, "Apktool", "apktool.jar");
            string Cert = Path.Combine(exeDirectory, "Resources", "testkey.x509.pem");
            string Key = Path.Combine(exeDirectory, "Resources", "testkey.pk8");
            if (!File.Exists(Apktool))
            {
                MessageBox.Show("Error: apktool.jar이 존재하지 않습니다.");
            }
            if (File.Exists(Apktool))
            {
                tboxApktool.Text = Apktool;
            }
            if (!File.Exists(Cert))
            {
                MessageBox.Show("Error: Cert가 존재하지 않습니다 => 경로 변경 필요");
            }
            if (File.Exists (Cert))
            {
                tboxCert.Text = Cert;
            }
            if (!File.Exists(Key))
            {
                MessageBox.Show("Error: Key가 존재하지 않습니다 => 경로 변경 필요");
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

        private void LoadSettings()
        {
            try
            {
                var test = Properties.Settings.Default.Language;
            }
            catch (ConfigurationErrorsException ex)
            {
                //MessageBox.Show("설정 파일에 오류가 발생했습니다."); //최초 실행 고려
                try
                {
                    Directory.Delete(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ReAPK"), true);
                    MessageBox.Show("지움");
                    return;
                }
                catch (Exception exception)
                {
                    MessageBox.Show("오류가 발생했습니다.");
                    MessageBox.Show(exception.Message);
                }


                //기본 설정 파일로 복구
                //Properties.Settings.Default.Language = "EN";
                //MessageBox.Show("로깅 12");
                //Properties.Settings.Default.AutoSign = false;
                //MessageBox.Show("로깅 1");
                //UpdateSettings();
                
                Properties.Settings.Default.Save();
                
            }
            MessageBox.Show($"로드세팅 호출됨, {Properties.Settings.Default.Language}");
            if (Properties.Settings.Default.Language == "EN")
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
            else if (Properties.Settings.Default.Language == "KO")
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
                MessageBox.Show("Language 로드 실패 - 영어로 설정");
                appSettings.Language = "EN";
                Properties.Settings.Default.Language = "EN";
                LoadSettings();
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
                process.Start();
                labState.Content = $"{fileName}.apk를 디컴파일 중...";
                string error = process.StandardError.ReadToEnd();   // 실행 중 발생한 에러 출력
                process.WaitForExit(); // 명령어가 끝날 때까지 대기

                // 결과 메시지 출력
                if (process.ExitCode == 0)
                {
                    MessageBox.Show($"디컴파일 완료!");
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
            labState.Content = $"{folderName}컴파일중.";
            process.Start();
            string error = process.StandardError.ReadToEnd();   // 실행 중 발생한 에러 출력
            process.WaitForExit(); // 명령어가 끝날 때까지 대기
            try
            {
                // APK 컴파일 실행
                if (process.ExitCode == 0)
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