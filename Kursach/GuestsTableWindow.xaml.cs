using System.Collections.Generic;
using System.Windows;

namespace Kursach
{
    public partial class GuestsTableWindow : Window
    {
        public GuestsTableWindow(List<User> users)
        {
            InitializeComponent();
            GuestsDataGrid.ItemsSource = users;
        }
    }
}