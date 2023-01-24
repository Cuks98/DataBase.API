using DataBase.API.Models;
using DataBaseAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using Wkhtmltopdf.NetCore;
using System.Text.Json;
using SautinSoft.Document;
using SautinSoft.Document.Drawing;
using SautinSoft.Document.Tables;

namespace DataBaseAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DataBaseController : ControllerBase
    {
        private readonly ILogger<DataBaseController> _logger;
        private readonly DataContext _context;
        private readonly IConfiguration _configuration;
        readonly IGeneratePdf _generatePdf;


        public DataBaseController(ILogger<DataBaseController> logger, DataContext dataContext, IConfiguration configuration, IGeneratePdf generatePdf)
        {
            _configuration = configuration;
            _logger = logger;
            _context = dataContext;
            _generatePdf = generatePdf;
        }

        

        [HttpGet("get-login")]
        public async Task<LoginResponce> Login(string email, string password)
        {
            var employees = await _context.Employees.ToListAsync();
            try
            {
                var employee = employees.Where(x => x.Email.Equals(email)).FirstOrDefault();
                if (employee == null) throw new Exception();
                using var sha256 = SHA256.Create();
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{password}{employee.salt}"));

                var sb = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    sb.Append(bytes[i].ToString("x2"));
                }
                password = sb.ToString();

                if (employee.Password != password) throw new Exception();

                string token = CreateToken(employee);

                var emoloyeResponce = new LoginResponce() { Employee = employee, Token = token };

                return emoloyeResponce;
            }
            catch (Exception ex)
            {
                return new LoginResponce();
            }
        }
        [HttpGet("get-users")]
        [Authorize(Roles = "1,2")]
        public async Task<UserResponse> GetUsers()
        {
            var users = await _context.Users.ToListAsync();

            if (users == null || users.Count == 0)
            {
                return new UserResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = "No users coul be found",
                    Users = new List<User> { }
                };
            }

            foreach (var user in users)
            {
                user.FillNotMappedData();
            }

            return new UserResponse()
            {
                ErrorId = null,
                ErrorMsg = "",
                Users = users
            };
        }

        [HttpGet("get-user")]
        [Authorize(Roles = "1,2")]
        public async Task<UserResponse> GetUser(int id)
        {
            var users = await _context.Users.ToListAsync();
            var user = users.Where(x => x.Id == id).FirstOrDefault();
            if (user == null)
            {
                return new UserResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = $"User with the id {id} does not exist in database.",
                    Users = new List<User>()
                };
            }
            return new UserResponse()
            {
                ErrorId = null,
                ErrorMsg = "",
                Users = new List<User>() { user }
            };
        }

        [HttpPost("register-new-user")]
        [Authorize(Roles = "1,2")]
        public async Task<UserResponse> RegisterNewUser(User user)
        {
            //var user = new User()
            //{
            //    FirstName = name,
            //    LastName = lastName,
            //    Email = email,
            //    FirstRegistered = DateTime.Today,
            //    DoB = new DateTime(yearOfBirth, monthOfBirth, dayOfBirth),
            //    RegisteredTo = DateTime.Today
            //};

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new UserResponse()
                {
                    ErrorId = 2,
                    ErrorMsg = $"Error while saving user to database. {ex}",
                    Users = new List<User>()
                };
            }
            return new UserResponse()
            {
                ErrorId = null,
                ErrorMsg = "",
                Users = new List<User> { user }
            };
        }

        [HttpPost("register-new-users")]
        [Authorize(Roles = "1,2")]
        public async Task<UserResponse> RegisterNewUsers(List<User> users)
        {
            foreach (var user in users)
            {
                _context.Add(user);
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new UserResponse()
                {
                    ErrorId = 2,
                    ErrorMsg = $"Error while saving user to database.",
                    Users = new List<User>()
                };
            }
            return new UserResponse() { ErrorId = null, ErrorMsg = "", Users = users };
        }

        [HttpPut("extend-membership")]
        [Authorize(Roles = "1,2")]
        public async Task<UserResponse> ExtendMembership(ExtendMembershipRequest extendMembershipRequest)
        {
            var users = await _context.Users.ToListAsync();
            var user = users.Where(x => x.Id == extendMembershipRequest.Id).FirstOrDefault();
            if (user == null)
            {
                return new UserResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = $"User with the id {extendMembershipRequest.Id} does not exist in database.",
                    Users = new List<User>()
                };
            }
            try
            {
                user.RegisteredTo = user.RegisteredTo.AddMonths(extendMembershipRequest.NumberOfMonths);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new UserResponse()
                {
                    ErrorId = 2,
                    ErrorMsg = $"Error while saving user to database.",
                    Users = new List<User>()
                };
            }
            return new UserResponse() { ErrorId = null, ErrorMsg = "", Users = new List<User> { user } };
        }

        [HttpDelete("remove-user")]
        [Authorize(Roles = "1,2")]
        public async Task<UserResponse> RemoveUser(int id)
        {
            var users = await _context.Users.ToListAsync();
            var user = users.Where(x => x.Id == id).FirstOrDefault();
            if (user == null)
            {
                return new UserResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = $"User with the id {id} does not exist in database.",
                    Users = new List<User>()
                };
            }
            try
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return new UserResponse()
                {
                    ErrorId = 2,
                    ErrorMsg = $"Error while saving user to database. {ex}",
                    Users = new List<User>()
                };
            }
            return new UserResponse() { ErrorId = null, ErrorMsg = "", Users = new List<User> { user } };
        }


        

        [HttpGet("get-employees")]
        [Authorize(Roles = "2")]
        public async Task<EmployeeResponse> GetEmployees()
        {
            var employees = await _context.Employees.ToListAsync();

            return new EmployeeResponse() { ErrorId = null, ErrorMsg = "", Employees = employees };
        }

        [HttpGet("get-employee")]
        [Authorize(Roles = "2")]
        public async Task<EmployeeResponse> GetEmployee(int id)
        {
            var employees = await _context.Employees.ToListAsync();
            var employee = employees.Where(x => x.Id == id).FirstOrDefault();

            if (employee == null)
            {
                return new EmployeeResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = $"Employee with id {id} does not exist in database",
                    Employees = employees
                };
            }

            return new EmployeeResponse() { ErrorId = null, ErrorMsg = "", Employees = new List<Employee>() { employee } };
        }

        [HttpPost("register-new-employee")]
        [Authorize(Roles = "2")]
        public async Task<EmployeeResponse> RegisterNewEmployeeAsync(Employee newEmployee)
        {
            var s = newEmployee.Password;
            
            
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes($"{s}{newEmployee.salt}"));

            var sb = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
            {
                sb.Append(bytes[i].ToString("x2"));
            }
            newEmployee.Password = sb.ToString();
            
            
            try
            {
                _context.Employees.Add(newEmployee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new EmployeeResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = "Error while saving data to database",
                    Employees = new List<Employee>()
                };
            }
            return new EmployeeResponse() { ErrorId = null, ErrorMsg = "", Employees = new List<Employee>() { newEmployee } };
        }

        [HttpPut("update-employee")]
        [Authorize(Roles = "2")]
        public async Task<EmployeeResponse> UpdateEmployee(Employee employee)
        {
            try
            {
                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new EmployeeResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = "Error while saving data to database",
                    Employees = new List<Employee>()
                };
            }
            return new EmployeeResponse() { ErrorId = null, ErrorMsg = "", Employees = new List<Employee>() { employee } };
        }

        [HttpDelete("remove-employee")]
        [Authorize(Roles = "2")]
        public async Task<EmployeeResponse> RemoveEmployee(int id)
        {
            var employees = await _context.Employees.ToListAsync();
            var employee = employees.Where(x => x.Id == id).FirstOrDefault();
            if (employee == null)
            {
                return new EmployeeResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = $"User with the id {id} does not exist in database.",
                    Employees = new List<Employee>()
                };
            }
            try
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new EmployeeResponse()
                {
                    ErrorId = 1,
                    ErrorMsg = $"Error saving data to database.",
                    Employees = new List<Employee>()
                };
            }
            return new EmployeeResponse() { ErrorId = null, ErrorMsg = "", Employees = new List<Employee>() { employee } };

        }




        //[HttpGet("get-trainers")]
        //[Authorize(Roles = "1,2")]
        //public async Task<TrainerResponse> GetTrainers()
        //{
        //    var trainers = await _context.Trainers.ToListAsync();
        //    return new TrainerResponse() { ErrorId = null, ErrorMsg = "", Trainers = trainers };
        //}

        //[HttpGet("get-trainer")]
        //[Authorize(Roles = "1,2")]
        //public async Task<TrainerResponse> GetTrainer(int id)
        //{
        //    var trainers = await _context.Trainers.ToListAsync();
        //    var trainer = trainers.Where(x => x.Id == id).FirstOrDefault();
        //    if (trainer == null)
        //    {
        //        return new TrainerResponse()
        //        {
        //            ErrorId = 1,
        //            ErrorMsg = $"User with the id {id} does not exist in database.",
        //            Trainers = new List<Trainer>()
        //        };
        //    }
        //    return new TrainerResponse()
        //    {
        //        ErrorId = null,
        //        ErrorMsg = "",
        //        Trainers = new List<Trainer>() { trainer }
        //    };
        //}

        //[HttpPost("register-new-trainer")]
        //[Authorize(Roles = "1,2")]
        //public async Task<TrainerResponse> RegisterNewTrainer(Trainer trainer)
        //{
        //    try
        //    {
        //        _context.Trainers.Add(trainer);
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        return new TrainerResponse()
        //        {
        //            ErrorId = 1,
        //            ErrorMsg = $"Error saving data to database.",
        //            Trainers = new List<Trainer>()
        //        };
        //    }
        //    return new TrainerResponse() { ErrorId = null, ErrorMsg = "", Trainers = new List<Trainer>() { trainer } };
        //}

        //[HttpPut("update-trainer")]
        //[Authorize(Roles = "1,2")]
        //public async Task<TrainerResponse> UpdateTrainer(Trainer trainerReq)
        //{
        //    //var trainers = await _context.Trainers.ToListAsync();
        //    //var trainer = trainers.Where(x => x.Id == trainerReq.Id).FirstOrDefault();

        //    try
        //    {
        //        _context.Trainers.Update(trainerReq);
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        return new TrainerResponse()
        //        {
        //            ErrorId = 1,
        //            ErrorMsg = $"Error saving data to database.",
        //            Trainers = new List<Trainer>()
        //        };
        //    }
        //    return new TrainerResponse() { ErrorId = null, ErrorMsg = "", Trainers = new List<Trainer>() { trainerReq } };
        //}

        //[HttpDelete("remove-trainer")]
        //[Authorize(Roles = "1,2")]
        //public async Task<TrainerResponse> RemoveTrainer(int id)
        //{
        //    var trainers = await _context.Trainers.ToListAsync();
        //    var trainer = trainers.Where(x => x.Id == id).FirstOrDefault();
        //    if (trainer == null)
        //    {
        //        return new TrainerResponse()
        //        {
        //            ErrorId = 1,
        //            ErrorMsg = $"User with the id {id} does not exist in database.",
        //            Trainers = new List<Trainer>()
        //        };
        //    }
        //    try
        //    {
        //        _context.Trainers.Remove(trainer);
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (Exception ex)
        //    {
        //        return new TrainerResponse()
        //        {
        //            ErrorId = 1,
        //            ErrorMsg = $"Error saving data to database.",
        //            Trainers = new List<Trainer>()
        //        };
        //    }
        //    return new TrainerResponse() { ErrorId = null, ErrorMsg = "", Trainers = new List<Trainer>() { trainer } };
        //}

        //[HttpGet("get-transaction-data")]
        //[Authorize(Roles = "1,2")]
        //public async Task<List<Transaction>> GetTransactionData()
        //{
        //    var filePath = @"C:\Users\filip.curin\source\repos\DataBase.API\DataBase.API\Data\TransactionData.json";
        //    var jsonData = System.IO.File.ReadAllText(filePath);
        //    var transactionList = JsonConvert.DeserializeObject<List<Transaction>>(jsonData) ?? new List<Transaction>();

        //    return transactionList;
        //}

        //[HttpGet("get-single-transaction-data")]
        //[Authorize(Roles = "1,2")]
        //public async Task<Transaction> GetSingleTransactionData(int id)
        //{
        //    var filePath = @"C:\Users\filip.curin\source\repos\DataBase.API\DataBase.API\Data\TransactionData.json";
        //    var jsonData = System.IO.File.ReadAllText(filePath);
        //    var transactionList = JsonConvert.DeserializeObject<List<Transaction>>(jsonData) ?? new List<Transaction>();

        //    var transaction = transactionList.Where(x=>x.Id == id).FirstOrDefault();

        //    if(transaction != null)
        //    {
        //        return transaction;

        //    }

        //    return null;
        //}

        //[HttpDelete("remove-transaction-data")]
        //[Authorize(Roles = "1,2")]
        //public async Task<bool> RemoveTransactionData(int id)
        //{
        //    var filePath = @"C:\Users\filip.curin\source\repos\DataBase.API\DataBase.API\Data\TransactionData.json";
        //    var jsonData = System.IO.File.ReadAllText(filePath);
        //    var transactionList = JsonConvert.DeserializeObject<List<Transaction>>(jsonData) ?? new List<Transaction>();

        //    var transactionToRemove = transactionList.Where(x => x.Id == id).FirstOrDefault();
        //    if(transactionToRemove != null)
        //    {
        //        transactionList.Remove(transactionToRemove);

        //        jsonData = JsonConvert.SerializeObject(transactionList);

        //        System.IO.File.WriteAllText(filePath, jsonData);

        //        return true;
        //    }
        //    return false;
        //}

        //[HttpPost("update-transaction-data")]
        //[Authorize(Roles = "1,2")]
        //public bool UpdateTransactionData(Transaction request)
        //{
        //    var filePath = @"C:\Users\filip.curin\source\repos\DataBase.API\DataBase.API\Data\TransactionData.json";
        //    var jsonData = System.IO.File.ReadAllText(filePath);
        //    var transactionList = JsonConvert.DeserializeObject<List<Transaction>>(jsonData) ?? new List<Transaction>();


        //    foreach(var transaction in transactionList)
        //    {
        //        if(transaction.Id == request.Id)
        //        {
        //            transaction.UserId = request.UserId;
        //            transaction.Date = request.Date;
        //            transaction.Amount = request.Amount;
        //        }
        //    }

        //    jsonData = JsonConvert.SerializeObject(transactionList);

        //    System.IO.File.WriteAllText(filePath, jsonData);

        //    return true;
        //}



        private string CreateToken(Employee employee)
        {
            string levelOfSecurita = employee.LevelOfSecurity.ToString();
            var claims = new[]
            {
                new Claim(ClaimTypes.Role, levelOfSecurita)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Key").Value));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(100),
                signingCredentials: cred);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        
        
    }
}
