using System;

namespace Ridics.Authentication.Service.Models.ViewModel.Users
{
    public class TwoFactorLoginViewModel
    {
        public int Id { get; set; }

        public string TokenProvider { get; set; }

        public string Token { get; set; }

        public DateTime CreateTime { get; set; }
    }
}