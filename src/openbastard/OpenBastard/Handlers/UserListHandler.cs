using System.Collections.Generic;
using OpenBastard.Resources;
using OpenRasta.Web;

namespace OpenBastard.Handlers
{
    public class UserListHandler
    {
        static List<User> _userRepository = new List<User>();

        /// <summary>
        /// Operation testing the addition of a user
        /// </summary>
        /// <param name="userToAdd"></param>
        /// <returns></returns>
        public OperationResult Post(User userToAdd)
        {
            AddUser(userToAdd);
            return new OperationResult.Created
                {
                    RedirectLocation = userToAdd.CreateUri(), 
                    ResponseResource = userToAdd
                };
        }

        public OperationResult Put(List<User> users)
        {
            _userRepository = users;
            return new OperationResult.OK
                {
                    ResponseResource = users
                };
        }

        void AddUser(User userToAdd)
        {
            _userRepository.Add(userToAdd);
            userToAdd.Id = _userRepository.Count;
        }
    }
}