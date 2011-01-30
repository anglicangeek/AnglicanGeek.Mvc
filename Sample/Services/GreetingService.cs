using System;

namespace Sample
{
    public class GreetingService : Sample.IGreetingService
    {
        public string GetGreeting()
        {
            return "Hello.";
        }
    }
}