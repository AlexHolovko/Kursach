using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using Newtonsoft.Json;
using System.IO;

namespace Kursach
{
    public partial class GuestsTableWindow : Window, INotifyPropertyChanged
    {
        private ObservableCollection<Guest> _guests;
        private const string UsersFilePath = @"C:\file\users_data.json";
        public ObservableCollection<Guest> Guests
        {
            get { return _guests; }
            set
            {
                _guests = value;
                OnPropertyChanged(nameof(Guests));
            }
        }

        public GuestsTableWindow()
        {
            InitializeComponent();
            Guests = new ObservableCollection<Guest>();
            DataContext = this;

            Loaded += GuestsTableWindow_Loaded;
        }

        private void GuestsTableWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadUsersData(UsersFilePath);
        }

        private void LoadUsersData(string jsonFilePath)
        {
            try
            {
                string json = File.ReadAllText(jsonFilePath);
                Guests = JsonConvert.DeserializeObject<ObservableCollection<Guest>>(json);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка під час завантаження даних із файлу: {ex.Message}", "Помилка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class Guest : INotifyPropertyChanged
        {
            private int _id;
            public int Id
            {
                get { return _id; }
                set
                {
                    if (_id != value)
                    {
                        _id = value;
                        OnPropertyChanged(nameof(Id));
                    }
                }
            }

            private string _guestName;
            public string GuestName
            {
                get { return _guestName; }
                set
                {
                    if (_guestName != value)
                    {
                        _guestName = value;
                        OnPropertyChanged(nameof(GuestName));
                    }
                }
            }
            private string _guestEmail;
            public string GuestEmail
            {
                get { return _guestEmail; }
                set
                {
                    if (_guestEmail != value)
                    {
                        _guestEmail = value;
                        OnPropertyChanged(nameof(GuestEmail));
                    }
                }
            }

            private string _guestPhone;
            public string GuestPhone
            {
                get { return _guestPhone; }
                set
                {
                    if (_guestPhone != value)
                    {
                        _guestPhone = value;
                        OnPropertyChanged(nameof(GuestPhone));
                    }
                }
            }

            private string _checkInDate;
            public string CheckInDate
            {
                get { return _checkInDate; }
                set
                {
                    if (_checkInDate != value)
                    {
                        _checkInDate = value;
                        OnPropertyChanged(nameof(CheckInDate));
                    }
                }
            }

            private string _checkOutDate;
            public string CheckOutDate
            {
                get { return _checkOutDate; }
                set
                {
                    if (_checkOutDate != value)
                    {
                        _checkOutDate = value;
                        OnPropertyChanged(nameof(CheckOutDate));
                    }
                }
            }

            private string _roomType;
            public string RoomType
            {
                get { return _roomType; }
                set
                {
                    if (_roomType != value)
                    {
                        _roomType = value;
                        OnPropertyChanged(nameof(RoomType));
                    }
                }
            }

            private string _roomNumber;
            public string RoomNumber
            {
                get { return _roomNumber; }
                set
                {
                    if (_roomNumber != value)
                    {
                        _roomNumber = value;
                        OnPropertyChanged(nameof(RoomNumber));
                    }
                }
            }

            private string _checkArrival;
            public string CheckArrival
            {
                get { return _checkArrival; }
                set
                {
                    if (_checkArrival != value)
                    {
                        _checkArrival = value;
                        OnPropertyChanged(nameof(CheckArrival));
                    }
                }
            }

            private string _checkDeparture;
            public string CheckDeparture
            {
                get { return _checkDeparture; }
                set
                {
                    if (_checkDeparture != value)
                    {
                        _checkDeparture = value;
                        OnPropertyChanged(nameof(CheckDeparture));
                    }
                }
            }


            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
