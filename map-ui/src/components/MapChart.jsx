import React, { useState, useEffect } from 'react';
import AmericaMap from './AmericaMap';
import AustraliaMap from './AustraliaMap';
import { getStates } from '../services/StatesService';

const MapChart = ({ country, forwardChecking, propogation, mrv, dc, lcv }) => {
  const [states, setStates] = useState([]);
  const [loading, setLoading] = useState(false);

  const env = 'dev';

  let baseURL;

  if (env === 'dev') {
    baseURL = 'http://localhost:5000/';
  } else if (env === 'prod') {
    baseURL = '';
  } else {
    baseURL = '';
  }

  useEffect(() => {
    setLoading(true);
    let cancelled = false;
    const colors =
      country === 'america'
        ? 'red;green;blue;yellow'
        : country === 'australia'
        ? 'red;green;blue'
        : '';
    (async () => {
      const statesJSON = await getStates(
        country,
        colors,
        forwardChecking,
        propogation,
        mrv,
        dc,
        lcv
      );
      if (!cancelled) {
        const states = statesJSON;
        setStates(states);
        setLoading(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [baseURL, country, forwardChecking, propogation, mrv, dc, lcv]);

  return (
    <>
      {!loading ? (
        <>
          {country === 'america' && states.length === 56 ? (
            <AmericaMap states={states} />
          ) : (
            country === 'australia' &&
            states.length === 8 && <AustraliaMap states={states} />
          )}
        </>
      ) : (
        <h1>LOADING</h1>
      )}
    </>
  );
};

export default MapChart;
