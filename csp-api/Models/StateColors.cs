namespace csp_api.Models
{
	public class StateColors
	{
		public string Name { get; set; }
		public string Color { get; set; }

		public StateColors(string name, string color)
		{
			Name = name;
			Color = color;
		}
	}
}
