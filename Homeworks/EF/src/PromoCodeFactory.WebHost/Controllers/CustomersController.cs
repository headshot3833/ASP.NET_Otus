using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PromoCodeFactory.Core.Abstractions.Repositories;
using PromoCodeFactory.Core.Domain.PromoCodeManagement;
using PromoCodeFactory.WebHost.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PromoCodeFactory.WebHost.Controllers
{
    /// <summary>
    /// Клиенты
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class CustomersController
        : ControllerBase
    {
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Preference> _preferenceRepository;

        public CustomersController(IRepository<Customer> customerRepository, IRepository<Preference> preferenceRepository)
        {
            _customerRepository = customerRepository;
            _preferenceRepository = preferenceRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CustomerShortResponse>>> GetCustomersAsync()
        {
            // Дожидаемся выполнения Task
            var customers = await _customerRepository.GetAllAsync();

            // Теперь можно вызывать LINQ
            var response = customers.Select(c => new CustomerShortResponse
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                
            });

            return Ok(response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CustomerResponse>> GetCustomerAsync(Guid id)
        {
            var customer = await _customerRepository.GetByIdAsync(id);

            var response = new CustomerResponse
            {
                Email = customer.Email,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Id = id,
                PromoCodes = customer.PromoCodes?.Select(pc => new PromoCodeShortResponse
                {
                    BeginDate = pc.BeginDate.ToString(),
                    EndDate = pc.EndDate.ToString(),
                    Code = pc.Code,
                    Id = pc.Id,
                    PartnerName = pc.PartnerName,
                    ServiceInfo = pc.ServiceInfo,
                }).ToList()
            };
            return Ok(response);
        }

        [HttpPost]
        public async Task<IActionResult> CreateCustomerAsync(CreateOrEditCustomerRequest request)
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                CustomerPreferences = request.PreferenceIds
                        .Select(prefId => new CustomerPreference
                        {
                            PreferenceId = prefId
                        })
                        .ToList()
            };
            try
            {
                await _customerRepository.AddAsync(customer);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        /// <summary>
        /// Обновления кастомера
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> EditCustomersAsync(Guid id, CreateOrEditCustomerRequest request)
        {
            var customer = await _customerRepository.GetByIdAsync(id);
            if (customer == null)
                return NotFound($"Customer with id {id} not found");


            customer.FirstName = request.FirstName;
            customer.LastName = request.LastName;
            customer.Email = request.Email;


            var allPreferences = await _preferenceRepository.GetAllAsync();


            var selectedPreferences = allPreferences
                .Where(p => request.PreferenceIds.Contains(p.Id))
                .ToList();


            customer.CustomerPreferences.Clear();
            foreach (var pref in selectedPreferences)
            {
                customer.CustomerPreferences.Add(new CustomerPreference
                {
                    CustomerId = customer.Id,
                    PreferenceId = pref.Id
                });
            }

            // Сохраняем изменения
            await _customerRepository.UpdateAsync(customer);

            return Ok(customer);


        }

        [HttpDelete]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                await _customerRepository.Delete(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
    }
}