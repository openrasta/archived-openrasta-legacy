namespace OpenBastard.Environments.Iis7
{
    public class Iis7ClassicEnvironment : Iis7Environment
    {
        public override string Name
        {
            get { return "IIS 7 - Classic Mode"; }
        }

        protected override bool Integrated
        {
            get { return false; }
        }
    }
}