using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Avocado.Domain.Abstract
{
    public interface IAuthorService
    {
        string SaveProfileImage(string userName, Stream fileStream, string fileName, string fileLocation);
    }
}
