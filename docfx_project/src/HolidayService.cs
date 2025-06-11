using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace GoogleStyleCalendar
{
    public static class HolidayService
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        // Pobiera święta publiczne z API dla podanego roku i kraju
        public static async Task<List<Holiday>> GetHolidaysAsync(int year, string countryCode)
        {
            string url = $"https://date.nager.at/api/v3/PublicHolidays/{year}/{countryCode}";

            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
                throw new Exception($"Błąd pobierania świąt: {response.StatusCode}");

            var json = await response.Content.ReadAsStringAsync();

            return JsonSerializer.Deserialize<List<Holiday>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }
    }
}