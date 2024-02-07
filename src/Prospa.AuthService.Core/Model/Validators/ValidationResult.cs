using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Prospa.AuthService.Core.Model.Validators
{
    public class ValidationResult
    {
        public string code { get; set; }
        public string message { get; set; }

        public Data data { get; set; }

    }

    public class Error
    {
        public List<string> error { get; set; }
        public string attribute { get; set; }
    }
    public class Data
    {
        public List<Error> error { get; set; }
    }
}
