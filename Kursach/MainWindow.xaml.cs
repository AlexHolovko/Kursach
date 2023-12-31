﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using Newtonsoft.Json;
using System.Collections.ObjectModel;
using Kursach;
using System.Globalization;
using System.Threading;

namespace Kursach
{
    public partial class MainWindow : Window
    {
        private const string UsersFilePath = @"file\users_data.json";
        private const string HotelFilePath = @"file\hotel_data.json";
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
            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.GetCultureInfo("uk-UA");
            System.Threading.Thread.CurrentThread.CurrentCulture =  System.Globalization.CultureInfo.GetCultureInfo("uk-UA");
        }
        private void OpenGuestsTableButton_Click(object sender, RoutedEventArgs e)
{
    GuestsTableWindow guestsTableWindow = new GuestsTableWindow(users);
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
        private decimal CalculateStayPrice(string roomType, int numberOfDays)
        {
            // Додайте логіку для обчислення вартості на основі типу номера та кількості днів
            decimal basePrice = 300; // Встановіть базову ціну за замовчуванням
            decimal priceMultiplier = 1.0m;

            switch (roomType)
            {
                case "Стандартний":
                    priceMultiplier = 1.0m;
                    break;
                case "Двомісний":
                    priceMultiplier = 1.5m;
                    break;
                case "Сімейний":
                    priceMultiplier = 2.0m;
                    break;
                case "Люкс":
                    priceMultiplier = 3.0m;
                    break;
                default:
                    // Обробити невідомі типи номерів
                    break;
            }

            decimal totalPrice = basePrice * priceMultiplier * numberOfDays;
            return totalPrice;
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

            int numberOfDays = (checkOutDate.Value - checkInDate.Value).Days;

            // Обчисліть загальну вартість
            decimal totalPrice = CalculateStayPrice(selectedRoomType, numberOfDays);

            MessageBox.Show($"Бронювання номера {selectedFloor + 1}-{selectedRoom + 1} пройшло успішно. Ваш id: {userId}\n" +
                            $"Загальна вартість проживання: {totalPrice:C}");
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
        public string? GuestName { get; set; } = string.Empty;
        public string? GuestEmail { get; set; } = string.Empty;
        public string? GuestPhone { get; set; } = string.Empty;
        public string? CheckInDate { get; set; } = string.Empty;
        public string? CheckOutDate { get; set; } = string.Empty;
        public string? RoomType { get; set; } = string.Empty;
        public string? RoomNumber { get; set; } = string.Empty;
    }
}
