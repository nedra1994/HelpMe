using System.Collections.Generic;

namespace HelpMe.Commun.Security.Identity.Abstraction
{
    public class Result<T>
     
    {
        private bool _Succeeded;
        public bool Succeeded => _Succeeded;
        public readonly IEnumerable<Erreur> Errors;
        public readonly T Value;

        public Result(IEnumerable<Erreur> errors, T value )
        {
            _Succeeded = false;
            Errors = errors;
            Value = value;
        }
        public Result(IEnumerable<Erreur> errors)
        {
            _Succeeded = false;
            Errors = errors;
          
        }
        public Result(bool succeeded, IEnumerable<Erreur> errors)
        {
            _Succeeded = succeeded;
            Errors = errors;
          
        }
        public Result(bool succeeded, IEnumerable<Erreur> errors, T value )
        {
            _Succeeded = succeeded;
            Errors = errors;
            Value = value;
        }
        public Result(T value)
        {
            _Succeeded = true;
            Value = value;
        }


    }
}
//#­307
