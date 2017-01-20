using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using AIAssignment2.Foundations;
using System.Threading;

namespace AIAssignment2.Presentation
{
    using GameLogic.Renjus;

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        event EventHandler<MovePlayedEventArgs> PlayerPlayed;

        private IRenju renju;
        private int index;
        private bool playerFirst;
        private bool acceptInput;

        public MainWindow()
        {
            InitializeComponent();
            PlayerPlayed += MainWindow_PlayerPlayed;
            board.MouseDown += board_MouseDown;
            index = 0;
            acceptInput = false;
            resetButton.IsEnabled = false;
        }

        void board_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (acceptInput)
            {
                var canvas = sender as Canvas;
                if (canvas != null)
                {
                    var move = canvas.PlayAt(e.GetPosition(canvas));
                    if (move != null)
                    {
                        canvas.DrawStone(move.Value.X, move.Value.Y, ++index, playerFirst);
                        canvas.UpdateLayout();
                        acceptInput = false;
                        raisePlayerPlayed(move.Value);
                    }
                }
            }
        }

        void MainWindow_PlayerPlayed(object sender, MovePlayedEventArgs e)
        {
            var move = renju.GetNextMove(e.Move);

            if (move.X < 0 || move.Y < 0)
                MessageBox.Show("Disallow move detected!");
            else
            {
                board.DrawStone(move.X, move.Y, ++index, !playerFirst);
                acceptInput = true;
            }
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            board.DrawBoard((int)e.NewValue);
        }

        private void raisePlayerPlayed(Move move)
        {
            var e = new MovePlayedEventArgs(move);
            var handler = PlayerPlayed;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        private void blackButton_Click(object sender, RoutedEventArgs e)
        {
            sizeChooser.IsEnabled = false;
            blackButton.IsEnabled = false;
            whiteButton.IsEnabled = false;
            resetButton.IsEnabled = true;
            playerFirst = false;

            renju = new ThreatRenju((int)sizeChooser.Value, 3);
            raisePlayerPlayed(new Move(-1, -1));
        }

        private void whiteButton_Click(object sender, RoutedEventArgs e)
        {
            sizeChooser.IsEnabled = false;
            blackButton.IsEnabled = false;
            whiteButton.IsEnabled = false;
            resetButton.IsEnabled = true;
            playerFirst = true;

            renju = new ThreatRenju((int)sizeChooser.Value, 3);
            acceptInput = true;
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            sizeChooser.IsEnabled = true;
            blackButton.IsEnabled = true;
            whiteButton.IsEnabled = true;
            resetButton.IsEnabled = false;
            acceptInput = false;
            renju = null;
            index = 0;

            board.DrawBoard((int)sizeChooser.Value);
        }
    }
}
