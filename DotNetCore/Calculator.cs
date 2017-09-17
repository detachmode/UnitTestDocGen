using System;

namespace DocGenerator
{
    

    public class Calculator
    {
        public static int Add(int v1, int v2, Person targetPerson, DateTime dt)
        {
            targetPerson.Age = v1+v2;
            return v1+v2;
        }
    }
}