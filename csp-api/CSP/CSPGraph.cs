using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace csp_api.CSP
{
	public class CSPGraph
	{
		// Lists of vertices and edges present in the graph
		public List<Vertex> Vertices { get; set; }
		public List<Edge> Edges { get; set; }

		// Function definitions for each method of choosing variables or values
		private Func<Dictionary<string, byte>, CSPGraph, Vertex> _selectUnassignedVariable;
		private Func<Vertex, Dictionary<string, byte>, CSPGraph, List<byte>> _orderDomainValues;
		private Func<Vertex, byte, Dictionary<string, byte>, CSPGraph, bool>? _inference;

		// Constructor for the CSP given the following arguments
		internal CSPGraph(List<string> nodes, Dictionary<string, List<string>> constraints, List<string> domain,
			Func<Dictionary<string, byte>, CSPGraph, Vertex> selectUnassignedVariable,
			Func<Vertex, Dictionary<string, byte>, CSPGraph, List<byte>> orderDomainValues,
			Func<Vertex, byte, Dictionary<string, byte>, CSPGraph, bool>? inference)
		{
			// Create a new list of vertices based on the nodes passed in
			this.Vertices = nodes.Select(node => new Vertex(node, Enumerable.Range(0, domain.Count).
							Select(num => (byte)num).ToList())).ToList();

			// Create a new, empty list for the edges
			this.Edges = new List<Edge>();

			// Initialize the edges based on the constraints passed in
			_initEdges(constraints);

			// Initialize the functions
			this._selectUnassignedVariable = selectUnassignedVariable;
			this._orderDomainValues = orderDomainValues;
			this._inference = inference;
		}

		// Method to create the edges based on a dictionary of constraints
		private void _initEdges(Dictionary<string, List<string>> constraints)
		{
			// For each vertex in the graph
			foreach (var v in Vertices)
			{
				// For each neighbor of the current vertex
				foreach (var neighbor in constraints[v.Element])
				{
					// Store the neighboring vertex
					var nv = this.Vertices.Find(vert => vert.Element == neighbor);

					if (nv == null)
					{
						throw new Exception("A vertex is null, this should not happen");
					}

					// Store a new edge between the current vertex and its neighbor
					var newEdge = new Edge(v, nv);

					// Add the edge to the current vertex and the CSP graph
					v.addEdge(newEdge);
					this.Edges.Add(newEdge);
				}
			}
		}

		// Method to get the result of creating the CSP graph
		public CSPResult RunCSP()
		{
			// Assume the program will no succeed
			var success = false;

			// Initialize the list of assignments for the values of each vertex
			var assignments = new Dictionary<string, byte>();

			// Initialize the number of backtracks done during the creation of the CSP graph
			var numBacktracks = 0;

			// Create a stop watch to record the time
			var stopwatch = new Stopwatch();

			// Start the stop watch
			stopwatch.Start();

			// Get the success of creating the CSP graph
			success = _cspHelper(assignments, ref numBacktracks);

			// Stop the stopwatch
			stopwatch.Stop();

			// Get the elapsed time from the stop watch
			var elapsedTime = stopwatch.ElapsedMilliseconds;

			// Get the results from the CSP after it has been created
			var results = new CSPResult(success, assignments, numBacktracks, elapsedTime);

			// Return the results 
			return results;
		}

		// Method to create a CSP graph based on the options chosen by the user
		private bool _cspHelper(Dictionary<string, byte> assignments, ref int numBacktracks)
		{
			// If all vertices have been assigned a value
			if (assignments.Count == this.Vertices.Count)
			{
				// Return true
				return true;
			}

			// Get the next unassigned value from the graph
			var unassigned = _selectUnassignedVariable(assignments, this);

			// For each value from the ordered domain of the unassigned vertex
			foreach (var value in _orderDomainValues(unassigned, assignments, this))
			{
				// Initialize the previous domain to null
				Dictionary<string, List<byte>>? previousDomain = null;

				// If the inference method is not initialized
				if (_inference != null)
				{
					// Assign the current value to the unassigned vertex
					_assign(assignments, unassigned.Element, value);

					// Get the current domain from the current list of vertices
					previousDomain = this.Vertices.ToDictionary(key => key.Element,
																value => new List<byte>(value.Domain));

					// Get the success of the inference step 
					var inferenceSuccess = _inference(unassigned, value, assignments, this);

					// If the inference succeeded 
					if (inferenceSuccess)
					{
						// Call this method again recursively with the new set of assignments
						var success = _cspHelper(assignments, ref numBacktracks);

						// If a solution was found
						if (success)
						{
							// Return true
							return true;
						}
					}

					// Backtrack since a solution could not be found with the current assignments
					_backtrack(assignments, previousDomain, ref unassigned, ref numBacktracks);
				}
				// Else the inference method is initialized
				else
				{
					// If the current value is consistent with the other assignments
					if (_isConsistent(unassigned, value, assignments))
					{
						// Assign the current value to the unassigned vertex
						_assign(assignments, unassigned.Element, value);

						// Call this method again recursively with the new set of assignments
						var success = _cspHelper(assignments, ref numBacktracks);

						// If a solution was found
						if (success)
						{
							return true;
						}

						// Backtrack since a solution could not be found with the current assignments
						_backtrack(assignments, previousDomain, ref unassigned, ref numBacktracks);
					}
				}

			}

			// Return false if no values could be used with the unassigned vertex
			return false;
		}

		// Method to check if a vertex assignment is consistent with its neighbors
		private bool _isConsistent(Vertex unassigned, byte value, Dictionary<string, byte> assignments)
		{
			// For each edge in the unassigned vertex
			foreach (var edge in unassigned.Edges)
			{
				// If the neighbor is already assigned and has the same value
				if (assignments.ContainsKey(edge.EndVertices.end.Element) &&
					assignments[edge.EndVertices.end.Element] == value)
				{
					// Return false
					return false;
				}
			}

			// Return true
			return true;
		}

		// Method to assign a value to a particular vertex
		private void _assign(Dictionary<string, byte> assignments, string element, byte value)
		{
			assignments.Add(element, value);
		}

		// Method to backtrack an assignment and update the domain
		private void _backtrack(Dictionary<string, byte> assignments,
			Dictionary<string, List<byte>>? previousDomain, ref Vertex unassigned, ref int numBacktracks)
		{
			// Increment the number of backtracks
			numBacktracks++;

			// Unassign the vertex
			assignments.Remove(unassigned.Element);

			// If the previous domain is not null
			if (previousDomain != null)
			{
				// For each vertex in the CSP graph
				foreach (var v in this.Vertices)
				{
					// Revert the domain of the current vertex
					v.Domain = previousDomain[v.Element];
				}
			}
		}
	}
}
