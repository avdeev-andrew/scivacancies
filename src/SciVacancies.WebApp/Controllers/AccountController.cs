﻿using System;
using System.Security.Claims;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.ConfigurationModel;
using SciVacancies.WebApp.Commands;
using SciVacancies.WebApp.Infrastructure.Identity;
using SciVacancies.WebApp.ViewModels;
using Thinktecture.IdentityModel.Client;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Text;
using System.IO;
using SciVacancies.WebApp.Models.OAuth;
using Microsoft.Framework.OptionsModel;
using AutoMapper;
using SciVacancies.WebApp.Engine;

namespace SciVacancies.WebApp.Controllers
{
    [ResponseCache(NoStore = true)]
    public class AccountController : Controller
    {
        private readonly IMediator _mediator;
        private readonly SciVacUserManager _userManager;
        private readonly IOptions<OAuthSettings> _oauthSettings;

        public AccountController(SciVacUserManager userManager, IMediator mediator, IOptions<OAuthSettings> oAuthSettings)
        {
            _mediator = mediator;
            _userManager = userManager;
            _oauthSettings = oAuthSettings;
        }

        public IActionResult LoginUser(string id)
        {
            var user = id == "user1" ? _userManager.FindByName("researcher1") : _userManager.FindByName("researcher2");
            var identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            var cp = new ClaimsPrincipal(identity);
            Context.Response.SignIn(DefaultAuthenticationTypes.ApplicationCookie, cp);
            return RedirectToHome();
        }
        public IActionResult LoginOrganization()
        {
            var user = _userManager.FindByName("organization1");
            var identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            var cp = new ClaimsPrincipal(identity);
            Context.Response.SignIn(DefaultAuthenticationTypes.ApplicationCookie, cp);
            return RedirectToHome();
        }

