import React, { useState, useEffect } from 'react';
import { BubbleLoader } from 'react-css-loaders';
import AmericaMap from './AmericaMap';
import AustraliaMap from './AustraliaMap';
import { getStates } from '../services/StatesService';

const MapChart = ({ country, forwardChecking, propagation, mrv, dc, lcv }) => {
  const [states, setStates] = useState([]);
  const [numBacktracks, setNumBacktracks] = useState(-1);
  const [elapsedTime, setElapsedTime] = useState(-1.0);
  const [loading, setLoading] = useState(false);

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
      const res = await getStates(
        country,
        colors,
        forwardChecking,
        propagation,
        mrv,
        dc,
        lcv
      );
      if (!cancelled) {
        setStates(res.assignments);
        setNumBacktracks(res.numBacktracks);
        setElapsedTime(res.elapsedTime / 1000);
        setLoading(false);
      }
    })();

    return () => {
      cancelled = true;
    };
  }, [country, forwardChecking, propagation, mrv, dc, lcv]);

  return (
    <>
      {!loading ? (
        <>
          <div style={{ margin: '.5% 10.5%' }}>
            <div>Elapsed Time: {elapsedTime} seconds</div>
            <div>Number of Backtracks: {numBacktracks}</div>
          </div>
          {country === 'america' && states.length === 56 ? (
            <AmericaMap states={states} />
          ) : (
            country === 'australia' &&
            states.length === 8 && <AustraliaMap states={states} />
          )}
        </>
      ) : (
        <div
          style={{
            position: 'absolute',
            left: '50%',
            top: '50%',
            transform: 'translate(-50%, -50%)',
          }}
        >
          <BubbleLoader />
        </div>
      )}
    </>
  );
};

export default MapChart;
