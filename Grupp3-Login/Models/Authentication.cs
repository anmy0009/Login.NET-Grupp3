﻿namespace Grupp3_Login.Models
{
    public class LoginRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }

    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public int UserId { get; set; }
        public string Role { get; set; }
    }
}

