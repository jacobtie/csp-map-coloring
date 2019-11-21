namespace csp_api.Models
{
	public class FillStatesAPIInput
	{
		public string? CountryName { get; set; }
		public string? Colors { get; set; }
		public bool ForwardChecking { get; set; }
		public bool Propagation { get; set; }
		public bool MRV { get; set; }
		public bool DC { get; set; }
		public bool LCV { get; set; }
	}
}
