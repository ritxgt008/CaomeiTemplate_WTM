using Microsoft.AspNetCore.Authentication.Cookies;

namespace WalkingTec.Mvvm.Core
{
    public class CookieOption
    {
        public string Issuer { get; set; } = "http://localhost";
        public string Audience { get; set; } = "http://localhost";
        public int Expires { get; set; } = 3600;
        public bool SlidingExpiration { get; set; } = true;
        public string LoginPath { get; set; } = "/Login/Login";
        public string LogoutPath { get; set; } = "/Login/Logout";
        public string AccessDeniedPath { get; set; } = "/Login/Login";
        public string Domain { get; set; } = "";
        public string ReturnUrlParameter { get; set; } = CookieAuthenticationDefaults.ReturnUrlParameter;
    }
}