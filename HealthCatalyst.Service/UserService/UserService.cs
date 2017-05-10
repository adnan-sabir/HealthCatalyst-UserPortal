using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HealthCatalyst.Domain.Data;
using HealthCatalyst.DataAccess.Repository;
using HealthCatalyst.DataAccess;

namespace HealthCatalyst.Service.UserService
{
    public interface IService<T>
    {
        IEnumerable<T> GetAll();
        void Add(T entity);
        void Delete(T entity);
    }

    public class UserService : IService<User>
    {
        private IRepository<User> userRepository;

        public UserService(IRepository<User> userRepository)
        {
            this.userRepository = userRepository;
        }

        public UserService()
        {
            this.userRepository = new Repository<User>(new UserContext());
        }

        public IEnumerable<User> GetAll()
        {
            return userRepository.GetAll();
        }

        public void Add(User user)
        {
            userRepository.Add(user);
        }

        public void Delete(User user)
        {
            userRepository.Delete(user);
        }
    }
}