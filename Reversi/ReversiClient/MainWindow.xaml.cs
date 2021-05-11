using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text.Json;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using ReversiClient.Data;
using ReversiClient.Models;
using ReversiClient.Repositories;
using Point = System.Drawing.Point;

namespace ReversiClient
{
    public partial class MainWindow : Window
    {
        private List<Button> _buttons;
        private IGame _game;
        private int _length;
        private Grid _grid;
        private GameRepository _repository;
        
        public MainWindow()
        {
            InitializeComponent();
            _repository = new GameRepository(new ReversiDbContext(
                @"data source=source;Initial Catalog=db;Integrated Security=True;"
            ));
            Width = 420;
            Height = 440;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            _grid = new Grid();
            Grid.Children.Add(_grid);
            SetButtons();
        }

        public void WriteMessage(string message)
        {
            Dispatcher.Invoke(() =>
            {
                _grid.Children.Clear();
                _grid.Children.Add(new TextBlock() {Text = message});
            });
        }

        public void SetButtons()
        {
            Dispatcher.Invoke(() =>
            {
                _grid.Children.Clear();

                var playerButton = new Button()
                {
                    Content = "Start game with player",
                    Margin = new Thickness(10, 80, 200, 300)
                };
                var botButton = new Button()
                {
                    Content = "Start game with bot",
                    Margin = new Thickness(200, 80, 10, 300)
                };
                var historyButton = new Button()
                {
                    Content = "Show history",
                    Margin = new Thickness(10, 180, 200, 200)
                };
                var saveButton = new Button()
                {
                    Content = "Load save",
                    Margin = new Thickness(200, 180, 10, 200)
                };
                

                playerButton.Click += (sender, args) =>
                {
                    _game = new GameWithPlayer(this, _repository);
                    _game.Play();
                };
                botButton.Click += (sender, args) =>
                {
                    _game = new GameWithBot(this, _repository);
                    _game.Play();
                };
                historyButton.Click += (sender, args) =>
                {
                    var listView = new ListView();
                    listView.View = new GridView()
                    {
                        Columns =
                        {
                            new GridViewColumn(){Header = "Your score", DisplayMemberBinding = new Binding("PlayerScore")},
                            new GridViewColumn(){Header = "Enemy score", DisplayMemberBinding = new Binding("EnemyScore")},
                            new GridViewColumn(){Header = "Time", DisplayMemberBinding = new Binding("Date")}
                        }
                    };
                    foreach (var match in _repository.GetHistory())
                        listView.Items.Add(match);

                    _grid.Children.Add(listView);
                };
                saveButton.Click += (sender, args) =>
                {
                    _grid.Children.Clear();
                    var combo = new ComboBox()
                    {
                        Margin = new Thickness(20,20,300,300)
                    };
                    foreach (var save in _repository.GetSaves())
                    {
                        combo.Items.Add(new ComboBoxItem() {Content = save});
                    }
                    
                    var button = new Button()
                    {
                        Margin = new Thickness(20,100,300,200),
                        Content = "Load"
                    };
                    button.Click += (o, eventArgs) =>
                    {
                        _game = new GameWithBot(this, _repository, combo.SelectionBoxItem as GameState);
                        _game.Play();
                    };

                    _grid.Children.Add(combo);
                    _grid.Children.Add(button);
                };
                
                _grid.Children.Add(playerButton);
                _grid.Children.Add(botButton);
                _grid.Children.Add(historyButton);
                _grid.Children.Add(saveButton);
            });
        }

        public void SetMap(int length,  RoutedEventHandler clickOnMap)
        {
            Dispatcher.Invoke(() =>
            {
                _length = length;
                var lengthOfPlace = 50;
                _buttons = new List<Button>();
                for (var i = 0; i < length; i++)
                for (var j = 0; j < length; j++)
                { 
                    var button = new Button
                    {
                        Name = 'a' + (i * length + j).ToString(),
                        Background = new SolidColorBrush(Colors.Green),
                        Margin = new Thickness(
                            j * lengthOfPlace,
                            i * lengthOfPlace,
                            Width - 20 - lengthOfPlace * (j + 1),
                            Height - 40 - lengthOfPlace * (i + 1))
                    };
                    button.Click += clickOnMap;
                    _buttons.Add(button);
                }
                _grid.Children.Clear();
                foreach (var button in _buttons)
                    _grid.Children.Add(button);
            });
        }

        public void SetPlaces(List<Point> places, int playerNumber)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var place in places)
                {
                    var ell = new Ellipse()
                    {
                        Width = 50, Height = 50,
                        Fill = new SolidColorBrush(playerNumber.Equals(0) ? Colors.Black : Colors.White)
                    };
                    _buttons[place.Y * _length + place.X].Content = ell;
                }
            });
        }
        
        public void SetPlaces(List<(Point, int)> map)
        {
            Dispatcher.Invoke(() =>
            {
                foreach (var place in map)
                {
                    if (place.Item2.Equals(-1))
                        continue;
                    var ell = new Ellipse()
                    {
                        Width = 50, Height = 50,
                        Fill = new SolidColorBrush(place.Item2.Equals(0) ? Colors.Black : Colors.White)
                    };
                    _buttons[place.Item1.Y * _length + place.Item1.X].Content = ell;
                }
            });
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            if (_game is GameWithBot)
            {
                new DialogueWindow(((GameWithBot) _game).SaveGame).Show();
                
            }
            _game?.End();
            base.OnClosing(e);
        }
    }
}