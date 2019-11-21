using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using csp_api.Services;
using csp_api.Models;

namespace csp_api.Controllers
{
	public class MapController : ControllerBase
	{
		private MapService _mapService = new MapService();

		[HttpGet("/map/color")]
		public async Task<IActionResult> GetMapColor([FromQuery] FillStatesAPIInput inputModel)
		{
			if (inputModel.Colors is null)
			{
				return BadRequest();
			}

			if (inputModel.CountryName is null)
			{
				return BadRequest();
			}

			if (!inputModel.ForwardChecking && inputModel.Propogation)
			{
				return BadRequest();
			}

			List<StateColors>? stateColors;
			try
			{
				stateColors = await _mapService.GetMapColors(new FillStatesServiceInput(inputModel));
			}
			catch
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			if (stateColors is null)
			{
				return StatusCode(StatusCodes.Status500InternalServerError);
			}

			return new ObjectResult(stateColors);
		}
	}
}
