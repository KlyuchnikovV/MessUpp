﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Messenger.Model;

namespace Messenger.DataLayer
{
    public interface IProfilesRepository
    {
        Profile CreateProfile(Profile newProfile);
        Profile GetProfileById(Guid id);
        Profile ChangeProfileData(Profile newData);
        void DeleteProfile(Guid id);
    }
}