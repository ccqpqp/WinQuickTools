using System.Collections.Generic;
using System.Windows;

namespace WinQuickTools.Windows
{
    public partial class EtDialog : Window
    {
        public string? SelectedValue { get; private set; }

        public EtDialog(string title, string message)
        {
            InitializeComponent();

            TitleText.Text = title;
            MessageText.Text = message;
        }

        private void Ok_Click(object sender, RoutedEventArgs e)
        {
            SelectedValue = ChoiceBox.SelectedItem as string;
            DialogResult = true;
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        // ALERT
        public static void Alert(string title, string msg)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                var dlg = new EtDialog(title, msg);
                dlg.ChoiceBox.Visibility = Visibility.Collapsed;
                dlg.ShowDialog();
            });
        }

        // CONFIRM
        public static bool Confirm(string title, string msg)
        {
            bool ok = false;

            Application.Current.Dispatcher.Invoke(() =>
            {
                var dlg = new EtDialog(title, msg);
                dlg.ChoiceBox.Visibility = Visibility.Collapsed;
                dlg.ShowDialog();
                ok = dlg.DialogResult == true;
            });

            return ok;
        }

        // SELECT
        public static string? Select(string title, string msg, List<string> options)
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