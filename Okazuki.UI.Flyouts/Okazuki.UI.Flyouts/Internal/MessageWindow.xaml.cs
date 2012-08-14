using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
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
    public sealed partial class MessageWindow : UserControl
    {
        public MessageWindow()
        {
            this.InitializeComponent();
        }

        public string Message
        {
            get { return this.textBlockMessage.Text; }
            set { this.textBlockMessage.Text = value; }
        }
    }
}
