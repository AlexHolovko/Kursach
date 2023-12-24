using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Kursach
{
    public partial class GuestDetailsWindow : Window
    {
        private MainWindow mainWindow;

        public GuestDetailsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.mainWindow = mainWindow;

            var roomTypes = new[] { "Стандартний", "Двомісний", "Сімейний", "Люкс" };
            NewRoomTypeComboBox.ItemsSource = roomTypes;
        }
        private void UpdateGuestDataButton_Click(object sender, RoutedEventArgs e)
        {
            string guestIdOrPhoneNumber = GuestIdOrPhoneNumberTextBox.Text;
            var guest = mainWindow.users.FirstOrDefault(u => u.Id.ToString() == guestIdOrPhoneNumber || u.GuestPhone == guestIdOrPhoneNumber);

            if (guest != null)
            {
                if (NewCheckInDatePicker.SelectedDate.HasValue)
                    guest.CheckInDate = NewCheckInDatePicker.SelectedDate?.ToShortDateString();

                if (NewArrivalTimePicker?.Value != null)
                {
                    TimeSpan arrivalTime = NewArrivalTimePicker.Value?.TimeOfDay ?? TimeSpan.Zero;
                  //  guest.CheckArrival = arrivalTime.ToString("HH:mm:ss");
                }

                if (NewDepartureTimePicker?.Value != null)
                {
                    TimeSpan departureTime = NewDepartureTimePicker.Value?.TimeOfDay ?? TimeSpan.Zero;
                //    guest.CheckDeparture = departureTime.ToString("HH:mm:ss");
                }

                if (NewCheckOutDatePicker.SelectedDate.HasValue)
                    guest.CheckOutDate = NewCheckOutDatePicker.SelectedDate?.ToShortDateString();

               

                if (NewRoomTypeComboBox.SelectedItem != null)
                    guest.RoomType = NewRoomTypeComboBox.SelectedItem as string;

                mainWindow.UpdateUsersJsonFile();

                MessageBox.Show($"Дані гостя з ID {guest.Id} успішно оновлені.");
                Close();
            }
            else
            {
                MessageBox.Show("Гість з вказаним ID або номером телефону не знайдений.");
            }
        }

    }
}
