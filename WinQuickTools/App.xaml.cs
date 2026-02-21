using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using WinQuickTools.Windows;

namespace WinQuickTools
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // ✅ 여기서부터: 어디서 죽는지 무조건 메시지 + 로그
            DispatcherUnhandledException += (_, ex) =>
            {
                Log(ex.Exception);
                System.Windows.MessageBox.Show(ex.Exception.ToString(), "DispatcherUnhandledException");
                ex.Handled = true;
                Shutdown();
            };

            AppDomain.CurrentDomain.UnhandledException += (_, ex) =>
            {
                var err = ex.ExceptionObject as Exception ?? new Exception(ex.ExceptionObject?.ToString());
                Log(err);
                System.Windows.MessageBox.Show(err.ToString(), "UnhandledException");
                Shutdown();
            };

            TaskScheduler.UnobservedTaskException += (_, ex) =>
            {
                Log(ex.Exception);
                System.Windows.MessageBox.Show(ex.Exception.ToString(), "UnobservedTaskException");
                ex.SetObserved();
                Shutdown();
            };
            // ✅ 여기까지

            try
            {
                // =========================
                // 우클릭 실행 모드
                // =========================
                if (e.Args.Length > 0)
                {
                    string cmd = e.Args[0];
                    string? path = e.Args.Length > 1 ? e.Args[1] : null;

                    ContextActions.Execute(cmd, path);

                    Current.Shutdown();
                    return;
                }

                // =========================
                // 일반 실행
                // =========================
                base.OnStartup(e);

                var win = new MainWindow();
                win.Show();
                win.Activate();
            }
            catch (Exception ex)
            {
                Log(ex);
                System.Windows.MessageBox.Show(ex.ToString(), "OnStartup catch");
                Shutdown();
            }
        }

        private static void Log(Exception? ex)
        {
            try
            {
                var p = Path.Combine(Path.GetTempPath(), "WinQuickTools.crash.log");
                File.AppendAllText(p, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}]\r\n{ex}\r\n\r\n");
            }
            catch { }
        }

        // SELECT 그대로 유지
        public static string? Choice(string title, string msg, List<string> options)
        {
            string? result = null;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var dlg = new EtDialog(title, msg);

                foreach (var o in options)
                    dlg.ChoiceBox.Items.Add(o);

                dlg.ChoiceBox.SelectedIndex = 0;

                if (dlg.ShowDialog() == true)
                    result = dlg.SelectedValue;
            });

            return result;
        }
    }
}