using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Data;

namespace WpfApp5
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            

            db = new DBClass();

            DataContext= db;
            
        }

        LogWindow1 ?Lw = null;
        DBClass db;

        private void Button_Click(object sender, RoutedEventArgs e)
        {

         //Void Method Only
          Thread th1 = new Thread(new ThreadStart(ffmpegProsseing));

            Lw = new LogWindow1();
            Lw.Show();

            th1.Start();

        }



        void ffmpegProsseing()
        {


            
            using (var ctoken = new CancellationTokenSource())
            {
                using (var _process = new Process())
                {

                    _process.StartInfo.CreateNoWindow = true;
                    _process.StartInfo.UseShellExecute = false;
                    _process.StartInfo.RedirectStandardInput = true;

                    _process.StartInfo.RedirectStandardError = true;
                    _process.StartInfo.FileName = "cmd.exe";
                    _process.EnableRaisingEvents = true;

                    //var ffmpc = new FfmpegQueryClass();
                    string inputString = @"""" + db.inputPath + @"""";

                    string outputString = @"""" + db.outputPath + ".mp4" + @"""" + " -y";
                    
                    string argu = inputString + " " + outputString;

                    _process.StartInfo.Arguments = "/c ffmpeg.exe -i " + argu;
                    _process.EnableRaisingEvents = true;


                    
                    _process.Exited += new EventHandler(ffmpeg_Exited);

                    _process.OutputDataReceived += _process_OutputDataReceived;


                    /////////以下と同じ_ パフォーマンス的にはこっちの方が速い
                    //process.ErrorDataReceived += ReceveData;
                    // void ReceveData(object sender, DataReceivedEventArgs e)
                    //{
                    //    throw new NotImplementedException();
                    //}
                    /////////
                    _process.ErrorDataReceived += new DataReceivedEventHandler(delegate (object obj, DataReceivedEventArgs e)
                    {
                        ///Access to Another thread
                        Dispatcher.Invoke(() =>
                        {
                            Lw.richText.AppendText(e.Data);
                            Lw.richText.AppendText(Environment.NewLine);

                            Debug.WriteLine(e.Data);
                            Debug.WriteLine(Environment.NewLine);


                            Lw.richText.ScrollToEnd();

                        });
                    });
                    _process.Start();

                    
                    _process.BeginErrorReadLine();
                    _process.WaitForExit();
                }
                
            }




             void ffmpeg_Exited(object? sender, EventArgs e)
            {
                ;
            }



        }

        private void _process_OutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            Debug.WriteLine(e);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
           input.Text = db.inputPath = @"C:\Users\USER\Videos\sample.mp4";
           outputText.Text = db.outputPath = db.inputPath + "_output";
        }
    }

}