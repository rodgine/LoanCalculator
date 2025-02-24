using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LoanAPI.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LoanAPI.Data;
using System.Text.Json.Serialization;
using System.Text.Json;
using LoanCalculator.API.Models;
using Microsoft.AspNetCore.Authorization;

[Authorize]
[Route("api/customer")]
[ApiController]
public class APICustomerController : ControllerBase
{
    private readonly LoanDbContext _context;

    public APICustomerController(LoanDbContext context)
    {
        _context = context;
    }

    // Insert into CustomerTbl with UnitID (linked to InventoryTbl)
    [HttpPost("create-customer")]
    public async Task<IActionResult> CreateCustomer([FromBody] CustomerTbl customer)
    {
        if (customer == null)
            return BadRequest("Invalid request.");

        // Check if the UnitID exists in InventoryTbl before inserting
        var inventory = await _context.InventoryTbl.FindAsync(customer.UnitID);
        if (inventory == null)
            return BadRequest("Invalid UnitID. Please use an existing UnitID from InventoryTbl.");

        try
        {
            _context.CustomerTbl.Add(customer);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomerById), new { id = customer.CustomerNo }, customer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    // Insert into CustomerDtl with valid CustomerNo
    [HttpPost("create-customer-detail")]
    public async Task<IActionResult> CreateCustomerDetail([FromBody] CustomerDtl customerDtl)
    {
        if (customerDtl == null)
            return BadRequest("Invalid request.");

        // Check if the CustomerNo exists before inserting
        var existingCustomer = await _context.CustomerTbl.FindAsync(customerDtl.CustomerNo);
        if (existingCustomer == null)
            return BadRequest("CustomerNo does not exist in CustomerTbl.");

        try
        {
            _context.CustomerDtl.Add(customerDtl);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetCustomerDetailById), new { id = customerDtl.Id }, customerDtl);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    // Fetch a single customer by ID
    [HttpGet("get-customer/{id}")]
    public async Task<IActionResult> GetCustomerById(int id)
    {
        var customer = await _context.CustomerTbl.FindAsync(id);
        if (customer == null)
            return NotFound("Customer not found.");
        return Ok(customer);
    }

    // Fetch a single customer detail by ID
    [HttpGet("get-customer-detail/{id}")]
    public async Task<IActionResult> GetCustomerDetailById(int id)
    {
        var customerDtl = await _context.CustomerDtl.FindAsync(id);
        if (customerDtl == null)
            return NotFound("Customer detail not found.");
        return Ok(customerDtl);
    }

    [HttpGet("get-all-customers")]
    public async Task<IActionResult> GetAllCustomers()
    {
        var customers = await (from c in _context.CustomerTbl
                               join d in _context.CustomerDtl on c.CustomerNo equals d.CustomerNo
                               join i in _context.InventoryTbl on c.UnitID equals i.UnitID
                               select new
                               {
                                   c.CustomerNo,
                                   c.LastName,
                                   c.FirstName,
                                   c.MiddleName,
                                   c.UnitID,
                                   d.EquityTerm,
                                   d.EquityAmount,
                                   d.MATerm,
                                   d.MAAmount,
                                   d.MIR,
                                   d.FIRE,
                                   d.LoanAmt,
                                   d.IntRate,
                                   i.Type,
                                   i.HousePrice,
                                   i.LotPrice
                               }).ToListAsync();

        // Use System.Text.Json to format output correctly
        var jsonOptions = new JsonSerializerOptions
        {
            ReferenceHandler = ReferenceHandler.IgnoreCycles, // Prevents circular references
            WriteIndented = true // Optional: makes JSON pretty
        };

        return new JsonResult(customers, jsonOptions);
    }


    // Fetch all customer details
    [HttpGet("get-all-customer-details")]
    public async Task<IActionResult> GetAllCustomerDetails()
    {
        var customerDetails = await _context.CustomerDtl.ToListAsync();
        return Ok(customerDetails);
    }

    // Fetch all inventory records
    [HttpGet("get-all-inventories")]
    public async Task<IActionResult> GetAllInventories()
    {
        var inventories = await _context.InventoryTbl.ToListAsync();
        return Ok(inventories);
    }

    // Fetch all related data for a specific customer (Customer, Customer Details, Inventory Info)
    [HttpGet("get-customer-full/{id}")]
    public async Task<IActionResult> GetCustomerFullData(int id)
    {
        var customer = await _context.CustomerTbl
            .Include(c => c.CustomerDtl)  // Ensure your CustomerTbl model has a navigation property for CustomerDtl
            .FirstOrDefaultAsync(c => c.CustomerNo == id);

        if (customer == null)
            return NotFound("Customer not found.");

        var inventory = await _context.InventoryTbl.FindAsync(customer.UnitID);

        var response = new
        {
            Customer = customer,
            CustomerDetails = customer.CustomerDtl,
            Inventory = inventory
        };

        return Ok(response);
    }

    // Update CustomerTbl
    [HttpPut("update-customer/{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerTbl updatedCustomer)
    {
        if (updatedCustomer == null || id != updatedCustomer.CustomerNo)
            return BadRequest("Invalid request.");

        var existingCustomer = await _context.CustomerTbl.FindAsync(id);
        if (existingCustomer == null)
            return NotFound("Customer not found.");

        existingCustomer.LastName = updatedCustomer.LastName;
        existingCustomer.FirstName = updatedCustomer.FirstName;
        existingCustomer.MiddleName = updatedCustomer.MiddleName;
        existingCustomer.UnitID = updatedCustomer.UnitID;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(existingCustomer);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    // Update CustomerDtl
    [HttpPut("update-customer-detail/{id}")]
    public async Task<IActionResult> UpdateCustomerDetail(int id, [FromBody] CustomerDtl updatedDetail)
    {
        if (updatedDetail == null || id != updatedDetail.CustomerNo)
            return BadRequest("Invalid request.");

        var existingDetail = await _context.CustomerDtl.FindAsync(id);
        if (existingDetail == null)
            return NotFound("Customer detail not found.");

        existingDetail.EquityTerm = updatedDetail.EquityTerm;
        existingDetail.EquityAmount = updatedDetail.EquityAmount;
        existingDetail.MATerm = updatedDetail.MATerm;
        existingDetail.MAAmount = updatedDetail.MAAmount;
        existingDetail.MIR = updatedDetail.MIR;
        existingDetail.FIRE = updatedDetail.FIRE;
        existingDetail.LoanAmt = updatedDetail.LoanAmt;
        existingDetail.IntRate = updatedDetail.IntRate;

        try
        {
            await _context.SaveChangesAsync();
            return Ok(existingDetail);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    // Delete Customer from CustomerTbl
    [HttpDelete("delete-customer/{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        var customer = await _context.CustomerTbl.FindAsync(id);
        if (customer == null)
            return NotFound("Customer not found.");

        _context.CustomerTbl.Remove(customer);

        try
        {
            await _context.SaveChangesAsync();
            return Ok($"Customer with ID {id} deleted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }

    // Delete Customer Detail from CustomerDtl
    [HttpDelete( "delete-customer-detail/{id}")]
    public async Task<IActionResult> DeleteCustomerDetail(int id)
    {
        var customerDetail = await _context.CustomerDtl.FindAsync(id);
        if (customerDetail == null)
            return NotFound("Customer detail not found.");

        _context.CustomerDtl.Remove(customerDetail);

        try
        {
            await _context.SaveChangesAsync();
            return Ok($"Customer detail with ID {id} deleted successfully.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal Server Error: {ex.Message}");
        }
    }


    // POST: api/loan/create-loan
    [HttpPost("create-loan")]
    public async Task<IActionResult> CreateLoan([FromBody] LoanDetail loanDetail)
    {
        if (loanDetail == null)
        {
            return BadRequest("Invalid loan details.");
        }

        try
        {
            _context.LoanDetails.Add(loanDetail);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Loan details saved successfully!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while saving loan details.", error = ex.Message });
        }
    }

    [HttpGet("get-all-loans")]
    public async Task<IActionResult> GetAllLoans()
    {
        var loans = await _context.LoanDetails.ToListAsync();

        var formattedLoans = loans.Select(loan => new
        {
            LoanId = loan.LoanId,
            CustomerNo = loan.CustomerNo,
            LoanAmount = Convert.ToDecimal(loan.LoanAmount),
            LoanTerm = loan.LoanTerm,
            DueDate = loan.DueDate.ToString("yyyy-MM-ddTHH:mm:ss"), // Ensure DateTime is serialized correctly
            Principal = Convert.ToDecimal(loan.Principal),
            Interest = Convert.ToDecimal(loan.Interest),
            Insurance = Convert.ToDecimal(loan.Insurance)
        });

        return Ok(formattedLoans);
    }

    // GET: api/loan/get-loan-by-id/{id}
    [HttpGet("get-loan-by-id/{id}")]
    public async Task<IActionResult> GetLoanById(int id)
    {
        var loan = await _context.LoanDetails.FindAsync(id);

        if (loan == null)
            return NotFound(new { message = "Loan not found." });

        return Ok(loan);
    }

}
