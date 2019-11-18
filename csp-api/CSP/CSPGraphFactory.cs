using System;
using System.Collections.Generic;
using System.Linq;
using csp_api.Models;

namespace csp_api.CSP
{
	public static class CSPGraphFactory
	{
		public static CSPGraph BuildCSPGraph(BuildCSPGraphInput inputModel)
		{
			Func<Dictionary<string, byte>, CSPGraph, Vertex> selectUnassignedVariable = (assignments, cspGraph) =>
			{
				var unassigned = cspGraph.Vertices.Find(vertex => !assignments.ContainsKey(vertex.Element));
				if (unassigned == null)
				{
					throw new Exception("Unassigned is not defined");
				}

				return unassigned;
			};

			Func<Vertex, Dictionary<string, byte>, CSPGraph, List<byte>> orderDomainValues = (unassigned, assignments, cspGraph) =>
			{
				return unassigned.Domain;
			};

			Func<Vertex, byte, Dictionary<string, byte>, CSPGraph, bool>? inference = null;

			if (inputModel.ForwardChecking)
			{
				inference = (unassigned, value, assignments, cspGraph) =>
				{
					var neighbors = cspGraph.Edges.Where(edge => edge.EndVertices.start == unassigned
													&& !assignments.ContainsKey(edge.EndVertices.end.Element))
												.Select(edge => edge.EndVertices.end).ToList();

					foreach (var neighbor in neighbors)
					{
						neighbor.Domain.Remove(value);

						if (neighbor.Domain.Count == 0)
						{
							return false;
						}
					}

					if (inputModel.Propogation)
					{
						// AC-3 Algorithm
					}

					return true;
				};
			}

			return new CSPGraph(inputModel.States, inputModel.Constraints, inputModel.Colors, selectUnassignedVariable, orderDomainValues, inference);
		}
	}
}
