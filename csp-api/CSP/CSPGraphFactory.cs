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
			Func<Dictionary<string, byte>, CSPGraph, Vertex> selectUnassignedVariable;

			if (inputModel.MRV)
			{
				selectUnassignedVariable = (assignments, cspGraph) =>
				{
					var unassigned = cspGraph.Vertices.Where(vertex => !assignments.ContainsKey(vertex.Element)).ToList();

					var minUnassigned = unassigned[0];
					var minLegalValues = unassigned[0].Domain.Count;

					foreach (var vertex in unassigned)
					{
						var takenValues = vertex.Edges.Where(edge => assignments.ContainsKey(edge.EndVertices.end.Element))
														.Select(edge => assignments[edge.EndVertices.end.Element])
														.Where(assignment => vertex.Domain.Contains(assignment))
														.Distinct().ToList();

						var legalValues = vertex.Domain.Count - takenValues.Count;

						if (legalValues < minLegalValues)
						{
							minUnassigned = vertex;
							minLegalValues = legalValues;
						}
					}

					return minUnassigned;
				};
			}
			else if (inputModel.DC)
			{
				selectUnassignedVariable = (assignments, cspGraph) =>
				{
					var unassigned = cspGraph.Vertices.Where(vertex => !assignments.ContainsKey(vertex.Element)).ToList();

					var maxUnassigned = unassigned[0];
					var maxEdges = maxUnassigned.Edges.Count;

					foreach (var vertex in unassigned)
					{
						if (vertex.Edges.Count > maxEdges)
						{
							maxUnassigned = vertex;
							maxEdges = vertex.Edges.Count;
						}
					}

					return maxUnassigned;
				};
			}
			else if (inputModel.MRV && inputModel.DC)
			{
				selectUnassignedVariable = (assignments, cspGraph) =>
				{
					var unassigned = cspGraph.Vertices.Where(vertex => !assignments.ContainsKey(vertex.Element)).ToList();

					var minUnassigned = new List<Vertex>();
					minUnassigned.Add(unassigned[0]);
					var minLegalValues = unassigned[0].Domain.Count;

					foreach (var vertex in unassigned)
					{
						var takenValues = vertex.Edges.Where(edge => assignments.ContainsKey(edge.EndVertices.end.Element))
														.Select(edge => assignments[edge.EndVertices.end.Element])
														.Where(assignment => vertex.Domain.Contains(assignment))
														.Distinct().ToList();

						var legalValues = vertex.Domain.Count - takenValues.Count;

						if (legalValues == minLegalValues)
						{
							minUnassigned.Add(vertex);
						}
						else if (legalValues < minLegalValues)
						{
							minUnassigned.Clear();
							minUnassigned.Add(vertex);
							minLegalValues = legalValues;
						}
					}

					var maxUnassigned = minUnassigned[0];

					if (minUnassigned.Count > 1)
					{
						var maxEdges = maxUnassigned.Edges.Count;

						foreach (var vertex in minUnassigned)
						{
							if (vertex.Edges.Count > maxEdges)
							{
								maxUnassigned = vertex;
								maxEdges = vertex.Edges.Count;
							}
						}
					}

					return maxUnassigned;
				};
			}
			else
			{
				selectUnassignedVariable = (assignments, cspGraph) =>
				{
					var unassigned = cspGraph.Vertices.Find(vertex => !assignments.ContainsKey(vertex.Element));
					if (unassigned == null)
					{
						throw new Exception("Unassigned is not defined");
					}

					return unassigned;
				};
			}

			Func<Vertex, Dictionary<string, byte>, CSPGraph, List<byte>> orderDomainValues;

			if (inputModel.LCV)
			{
				orderDomainValues = (unassigned, assignments, cspGraph) =>
				{
					var neighbors = unassigned.Edges.Where(edge => !assignments.ContainsKey(edge.EndVertices.end.Element))
												.Select(edge => edge.EndVertices.end).ToList();

					var lcvHeuristics = new int[unassigned.Domain.Count];

					for (int i = 0; i < unassigned.Domain.Count; i++)
					{
						lcvHeuristics[i] = 0;
						foreach (var neighbor in neighbors)
						{
							if (neighbor.Domain.Contains(unassigned.Domain[i]))
							{
								++lcvHeuristics[i];
							}
						}
					}

					var sortedDomain = new List<byte>(unassigned.Domain);
					for (int i = 0; i < lcvHeuristics.Length - 1; i++)
					{
						for (int j = 0; j < lcvHeuristics.Length - i - 1; j++)
						{
							if (lcvHeuristics[j] > lcvHeuristics[j + 1])
							{
								byte sdTemp = sortedDomain[j];
								sortedDomain[j] = sortedDomain[j + 1];
								sortedDomain[j + 1] = sdTemp;

								int lcvHTemp = lcvHeuristics[j];
								lcvHeuristics[j] = lcvHeuristics[j + 1];
								lcvHeuristics[j + 1] = lcvHTemp;
							}
						}
					}

					return sortedDomain;
				};
			}
			else
			{
				orderDomainValues = (unassigned, assignments, cspGraph) =>
				{
					return unassigned.Domain;
				};
			}

			Func<Vertex, byte, Dictionary<string, byte>, CSPGraph, bool>? inference = null;

			if (inputModel.ForwardChecking)
			{
				Func<Dictionary<string, byte>, CSPGraph, bool> propogate;

				if (inputModel.Propagation)
				{
					propogate = (assignments, cspGraph) =>
					{
						// AC-3 Algorithm

						var edgesQueue = new Queue<Edge>(cspGraph.Edges);

						while (edgesQueue.Any())
						{
							var currentEdge = edgesQueue.Dequeue();

							// Remove-Inconsistent-Values
							var (vertexX, vertexY) = currentEdge.EndVertices;

							if (assignments.ContainsKey(vertexX.Element) || vertexY.Domain.Count > 1)
							{
								continue;
							}

							var removed = false;
							byte removedElement = 0;

							foreach (var element in vertexX.Domain)
							{
								if (vertexY.Domain[0] == element)
								{
									removed = true;
									removedElement = element;
									break;
								}
							}

							if (removed)
							{
								vertexX.Domain.Remove(removedElement);

								if (!vertexX.Domain.Any())
								{
									return false;
								}

								var neighbors = vertexX.Edges.Where(edge => !assignments.ContainsKey(edge.EndVertices.end.Element));

								foreach (var neighbor in neighbors)
								{
									edgesQueue.Enqueue(new Edge(neighbor.EndVertices.end, neighbor.EndVertices.start));
								}
							}
						}

						return true;
					};
				}
				else
				{
					propogate = (assignments, cspGraph) =>
					{
						return true;
					};
				}

				inference = (unassigned, value, assignments, cspGraph) =>
				{
					var neighbors = unassigned.Edges.Where(edge => !assignments.ContainsKey(edge.EndVertices.end.Element))
												.Select(edge => edge.EndVertices.end).ToList();

					foreach (var neighbor in neighbors)
					{
						neighbor.Domain.Remove(value);

						if (neighbor.Domain.Count == 0)
						{
							return false;
						}
					}

					var valid = propogate(assignments, cspGraph);

					return valid;
				};
			}

			return new CSPGraph(inputModel.States, inputModel.Constraints, inputModel.Colors, selectUnassignedVariable, orderDomainValues, inference);
		}
	}
}
