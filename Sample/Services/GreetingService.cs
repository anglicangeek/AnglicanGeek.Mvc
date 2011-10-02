using System;

namespace Sample
{
    public class GreetingService : Sample.IGreetingService
    {
        public string GetGreeting(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "Hello.";
            else
                return "Hello " + name + ".";
        }
    }
}