﻿using System;

namespace Messenger.Model 
{
    public class Profile
    {
        public Guid Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Guid Avatar { get; set; }
    }
}
