﻿using AutoMapper;
using Dental_Clinic_System.Helper;
using Dental_Clinic_System.Models.Data;
using Dental_Clinic_System.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using NuGet.Common;
using System.Globalization;
using System.Security.Claims;
using System.Text.Encodings.Web;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Dental_Clinic_System.Controllers
{
    public class AccountController : Controller
    {
        private readonly DentalClinicDbContext _context;
        private readonly IMapper _mapper;
        private readonly IEmailSender _emailSender;

        public AccountController(DentalClinicDbContext context, IMapper mapper, IEmailSender emailSender)
        {
            _context = context;
            _mapper = mapper;
            _emailSender = emailSender;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterVM model)
        {
            if (ModelState.IsValid)
            {

                var patient = _context.Accounts.SingleOrDefault(p => p.Username == model.Username);
                if (patient != null)
                {
                    if (patient.Email == model.Email)
                    {
                        ModelState.AddModelError("errorRegister", "Email đã tồn tại");
                        return View();
                    }
                    if (patient.PhoneNumber == model.PhoneNumber)
                    {
                        ModelState.AddModelError("errorRegister", "Số điện thoại đã tồn tại");
                        return View();
                    }
                    ModelState.AddModelError("errorRegister", "Tên đăng nhập đã tồn tại");
                    return View();
                }

                var checkEmail = _context.Accounts.SingleOrDefault(p => p.Email == model.Email);
                if (checkEmail != null)
                {
                    ModelState.AddModelError("errorRegister", "Email đã tồn tại");
                    return View();
                }

                var checkPhoneNumber = _context.Accounts.SingleOrDefault(p => p.PhoneNumber == model.PhoneNumber);
                if (checkPhoneNumber != null)
                {
                    ModelState.AddModelError("errorRegister", "Số điện thoại đã tồn tại");
                    return View();
                }

                // Manual Mapping of Specific Values OR patient = _mapper.Map<Account>(model); for ALL Values
                patient = new Account
                {
                    Username = model.Username,
                    Password = DataEncryptionExtensions.ToMd5Hash(model.Password),
                    Email = null,
                    PhoneNumber = null,
                    AccountStatus = "Chưa Kích Hoạt", // Account is inactive until email is confirmed
                    Role = "Bệnh Nhân"
                };

                _context.Add(patient);
                await _context.SaveChangesAsync();

                var code = Guid.NewGuid().ToString(); // Generate a unique code for confirmation register
                var confirmationLink = Url.Action("ConfirmEmail", "Account", new { code = code, username = DataEncryptionExtensions.Encrypt(model.Username), email = DataEncryptionExtensions.Encrypt(model.Email), phonenumber = DataEncryptionExtensions.Encrypt(model.PhoneNumber) }, HttpContext.Request.Scheme);

                // Send confirmation email
                await _emailSender.SendEmailAsync(model.Email, "Xác nhận email", confirmationLink);

                // Debugging: Print all claims to console
                //foreach (var claim in ClaimsHelper.GetCurrentClaims(HttpContext.User))
                //{
                //    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                //}

                return RedirectToAction("Index", "Home");
                //return RedirectToAction("ConfirmEmail", "Account");
            }

            // Log ModelState errors and going so far by this stop which you got bugs LOL
            //foreach (var state in ModelState)
            //{
            //    foreach (var error in state.Value.Errors)
            //    {
            //        Console.WriteLine($"Property: {state.Key}, Error: {error.ErrorMessage}");
            //    }
            //}
            Console.WriteLine("Not valid at Register (POST)!");

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(string code, string username, string email, string phonenumber)
        {
            username = DataEncryptionExtensions.Decrypt(username);
            email = DataEncryptionExtensions.Decrypt(email);
            phonenumber = DataEncryptionExtensions.Decrypt(phonenumber);

            var user = _context.Accounts.FirstOrDefault(u => u.Username == username);
            if (user == null || code.IsNullOrEmpty())
            {
                Console.WriteLine("Error At ConfirmEmail (GET)");
                return RedirectToAction("Index", "Home");
            }

            user.AccountStatus = "Hoạt Động";
            user.Email = email;
            user.PhoneNumber = phonenumber;
            _context.Update(user);
            await _context.SaveChangesAsync();

            //return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpGet]
        public IActionResult Login(string? ReturnUrl)
        {
            string culture = "or-IN";
            Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)), new CookieOptions()
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1)
            });
            CultureInfo.CurrentCulture = new CultureInfo(culture);

            if (Request.Cookies.TryGetValue("RememberMeCredentials", out string rememberMeValue))
            {
                var values = rememberMeValue.Split('|');
                if (values.Length == 2)
                {
                    ViewBag.RememberMeUsername = values[0];
                    ViewBag.RememberMePassword = values[1];
                }
            }

            if (TempData.ContainsKey("LoginFlag"))
            {
                ViewBag.LoginFlag = TempData["LoginFlag"];
            }
            ViewBag.ReturnUrl = ReturnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM model, string? ReturnUrl)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (ModelState.IsValid)
            {
                var patient = _context.Accounts.SingleOrDefault(p => p.Username == model.Username);
                if (patient == null)
                {
                    ModelState.AddModelError("errorLogin", "Sai thông tin đăng nhập");
                }
                else
                {
                    switch (patient.AccountStatus.ToUpper())
                    {
                        case "BỊ KHÓA":
                            ModelState.AddModelError("errorLogin", "Tài khoản đã bị khóa");
                            ModelState.AddModelError("errorLoginSolution", "Vui lòng liên hệ với Support qua email - support@gmail.com");
                            return View();
                        case "BANNED":
                            ModelState.AddModelError("errorLogin", "Tài khoản đã bị khóa");
                            ModelState.AddModelError("errorLoginSolution", "Vui lòng liên hệ với Support qua email - support@gmail.com");
                            return View();
                        case "CHƯA KÍCH HOẠT":
                            ModelState.AddModelError("errorLogin", "Tài khoản chưa kích hoạt");
                            ModelState.AddModelError("errorLoginSolution", "Vui lòng kiểm tra email của bạn để được kích hoạt");
                            return View();
                        case "NOT ACTIVE":
                            ModelState.AddModelError("errorLogin", "Tài khoản chưa kích hoạt");
                            ModelState.AddModelError("errorLoginSolution", "Vui lòng kiểm tra email của bạn để được kích hoạt");
                            return View();
                    }
                    if (Helper.DataEncryptionExtensions.ToMd5Hash(model.Password) != patient.Password)
                    {
                        ModelState.AddModelError("errorLogin", "Sai thông tin đăng nhập");
                    }
                    else
                    {
                        var claims = new List<Claim> {
                            new Claim(ClaimTypes.NameIdentifier, patient.ID.ToString()),
                                new Claim(ClaimTypes.Email, patient.Email),
                                new Claim(ClaimTypes.Role, "Bệnh Nhân"),
                                new Claim(ClaimTypes.Name, patient.Email) // Ensure Name claim is added
                            };

                        // Add non-null claims using the utility method
                        claims.AddClaimIfNotNull(ClaimTypes.GivenName, patient.LastName);
                        claims.AddClaimIfNotNull(ClaimTypes.Surname, patient.FirstName);
                        claims.AddClaimIfNotNull(ClaimTypes.Gender, patient.Gender);
                        claims.AddClaimIfNotNull(ClaimTypes.StreetAddress, patient.Address);
                        claims.AddClaimIfNotNull(ClaimTypes.MobilePhone, patient.PhoneNumber);
                        claims.AddClaimIfNotNull(ClaimTypes.DateOfBirth, patient.DateOfBirth.ToString());

                        await ClaimsHelper.AddNewClaimsAsync(HttpContext, claims);

                        if (model.RememberMe)
                        {
                            Response.Cookies.Append("RememberMeCredentials", $"{model.Username}|{model.Password}", new CookieOptions
                            {
                                Expires = DateTime.UtcNow.AddDays(30),
                                HttpOnly = true,
                                Secure = true,
                                SameSite = SameSiteMode.None
                            });
                        }
                        else
                        {
                            // Optionally, remove the "RememberMe" cookie
                            Response.Cookies.Delete("RememberMeCredentials");
                        }

                        // Debugging: Print all claims to console
                        //foreach (var claim in ClaimsHelper.GetCurrentClaims(HttpContext.User))
                        //{
                        //    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                        //}

                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
            }
            return View();
        }

        public async Task LoginByGoogle()
        {
            await HttpContext.ChallengeAsync(GoogleDefaults.AuthenticationScheme, new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleResponse")
            });
        }

        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (result?.Principal?.Identities == null)
            {
                // Handle the error when authentication fails
                return RedirectToAction("Login", "Account");
            }

            var claims = result.Principal.Identities.FirstOrDefault().Claims.Select(claim => new
            {
                claim.Issuer,
                claim.OriginalIssuer,
                claim.Type,
                claim.Value
            }).ToList();



            var emailClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);
            if (emailClaim != null)
            {
                var email = emailClaim.Value;
                var user = await _context.Accounts.SingleOrDefaultAsync(u => u.Email == email);

                if (user == null)
                {
                    string randomKey = Util.GenerateRandomKey();
                    var dateOfBirthClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.DateOfBirth)?.Value;

                    // Parse the Date of Birth claim value
                    DateOnly? dateOfBirth = null;
                    if (DateOnly.TryParse(dateOfBirthClaim, out var parsedDateOfBirth))
                    {
                        dateOfBirth = parsedDateOfBirth;
                    }
                    // User does not exist in the database, add new one
                    var newUser = new Account
                    {
                        Email = email,
                        AccountStatus = "Hoạt Động",
                        Role = "Bệnh Nhân",
                        Username = DataEncryptionExtensions.ToSHA256Hash(email, randomKey),
                        Password = DataEncryptionExtensions.ToMd5Hash(randomKey, randomKey),
                        // Get other information from claim
                        FirstName = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
                        LastName = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                        IsLinked = null
                    };

                    _context.Accounts.Add(newUser);
                    await _context.SaveChangesAsync();
                    user = newUser;
                }

                if (user.IsLinked is false)
                {
                    Console.WriteLine("GET HERE!!!");
                    return RedirectToAction("LinkWithGoogleView", "account", new { emailLinked = DataEncryptionExtensions.Encrypt(email) });
                }

                if(user.IsLinked is true)
                {
                    // Get identity first
                    var identity = result.Principal.Identities.FirstOrDefault();

                    if (identity != null)
                    {
                        // Create a list to store updated claims
                        var updatedGoogleClaims = new List<Claim>();

                        // Iterate over existing claims and update the issuer
                        foreach (var claim in identity.Claims)
                        {
                            var newClaim = new Claim(
                                claim.Type,
                                claim.Value,
                                claim.ValueType,
                                "IsLinkedWithGoogle", // New issuer value
                                claim.OriginalIssuer
                            );
                            updatedGoogleClaims.Add(newClaim);
                        }

                        // Remove old claims and add updated claims
                        foreach (var claim in identity.Claims.ToList())
                        {
                            identity.RemoveClaim(claim);
                        }

                        foreach (var newClaim in updatedGoogleClaims)
                        {
                            identity.AddClaim(newClaim);
                        }
                    }
                }

                // Populate the PatientVM view model

                var updatedClaims = ClaimsHelper.GetCurrentClaims(User);

                // Update or add new claims
                updatedClaims.AddOrUpdateClaim(ClaimTypes.NameIdentifier, user.ID.ToString());
                updatedClaims.AddOrUpdateClaim(ClaimTypes.Role, "Bệnh Nhân");
                updatedClaims.AddOrUpdateClaimForLinkWithGoogle(ClaimTypes.GivenName, user.LastName);
                updatedClaims.AddOrUpdateClaimForLinkWithGoogle(ClaimTypes.Surname, user.FirstName);
                updatedClaims.AddOrUpdateClaim(ClaimTypes.MobilePhone, user.PhoneNumber);
                updatedClaims.AddOrUpdateClaim(ClaimTypes.StreetAddress, user.Address);
                updatedClaims.AddOrUpdateClaim(ClaimTypes.Gender, user.Gender);
                updatedClaims.AddOrUpdateClaim(ClaimTypes.DateOfBirth, user.DateOfBirth.ToString());

                await ClaimsHelper.UpdateClaimsAsync(HttpContext, updatedClaims);


                // Store the PatientVM in TempData to pass it to the next request
                //TempData["PatientVM"] = JsonConvert.SerializeObject(patientVM);

                // User login
                var claimsIdentity = new ClaimsIdentity(result.Principal.Identities.First().Claims, CookieAuthenticationDefaults.AuthenticationScheme);
                // Ensure the identity has a unique Name claim
                if (!claimsIdentity.HasClaim(c => c.Type == ClaimTypes.Name))
                {
                    claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, email));
                }
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
            }

            ViewBag.IsGoogleUser = true;
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public async Task<IActionResult> LinkWithGoogleView(string emailLinked)
        {
            ViewBag.UserLinked = emailLinked;
            return View();
        }

        [HttpGet("link-with-google/{emailLinked}")]
        public async Task<IActionResult> LinkWithGoogle(string emailLinked)
        {
            // If user agree to link with their account with google
            var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            emailLinked = DataEncryptionExtensions.Decrypt(emailLinked);
            var user = await _context.Accounts.SingleOrDefaultAsync(u => u.Email == emailLinked);

            if (user == null)
            {
                return NotFound();
            }

            // Get identity first
            var identity = result.Principal.Identities.FirstOrDefault();

            if (identity != null)
            {
                // Create a list to store updated claims
                var updatedGoogleClaims = new List<Claim>();

                // Iterate over existing claims and update the issuer
                foreach (var claim in identity.Claims)
                {
                    var newClaim = new Claim(
                        claim.Type,
                        claim.Value,
                        claim.ValueType,
                        "IsLinkedWithGoogle", // New issuer value
                        claim.OriginalIssuer
                    );
                    updatedGoogleClaims.Add(newClaim);
                }

                // Remove old claims and add updated claims
                foreach (var claim in identity.Claims.ToList())
                {
                    identity.RemoveClaim(claim);
                }

                foreach (var newClaim in updatedGoogleClaims)
                {
                    identity.AddClaim(newClaim);
                }
            }

            // Copyright from GoogleResponse method

            var updatedClaims = ClaimsHelper.GetCurrentClaims(User);

            // Update or add new claims
            updatedClaims.AddOrUpdateClaim(ClaimTypes.NameIdentifier, user.ID.ToString());
            updatedClaims.AddOrUpdateClaim(ClaimTypes.Role, "Bệnh Nhân");
            updatedClaims.AddOrUpdateClaim(ClaimTypes.MobilePhone, user.PhoneNumber);
            updatedClaims.AddOrUpdateClaimForLinkWithGoogle(ClaimTypes.GivenName, user.LastName);
            updatedClaims.AddOrUpdateClaimForLinkWithGoogle(ClaimTypes.Surname, user.FirstName);
            updatedClaims.AddOrUpdateClaim(ClaimTypes.StreetAddress, user.Address);
            updatedClaims.AddOrUpdateClaim(ClaimTypes.Gender, user.Gender);
            updatedClaims.AddOrUpdateClaim(ClaimTypes.DateOfBirth, user.DateOfBirth.ToString());

            await ClaimsHelper.UpdateClaimsAsync(HttpContext, updatedClaims);

            // User login
            var claimsIdentity = new ClaimsIdentity(result.Principal.Identities.First().Claims, CookieAuthenticationDefaults.AuthenticationScheme);
            // Ensure the identity has a unique Name claim
            if (!claimsIdentity.HasClaim(c => c.Type == ClaimTypes.Name))
            {
                claimsIdentity.AddClaim(new Claim(ClaimTypes.Name, user.Email));
            }

            // Set IsLinked to TRUE
            user.IsLinked = true;
            _context.Update(user);
            await _context.SaveChangesAsync();

            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "Bệnh Nhân")]
        [HttpGet]
        public IActionResult Profile()
        {

            // Extract the Date of Birth claim value
            var dateOfBirthClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.DateOfBirth)?.Value;

            // Parse the Date of Birth claim value
            DateOnly? dateOfBirth = null;
            if (DateOnly.TryParse(dateOfBirthClaim, out var parsedDateOfBirth))
            {
                dateOfBirth = parsedDateOfBirth;
            }

            // Prefill the model with data from claims or other sources
            var model = new PatientVM
            {
                FirstName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname)?.Value,
                LastName = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value,
                PhoneNumber = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.MobilePhone)?.Value,
                Email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                Gender = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Gender)?.Value,
                Address = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.StreetAddress)?.Value,
                DateOfBirth = dateOfBirth
            };

            return View(model);
        }


        [HttpPost]
        public async Task<IActionResult> Profile(PatientVM model)
        {
            var emailClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            if (ModelState.IsValid)
            {
                var user = await _context.Accounts
                    .FirstOrDefaultAsync(u => u.Email == emailClaim);

                if (user == null)
                {
                    await Console.Out.WriteLineAsync("ERROR HERE POST");
                    return NotFound();
                }



                // Check if email already exists
                var emailExists = await _context.Accounts
                    .AnyAsync(u => u.Email == model.Email && u.Email != emailClaim);
                if (emailExists)
                {
                    TempData["EmailError"] = "Email đã tồn tại.";
                    model.Email = user.Email; // Reset to the current email
                    return View(model);
                }

                // Check if phone number already exists
                var phoneExists = await _context.Accounts
                    .AnyAsync(u => u.PhoneNumber == model.PhoneNumber && u.PhoneNumber != user.PhoneNumber);
                if (phoneExists && !model.PhoneNumber.IsNullOrEmpty())
                {
                    TempData["PhoneNumberError"] = "Số điện thoại đã tồn tại";
                    model.PhoneNumber = user.PhoneNumber; // Reset to the current phonenumber
                    return View(model);
                }

                user.FirstName = model.FirstName;
                user.LastName = model.LastName;
                user.PhoneNumber = model.PhoneNumber;
                user.Gender = model.Gender;
                user.Email = model.Email;
                user.Address = model.Address;
                user.DateOfBirth = model.DateOfBirth;


                var updatedClaims = ClaimsHelper.GetCurrentClaims(User);

                // Update or add new claims
                updatedClaims.AddOrUpdateClaim(ClaimTypes.GivenName, model.LastName);
                updatedClaims.AddOrUpdateClaim(ClaimTypes.Surname, model.FirstName);
                updatedClaims.AddOrUpdateClaim(ClaimTypes.Gender, model.Gender);
                updatedClaims.AddOrUpdateClaim(ClaimTypes.StreetAddress, model.Address);
                updatedClaims.AddOrUpdateClaim(ClaimTypes.DateOfBirth, model.DateOfBirth.ToString());

                // Ensure Name claim is present
                updatedClaims.EnsureNameClaim(ClaimTypes.Name, user.Email);

                await ClaimsHelper.UpdateClaimsAsync(HttpContext, updatedClaims);

                // Debugging: Print all claims to console
                //foreach (var claim in ClaimsHelper.GetCurrentClaims(HttpContext.User))
                //{
                //    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
                //}

                await _context.SaveChangesAsync();
            }
            else
            {
                await Console.Out.WriteLineAsync("Null at Profile (POST)");
            }

            return View(model);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordVM model)
        {
            int id = Int32.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value);
            var user = await _context.Accounts
                .FirstOrDefaultAsync(u => u.ID == id);



            if (user == null)
            {
                TempData["ChangePasswordMessageFailed"] = "Mật khẩu thay đổi thất bại.";
                return RedirectToAction("Profile", "Account");
            }

            if(DataEncryptionExtensions.ToMd5Hash(model.Password) != user.Password)
            {
                TempData["ChangePasswordMessageFailed"] = "Mật khẩu thay đổi thất bại.";
                return RedirectToAction("Profile", "Account");
            }

            if (model.NewPassword != model.ConfirmPassword)
            {
                TempData["ChangePasswordMessageFailed"] = "Mật khẩu mới và mật khẩu xác nhận không giống.";
                return RedirectToAction("Profile", "Account");
            }

            if (model.NewPassword != model.Password && model.NewPassword.Length <= 30)
            {
                user.Password = DataEncryptionExtensions.ToMd5Hash(model.NewPassword);
                _context.Accounts.Update(user);
                await _context.SaveChangesAsync();
                TempData["ChangePasswordMessageSuccessfully"] = "Mật khẩu thay đổi thành công.";
                return RedirectToAction("Profile", "Account");
            }
            else
            {
                TempData["ChangePasswordMessageFailed"] = "Mật khẩu thay đổi thất bại.";
                return RedirectToAction("Profile", "Account");
            }
        }



        [AllowAnonymous, HttpGet("forgot-password")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [AllowAnonymous, HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Accounts.SingleOrDefaultAsync(u => u.Email == model.Email);
                if (user == null)
                {
                    TempData["ForgotPasswordMessage"] = "User not found.";
                    return View(model);
                }

                var code = Guid.NewGuid().ToString(); // Generate a unique code for password reset
                var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.ID, code = code }, protocol: HttpContext.Request.Scheme);

                await _emailSender.SendEmailAsync(model.Email, "Reset Password",
                    $"Vui lòng click vào link sau để đặt lại mật khẩu <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>here</a>.");

                TempData["ForgotPasswordMessage"] = "Liên kết đặt lại mật khẩu đã được gửi đến email của bạn.";
                return RedirectToAction("ForgotPasswordConfirmation");
            }
            return View(model);
        }

        [AllowAnonymous]
        [HttpGet("reset-password")]
        public IActionResult ResetPassword(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest("Phải cung cấp mã để đặt lại mật khẩu.");
            }
            var model = new ResetPasswordForForgotVM { UserId = userId, Code = code };
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordForForgotVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Accounts.FindAsync(model.UserId);
            if (user == null)
            {
                TempData["ResetPasswordMessage"] = "Người dùng không tồn tại";
                return View(model);
            }

            user.Password = DataEncryptionExtensions.ToMd5Hash(model.Password, model.Password);
            _context.Accounts.Update(user);
            await _context.SaveChangesAsync();

            TempData["ResetPasswordMessage"] = "Mật khẩu đã đặt lại thành công";
            return RedirectToAction("ResetPasswordConfirmation");
        }

        [AllowAnonymous]
        [HttpGet("reset-password-confirmation")]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpGet("forgot-password-confirmation")]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }
    }
}