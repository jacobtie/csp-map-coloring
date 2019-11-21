import axios from 'axios';

const env = 'dev';

let baseURL;

if (env === 'dev') {
  baseURL = 'http://localhost:5000/';
} else if (env === 'prod') {
  baseURL = '';
} else {
  baseURL = '';
}

export const getStates = async (
  country,
  colors,
  forwardChecking,
  propagation,
  mrv,
  dc,
  lcv
) => {
  const res = await axios.get(
    `${baseURL}map/color?countryName=${country}&colors=${colors}&forwardChecking=${forwardChecking}&propagation=${propagation}&mrv=${mrv}&dc=${dc}&lcv=${lcv}`
  );

  return res.data;
};

export const getAustraliaGeo = async () => {
  const res = await axios.get(
    'https://raw.githubusercontent.com/tonywr71/GeoJson-Data/master/australian-states.json'
  );

  return res.data;
};
