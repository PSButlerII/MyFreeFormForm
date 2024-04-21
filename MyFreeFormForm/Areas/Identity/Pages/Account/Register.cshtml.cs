// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MyFreeFormForm.Data;

namespace MyFreeFormForm.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<MyIdentityUsers> _signInManager;
        private readonly UserManager<MyIdentityUsers> _userManager;
        private readonly IUserStore<MyIdentityUsers> _userStore;
        private readonly IUserEmailStore<MyIdentityUsers> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;

        public RegisterModel(
            UserManager<MyIdentityUsers> userManager,
            IUserStore<MyIdentityUsers> userStore,
            SignInManager<MyIdentityUsers> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [Display(Name = "City")]
            public string City { get; set; }

            [Required]
            [Display(Name = "State")]
            public string State { get; set; }

            [Required]
            [Display(Name = "Zip Code")]
            public string Zip { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.City = Input.City;
                user.State = Input.State;
                user.Zip = Input.Zip;

                //TODO: Create unique username based on first and last name and 4 digit unique sequence of numbers and letters
                user.UserName = CreateUserName(Input.FirstName, Input.LastName);

                await _userStore.SetUserNameAsync(user, user.UserName, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");

                    var userId = await _userManager.GetUserIdAsync(user);
                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                        protocol: Request.Scheme);
                    var callbckUrl = HtmlEncoder.Default.Encode(callbackUrl);
                    var emailHtml = Htmldoc(callbckUrl);
                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        emailHtml);

                    if (_userManager.Options.SignIn.RequireConfirmedAccount)
                    {
                        return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                    }
                    else
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        return LocalRedirect(returnUrl);
                    }
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }


        public string Htmldoc(string url)
        {
            var html = @"<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Strict//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-strict.dtd"">
<html data-editor-version=""2"" class=""sg-campaigns"" xmlns=""http://www.w3.org/1999/xhtml"">
    <head>
      <meta http-equiv=""Content-Type"" content=""text/html; charset=utf-8"">
      <meta name=""viewport"" content=""width=device-width, initial-scale=1, minimum-scale=1, maximum-scale=1"">
      <!--[if !mso]><!-->
      <meta http-equiv=""X-UA-Compatible"" content=""IE=Edge"">
      <!--<![endif]-->
      <!--[if (gte mso 9)|(IE)]>
      <xml>
        <o:OfficeDocumentSettings>
          <o:AllowPNG/>
          <o:PixelsPerInch>96</o:PixelsPerInch>
        </o:OfficeDocumentSettings>
      </xml>
      <![endif]-->
      <!--[if (gte mso 9)|(IE)]>
  <style type=""text/css"">
    body {width: 600px;margin: 0 auto;}
    table {border-collapse: collapse;}
    table, td {mso-table-lspace: 0pt;mso-table-rspace: 0pt;}
    img {-ms-interpolation-mode: bicubic;}
  </style>
<![endif]-->
      <style type=""text/css"">
    body, p, div {
      font-family: inherit;
      font-size: 14px;
    }
    body {
      color: #000000;
    }
    body a {
      color: #1188E6;
      text-decoration: none;
    }
    p { margin: 0; padding: 0; }
    table.wrapper {
      width:100% !important;
      table-layout: fixed;
      -webkit-font-smoothing: antialiased;
      -webkit-text-size-adjust: 100%;
      -moz-text-size-adjust: 100%;
      -ms-text-size-adjust: 100%;
    }
    img.max-width {
      max-width: 100% !important;
    }
    .column.of-2 {
      width: 50%;
    }
    .column.of-3 {
      width: 33.333%;
    }
    .column.of-4 {
      width: 25%;
    }
    ul ul ul ul  {
      list-style-type: disc !important;
    }
    ol ol {
      list-style-type: lower-roman !important;
    }
    ol ol ol {
      list-style-type: lower-latin !important;
    }
    ol ol ol ol {
      list-style-type: decimal !important;
    }
    @media screen and (max-width:480px) {
      .preheader .rightColumnContent,
      .footer .rightColumnContent {
        text-align: left !important;
      }
      .preheader .rightColumnContent div,
      .preheader .rightColumnContent span,
      .footer .rightColumnContent div,
      .footer .rightColumnContent span {
        text-align: left !important;
      }
      .preheader .rightColumnContent,
      .preheader .leftColumnContent {
        font-size: 80% !important;
        padding: 5px 0;
      }
      table.wrapper-mobile {
        width: 100% !important;
        table-layout: fixed;
      }
      img.max-width {
        height: auto !important;
        max-width: 100% !important;
      }
      a.bulletproof-button {
        display: block !important;
        width: auto !important;
        font-size: 80%;
        padding-left: 0 !important;
        padding-right: 0 !important;
      }
      .columns {
        width: 100% !important;
      }
      .column {
        display: block !important;
        width: 100% !important;
        padding-left: 0 !important;
        padding-right: 0 !important;
        margin-left: 0 !important;
        margin-right: 0 !important;
      }
      .social-icon-column {
        display: inline-block !important;
      }
    }
  </style>
      <!--user entered Head Start--><link href=""https://fonts.googleapis.com/css?family=Muli&display=swap"" rel=""stylesheet""><style>
body {font-family: 'Muli', sans-serif;}
</style><!--End Head user entered-->
    </head>
    <body>
      <center class=""wrapper"" data-link-color=""#1188E6"" data-body-style=""font-size:14px; font-family:inherit; color:#000000; background-color:#FFFFFF;"">
        <div class=""webkit"">
          <table cellpadding=""0"" cellspacing=""0"" border=""0"" width=""100%"" class=""wrapper"" bgcolor=""#FFFFFF"">
            <tr>
              <td valign=""top"" bgcolor=""#FFFFFF"" width=""100%"">
                <table width=""100%"" role=""content-container"" class=""outer"" align=""center"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                  <tr>
                    <td width=""100%"">
                      <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"">
                        <tr>
                          <td>
                            <!--[if mso]>
    <center>
    <table><tr><td width=""600"">
  <![endif]-->
                                    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""width:100%; max-width:600px;"" align=""center"">
                                      <tr>
                                        <td role=""modules-container"" style=""padding:0px 0px 0px 0px; color:#000000; text-align:left;"" bgcolor=""#FFFFFF"" width=""100%"" align=""left""><table class=""module preheader preheader-hide"" role=""module"" data-type=""preheader"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""display: none !important; mso-hide: all; visibility: hidden; opacity: 0; color: transparent; height: 0; width: 0;"">
    <tr>
      <td role=""module-content"">
        <p></p>
      </td>
    </tr>
  </table><table border=""0"" cellpadding=""0"" cellspacing=""0"" align=""center"" width=""100%"" role=""module"" data-type=""columns"" style=""padding:30px 20px 30px 20px;"" bgcolor=""#f6f6f6"" data-distribution=""1"">
    <tbody>
      <tr role=""module-content"">
        <td height=""100%"" valign=""top""><table width=""540"" style=""width:540px; border-spacing:0; border-collapse:collapse; margin:0px 10px 0px 10px;"" cellpadding=""0"" cellspacing=""0"" align=""left"" border=""0"" bgcolor="""">
      <tbody>
        <tr>
          <td style=""padding:0px;margin:0px;border-spacing:0;""><table class=""wrapper"" role=""module"" data-type=""image"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""72aac1ba-9036-4a77-b9d5-9a60d9b05cba"">
    <tbody>
      <tr>
        <td style=""font-size:6px; line-height:10px; padding:0px 0px 0px 0px;"" valign=""top"" align=""center"">
          <img class=""max-width"" border=""0"" style=""display:block; color:#000000; text-decoration:none; font-family:Helvetica, arial, sans-serif; font-size:16px;"" width=""29"" alt="""" data-proportionally-constrained=""true"" data-responsive=""false"" src=""http://cdn.mcauto-images-production.sendgrid.net/954c252fedab403f/9200c1c9-b1bd-47ed-993c-ee2950a0f239/29x27.png"" height=""27"">
        </td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""spacer"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""331cde94-eb45-45dc-8852-b7dbeb9101d7"">
    <tbody>
      <tr>
        <td style=""padding:0px 0px 20px 0px;"" role=""module-content"" bgcolor="""">
        </td>
      </tr>
    </tbody>
  </table><table class=""wrapper"" role=""module"" data-type=""image"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""d8508015-a2cb-488c-9877-d46adf313282"">
    <tbody>
      <tr>
        <td style=""font-size:6px; line-height:10px; padding:0px 0px 0px 0px;"" valign=""top"" align=""center"">
          <img class=""max-width"" border=""0"" style=""display:block; color:#000000; text-decoration:none; font-family:Helvetica, arial, sans-serif; font-size:16px;"" width=""100"" alt="""" data-proportionally-constrained=""true"" data-responsive=""false"" src=""http://cdn.mcauto-images-production.sendgrid.net/3fd9e9355cc007a4/62281b04-ba26-492f-80bc-d1e2ac41fee6/128x128.png"" height=""128"">
        </td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""spacer"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""27716fe9-ee64-4a64-94f9-a4f28bc172a0"">
    <tbody>
      <tr>
        <td style=""padding:0px 0px 30px 0px;"" role=""module-content"" bgcolor="""">
        </td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""948e3f3f-5214-4721-a90e-625a47b1c957"" data-mc-module-version=""2019-10-22"">
    <tbody>
      <tr>
        <td style=""padding:50px 30px 18px 30px; line-height:36px; text-align:inherit; background-color:#ffffff;"" height=""100%"" valign=""top"" bgcolor=""#ffffff"" role=""module-content""><div><div style=""font-family: inherit; text-align: center""><span style=""font-size: 43px"">Thanks for signing up!</span></div><div></div></div></td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""text"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""a10dcb57-ad22-4f4d-b765-1d427dfddb4e"" data-mc-module-version=""2019-10-22"">
    <tbody>
      <tr>
        <td style=""padding:18px 30px 18px 30px; line-height:22px; text-align:inherit; background-color:#ffffff;"" height=""100%"" valign=""top"" bgcolor=""#ffffff"" role=""module-content""><div><div style=""font-family: inherit; text-align: center""><span style=""font-size: 18px"">Please verify your email address to</span><span style=""color: #000000; font-size: 18px; font-family: arial, helvetica, sans-serif""> get access to thousands of exclusive job listings</span><span style=""font-size: 18px"">.</span></div>
<div style=""font-family: inherit; text-align: center""><span style=""color: #ffbe00; font-size: 18px""><strong>Thank you!</strong></span></div><div></div></div></td>
      </tr>
    </tbody>
  </table><table class=""module"" role=""module"" data-type=""spacer"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""7770fdab-634a-4f62-a277-1c66b2646d8d"">
    <tbody>
      <tr>
        <td style=""padding:0px 0px 20px 0px;"" role=""module-content"" bgcolor=""#ffffff"">
        </td>
      </tr>
    </tbody>
  </table><table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""module"" data-role=""module-button"" data-type=""button"" role=""module"" style=""table-layout:fixed;"" width=""100%"" data-muid=""d050540f-4672-4f31-80d9-b395dc08abe1"">
      <tbody>
        <tr>
          <td align=""center"" bgcolor=""#ffffff"" class=""outer-td"" style=""padding:0px 0px 0px 0px; background-color:#ffffff;"">
            <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""wrapper-mobile"" style=""text-align:center;"">
              <tbody>
                <tr>
                <td align=""center"" bgcolor=""#ffbe00"" class=""inner-td"" style=""border-radius:6px; font-size:16px; text-align:center; background-color:inherit;"">
                  <a href=""" + url + @""" style=""background-color:#ffbe00; border:1px solid #ffbe00; border-color:#ffbe00; border-radius:0px; border-width:1px; color:#000000; display:inline-block; font-size:14px; font-weight:normal; letter-spacing:0px; line-height:normal; padding:12px 40px 12px 40px; text-align:center; text-decoration:none; border-style:solid; font-family:inherit;"" target=""_blank"">Verify Email Now</a>
                </td>
                </tr>
              </tbody>
            </table>
          </td>
        </tr>
      </tbody>
    </table><table class=""module"" role=""module"" data-type=""spacer"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""7770fdab-634a-4f62-a277-1c66b2646d8d.1"">
    <tbody>
      <tr>
        <td style=""padding:0px 0px 50px 0px;"" role=""module-content"" bgcolor=""#ffffff"">
        </td>
      </tr>
    </tbody>
  </table>
<table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""module"" data-role=""module-button"" data-type=""button"" role=""module"" style=""table-layout:fixed;"" width=""100%"" data-muid=""d050540f-4672-4f31-80d9-b395dc08abe1.1"">
      <tbody>
        <tr>
          <td align=""center"" bgcolor=""#6e6e6e"" class=""outer-td"" style=""padding:0px 0px 0px 0px; background-color:#6e6e6e;"">
            <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""wrapper-mobile"" style=""text-align:center;"">
              <tbody>
                <tr>
                <td align=""center"" bgcolor=""#ffbe00"" class=""inner-td"" style=""border-radius:6px; font-size:16px; text-align:center; background-color:inherit;"">
                  <a href=""http://support@rcndevlabs.com"" style=""background-color:#ffbe00; border:1px solid #ffbe00; border-color:#ffbe00; border-radius:0px; border-width:1px; color:#000000; display:inline-block; font-size:14px; font-weight:normal; letter-spacing:0px; line-height:normal; padding:12px 40px 12px 40px; text-align:center; text-decoration:none; border-style:solid; font-family:inherit;"" target=""_blank"">Contact Support</a>
                </td>
                </tr>
              </tbody>
            </table>
          </td>
        </tr>
      </tbody>
    </table><table class=""module"" role=""module"" data-type=""spacer"" border=""0"" cellpadding=""0"" cellspacing=""0"" width=""100%"" style=""table-layout: fixed;"" data-muid=""c37cc5b7-79f4-4ac8-b825-9645974c984e"">
    <tbody>
      <tr>
        <td style=""padding:0px 0px 30px 0px;"" role=""module-content"" bgcolor=""6E6E6E"">
        </td>
      </tr>
    </tbody>
  </table></td>
        </tr>
      </tbody>
    </table></td>
      </tr>
    </tbody>
  </table><div data-role=""module-unsubscribe"" class=""module"" role=""module"" data-type=""unsubscribe"" style=""color:#444444; font-size:12px; line-height:20px; padding:16px 16px 16px 16px; text-align:Center;"" data-muid=""4e838cf3-9892-4a6d-94d6-170e474d21e5""><div class=""Unsubscribe--addressLine""></div><p style=""font-size:12px; line-height:20px;""><a target=""_blank"" class=""Unsubscribe--unsubscribeLink zzzzzzz"" href=""{{{unsubscribe}}}"" style="""">Unsubscribe</a> - <a href=""{{{unsubscribe_preferences}}}"" target=""_blank"" class=""Unsubscribe--unsubscribePreferences"" style="""">Unsubscribe Preferences</a></p></div><table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""module"" data-role=""module-button"" data-type=""button"" role=""module"" style=""table-layout:fixed;"" width=""100%"" data-muid=""550f60a9-c478-496c-b705-077cf7b1ba9a"">
      <tbody>
        <tr>
          <td align=""center"" bgcolor="""" class=""outer-td"" style=""padding:0px 0px 20px 0px;"">
            <table border=""0"" cellpadding=""0"" cellspacing=""0"" class=""wrapper-mobile"" style=""text-align:center;"">
              <tbody>
                <tr>
                <td align=""center"" bgcolor=""#f5f8fd"" class=""inner-td"" style=""border-radius:6px; font-size:16px; text-align:center; background-color:inherit;""><a href=""https://sendgrid.com/"" style=""background-color:#f5f8fd; border:1px solid #f5f8fd; border-color:#f5f8fd; border-radius:25px; border-width:1px; color:#a8b9d5; display:inline-block; font-size:10px; font-weight:normal; letter-spacing:0px; line-height:normal; padding:5px 18px 5px 18px; text-align:center; text-decoration:none; border-style:solid; font-family:helvetica,sans-serif;"" target=""_blank"">♥ POWERED BY TWILIO SENDGRID</a></td>
                </tr>
              </tbody>
            </table>
          </td>
        </tr>
      </tbody>
    </table></td>
                                      </tr>
                                    </table>
                                    <!--[if mso]>
                                  </td>
                                </tr>
                              </table>
                            </center>
                            <![endif]-->
                          </td>
                        </tr>
                      </table>
                    </td>
                  </tr>
                </table>
              </td>
            </tr>
          </table>
        </div>
      </center>
    </body>
  </html>";
            return html;
        }

        private MyIdentityUsers CreateUser()
        {
            try
            {
                return Activator.CreateInstance<MyIdentityUsers>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(MyIdentityUsers)}'. " +
                    $"Ensure that '{nameof(MyIdentityUsers)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<MyIdentityUsers> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<MyIdentityUsers>)_userStore;
        }

        private string CreateUserName(string firstName, string lastName)
        {
            var random = new Random();
            var randomString = new string(Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789", 5)
                               .Select(s => s[random.Next(s.Length)]).ToArray());

            // return the first 3 letters of the first name, the first 3 letters of the last name, and the random string
            return firstName.Substring(0, 3) + lastName.Substring(0, 3) + randomString;
        }
    }
}
