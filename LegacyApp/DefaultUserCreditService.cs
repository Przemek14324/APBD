using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LegacyApp;

public class DefaultUserCreditService : IUserCreditService
{
    public int GetCreditLimit(string lastName, DateTime dateOfBirth)
    {
        // Default implementation or throw an exception if this should never be used without a real implementation.
        throw new NotImplementedException();
    }
}
