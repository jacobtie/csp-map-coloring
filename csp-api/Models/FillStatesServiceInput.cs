using System;
using System.Collections.Generic;

namespace csp_api.Models
{
	public class FillStatesServiceInput
	{
		public string CountryName { get; set; }
		public List<string> Colors { get; set; }
		public bool ForwardChecking { get; set; }
		public bool Propogation { get; set; }
		public bool MRV { get; set; }
		public bool DC { get; set; }
		public bool LCV { get; set; }

		public FillStatesServiceInput(FillStatesAPIInput inputModel)
		{
			if (inputModel.CountryName == null)
			{
				throw new Exception("Country name is null");
			}
			if (inputModel.Colors == null)
			{
				throw new Exception("Colors is null");
			}
			this.CountryName = inputModel.CountryName;
			this.Colors = new List<string>(inputModel.Colors.Split(';'));
			this.ForwardChecking = inputModel.ForwardChecking;
			this.Propogation = inputModel.Propogation;
			this.MRV = inputModel.MRV;
			this.DC = inputModel.DC;
			this.LCV = inputModel.LCV;
		}
	}
}
