using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZSharpTextHelper.Global
{
    public class Person
    {
        string _firstName;
        string FirstName
        {
            get
            {
                return _firstName;
            }
            set
            {
                _firstName = value;
            }
        }

        string _lastName;
        string LastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                _lastName = value;
            }
        }

        DateTime _birthDate;
        DateTime BirthDate
        {
            get
            {
                return _birthDate;
            }
            set
            {
                _birthDate = value;
            }
        }

        private string name;  // the name field 
        public string Name    // the Name property
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
            }
        }
    }
}
