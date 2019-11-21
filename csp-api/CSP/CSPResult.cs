using System.Collections.Generic;

namespace csp_api.CSP
{
	public class CSPResult
	{
		public bool Success { get; set; }
		public Dictionary<string, byte> Assignments { get; set; }
		public int NumBacktracks { get; set; }
		public long ElapsedTime { get; set; }

		public CSPResult(bool success, Dictionary<string, byte> assignments, int numBacktracks, long elapsedTime)
		{
			this.Success = success;
			this.Assignments = assignments;
			this.NumBacktracks = numBacktracks;
			this.ElapsedTime = elapsedTime;
		}
	}
}
