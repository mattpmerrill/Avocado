using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Avocado.Domain.Abstract
{
    public interface IAuthorService
    {
        void SaveImage(string userName, Stream fileStream, string fileName, string fileLocation);
    }
}
