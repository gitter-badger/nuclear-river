using NDesk.Options;

namespace NuClear.AdvancedSearch.Replication.Bulk
{
    internal sealed class Options
    {
	    public bool Facts { get; private set; }
		public bool CustomerIntelligence { get; private set; }
		public bool Statistics { get; private set; }

		public static Options Parse(string[] args)
        {
			var result = new Options();
            var optionSet =
                new OptionSet
                {
                    { "fact", "Specify fact types to reimport", x => result.Facts = true },
                    { "ci", "Specify CI types to reimport", x => result.CustomerIntelligence = true },
					{ "statistics", "Specify statistics types to reimport", x => result.Statistics = true },
				};

            optionSet.Parse(args);
			return result;
        }
    }
}