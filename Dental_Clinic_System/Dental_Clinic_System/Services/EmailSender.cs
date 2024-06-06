﻿using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net.Mail;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Dental_Clinic_System.Models.Data;
using Microsoft.EntityFrameworkCore;

namespace Dental_Clinic_System.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<EmailSender> _logger;
        private readonly DentalClinicDbContext _context;

        public EmailSender(IConfiguration configuration, ILogger<EmailSender> logger, DentalClinicDbContext context)
        {
            _configuration = configuration;
            _logger = logger;
            _context = context;
        }

        //public async Task SendEmailAsync(string email, string subject, string message)
        //{
        //    // Get username from the email
        //    var user = _context.Accounts.FirstOrDefault(u => u.Email == email);
        //    string username = user?.Username;
        //    try
        //    {
        //        var smtpClient = new SmtpClient(_configuration["Email:Smtp:Host"])
        //        {
        //            Port = int.Parse(_configuration["Email:Smtp:Port"]),
        //            Credentials = new NetworkCredential(_configuration["Email:Smtp:Username"], _configuration["Email:Smtp:Password"]),
        //            EnableSsl = true,
        //        };

        //        var mailMessage = new MailMessage
        //        {
        //            From = new MailAddress(_configuration["Email:FromAddress"], _configuration["Email:FromName"]),
        //            Subject = subject,
        //            Body = message,
        //            IsBodyHtml = true,
        //        };

        //        mailMessage.To.Add(email);

        //        // Optional: Add additional headers to improve deliverability
        //        mailMessage.Headers.Add("X-Priority", "1");
        //        mailMessage.Headers.Add("X-MSMail-Priority", "High");
        //        mailMessage.Headers.Add("Importance", "High");

        //        await smtpClient.SendMailAsync(mailMessage);
        //        _logger.LogInformation($"Email sent to {email} with subject {subject}");
        //    }
        //    catch (SmtpException ex)
        //    {
        //        _logger.LogError(ex, $"Error sending email to {email} with subject {subject}");
        //        throw; // Re-throw the exception if you want the caller to handle it
        //    }
        //}

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            // Get username from the email
            var user = _context.Accounts.FirstOrDefault(u => u.Email == email);
            string username = user?.Username;
            try
            {
                var smtpClient = new SmtpClient(_configuration["Email:Smtp:Host"])
                {
                    Port = int.Parse(_configuration["Email:Smtp:Port"]),
                    Credentials = new NetworkCredential(_configuration["Email:Smtp:Username"], _configuration["Email:Smtp:Password"]),
                    EnableSsl = true,
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["Email:FromAddress"], _configuration["Email:FromName"]),
                    Subject = subject,
                    Body = $"<tbody>\r\n        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"1\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:24px;border-collapse:collapse;font-family:inherit\" height=\"24\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr>\r\n        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"><tbody><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;height:100%;overflow:hidden;width:72px;border-collapse:collapse;font-family:inherit\" width=\"72\" height=\"100%\"><div style=\"height:100%;overflow:hidden;width:72px;font-family:inherit\"></div></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><h1 style=\"font-size:32px;font-weight:500;letter-spacing:0.01em;color:#141212;text-align:center;line-height:39px;margin:0;font-family:inherit\">Xác Minh Email Của Bạn</h1></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;height:100%;overflow:hidden;width:72px;border-collapse:collapse;font-family:inherit\" width=\"72\" height=\"100%\"><div style=\"height:100%;overflow:hidden;width:72px;font-family:inherit\"></div></td></tr></tbody></table></td></tr>\r\n        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\">\r\n            <td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\">\r\n                <table cellpadding=\"0\" cellspacing=\"0\" style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\">\r\n                    <tbody>\r\n                        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"3\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:64px;border-collapse:collapse;font-family:inherit\" height=\"64\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr>\r\n                        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\">\r\n                            <td style=\"margin:0;padding:0;border:none;border-spacing:0;height:100%;overflow:hidden;width:72px;border-collapse:collapse;font-family:inherit\" width=\"72\" height=\"100%\"><div style=\"height:100%;overflow:hidden;width:72px;font-family:inherit\"></div></td>\r\n                            <td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\">\r\n                                <table cellpadding=\"0\" cellspacing=\"0\" style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;background-color:#f9f9f9;border-collapse:collapse\" width=\"100%\" bgcolor=\"#F9F9F9\">\r\n                                    <tbody>\r\n                                        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"3\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:40px;border-collapse:collapse;font-family:inherit\" height=\"40\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr>\r\n                                        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\">\r\n                                            <td style=\"margin:0;padding:0;border:none;border-spacing:0;height:100%;overflow:hidden;width:38px;border-collapse:collapse;font-family:inherit\" width=\"38\" height=\"100%\"><div style=\"height:100%;overflow:hidden;width:38px;font-family:inherit\"></div></td>\r\n                                            <td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\">\r\n                                                <table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;table-layout:fixed;border-collapse:collapse\" width=\"100%\">\r\n                                                    <tbody>\r\n                                                        <tr><td><h2 style=\"font-size:25.63px;font-weight:700;line-height:100%;color:#333;margin:0;text-align:center;font-family:inherit\">{username} </h2></td></tr>\r\n                                                        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"1\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:8px;border-collapse:collapse;font-family:inherit\" height=\"8\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr>\r\n                                                        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><p style=\"margin:0;padding:0;font-weight:500;font-size:18px;line-height:140%;letter-spacing:-0.01em;color:#666;font-family:inherit\">Bạn đã xác nhận địa chỉ email của Tài khoản Dental Care. Vui lòng xác minh email để xác nhận.<br>Nếu bạn không yêu cầu bất kỳ thay đổi nào, hãy xóa email này. Nếu có thắc mắc, vui lòng liên hệ <a href=\"Rivinger7@gmail.com\" style=\"color:#bd2225;text-decoration:underline\" target=\"_blank\">Bộ Phận Hỗ Trợ Dental Care</a>.</p></td></tr>\r\n                                                        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"1\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:40px;border-collapse:collapse;font-family:inherit\" height=\"40\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr>\r\n                                                        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\">\r\n                                                            <td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\">\r\n                                                                <div style=\"font-family:inherit\">\r\n\r\n                                                                    <a href=\"{message}\" style=\"min-width:300px;background:#1376f8;border-radius:12.8px;padding:25.5px 19px 26.5px 19px;text-align:center;font-size:18px;font-weight:700;color:#fff;display:inline-block;text-decoration:none;line-height:120%\" target=\"_blank\">Xác Minh Email</a>\r\n\r\n\r\n\r\n\r\n                                                                </div>\r\n                                                            </td>\r\n                                                        </tr>\r\n                                                    </tbody>\r\n                                                </table>\r\n                                            </td>\r\n                                            <td style=\"margin:0;padding:0;border:none;border-spacing:0;height:100%;overflow:hidden;width:38px;border-collapse:collapse;font-family:inherit\" width=\"38\" height=\"100%\"><div style=\"height:100%;overflow:hidden;width:38px;font-family:inherit\"></div></td>\r\n                                        </tr>\r\n                                        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"3\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:48px;border-collapse:collapse;font-family:inherit\" height=\"48\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr>\r\n                                    </tbody>\r\n                                </table>\r\n                            </td>\r\n                            <td style=\"margin:0;padding:0;border:none;border-spacing:0;height:100%;overflow:hidden;width:72px;border-collapse:collapse;font-family:inherit\" width=\"72\" height=\"100%\"><div style=\"height:100%;overflow:hidden;width:72px;font-family:inherit\"></div></td>\r\n                        </tr>\r\n                        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"3\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:48px;border-collapse:collapse;font-family:inherit\" height=\"48\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr>\r\n                    </tbody>\r\n                </table>\r\n            </td>\r\n        </tr>\r\n        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;font-size:16px;text-align:center;line-height:140%;letter-spacing:-0.01em;color:#666;border-collapse:collapse\" width=\"100%\" align=\"center\"><tbody><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;height:100%;overflow:hidden;width:100px;border-collapse:collapse;font-family:inherit\" width=\"100\" height=\"100%\"><div style=\"height:100%;overflow:hidden;width:100px;font-family:inherit\"></div></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\">Nếu bạn không phải là người gửi yêu cầu này, hãy đổi mật khẩu tài khoản ngay lập tức để tránh việc bị truy cập trái phép.</td><td style=\"margin:0;padding:0;border:none;border-spacing:0;height:100%;overflow:hidden;width:100px;border-collapse:collapse;font-family:inherit\" width=\"100\" height=\"100%\"><div style=\"height:100%;overflow:hidden;width:100px;font-family:inherit\"></div></td></tr><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"3\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:80px;border-collapse:collapse;font-family:inherit\" height=\"80\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr></tbody></table></td></tr>\r\n        <tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"><tbody><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;height:100%;overflow:hidden;width:72px;border-collapse:collapse;font-family:inherit\" width=\"72\" height=\"100%\"><div style=\"height:100%;overflow:hidden;width:72px;font-family:inherit\"></div></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;font-size:11.24px;line-height:140%;letter-spacing:-0.01em;color:#999;table-layout:fixed;border-collapse:collapse\" width=\"100%\"><tbody><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;display:inline-table;width:auto;border-collapse:collapse\"><tbody><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><a href=\"https://localhost:7165/\" style=\"color:#bd2225;text-decoration:underline\" target=\"_blank\"><img src=\"https://firebasestorage.googleapis.com/v0/b/auth-demo-123e3.appspot.com/o/Dental%20Care%20Logo%2Flogo.png?alt=media&token=c4e58d3c-574a-4050-b4a9-163736bd90cd\" alt=\"Logo Dental Care\" style=\"border:0;height:auto;line-height:100%;outline:none;text-decoration:none;width:142px\" width=\"142\" class=\"CToWUd\" data-bit=\"iit\"></a></td></tr></tbody></table></td></tr><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"><tbody><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"1\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:48px;border-collapse:collapse;font-family:inherit\" height=\"48\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;table-layout:fixed;border-collapse:collapse\" width=\"100%\"><tbody><tr style=\"margin:0;padding:0;border:none;border-spacing:0;height:44px;width:100%;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;width:44px;height:44px;border-collapse:collapse;font-family:inherit\" width=\"44\" height=\"44\"><a href=\"https://www.facebook.com/profile.php?id=100093052218614\" style=\"color:#bd2225;text-decoration:underline\" target=\"_blank\"><img alt=\"Biểu tượng Facebook\" src=\"https://firebasestorage.googleapis.com/v0/b/auth-demo-123e3.appspot.com/o/Dental%20Care%20Logo%2Ffacebookmail.png?alt=media&token=f88279fd-f9ad-455a-a093-5cd854e0e440\" style=\"border:0;line-height:100%;outline:none;text-decoration:none;width:44px;height:44px\" width=\"44\" height=\"44\" class=\"CToWUd\" data-bit=\"iit\"></a></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;width:24px;height:44px;border-collapse:collapse;font-family:inherit\" width=\"24\" height=\"44\"></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;width:44px;height:44px;border-collapse:collapse;font-family:inherit\" width=\"44\" height=\"44\"><a href=\"https://www.facebook.com/profile.php?id=100093052218614\" style=\"color:#bd2225;text-decoration:underline\" target=\"_blank\"><img alt=\"Biểu tượng Instagram\" src=\"https://firebasestorage.googleapis.com/v0/b/auth-demo-123e3.appspot.com/o/Dental%20Care%20Logo%2Finstagrammail.png?alt=media&token=8bb0b106-6666-461a-9f94-df6b4d8872fb\" style=\"border:0;line-height:100%;outline:none;text-decoration:none;width:44px;height:44px\" width=\"44\" height=\"44\" class=\"CToWUd\" data-bit=\"iit\"></a></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;width:24px;height:44px;border-collapse:collapse;font-family:inherit\" width=\"24\" height=\"44\"></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;width:44px;height:44px;border-collapse:collapse;font-family:inherit\" width=\"44\" height=\"44\"><a href=\"https://www.facebook.com/profile.php?id=100093052218614\" style=\"color:#bd2225;text-decoration:underline\" target=\"_blank\"><img alt=\"Biểu tượng YouTube\" src=\"https://firebasestorage.googleapis.com/v0/b/auth-demo-123e3.appspot.com/o/Dental%20Care%20Logo%2Fyoutubemail.png?alt=media&token=725229c6-d9a8-4635-95fb-3383df77de83\" style=\"border:0;line-height:100%;outline:none;text-decoration:none;width:44px;height:44px\" width=\"44\" height=\"44\" class=\"CToWUd\" data-bit=\"iit\"></a></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;width:24px;height:44px;border-collapse:collapse;font-family:inherit\" width=\"24\" height=\"44\"></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;width:44px;height:44px;border-collapse:collapse;font-family:inherit\" width=\"44\" height=\"44\"><a href=\"https://www.facebook.com/profile.php?id=100093052218614\" style=\"color:#bd2225;text-decoration:underline\" target=\"_blank\"><img alt=\"Biểu tượng Twitter\" src=\"https://firebasestorage.googleapis.com/v0/b/auth-demo-123e3.appspot.com/o/Dental%20Care%20Logo%2Ftwittermail.png?alt=media&token=b86058d2-11ce-438f-aa25-915dbfb1c1bb\" style=\"border:0;line-height:100%;outline:none;text-decoration:none;width:44px;height:44px\" width=\"44\" height=\"44\" class=\"CToWUd\" data-bit=\"iit\"></a></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"></td></tr></tbody></table></td></tr><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"1\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:32px;border-collapse:collapse;font-family:inherit\" height=\"32\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr></tbody></table></td></tr><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><table cellpadding=\"0\" cellspacing=\"0\" style=\"padding:0;border:none;border-spacing:0;width:100%;margin:0 auto;border-collapse:collapse\" width=\"100%\"><tbody><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><span style=\"display:inline;vertical-align:middle;font-weight:800;font-size:12.64px;letter-spacing:0.08em;white-space:nowrap;font-family:inherit\"><a href=\"#\" style=\"text-decoration:none;text-transform:uppercase;color:#999;vertical-align:middle\" target=\"_blank\" >Chính sách Quyền riêng tư</a></span><span style=\"display:inline;vertical-align:middle;font-weight:800;font-size:12.64px;letter-spacing:0.08em;white-space:nowrap;font-family:inherit\"><img src=\"https://ci3.googleusercontent.com/meips/ADKq_NbM3Nyr77fPOiupqmiHDcc6ktxhIgsg0vznSDjfXk9IIZX3_DDLzI3WZHRBdbBY_YnLXAQm_LpTNGW_UJsOMfHLlHTvEtduHIu0A09Hna5b584BoHzW420iY5MMnKbtwAm1U37j1DmJnTT_66ldmILXpc1CII44z0RTyUrdEbb0QOBtZg=s0-d-e1-ft#http://cdn.mcauto-images-production.sendgrid.net/6c20475da3226ec8/e457af8c-5531-4df1-a265-127217b6d80a/8x8.png\" style=\"border:0;height:auto;line-height:100%;outline:none;text-decoration:none;width:4px;vertical-align:middle;margin:4px 16px\" width=\"4\" class=\"CToWUd\" data-bit=\"iit\"><a href=\"#\" style=\"text-decoration:none;text-transform:uppercase;color:#999;vertical-align:middle\" target=\"_blank\" >Hỗ trợ</a></span><span style=\"display:inline;vertical-align:middle;font-weight:800;font-size:12.64px;letter-spacing:0.08em;white-space:nowrap;font-family:inherit\"><img src=\"https://ci3.googleusercontent.com/meips/ADKq_NbM3Nyr77fPOiupqmiHDcc6ktxhIgsg0vznSDjfXk9IIZX3_DDLzI3WZHRBdbBY_YnLXAQm_LpTNGW_UJsOMfHLlHTvEtduHIu0A09Hna5b584BoHzW420iY5MMnKbtwAm1U37j1DmJnTT_66ldmILXpc1CII44z0RTyUrdEbb0QOBtZg=s0-d-e1-ft#http://cdn.mcauto-images-production.sendgrid.net/6c20475da3226ec8/e457af8c-5531-4df1-a265-127217b6d80a/8x8.png\" style=\"border:0;height:auto;line-height:100%;outline:none;text-decoration:none;width:4px;vertical-align:middle;margin:4px 16px\" width=\"4\" class=\"CToWUd\" data-bit=\"iit\"><a href=\"#\" style=\"text-decoration:none;text-transform:uppercase;color:#999;vertical-align:middle\" target=\"_blank\">Điều khoản Sử dụng</a></span></td></tr><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"1\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:16px;border-collapse:collapse;font-family:inherit\" height=\"16\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr></tbody></table></td></tr><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><span style=\"font-family:inherit\">Đây là dịch vụ thư thông báo.</span></td></tr><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"1\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:16px;border-collapse:collapse;font-family:inherit\" height=\"16\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><span style=\"font-family:inherit\">Tập Đoàn&nbsp;Dental&nbsp;Care, Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh 700000 ©&nbsp;2024&nbsp;Dental&nbsp;Care.&nbsp;Đã&nbsp;đăng ký&nbsp;bản quyền</span></td></tr><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"1\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:16px;border-collapse:collapse;font-family:inherit\" height=\"16\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td style=\"margin:0;padding:0;border:none;border-spacing:0;text-align:center;border-collapse:collapse;font-family:inherit\" align=\"center\"><span style=\"font-family:inherit\">© năm 2024 bởi Tập Đoàn Dental Care, Dental Care, Nền tảng đặt lịch nha sĩ và các logo liên quan là nhãn hiệu, nhãn hiệu dịch vụ và/hoặc nhãn hiệu đã đăng ký của Tập Đoàn Dental Care.</span></td></tr></tbody></table></td><td style=\"margin:0;padding:0;border:none;border-spacing:0;height:100%;overflow:hidden;width:72px;border-collapse:collapse;font-family:inherit\" width=\"72\" height=\"100%\"><div style=\"height:100%;overflow:hidden;width:72px;font-family:inherit\"></div></td></tr><tr style=\"margin:0;padding:0;border:none;border-spacing:0;border-collapse:collapse;font-family:inherit\"><td colspan=\"3\" style=\"margin:0;padding:0;border:none;border-spacing:0;height:64px;border-collapse:collapse;font-family:inherit\" height=\"64\"><table style=\"margin:0;padding:0;border:none;border-spacing:0;width:100%;border-collapse:collapse\" width=\"100%\"></table></td></tr></tbody></table></td></tr>\r\n    </tbody>",
                    IsBodyHtml = true,
                };

                mailMessage.To.Add(email);

                // Optional: Add additional headers to improve deliverability
                mailMessage.Headers.Add("X-Priority", "1");
                mailMessage.Headers.Add("X-MSMail-Priority", "High");
                mailMessage.Headers.Add("Importance", "High");

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation($"Email sent to {email} with subject {subject}");
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, $"Error sending email to {email} with subject {subject}");
                throw; // Re-throw the exception if you want the caller to handle it
            }
        }
    }
}