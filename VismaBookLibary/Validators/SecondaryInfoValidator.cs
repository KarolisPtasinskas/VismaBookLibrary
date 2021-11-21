using System;

namespace VismaBookLibary.Validators
{
    public static class SecondaryInfoValidator
    {
        public static bool CheckLendPeriod(string weeks)
        {
            if (weeks == null || weeks == "")
            {
                return false;
            }

            if (weeks.Length > 1)
            {
                return false;
            }

            if (Int32.Parse(weeks) > 8 || Int32.Parse(weeks) == 0)
            {
                return false;
            }

            return true;
        }
    }
}
