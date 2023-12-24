using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Kursach;

namespace Kursach
{
    public partial class MainWindow : Window
    {
        private const string UsersFilePath = @"C:\file\users_data.json";
        private const string HotelFilePath = @"C:\file\hotel_data.json";
        private const int Floors = 4;
        private const int RoomsPerFloor = 10;
        private int[,] hotelMatrix = new int[Floors, RoomsPerFloor];
        public List<User> users { get; private set; } = new List<User>();

        public MainWindow()
        {
            InitializeComponent();
            List<string> roomTypes = new List<string>
            {
                "Стандартний",
                "Двомісний",
                "Сімейний",
                "Люкс",
            };
            RoomsTypeComboBox.ItemsSource = roomTypes;
            InitializeHotelMatrix();
            InitializeUsers();
            DataContext = this;
        }
        private void OpenGuestsTableButton_Click(object sender, RoutedEventArgs e)
        {
            GuestsTableWindow guestsTableWindow = new GuestsTableWindow();
            guestsTableWindow.Show();
        }
        private void InitializeUsers()
        {
            if (File.Exists(UsersFilePath))
            {
                string json = File.ReadAllText(UsersFilePath);
                users = JsonConvert.DeserializeObject<List<User>>(json) ?? new List<User>();
            }
        }

        private void InitializeHotelMatrix()
        {
            if (File.Exists(HotelFilePath))
            {
                string json = File.ReadAllText(HotelFilePath);
                hotelMatrix = JsonConvert.DeserializeObject<int[,]>(json) ?? new int[Floors, RoomsPerFloor];
            }
            else
            {
                for (int i = 0; i < Floors; i++)
                {
                    for (int j = 0; j < RoomsPerFloor; j++)
                    {
                        hotelMatrix[i, j] = 0;
                    }
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            string selectedRoomType = RoomsTypeComboBox.SelectedItem as string;

            int selectedFloor = GetFloorIndexByRoomType(selectedRoomType);
            int selectedRoom = GetFirstAvailableRoom(selectedFloor);

            if (selectedFloor == -1 || selectedRoom == -1)
            {
                MessageBox.Show("Не вдалося знайти вільний номер обраного типу.");
                return;
            }

            DateTime? checkInDate = CheckInDatePicker.SelectedDate;
            DateTime? checkOutDate = CheckOutDatePicker.SelectedDate;

            string name = NameTextBox.Text;
            string email = EmailTextBox.Text;
            string phone = PhoneTextBox.Text;

            if (!IsValidBookingDates(checkInDate, checkOutDate) || !IsValidName(name) || !IsValidEmail(email) || !IsValidPhoneNumber(phone))
            {
                MessageBox.Show("Будь ласка, перевірте правильність введених даних.");
                return;
            }

            int userId = GenerateUserId();
            User user = new User
            {
                Id = userId,
                GuestName = name,
                GuestEmail = email,
                GuestPhone = phone,
                CheckInDate = checkInDate.Value.ToShortDateString(),
                CheckOutDate = checkOutDate.Value.ToShortDateString(),
                RoomType = selectedRoomType,
                RoomNumber = $"{selectedFloor + 1}-{selectedRoom + 1}",
            };

            hotelMatrix[selectedFloor, selectedRoom] = 1;
            users.Add(user);

            UpdateHotelJsonFile();
            UpdateUsersJsonFile();

            MessageBox.Show($"Бронювання номера {selectedFloor + 1}-{selectedRoom + 1} пройшло успішно. Ваш id: {userId}");
        }
        private void OpenGuestDetailsButton_Click(object sender, RoutedEventArgs e)
        {
            GuestDetailsWindow guestDetailsWindow = new GuestDetailsWindow(this);
            guestDetailsWindow.Show();
        }
        private bool IsValidName(string name)
        {
            return !string.IsNullOrWhiteSpace(name) && name.Length >= 3;
        }

        private bool IsValidEmail(string email)
        {
            return !string.IsNullOrWhiteSpace(email) && email.Contains("@") && email.Length >= 5;
        }

        private bool IsValidPhoneNumber(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone) && phone.All(char.IsDigit) && phone.Length >= 5 && phone.Length <= 15;
        }
        private int GetFloorIndexByRoomType(string roomType)
        {
            switch (roomType)
            {
                case "Стандартний":
                    return 0;
                case "Двомісний":
                    return 2;
                case "Сімейний":
                    return 2;
                case "Люкс":
                    return 3;
                default:
                    return -1;
            }
        }
        private bool IsIndexValid(int row, int col)
        {
            return row >= 0 && row < hotelMatrix.GetLength(0) && col >= 0 && col < hotelMatrix.GetLength(1);
        }

        private bool IsRoomAvailable(int floor, int room)
        {
            return IsIndexValid(floor, room) && hotelMatrix[floor, room] == 0;
        }

        private int GetFirstAvailableRoom(int floorIndex)
        {
            for (int i = 0; i < RoomsPerFloor; i++)
            {
                if (IsRoomAvailable(floorIndex, i))
                {
                    return i;
                }
            }
            return -1;
        }

        private void UpdateHotelJsonFile()
        {
            string json = JsonConvert.SerializeObject(hotelMatrix);
            File.WriteAllText(HotelFilePath, json);
        }

        public void UpdateUsersJsonFile()
        {
            string json = JsonConvert.SerializeObject(users, new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            });
            File.WriteAllText(UsersFilePath, json);
        }

        private bool IsValidBookingDates(DateTime? checkInDate, DateTime? checkOutDate)
        {
            return checkInDate.HasValue && checkOutDate.HasValue && checkInDate <= checkOutDate && checkInDate >= DateTime.Today;
        }

        private int GenerateUserId()
        {
            return users.Any() ? users.Max(u => u.Id) + 1 : 1;
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string? GuestName { get; set; }
        public string? GuestEmail { get; set; }
        public string? GuestPhone { get; set; }
        public string? CheckInDate { get; set; }
        public string? CheckOutDate { get; set; }
        public string? RoomType { get; set; }
        public string? RoomNumber { get; set; }
        public DateTime? CheckArrival { get; set; }
        public DateTime? CheckDeparture { get; set; }
    }
}
