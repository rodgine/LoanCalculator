using Microsoft.AspNetCore.Mvc;
using LoanCalculator.Models;
using System.Net.Http;
using System.Text.Json;
using LoanCalculator.Services;

namespace LoanCalculator.Controllers
{
    public class CustomerController : Controller
    {
        private readonly LoanApiService _apiService;

        public CustomerController(LoanApiService apiService)
        {
            _apiService = apiService;
        }
        public async Task<IActionResult> Index()
        {
            //Retrieve Token from Session
            var token = HttpContext.Session.GetString("Token");

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Login", "Account"); // Redirect if not authenticated
            }

            var loanDetails = await _apiService.GetLoanDetailsAsync(token);
            return View(loanDetails);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerDetail(decimal LoanAmt, DateTime DueDate, int LoanTerm)
        {
            if (LoanAmt <= 0 || LoanTerm <= 0)
            {
                TempData["ErrorMessage"] = "Invalid loan amount or term.";
                return RedirectToAction("Index");
            }

            try
            {
                Console.WriteLine($"[INFO] Sending LoanAmt: {LoanAmt}, DueDate: {DueDate}, LoanTerm: {LoanTerm}");

                // ✅ Get Token from Session
                var token = HttpContext.Session.GetString("Token");

                if (string.IsNullOrEmpty(token))
                {
                    TempData["ErrorMessage"] = "Session expired. Please log in again.";
                    return RedirectToAction("Login", "Account");
                }

                var CustomerNo = 739664;

                // ✅ Pass Token to API Call
                bool isSuccess = await _apiService.CreateCustomerDetailAsync(LoanAmt, DueDate, LoanTerm, CustomerNo, token);

                if (isSuccess)
                {
                    TempData["SuccessMessage"] = "Customer details created successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid inputs. Make sure you fill up the form correctly!";
                    Console.WriteLine("[ERROR] API request failed. Possible validation error or server issue.");
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while processing the request.";
                Console.WriteLine($"[EXCEPTION] {ex}");
            }

            return RedirectToAction("Index");
        }

    }
}
