namespace OpenBastard.Hosting.Iis7
{
    public static class Iis7Starter
    {
        public static Iis7Server Start(string[] parameters)
        {
            // Debugger.Launch();
            return new Iis7Server(parameters[0], int.Parse(parameters[1]), int.Parse(parameters[2]), bool.Parse(parameters[3]));
        }
    }
}