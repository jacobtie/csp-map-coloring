import React, { useEffect, useRef } from 'react';
import { select, geoPath, geoMercator } from 'd3';
import { getAustraliaGeo } from '../services/StatesService';

// Australia map code modified from https://bl.ocks.org/GerardoFurtado/02aa65e5522104cb692e
const AustraliaMap = ({ states }) => {
  const mainRef = useRef(null);

  useEffect(() => {
    let cancel = false;
    (async () => {
      if (mainRef !== null) {
        const width = mainRef.current.clientWidth;
        const height = mainRef.current.clientHeight;

        const svg = select('#svganchor')
          .append('svg')
          .attr('width', width)
          .attr('height', height);

        const projection = geoMercator()
          .center([132, -28])
          .translate([width / 2, height / 2])
          .scale(600);

        const path = geoPath().projection(projection);

        const australiaGeo = await getAustraliaGeo();

        if (!cancel) {
          svg
            .selectAll('path')
            .data(australiaGeo.features)
            .enter()
            .append('path')
            .attr('d', path)
            .attr('stroke', 'dimgray')
            .attr(
              'fill',
              d =>
                states.find(state => state.name === d.properties.STATE_NAME)
                  .color
            );
        }
      }
    })();

    return () => {
      cancel = true;
    };
  }, [states]);

  return (
    <div
      id="svganchor"
      ref={mainRef}
      style={{ width: '80%', height: '80%', margin: 'auto' }}
    ></div>
  );
};

export default AustraliaMap;
