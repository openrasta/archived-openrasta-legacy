namespace OpenBastard.Environments.Iis7
{
    public class Iis7IntegratedEnvironment : Iis7Environment
    {
        public override string Name
        {
            get { return "IIS 7 - Integrated Mode"; }
        }

        protected override bool Integrated
        {
            get { return true; }
        }
    }
}