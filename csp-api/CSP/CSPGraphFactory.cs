using System;
using System.Collections.Generic;
using System.Linq;
using csp_api.Models;

namespace csp_api.CSP
{
	public static class CSPGraphFactory
	{
		// Method to build the methods to be used during the creation of the CSP graph
		// based on the options chosen by the user
		public static CSPGraph BuildCSPGraph(BuildCSPGraphInput inputModel)
		{
			// Declaration for the function to select the next vertex to be assigned
			Func<Dictionary<string, byte>, CSPGraph, Vertex> selectUnassignedVariable;

			// If the user chose MRV and Degree Constraint
			if (inputModel.MRV && inputModel.DC)
			{
				selectUnassignedVariable = (assignments, cspGraph) =>
				{
					// Get a list of the unassigned vertices in the graph
					var unassigned = cspGraph.Vertices.Where(vertex => !assignments.ContainsKey(vertex.Element)).ToList();

					// Create a list of the minimum unassigned values
					var minUnassigned = new List<Vertex>();

					// Add the first unassigned vertex to the list
					minUnassigned.Add(unassigned[0]);

					// Store the number of values in the domain of the current vertex
					var minLegalValues = unassigned[0].Domain.Count;

					// For each unassigned vertex
					foreach (var vertex in unassigned)
					{
						// Get the assignments of the neighboring vertices
						var takenValues = vertex.Edges.Where(edge => assignments.ContainsKey(edge.EndVertices.end.Element))
														.Select(edge => assignments[edge.EndVertices.end.Element])
														.Where(assignment => vertex.Domain.Contains(assignment))
														.Distinct().ToList();

						// Calculate the number of legal values for the current vertex
						var legalValues = vertex.Domain.Count - takenValues.Count;

						// If the number of legal values is equal to the number of minimum legal values
						if (legalValues == minLegalValues)
						{
							// Add the current vertex to the list of minimum vertices
							minUnassigned.Add(vertex);
						}
						// Else if the number of legal values is less than the current minimum
						// number of legal values
						else if (legalValues < minLegalValues)
						{
							// Clear the list of minimum vertices
							minUnassigned.Clear();

							// Add the current vetex to the list of minimum vertices
							minUnassigned.Add(vertex);

							// Update the minimum number of legal values
							minLegalValues = legalValues;
						}
					}

					// Set the maximum unassigned vertex equal to the first in the minimum list
					var maxUnassigned = minUnassigned[0];

					// If the number of minimum vertices is greater than one
					if (minUnassigned.Count > 1)
					{
						// Get the edges of the first vertex
						var maxEdges = maxUnassigned.Edges.Count;

						// For each minimum vertex
						foreach (var vertex in minUnassigned)
						{
							// If the current vertex has more edges than the current max
							if (vertex.Edges.Count > maxEdges)
							{
								// Update the max vertex and the max edges
								maxUnassigned = vertex;
								maxEdges = vertex.Edges.Count;
							}
						}
					}

					// return the max vertex of the minimum domains
					return maxUnassigned;
				};
			}
			// Else if the user only chose MRV
			else if (inputModel.MRV)
			{
				selectUnassignedVariable = (assignments, cspGraph) =>
				{
					// Get a list of the unassigned vertices in the graph
					var unassigned = cspGraph.Vertices.Where(vertex => !assignments.ContainsKey(vertex.Element)).ToList();

					// Get the first vertex
					var minUnassigned = unassigned[0];

					// Store the number of values in the domain of the current vertex
					var minLegalValues = unassigned[0].Domain.Count;

					// For each unassigned vertex
					foreach (var vertex in unassigned)
					{
						// Get the assignments of the neighboring vertices
						var takenValues = vertex.Edges.Where(edge => assignments.ContainsKey(edge.EndVertices.end.Element))
														.Select(edge => assignments[edge.EndVertices.end.Element])
														.Where(assignment => vertex.Domain.Contains(assignment))
														.Distinct().ToList();

						// Calculate the number of legal values for the current vertex
						var legalValues = vertex.Domain.Count - takenValues.Count;

						// If the number of legal values is less than the current minimum
						// number of legal values
						if (legalValues < minLegalValues)
						{
							// Add the current vetex to the list of minimum vertices
							minUnassigned = vertex;

							// Update the minimum number of legal values
							minLegalValues = legalValues;
						}
					}

					return minUnassigned;
				};
			}
			// Else if the user only chose Degree Constraint
			else if (inputModel.DC)
			{
				selectUnassignedVariable = (assignments, cspGraph) =>
				{
					// Get all unassigned vertices
					var unassigned = cspGraph.Vertices.Where(vertex => !assignments.ContainsKey(vertex.Element)).ToList();

					// Store the first vertex
					var maxUnassigned = unassigned[0];

					// Get the edges of the first vertex
					var maxEdges = maxUnassigned.Edges.Count;

					// For each unassigned vertex
					foreach (var vertex in unassigned)
					{
						// If the current vertex has more edges than the max vertex
						if (vertex.Edges.Count > maxEdges)
						{
							// Update the maxes
							maxUnassigned = vertex;
							maxEdges = vertex.Edges.Count;
						}
					}

					// Return the max vertex
					return maxUnassigned;
				};
			}
			// Else the user chose neither
			else
			{
				selectUnassignedVariable = (assignments, cspGraph) =>
				{
					// Find the first unassigned vertex in the CSP graph
					var unassigned = cspGraph.Vertices.Find(vertex => !assignments.ContainsKey(vertex.Element));
					
					if (unassigned == null)
					{
						throw new Exception("Unassigned is not defined");
					}

					// Return the first unassigned vertex
					return unassigned;
				};
			}

			// Initialize function order domains based on the options chosen by the user
			Func<Vertex, Dictionary<string, byte>, CSPGraph, List<byte>> orderDomainValues;

			// If the user chose LCV
			if (inputModel.LCV)
			{
				orderDomainValues = (unassigned, assignments, cspGraph) =>
				{
					// Get the unassigned neighbors of the unassigned vertex
					var neighbors = unassigned.Edges.Where(edge => !assignments.ContainsKey(edge.EndVertices.end.Element))
												.Select(edge => edge.EndVertices.end).ToList();

					// Create an array of integers for each value in the domain of the unassigned vertex
					var lcvHeuristics = new int[unassigned.Domain.Count];

					// For every value in the domain of the unassigned vertex
					for (int i = 0; i < unassigned.Domain.Count; i++)
					{
						// Set the current heuristic of the domain to zero
						lcvHeuristics[i] = 0;

						// For each neighbor of the unassigned vertex
						foreach (var neighbor in neighbors)
						{
							// If the neighbor and the unassigned vertex share a value
							if (neighbor.Domain.Contains(unassigned.Domain[i]))
							{
								// Increment the heuristic of this domain value
								++lcvHeuristics[i];
							}
						}
					}

					// Create a list of the domains to be sorted
					var sortedDomain = new List<byte>(unassigned.Domain);

					// Perform bubble sort on the list
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

					// Return the sorted domain
					return sortedDomain;
				};
			}
			// Else the user did not choose LCV
			else
			{
				orderDomainValues = (unassigned, assignments, cspGraph) =>
				{
					// Return the original domain
					return unassigned.Domain;
				};
			}

			// Initialze function to perform the inference step in forward checking
			Func<Vertex, byte, Dictionary<string, byte>, CSPGraph, bool>? inference = null;

			// If the user chose forward checking
			if (inputModel.ForwardChecking)
			{
				// Initialized function to perform propogation
				Func<Dictionary<string, byte>, CSPGraph, bool> propogate;

				// If the user chose propogation
				if (inputModel.Propagation)
				{
					propogate = (assignments, cspGraph) =>
					{
						// Store all of the edges in the CSP in a queue
						var edgesQueue = new Queue<Edge>(cspGraph.Edges);

						// While there are still edges in the queue
						while (edgesQueue.Any())
						{
							// Get the next edge from the queue
							var currentEdge = edgesQueue.Dequeue();

							// Get the start and end vertices of the current edge
							var (vertexX, vertexY) = currentEdge.EndVertices;

							// If the starting vertex of the current edge is assigned or 
							// the ending vertex has more the one value in its domain
							if (assignments.ContainsKey(vertexX.Element) || vertexY.Domain.Count > 1)
							{
								// Continue to the next iteration
								continue;
							}

							// Set removed equal to false and the removed element as zero
							var removed = false;
							byte removedElement = 0;

							// For each element in the domain of the current starting vertex
							foreach (var element in vertexX.Domain)
							{
								// If the first domain of the ending vertex is equal to the current element
								if (vertexY.Domain[0] == element)
								{
									// Set removed equal to true and store the current element
									removed = true;
									removedElement = element;

									// Break from the for loop
									break;
								}
							}

							// If any values need to be removed
							if (removed)
							{
								// Remove the value from the starting vertex's domain
								vertexX.Domain.Remove(removedElement);

								// If there are no values in the domain
								if (!vertexX.Domain.Any())
								{
									// Return false
									return false;
								}

								// Get the unassigned neighbors of the starting vertex
								var neighbors = vertexX.Edges.Where(edge => !assignments.ContainsKey(edge.EndVertices.end.Element));

								// Foreach neighbor in neighbors
								foreach (var neighbor in neighbors)
								{
									// Add all of the neighbors back into the queue in reversed order
									edgesQueue.Enqueue(new Edge(neighbor.EndVertices.end, neighbor.EndVertices.start));
								}
							}
						}

						// Return true
						return true;
					};
				}
				// Else the user did not choose propogate
				else
				{
					propogate = (assignments, cspGraph) =>
					{
						// Return true
						return true;
					};
				}

				inference = (unassigned, value, assignments, cspGraph) =>
				{
					// Get the unassigned neighbors of the unassigned vertex
					var neighbors = unassigned.Edges.Where(edge => !assignments.ContainsKey(edge.EndVertices.end.Element))
												.Select(edge => edge.EndVertices.end).ToList();

					// For each neighbor in neighbors
					foreach (var neighbor in neighbors)
					{
						// Remove the value from the domain of the current neighbor
						neighbor.Domain.Remove(value);

						// If there are no values in the current neighbor's domain
						if (neighbor.Domain.Count == 0)
						{
							/// Return false
							return false;
						}
					}

					// Run the propogate method
					var valid = propogate(assignments, cspGraph);

					// Return the result of propogation
					return valid;
				};
			}

			// Return a new CSP graph with the corresponding methods and structure
			return new CSPGraph(inputModel.States, inputModel.Constraints, inputModel.Colors, selectUnassignedVariable, orderDomainValues, inference);
		}
	}
}
