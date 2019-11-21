using System.Collections.Generic;

namespace csp_api.Models
{
	public class BuildCSPGraphInput
	{
		public List<string> States { get; set; }
		public List<string> Colors { get; set; }
		public Dictionary<string, List<string>> Constraints { get; set; }
		public bool MRV { get; set; }
		public bool DC { get; set; }
		public bool LCV { get; set; }
		public bool ForwardChecking { get; set; }
		public bool Propogation { get; set; }

		public BuildCSPGraphInput(List<string> states, List<string> colors, Dictionary<string, List<string>> constraints, bool mrv, bool dc, bool lcv, bool forwardChecking, bool propogation)
		{
			this.States = states;
			this.Colors = colors;
			this.Constraints = constraints;
			this.MRV = mrv;
			this.DC = dc;
			this.LCV = lcv;
			this.ForwardChecking = forwardChecking;
			this.Propogation = propogation;
		}
	}
}
