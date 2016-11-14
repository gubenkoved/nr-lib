using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace NRLib
{
    public static class Commands
    {
        public static RoutedUICommand OpenContainingFolderCommand = new RoutedUICommand("Open containing folder", "Open containing folder", typeof(Commands));
        public static RoutedUICommand OpenFileCommand = new RoutedUICommand("Open file", "Open file", typeof(Commands));
        public static RoutedUICommand SearchCommand = new RoutedUICommand("Search", "Search", typeof(Commands));
        public static RoutedUICommand ChangeLibDirCommand = new RoutedUICommand("Search", "Search", typeof(Commands));
        public static RoutedUICommand ToogleFavoriteCommand = new RoutedUICommand("Toogle favorite", "ToogleFavorite", typeof(Commands));
        public static RoutedUICommand ShowFavoritesCommand = new RoutedUICommand("Show favorites", "ShowFavorites", typeof(Commands));
    }
}
