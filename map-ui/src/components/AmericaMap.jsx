import React from 'react';
import { ComposableMap, Geographies, Geography } from 'react-simple-maps';

const AmericaMap = ({ states }) => {
  const geoUrl = 'https://cdn.jsdelivr.net/npm/us-atlas@3/states-10m.json';

  return (
    <ComposableMap projection={'geoAlbersUsa'} width={1500}>
      <Geographies geography={geoUrl}>
        {({ geographies }) =>
          geographies.map(geo => {
            const state = states.find(
              state => state.name === geo.properties.name
            );

            return (
              <Geography
                key={geo.rsmKey}
                geography={geo}
                fill={state.color}
                onFocus={e => e.target.blur()}
              />
            );
          })
        }
      </Geographies>
    </ComposableMap>
  );
};

export default AmericaMap;
