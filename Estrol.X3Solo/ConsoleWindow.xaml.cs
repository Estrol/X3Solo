using System.Windows;
using System.Windows.Input;

namespace Estrol.X3Solo {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ConsoleWindow : Window {
        public delegate void EventMessage(object sender, string message);
        public event EventMessage OnMessage;

        public ConsoleWindow() {
            InitializeComponent();
        }

        private void TopBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }

        private void BtnExit(object sender, RoutedEventArgs e) {
            Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            Hide();
            e.Cancel = true;
        }

        public void BtnSend(object sender, RoutedEventArgs e) {
            string input = Input.Text;
            Input.Text = "";

            OnMessage?.Invoke(this, input);
        }

        public void Write(string text) {
            Output.Dispatcher.Invoke(() => {
                Output.AppendText(text);
            });
        }

        public void Write(char text) {
            Output.Dispatcher.Invoke(() => {
                Output.AppendText(text.ToString());
            });
        }
    }
}
