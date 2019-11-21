using System.Collections.Generic;

namespace csp_api.Models
{
	public class MapColoringResult
	{
		public List<StateColors> Assignments { get; set; }
		public int NumBacktracks { get; set; }
		public long ElapsedTime { get; set; }

		public MapColoringResult(List<StateColors> assignments, int numBacktracks, long elapsedTime)
		{
			this.Assignments = assignments;
			this.NumBacktracks = numBacktracks;
			this.ElapsedTime = elapsedTime;
		}
	}
}
