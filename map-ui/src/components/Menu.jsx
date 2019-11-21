import React, { useState } from 'react';

const Menu = ({
  countryName,
  setCountryName,
  forwardChecking,
  setForwardChecking,
  propagation,
  setPropagation,
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
  const [tempPropagation, setTempPropagation] = useState(propagation);
  const [tempMRV, setTempMRV] = useState(mrv);
  const [tempDC, setTempDC] = useState(dc);
  const [tempLCV, setTempLCV] = useState(lcv);

  const handleForwardCheckingClick = () => {
    if (tempForwardChecking) {
      setTempPropagation(false);
    }
    setTempForwardChecking(!tempForwardChecking);
  };

  const handleRunButton = () => {
    setCountryName(tempCountryName);
    setForwardChecking(tempForwardChecking);
    setPropagation(tempPropagation);
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
        value={tempCountryName}
      >
        <option value="australia">Australia</option>
        <option value="america">America</option>
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
        className={`btn ${tempPropagation ? 'btn-success' : ''}`}
        onClick={() => setTempPropagation(!tempPropagation)}
        disabled={!tempForwardChecking}
      >
        Propagation
      </button>
      <button
        type="button"
        style={formElementStyle}
        className={`btn ${tempMRV ? 'btn-success' : ''}`}
        onClick={() => setTempMRV(!tempMRV)}
      >
        MRV
      </button>
      <button
        type="button"
        style={formElementStyle}
        className={`btn ${tempDC ? 'btn-success' : ''}`}
        onClick={() => setTempDC(!tempDC)}
      >
        Degree Constraint
      </button>
      <button
        type="button"
        style={formElementStyle}
        className={`btn ${tempLCV ? 'btn-success' : ''}`}
        onClick={() => setTempLCV(!tempLCV)}
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
