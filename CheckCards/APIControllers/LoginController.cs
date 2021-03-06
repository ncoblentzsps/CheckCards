﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CheckCards.Data;
using CheckCards.Models.ViewModels;
using CheckCards.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CheckCards.APIControllers
{
    [Route("api/v0.999/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private SignInManager<ApplicationUser> signInManager;
        private UserManager<ApplicationUser> userManager;
        private IAServices AServices;

        private static string Failed = "Login Failed.";

        public LoginController(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, IAServices AServices)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.AServices = AServices;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] LoginRequestViewModel model)
        {
            if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                return new UnauthorizedResult();

            var user = await userManager.FindByNameAsync(model.Username);
            if (user != null)
            {
                var result = await signInManager.CheckPasswordSignInAsync(user, model.Password, false);
                if (result.Succeeded)
                {
                    await AServices.SendTwoFactorCodeAsync(user);
                    return new OkResult();
                }
            }
            return new UnauthorizedResult();
        }
    }


}