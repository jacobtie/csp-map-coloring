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
		// Method to get the map colors
		public async Task<MapColoringResult> GetMapColors(FillStatesServiceInput inputModel)
		{
			// Create a new random object
			var rand = new Random();

			// Read the list of nodes and constraints from the files
			var (nodes, constraints) = await _getConstraintsFromFile(inputModel.CountryName);

			// Randomize the order of the list of vertices
			nodes = nodes.OrderBy(node => rand.Next()).ToList();

			// Create a CSP graph using the data from the file and from the user interface
			var cspGraph = CSPGraphFactory.BuildCSPGraph(
				new BuildCSPGraphInput(nodes, inputModel.Colors, constraints, inputModel.MRV, inputModel.DC, inputModel.LCV,
					inputModel.ForwardChecking, inputModel.Propagation));

			// Create a CSP graph from the basic graph
			var results = cspGraph.RunCSP();

			// If a solution could not be found
			if (!results.Success)
			{
				throw new Exception("Solution not found!");
			}

			// Assign each state their respective colors
			var stateColors = results.Assignments.Select(result => new StateColors(result.Key, inputModel.Colors[result.Value])).ToList();

			// Create a result of the map coloring
			var mapColoringResults = new MapColoringResult(stateColors, results.NumBacktracks, results.ElapsedTime);

			// Return the map coloring result
			return mapColoringResults;
		}

		// Method to read the states and constraints from a file
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
