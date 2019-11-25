using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace csp_api.CSP
{
	public class CSPGraph
	{
		public List<Vertex> Vertices { get; set; }
		public List<Edge> Edges { get; set; }

		private Func<Dictionary<string, byte>, CSPGraph, Vertex> _selectUnassignedVariable;
		private Func<Vertex, Dictionary<string, byte>, CSPGraph, List<byte>> _orderDomainValues;
		private Func<Vertex, byte, Dictionary<string, byte>, CSPGraph, bool>? _inference;

		internal CSPGraph(List<string> nodes, Dictionary<string, List<string>> constraints, List<string> domain,
			Func<Dictionary<string, byte>, CSPGraph, Vertex> selectUnassignedVariable,
			Func<Vertex, Dictionary<string, byte>, CSPGraph, List<byte>> orderDomainValues,
			Func<Vertex, byte, Dictionary<string, byte>, CSPGraph, bool>? inference)
		{
			this.Vertices = nodes.Select(node => new Vertex(node, Enumerable.Range(0, domain.Count).
							Select(num => (byte)num).ToList())).ToList();
			this.Edges = new List<Edge>();

			_initEdges(constraints);

			this._selectUnassignedVariable = selectUnassignedVariable;
			this._orderDomainValues = orderDomainValues;
			this._inference = inference;
		}

		private void _initEdges(Dictionary<string, List<string>> constraints)
		{
			foreach (var v in Vertices)
			{
				foreach (var neighbor in constraints[v.Element])
				{
					var nv = this.Vertices.Find(vert => vert.Element == neighbor);

					if (nv == null)
					{
						throw new Exception("A vertex is null, this should not happen");
					}

					var newEdge = new Edge(v, nv);

					v.addEdge(newEdge);
					this.Edges.Add(newEdge);
				}
			}
		}

		public CSPResult RunCSP()
		{
			var success = false;
			var assignments = new Dictionary<string, byte>();
			var numBacktracks = 0;
			var stopwatch = new Stopwatch();

			stopwatch.Start();
			success = _cspHelper(assignments, ref numBacktracks);
			stopwatch.Stop();

			var elapsedTime = stopwatch.ElapsedTicks / 10000;

			var results = new CSPResult(success, assignments, numBacktracks, elapsedTime);

			return results;
		}

		private bool _cspHelper(Dictionary<string, byte> assignments, ref int numBacktracks)
		{
			if (assignments.Count == this.Vertices.Count)
			{
				return true;
			}

			var unassigned = _selectUnassignedVariable(assignments, this);

			foreach (var value in _orderDomainValues(unassigned, assignments, this))
			{
				Dictionary<string, List<byte>>? previousDomain = null;

				if (_inference != null)
				{
					_assign(assignments, unassigned.Element, value);
					previousDomain = this.Vertices.ToDictionary(key => key.Element, 
																value => new List<byte>(value.Domain));

					var inferenceSuccess = _inference(unassigned, value, assignments, this);

					if (inferenceSuccess)
					{
						var success = _cspHelper(assignments, ref numBacktracks);

						if (success)
						{
							return true;
						}
					}

					_backtrack(assignments, previousDomain, ref unassigned, ref numBacktracks);
				}
				else
				{
					if (_isConsistent(unassigned, value, assignments))
					{
						_assign(assignments, unassigned.Element, value);
						var success = _cspHelper(assignments, ref numBacktracks);
						
						if (success)
						{
							return true;
						}

						_backtrack(assignments, previousDomain, ref unassigned, ref numBacktracks);
					}
				}

			}

			return false;
		}

		private bool _isConsistent(Vertex unassigned, byte value, Dictionary<string, byte> assignments)
		{
			foreach (var edge in unassigned.Edges)
			{
				if(assignments.ContainsKey(edge.EndVertices.end.Element) &&
					assignments[edge.EndVertices.end.Element] == value)
				{
					return false;
				}
			}

			return true;
		}

		private void _assign(Dictionary<string, byte> assignments, string element, byte value)
		{
			assignments.Add(element, value);
		}

		private void _backtrack(Dictionary<string, byte> assignments,
			Dictionary<string, List<byte>>? previousDomain, ref Vertex unassigned, ref int numBacktracks)
		{
			numBacktracks++;
			assignments.Remove(unassigned.Element);

			if (previousDomain != null)
			{
				foreach (var v in this.Vertices)
				{
					v.Domain = previousDomain[v.Element];
				}
			}
		}
	}
}
