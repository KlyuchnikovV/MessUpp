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
        IEnumerable<Profile> FindProfiles(string[] names);
        Profile GetProfile(string login);
        Profile GetProfile(string login, string password);
        IEnumerable<Profile> FindProfiles(string name);
    }
}
