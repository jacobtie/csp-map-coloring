import React, { useState } from 'react';
import MapChart from './components/MapChart';
import Menu from './components/Menu';

function App() {
  const [countryName, setCountryName] = useState('australia');
  const [forwardChecking, setForwardChecking] = useState(false);
  const [propagation, setPropagation] = useState(false);
  const [mrv, setMRV] = useState(false);
  const [dc, setDC] = useState(false);
  const [lcv, setLCV] = useState(false);

  return (
    <div className="App" style={{ width: '100%', height: '100%' }}>
      <Menu
        countryName={countryName}
        setCountryName={setCountryName}
        forwardChecking={forwardChecking}
        setForwardChecking={setForwardChecking}
        propagation={propagation}
        setPropagation={setPropagation}
        mrv={mrv}
        setMRV={setMRV}
        dc={dc}
        setDC={setDC}
        lcv={lcv}
        setLCV={setLCV}
      />
      <MapChart
        country={countryName}
        forwardChecking={forwardChecking}
        propagation={propagation}
        mrv={mrv}
        dc={dc}
        lcv={lcv}
      />
    </div>
  );
}

export default App;
