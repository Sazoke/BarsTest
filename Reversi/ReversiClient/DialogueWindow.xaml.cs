using System;
using System.Windows;
using System.Windows.Controls;

namespace ReversiClient
{
    public partial class DialogueWindow : Window
    {
        public DialogueWindow(Action<string> save)
        {
            InitializeComponent();
            Width = 420;
            Height = 440;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            var block = new TextBox()
            {
                Text = "Write save name:",
                Margin = new Thickness(50, 50, 50, 300)
            };
            var nameBox = new TextBox()
            {
                Margin = new Thickness(50, 100, 50, 250)
            };
            var yesButton = new Button()
            {
                Content = "Yes",
                Margin = new Thickness(50, 250, 250, 100)
            };
            yesButton.Click += (sender, args) =>
            {
                save(nameBox.Text);
                Close();
            };
            var noButton = new Button()
            {
                Content = "No",
                Margin = new Thickness(250, 250, 50, 100)
            };
            noButton.Click += (sender, args) => Close();

            Grid.Children.Add(block);
            Grid.Children.Add(nameBox);
            Grid.Children.Add(yesButton);
            Grid.Children.Add(noButton);
        }
    }
}