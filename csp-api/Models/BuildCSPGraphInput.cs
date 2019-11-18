using System.Collections.Generic;

namespace csp_api.Models
{
	public class BuildCSPGraphInput
	{
		public List<string> States { get; set; }
		public List<string> Colors { get; set; }
		public Dictionary<string, List<string>> Constraints { get; set; }
		public Heuristic ChosenHeuristic { get; set; }
		public bool ForwardChecking { get; set; }
		public bool Propogation { get; set; }

		public BuildCSPGraphInput(List<string> states, List<string> colors, Dictionary<string, List<string>> constraints, Heuristic chosenHeuristic, bool forwardChecking, bool propogation)
		{
			this.States = states;
			this.Colors = colors;
			this.Constraints = constraints;
			this.ChosenHeuristic = chosenHeuristic;
			this.ForwardChecking = forwardChecking;
			this.Propogation = propogation;
		}
	}
}
