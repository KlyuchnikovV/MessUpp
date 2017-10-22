using System;
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
        Profile GetProfile(Guid id);
        Profile ChangeProfileData(Profile newData);
        void DeleteProfile(Guid id);

        // Additional methods.
        IEnumerable<Chat> GetProfileChats(Guid id);
        IEnumerable<Profile> GetProfiles(string name, string surname);
        Profile GetProfile(string login);
    }
}
