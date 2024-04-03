using System;

namespace LegacyApp
{
    public class UserService
    {
        private readonly IClientRepository _clientRepository;
        private readonly IUserCreditService _userCreditService;

        public UserService(IClientRepository clientRepository = null, IUserCreditService userCreditService = null)
        {
            _clientRepository = clientRepository ?? new DefaultClientRepository();
            _userCreditService = userCreditService ?? new DefaultUserCreditService();
        }

        public bool AddUser(string firstName, string lastName, string email, DateTime dateOfBirth, int clientId)
        {
            if (!IsNameValid(firstName, lastName) || !IsEmailValid(email) || !IsAgeValid(dateOfBirth))
            {
                return false;
            }

            var client = GetClient(clientId);
            if (client == null)
            {
                return false;
            }

            var user = CreateUser(firstName, lastName, email, dateOfBirth, client);

            SetCreditLimit(user, client);

            if (!IsCreditLimitValid(user))
            {
                return false;
            }

            SaveUser(user);
            return true;
        }

        private bool IsNameValid(string firstName, string lastName)
        {
            return !string.IsNullOrEmpty(firstName) && !string.IsNullOrEmpty(lastName);
        }

        private bool IsEmailValid(string email)
        {
            return email.Contains("@") && email.Contains(".");
        }

        private bool IsAgeValid(DateTime dateOfBirth)
        {
            var age = CalculateAge(dateOfBirth);
            return age >= 21;
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            var now = DateTime.Now;
            int age = now.Year - dateOfBirth.Year;
            if (now.Month < dateOfBirth.Month || (now.Month == dateOfBirth.Month && now.Day < dateOfBirth.Day)) age--;
            return age;
        }

        private Client GetClient(int clientId)
        {
            return _clientRepository.GetById(clientId);
        }

        private User CreateUser(string firstName, string lastName, string email, DateTime dateOfBirth, Client client)
        {
            return new User
            {
                Client = client,
                DateOfBirth = dateOfBirth,
                EmailAddress = email,
                FirstName = firstName,
                LastName = lastName,
                HasCreditLimit = true
            };
        }

        private void SetCreditLimit(User user, Client client)
        {
            if (client.Type == "VeryImportantClient")
            {
                user.HasCreditLimit = false;
            }
            else
            {
                var creditLimit = _userCreditService.GetCreditLimit(user.LastName, user.DateOfBirth);
                user.CreditLimit = client.Type == "ImportantClient" ? creditLimit * 2 : creditLimit;
            }
        }

        private bool IsCreditLimitValid(User user)
        {
            return !user.HasCreditLimit || user.CreditLimit >= 500;
        }

        private void SaveUser(User user)
        {
            UserDataAccess.AddUser(user);
        }
    }
}
