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
		public async Task<List<StateColors>> GetMapColors(FillStatesServiceInput inputModel)
		{
			var (nodes, constraints) = await _getConstraintsFromFile(inputModel.CountryName);
			var chosenHeuristic = (inputModel.MRV, inputModel.DC, inputModel.LCV) switch
			{
				(false, false, false) => Heuristic.None,
				(true, false, false) => Heuristic.MRV,
				(false, true, false) => Heuristic.DC,
				(false, false, true) => Heuristic.LCV,
				(_, _, _) => throw new Exception("Bad heuristic combination"),
			};

			var cspGraph = CSPGraphFactory.BuildCSPGraph(
				new BuildCSPGraphInput(nodes, inputModel.Colors, constraints, chosenHeuristic,
					inputModel.ForwardChecking, inputModel.Propogation));

			var (success, results) = cspGraph.RunCSP();

			if (!success)
			{
				throw new Exception("Solution not found!");
			}

			var stateColors = results.Select(result => new StateColors(result.Key, inputModel.Colors[result.Value])).ToList();

			return stateColors;
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
