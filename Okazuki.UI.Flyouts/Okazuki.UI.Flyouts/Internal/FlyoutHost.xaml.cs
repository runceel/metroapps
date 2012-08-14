using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// ユーザー コントロールのアイテム テンプレートについては、http://go.microsoft.com/fwlink/?LinkId=234236 を参照してください

namespace Okazuki.UI.Flyouts.Internal
{
    public sealed partial class FlyoutHost : UserControl
    {
        public FlyoutHost()
        {
            this.InitializeComponent();
        }

        public event Action<IUICommand> Completed;
        private void RaiseCompleted(IUICommand command)
        {
            var h = this.Completed;
            if (h != null)
            {
                h(command);
            }
        }

        public FrameworkElement Pane
        {
            get { return this.contentControlPane.Content as FrameworkElement; }
            set { this.contentControlPane.Content = value; }
        }

        private void CommandButton_Click(object sender, RoutedEventArgs e)
        {
            var command = ((Button)sender).DataContext as IUICommand;
            if (command == null)
            {
                return;
            }

            if (command.Invoked == null)
            {
                return;
            }

            command.Invoked(command);
            this.RaiseCompleted(command);
        }
    }
}
