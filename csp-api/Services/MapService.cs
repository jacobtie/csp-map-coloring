using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using csp_api.Models;

namespace csp_api.Services
{
	public class MapService
	{
		public async Task<List<StateColors>> GetMapColors(FillStatesServiceInput inputModel)
		{
			var stateColors = new List<StateColors>();

			await FillStates(stateColors, inputModel.CountryName, inputModel.Colors);

			return stateColors;
		}

		private async Task FillStates(List<StateColors> stateColors, string country, List<string> colors)
		{
			try
			{
				using (var reader = new StreamReader($"country-graphs/{country.ToLower()}.txt"))
				{
					var index = 0;
					while (!reader.EndOfStream)
					{
						var line = await reader.ReadLineAsync();
						var state = line?.Split(",")[0] ?? throw new Exception("Failed reading line");
						stateColors.Add(new StateColors(state, colors[index++ % colors.Count]));
					}
				}
			}
			catch
			{
				throw new Exception("Failed reading file");
			}
		}
	}
}
