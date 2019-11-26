using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using csp_api.CSP;
using csp_api.Models;

namespace csp_api.Services
{
	public class MapService
	{
		public async Task<MapColoringResult> GetMapColors(FillStatesServiceInput inputModel)
		{
			var rand = new Random();
			var (nodes, constraints) = await _getConstraintsFromFile(inputModel.CountryName);

			nodes = nodes.OrderBy(node => rand.Next()).ToList();

			var cspGraph = CSPGraphFactory.BuildCSPGraph(
				new BuildCSPGraphInput(nodes, inputModel.Colors, constraints, inputModel.MRV, inputModel.DC, inputModel.LCV,
					inputModel.ForwardChecking, inputModel.Propagation));

			var results = cspGraph.RunCSP();

			if (!results.Success)
			{
				throw new Exception("Solution not found!");
			}

			var stateColors = results.Assignments.Select(result => new StateColors(result.Key, inputModel.Colors[result.Value])).ToList();

			var mapColoringResults = new MapColoringResult(stateColors, results.NumBacktracks, results.ElapsedTime);

			return mapColoringResults;
		}

		private async Task<(List<string> nodes, Dictionary<string, List<string>> constraints)> _getConstraintsFromFile(string country)
		{
			var lines = new List<List<string>>();

			try
			{
				using (var reader = new StreamReader($"country-graphs/{country.ToLower()}.txt"))
				{
					while (!reader.EndOfStream)
					{
						var line = await reader.ReadLineAsync();
						if (line is null)
						{
							throw new Exception("Failed to read line");
						}
						lines.Add(line.Split(",").ToList());
					}
				}
			}
			catch
			{
				throw new Exception("Failed to read file");
			}

			var nodes = lines.Select(line => line.First()).ToList();
			var constraints = lines.ToDictionary(line => line[0], line => line.GetRange(1, line.Count - 1));

			return (nodes, constraints);
		}
	}
}
