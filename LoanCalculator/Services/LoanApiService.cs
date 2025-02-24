using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LoanCalculator.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LoanCalculator.Services
{
    public class LoanApiService
    {
        private readonly HttpClient _httpClient;

        public LoanApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // Fetch All Customer Loans
        public async Task<IEnumerable<LoanDetail>> GetLoanDetailsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var responseMessage = await _httpClient.GetAsync("https://localhost:7159/api/customer/get-all-loans");

            if (!responseMessage.IsSuccessStatusCode)
            {
                throw new Exception($"API request failed: {responseMessage.StatusCode}");
            }

            string jsonResponse = await responseMessage.Content.ReadAsStringAsync();
            Console.WriteLine("Received JSON: " + jsonResponse); // Debugging line

            try
            {
                var response = JsonConvert.DeserializeObject<IEnumerable<LoanDetail>>(jsonResponse);
                return response ?? new List<LoanDetail>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] JSON Deserialization Failed: {ex.Message}");
                throw;
            }
        }

        // Create New Loan Detail (Sends Data to API)
        public async Task<bool> CreateCustomerDetailAsync(decimal loanAmt, DateTime dueDate, int loanTerm, int customerNo, string token)
        {
            try
            {
                if (loanAmt <= 0 || loanTerm <= 0)
                {
                    Console.WriteLine("[ERROR] Invalid Loan Amount or Loan Term. Must be greater than 0.");
                    return false;
                }

                // Get today's date
                DateTime startDate = DateTime.Today;
                DateTime expectedDueDate = startDate.AddMonths(loanTerm); // Expected due date

                // Validate if the due date matches the selected loan term
                int actualMonths = ((dueDate.Year - startDate.Year) * 12) + (dueDate.Month - startDate.Month);

                if (actualMonths != loanTerm)
                {
                    Console.WriteLine($"[WARNING] Selected LoanTerm: {loanTerm} months, but DueDate suggests {actualMonths} months.");
                }

                // Compute values
                decimal principal = loanAmt / loanTerm;
                decimal interest = principal * 0.05m;
                decimal insurance = interest * 0.01m;

                // Create loan detail object
                var loanRecord = new
                {
                    CustomerNo = customerNo,
                    LoanAmount = loanAmt,
                    LoanTerm = loanTerm,  // Use the original loanTerm selected by the user
                    DueDate = expectedDueDate, // Use expected due date
                    Principal = principal,
                    Interest = interest,
                    Insurance = insurance
                };

                // Convert to JSON
                var json = JsonConvert.SerializeObject(loanRecord);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Add Bearer Token for authentication
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Send POST request to your API
                var response = await _httpClient.PostAsync("https://localhost:7159/api/customer/create-loan", content);

                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("[INFO] Loan details saved successfully.");
                    return true;
                }
                else
                {
                    Console.WriteLine($"[ERROR] Failed to save loan details. Status Code: {response.StatusCode}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EXCEPTION] Failed to send loan details: {ex}");
                return false;
            }
        }

    }
}
