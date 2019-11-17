import React, { useState } from 'react';

const Menu = ({
  countryName,
  setCountryName,
  forwardChecking,
  setForwardChecking,
  propogation,
  setPropogation,
  mrv,
  setMRV,
  dc,
  setDC,
  lcv,
  setLCV,
}) => {
  const [tempCountryName, setTempCountryName] = useState(countryName);
  const [tempForwardChecking, setTempForwardChecking] = useState(
    forwardChecking
  );
  const [tempPropogation, setTempPropogation] = useState(propogation);
  const [tempMRV, setTempMRV] = useState(mrv);
  const [tempDC, setTempDC] = useState(dc);
  const [tempLCV, setTempLCV] = useState(lcv);

  const handleForwardCheckingClick = () => {
    if (tempForwardChecking) {
      setTempPropogation(false);
    }
    setTempForwardChecking(!tempForwardChecking);
  };

  const handleHeuristicClick = (val, fn) => {
    setTempMRV(false);
    setTempDC(false);
    setTempLCV(false);
    fn(val);
  };

  const handleRunButton = () => {
    setCountryName(tempCountryName);
    setForwardChecking(tempForwardChecking);
    setPropogation(tempPropogation);
    setMRV(tempMRV);
    setDC(tempDC);
    setLCV(tempLCV);
  };

  const formElementStyle = {
    margin: '0 .35rem',
  };

  return (
    <div
      style={{
        display: 'flex',
        flexDirection: 'row',
        justifyContent: 'center',
        padding: '1% 10% 0px 10%',
      }}
    >
      <select
        onChange={e => setTempCountryName(e.target.value)}
        className="form-control"
        style={formElementStyle}
      >
        <option value="america">America</option>
        <option value="australia">Australia</option>
      </select>
      <button
        type="button"
        style={formElementStyle}
        className={`btn ${tempForwardChecking ? 'btn-success' : ''}`}
        onClick={handleForwardCheckingClick}
      >
        Forward Checking
      </button>
      <button
        type="button"
        style={formElementStyle}
        className={`btn ${tempPropogation ? 'btn-success' : ''}`}
        onClick={() => setTempPropogation(!tempPropogation)}
        disabled={!tempForwardChecking}
      >
        Propogation
      </button>
      <button
        type="button"
        style={formElementStyle}
        className={`btn ${tempMRV ? 'btn-success' : ''}`}
        onClick={() => handleHeuristicClick(!tempMRV, setTempMRV)}
      >
        MRV
      </button>
      <button
        type="button"
        style={formElementStyle}
        className={`btn ${tempDC ? 'btn-success' : ''}`}
        onClick={() => handleHeuristicClick(!tempDC, setTempDC)}
      >
        Degree Constraint
      </button>
      <button
        type="button"
        style={formElementStyle}
        className={`btn ${tempLCV ? 'btn-success' : ''}`}
        onClick={() => handleHeuristicClick(!tempLCV, setTempLCV)}
      >
        LCV
      </button>
      <button
        type="button"
        style={formElementStyle}
        className={'btn btn-primary'}
        onClick={handleRunButton}
      >
        Run CSP
      </button>
    </div>
  );
};

export default Menu;