        [PageTitle("Вход")]
        [ResponseCache(NoStore = true)]
        [HttpPost]
        [HttpGet]
        public ActionResult Login(AccountLoginViewModel model)
        {
            //TODO - uncomment to pin it down to any job

            switch (model.User)
            {
                case AuthorizeUserTypes.Researcher:
                    switch (model.Resource)
                    {
                        case AuthorizeResourceTypes.OwnAuthorization:
                            //TODO - это заглушка не проверяет пароль
                            var user = _userManager.FindByName(model.Login);
                            var identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                            var cp = new ClaimsPrincipal(identity);
                            Context.Response.SignIn(DefaultAuthenticationTypes.ApplicationCookie, cp);

                            return RedirectToHome();
                        case AuthorizeResourceTypes.ScienceMap:
                            SetAuthorizationCookies(AuthorizeUserTypes.Researcher, AuthorizeResourceTypes.ScienceMap);

                            return Redirect(GetOAuthAuthorizationUrl(_oauthSettings.Options.Mapofscience));
                    }
                    break;
                case AuthorizeUserTypes.Organization:
                    switch (model.Resource)
                    {
                        case AuthorizeResourceTypes.Sciencemon:
                            SetAuthorizationCookies(AuthorizeUserTypes.Organization, AuthorizeResourceTypes.Sciencemon);

                            return Redirect(GetOAuthAuthorizationUrl(_oauthSettings.Options.Sciencemon));
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            return null;
        }

        private void SetAuthorizationCookies(AuthorizeUserTypes accountType, AuthorizeResourceTypes authorizationType)
        {
            Response.Cookies.Append("account_type", accountType.ToString(), new Microsoft.AspNet.Http.CookieOptions { Expires = DateTime.Now.AddMinutes(10) });
            Response.Cookies.Append("authorization_type", authorizationType.ToString(), new Microsoft.AspNet.Http.CookieOptions { Expires = DateTime.Now.AddMinutes(10) });
        }
        private Tuple<AuthorizeUserTypes, AuthorizeResourceTypes> GetAuthorizationCookies()
        {
            AuthorizeUserTypes accountType = (AuthorizeUserTypes)Enum.Parse(typeof(AuthorizeUserTypes), Request.Cookies["account_type"]);
            AuthorizeResourceTypes authorizationType = (AuthorizeResourceTypes)Enum.Parse(typeof(AuthorizeResourceTypes), Request.Cookies["authorization_type"]);

            return new Tuple<AuthorizeUserTypes, AuthorizeResourceTypes>(accountType, authorizationType);
        }

        #region OAuth

        private string SetStateCookie()
        {
            string state = Guid.NewGuid().ToString("N");
            Response.Cookies.Append("state", state, new Microsoft.AspNet.Http.CookieOptions { Expires = DateTime.Now.AddMinutes(10) });

            return state;
        }
        private string SetNonceCookie()
        {
            var nonce = Guid.NewGuid().ToString("N");
            Response.Cookies.Append("nonce", nonce, new Microsoft.AspNet.Http.CookieOptions { Expires = DateTime.Now.AddMinutes(10) });

            return nonce;
        }
        private string GetStateCookie()
        {
            return Request.Cookies["state"];
        }
        private string GetNonceCookie()
        {
            return Request.Cookies["nonce"];
        }
        private string GetCodeFromQuery()
        {
            return Request.Query["code"];
        }
        private string GetStateFromQuery()
        {
            return Request.Query["state"];
        }
        private string GetNonceFromQuery()
        {
            return Request.Query["nonce"];
        }

        private string GetOAuthAuthorizationUrl(OAuthProviderSettings oauth)
        {
            OAuth2Client client = new OAuth2Client(new Uri(oauth.AuthorizationEndpoint));

            return client.CreateCodeFlowUrl(oauth.ClientId, oauth.Scope, oauth.RedirectUrl, SetStateCookie(), SetNonceCookie());
        }
        private async Task<TokenResponse> GetOAuthTokensAsync(OAuthProviderSettings oauth, string code)
        {
            var client = new OAuth2Client(new Uri(oauth.TokenEndpoint), oauth.ClientId, oauth.ClientSecret);

            return await client.RequestAuthorizationCodeAsync(code, oauth.RedirectUrl);
        }
        private async Task<IEnumerable<Claim>> GetOAuthUserClaimsAsync(OAuthProviderSettings oauth, string accessToken)
        {
            UserInfoClient client = new UserInfoClient(new Uri(oauth.UserEndpoint), accessToken);
            UserInfoResponse userInfo = await client.GetAsync();

            List<Claim> claims = new List<Claim>();
            userInfo.Claims.ToList().ForEach(f => claims.Add(new Claim(f.Item1, f.Item2)));

            return claims;
        }

        #endregion
        #region API
        //общаемся с информикой
        private string GetOrganizationInfo(string inn)
        {
            WebRequest req = WebRequest.Create(@"http://www.sciencemon.ru/ext-api/v1.0/org/" + inn);
            req.Method = "GET";
            req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes("dev:informika"));
            HttpWebResponse response = req.GetResponse() as HttpWebResponse;
            string responseString = "";
            using (Stream stream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(stream, Encoding.UTF8);
                responseString = reader.ReadToEnd();
            }
            return responseString;
        }
        //общаемся с картой науки
        private string GetResearcherProfile(string accessToken)
        {
            return "stub";
        }

        #endregion


        [PageTitle("Оставить отзыв")]
        //Сюда редиректит после OAuth авторизации
        public async Task<ActionResult> Callback()
        {
            Tuple<AuthorizeUserTypes, AuthorizeResourceTypes> authorizationCookies = GetAuthorizationCookies();

            switch (authorizationCookies.Item1)
            {
                case AuthorizeUserTypes.Admin:

                    break;
                case AuthorizeUserTypes.Organization:
                    switch (authorizationCookies.Item2)
                    {
                        case AuthorizeResourceTypes.Sciencemon:
                            if (!String.IsNullOrEmpty(GetStateFromQuery()) && !String.IsNullOrEmpty(GetStateCookie()) && GetStateFromQuery().Equals(GetStateCookie()))
                            {
                                if (!String.IsNullOrEmpty(GetCodeFromQuery()))
                                {
                                    TokenResponse tokenResponse = await GetOAuthTokensAsync(_oauthSettings.Options.Sciencemon, GetCodeFromQuery());

                                    if (!String.IsNullOrEmpty(tokenResponse.AccessToken))
                                    {
                                        List<Claim> claims = new List<Claim>();

                                        claims.AddRange(await GetOAuthUserClaimsAsync(_oauthSettings.Options.Sciencemon, tokenResponse.AccessToken));

                                        claims.Add(new Claim("access_token", tokenResponse.AccessToken));
                                        claims.Add(new Claim("expires_in", DateTime.Now.AddSeconds(tokenResponse.ExpiresIn).ToString()));
                                        if (!String.IsNullOrEmpty(tokenResponse.RefreshToken)) claims.Add(new Claim("refresh_token", tokenResponse.RefreshToken));

                                        OAuthOrgClaim orgClaim = JsonConvert.DeserializeObject<OAuthOrgClaim>(claims.Find(f => f.Type.Equals("org")).Value);

                                        var orgUser = _userManager.FindByName(orgClaim.Inn);

                                        if (orgUser == null)
                                        {
                                            OAuthOrgInformation organizationInformation = JsonConvert.DeserializeObject<OAuthOrgInformation>(GetOrganizationInfo(orgClaim.Inn));
                                            AccountOrganizationRegisterViewModel orgModel = Mapper.Map<AccountOrganizationRegisterViewModel>(organizationInformation);

                                            var command = new RegisterUserOrganizationCommand
                                            {
                                                Data = orgModel
                                            };
                                            var user = _mediator.Send(command);

                                            var identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                                            var cp = new ClaimsPrincipal(identity);
                                            //at first, signing out...
                                            Context.Response.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                                            //... and then, signing in
                                            Context.Response.SignIn(DefaultAuthenticationTypes.ApplicationCookie, cp);
                                        }
                                        else
                                        {
                                            var identity = _userManager.CreateIdentity(orgUser, DefaultAuthenticationTypes.ApplicationCookie);
                                            var cp = new ClaimsPrincipal(identity);
                                            Context.Response.SignIn(DefaultAuthenticationTypes.ApplicationCookie, cp);
                                        }
                                    }
                                    else throw new ArgumentNullException("Token response is null");
                                }
                                else throw new ArgumentNullException("Oauth authorization code is null or empty");
                            }
                            else throw new ArgumentException("Oauth state mismatch");
                            break;
                        default:
                            break;
                    }
                    break;
                case AuthorizeUserTypes.Researcher:
                    switch (authorizationCookies.Item2)
                    {
                        case AuthorizeResourceTypes.ScienceMap:
                            if (!String.IsNullOrEmpty(GetStateFromQuery()) && !String.IsNullOrEmpty(GetStateCookie()) && GetStateFromQuery().Equals(GetStateCookie()))
                            {
                                if (!String.IsNullOrEmpty(GetCodeFromQuery()))
                                {
                                    TokenResponse tokenResponse = await GetOAuthTokensAsync(_oauthSettings.Options.Sciencemon, GetCodeFromQuery());

                                    if (!String.IsNullOrEmpty(tokenResponse.AccessToken))
                                    {
                                        List<Claim> claims = new List<Claim>();

                                        claims.AddRange(await GetOAuthUserClaimsAsync(_oauthSettings.Options.Sciencemon, tokenResponse.AccessToken));

                                        claims.Add(new Claim("access_token", tokenResponse.AccessToken));
                                        claims.Add(new Claim("expires_in", DateTime.Now.AddSeconds(tokenResponse.ExpiresIn).ToString()));
                                        if (!String.IsNullOrEmpty(tokenResponse.RefreshToken)) claims.Add(new Claim("refresh_token", tokenResponse.RefreshToken));

                                        var resUser = _userManager.FindByEmail(claims.Find(f => f.Type.Equals("email")).Value);

                                        if (resUser == null)
                                        {
                                            OAuthResProfile researcherProfile = JsonConvert.DeserializeObject<OAuthResProfile>(GetResearcherProfile(tokenResponse.AccessToken));
                                            AccountResearcherRegisterViewModel resModel = Mapper.Map<AccountResearcherRegisterViewModel>(researcherProfile);

                                            var command = new RegisterUserResearcherCommand
                                            {
                                                Data = resModel
                                            };

                                            var user = _mediator.Send(command);

                                            var identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                                            var cp = new ClaimsPrincipal(identity);
                                            //at first, signing out...
                                            Context.Response.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                                            //... and then, signing in
                                            Context.Response.SignIn(DefaultAuthenticationTypes.ApplicationCookie, cp);
                                        }
                                        else
                                        {
                                            var identity = _userManager.CreateIdentity(resUser, DefaultAuthenticationTypes.ApplicationCookie);
                                            var cp = new ClaimsPrincipal(identity);
                                            Context.Response.SignIn(DefaultAuthenticationTypes.ApplicationCookie, cp);
                                        }
                                    }
                                    else throw new ArgumentNullException("Token response is null");
                                }
                                else throw new ArgumentNullException("Oauth authorization code is null or empty");
                            }
                            else throw new ArgumentException("Oauth state mismatch");
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }

            return RedirectToHome();
        }

        [PageTitle("Регистрация")]
        public ViewResult Register() => View(new AccountResearcherRegisterViewModel());
        [PageTitle("Регистрация")]
        public ViewResult RegisterFilled() => View("register", new AccountResearcherRegisterViewModel
        {
            SecondName = "Фамилько",
            FirstName = "Имён",
            Patronymic = "Отчествович",
            UserName = "researcher@mail.scivacancies.org",
            Phone = "35413541",
            BirthYear = DateTime.Now.AddYears(-40).Year,
            Email = "researcher@mail.scivacancies.org"
        });

        [PageTitle("Регистрация")]
        [HttpPost]
        public IActionResult Register(AccountResearcherRegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            if (!model.Agreement)
            {
                ModelState.AddModelError("Agreement", "Нет согласия на обработку персональных данных");
                return View(model);
            }


            //var command = new RegisterUserResearcherCommand
            //{
            //    Data = model
            //};
            //var user = _mediator.Send(command);
            //_userManager.AddToRole(user.Id, ConstTerms.RequireRoleResearcher);
            var createUserResearcherCommand1 = new RegisterUserResearcherCommand
            {
                Data = new AccountResearcherRegisterViewModel
                {
                    Agreement = model.Agreement,
                    Email = model.Email,
                    Phone = model.Phone,
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    SecondName = model.SecondName,
                    Patronymic = model.Patronymic,
                    FirstNameEng = model.FirstNameEng,
                    SecondNameEng = model.SecondNameEng,
                    PatronymicEng = model.PatronymicEng,
                    BirthYear = model.BirthYear,
                    Password = model.Password,
                    ConfirmPassword = model.ConfirmPassword
                }
            };
            var user = _mediator.Send(createUserResearcherCommand1);
            var researcherGuid1 = Guid.Parse(user.Claims.Single(s => s.ClaimType.Equals(ConstTerms.ClaimTypeResearcherId)).ClaimValue);


            var identity = _userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
            var cp = new ClaimsPrincipal(identity);

            //signing out...
            Context.Response.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            //...before signing in
            Context.Response.SignIn(DefaultAuthenticationTypes.ApplicationCookie, cp);

            return RedirectToHome();
        }

        [PageTitle("Восстановление доступа к Системе")]
        public ViewResult ForgotPassword()
        {
            return View();
        }
        [PageTitle("Восстановление доступа к Системе")]
        public ViewResult RestorePassword()
        {
            return View();
        }

        [PageTitle("Выход")]
        [ResponseCache(NoStore = true)]
        public ActionResult Logout()
        {
            Context.Response.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToHome();
        }

        [ResponseCache(NoStore = true)]
        private RedirectToActionResult RedirectToHome()
        {
            return RedirectToAction("index", "home");
        }
    }
}
